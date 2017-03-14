using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using FirebirdSql.Data.FirebirdClient;
using System.Xml;

namespace Sorter
{
    class Program
    {
        private static string dbPath, dbUser, dbPassword;
        private static string exportPath, trailerPath;
        private static string content = "";
        private static List<Film> films = new List<Film>();

        private static bool FilmsEquals()
        {
            return false;
        }

        private static void LoadConfig()
        {
            XmlDocument config = new XmlDocument();
            config.Load("config.xml");
            XmlElement root = config.DocumentElement;

            XmlElement dbParams = root["database"];
            dbPath = dbParams["path"].InnerText;
            dbUser = dbParams["user"].InnerText;
            dbPassword = dbParams["password"].InnerText;

            XmlElement generalParams = root["general"];
            trailerPath = generalParams["trailerPath"].InnerText;
            exportPath = generalParams["exportPath"].InnerText;
        }

        private static void LoadTimetableFromDB(DateTime date)
        {
            string sql = String.Format(@"select distinct
                         --ai.hall_id,
                         --f.film_id,
                         f.name,
                         f.minutes,
                         f.age_restriction,
                         h.hallname,
                         ai.time_begin as session_time,
                         iif(pr.price_b is null, pr.price, pr.price+pr.price_b) as price,
                         prs.name as price_sheme_name,
                         pl.name as place_name

                        from cdt_levels l
                        inner join (
                         select sd1.session_detail_id, sd1.price_sheme_id, sd1.time_begin, sd1.film_id, s1.hall_id, sd1.hall_places_map_id, sd1.geometry_id
                         from cdt_session_details sd1
                         inner join cdt_sessions s1 on s1.session_id = sd1.session_id
                          and s1.session_date = '{0:dd.MM.yyyy}'
                         where (s1.deleted = 0 and sd1.deleted = 0)
                         ) ai on ai.hall_id = l.hall_id
                        inner join cdt_hallplan hp on ((hp.hall_id = ai.hall_id)
                         and (hp.level_id = l.level_id))
                        left join cdt_hall_places_map_objects m on ((m.hallplan_id = hp.hallplan_id)
                         and (m.hall_places_map_id = ai.hall_places_map_id))
                        inner join cdt_halls h on h.hall_id = ai.hall_id
                        inner join cdt_films f on f.film_id = ai.film_id
                        inner join cdt_places pl on pl.place_id = iif(m.place_id is null, hp.category , m.place_id)
                        inner join cdt_price_shemes prs on ai.price_sheme_id = prs.price_sheme_id
                        inner join cdt_prices pr on pl.place_id = pr.place_id
                         and (pr.price_sheme_id = ai.price_sheme_id)
                         and (pr.ticket_id = (
                          select t.ticket_id
                          from cdt_tickets t
                          where (t.deleted = 0) and (t.main = 1)))
                        where (l.hall_id = ai.hall_id)
                         and (l.geometry_id = ai.geometry_id)
                         and (l.deleted = 0)
                         and (hp.obj_type = 3)
                        order by f.name asc, session_time asc, price asc", date);

            FbConnectionStringBuilder connStr = new FbConnectionStringBuilder();
            connStr.Database = dbPath;
            connStr.UserID = dbUser;
            connStr.Password = dbPassword;
            using (FbConnection conn = new FbConnection(connStr.ToString()))
            {
                conn.Open();
                FbCommand selectCmd = new FbCommand(sql, conn);
                FbDataReader reader = selectCmd.ExecuteReader();
                while (reader.Read())
                {
                    string filmName = reader.GetString(0);
                    int minutes = reader.GetInt32(1);
                    string duration = String.Format("{0}:{1:00}", minutes / 60, minutes % 60);
                    string ageRestriction = reader.GetString(2) + "+";

                    Film currentFilm = new Film(filmName, trailerPath, duration, ageRestriction);
                    int idx = films.FindIndex(f => (f.FilmName == currentFilm.FilmName));
                    if (idx < 0)
                    {
                        films.Add(currentFilm);
                        idx = films.Count - 1;
                    }

                    string sessionTime = reader.GetString(4);
                    string sessionPrice = reader.GetString(5);
                    string sessionHall = reader.GetString(3);
                    films[idx].AddSession(sessionTime, sessionPrice, sessionHall);
                }
            }
        }

        private static void ExportCSV(string fileName)
        {
            films.Sort();
            foreach(Film film in films)
            {
                content += film.ToString();
            }

            using(StreamWriter writer = new StreamWriter(exportPath + fileName, false, Encoding.Default))
            {
                writer.Write(content.Trim());
            }
        }

        private static void CheckTrailers()
        {
            bool trailersOK = true;
            using(StreamWriter writer = new StreamWriter("log.txt", false, Encoding.Default))
            {
                foreach (Film film in films)
                {
                    if (!File.Exists(trailerPath + film.TrailerFileName))
                    {
                        writer.WriteLine("{0} - Не найден трейлер {1} для фильма \"{2}\"", DateTime.Now, film.TrailerFileName, film.FilmName);
                        trailersOK = false;
                    }
                }
            }
            if(trailersOK)
            {
                File.Delete("log.txt");
            }
        }

        private static void Read(string fileName)
        {
            fileName = DateTime.Now.ToString("dd-MM-yyyy") + "\\" + fileName;
            Console.WriteLine("Reading file {0}...", fileName);
            using (StreamReader reader = new StreamReader(fileName, Encoding.Default))
            {
                content += reader.ReadToEnd();
                content += ";;;;;;;;;;;;;\n;;;;;;;;;;;;;";
            }
        }

        private static void Process(string fileName)
        {
            Console.WriteLine("Processing...");

            string[] separators = { ";;;;;;;;;;;;;" };
            string[] items = content.Split(separators, 0);
            foreach(string item in items)
            {
                if(!string.IsNullOrWhiteSpace(item))
                {
                    string[] parts = item.Split(';');

                    string filmName = parts[13].Trim();
                    filmName = filmName.Replace(" VIP", "");
                    string trailerPath = parts[26];
                    string duration = parts[39];
                    string ageRestriction = parts[52];

                    Film currentFilm = new Film(filmName, trailerPath, duration, ageRestriction);
                    int idx = films.FindIndex(f => (f.FilmName == currentFilm.FilmName));
                    if (idx < 0)
                    {
                        films.Add(currentFilm);
                        idx = films.Count - 1;
                    }

                    for(int i = 16; i < 25; i ++)
                    {
                        string time = parts[i].Trim();
                        string price = parts[i + 13];
                        string hall = parts[i + 26];
                        if(string.IsNullOrWhiteSpace(time))
                        {
                            break;
                        }
                        else
                        {
                            films[idx].AddSession(time, price, hall);
                        }
                    }
                }
            }

            films.Sort();
            content = "";
            foreach (Film film in films)
            {
                content += film.ToString();
                //Console.WriteLine(film);

            }
            content = content.Trim();

            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.Default))
            {
                writer.Write(content);
            }
        }

        static void Main(string[] args)
        {
            LoadConfig();

            DateTime currentDate = DateTime.Now;
            if (args.Length > 0)
            {
                DateTime.TryParse(args[0], out currentDate);
            }
            LoadTimetableFromDB(currentDate);
            ExportCSV("source.csv");

            CheckTrailers();

            //Read("source.csv");
            //Read("source_imax.csv");
            //File.Delete("source_imax.csv");
            //Process("source.csv");

            Console.WriteLine("\nDone, quit in 3 secs!");
            Thread.Sleep(3000);
        }
    }
}
