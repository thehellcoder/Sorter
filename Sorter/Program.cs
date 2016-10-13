using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Sorter
{
    class Program
    {
        private static string content = "";
        private static List<Film> films = new List<Film>();

        private static bool FilmsEquals()
        {
            return false;
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
            Read("source.csv");
            Read("source_imax.csv");
            //File.Delete("source_imax.csv");
            Process("source.csv");

            //Console.WriteLine("\nPress any key...");
            //Console.ReadKey();

            Console.WriteLine("\nDone, quit in 3 secs!");
            Thread.Sleep(3000);
        }
    }
}
