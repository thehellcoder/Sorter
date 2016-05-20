using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sorter
{
    public class Film: IComparable
    {
        private const int SESSIONS_IN_ROW = 9;
        private string filmName;
        private string format;
        private string trailerPath;
        private string trailerFilename;
        private string duration;
        private string ageRestriction;
        private List<Session> sessions = new List<Session>();

        public Film(string filmName, string trailerPath, string duration, string ageRestriction)
        {
            this.filmName = filmName.Trim();
            this.trailerPath = trailerPath.Trim();
            this.duration = duration.Trim();
            this.ageRestriction = ageRestriction.Trim();

            this.format = "";
            string[] formats = { "IMAX 2D", "IMAX 3D", "2D", "3D" };
            foreach(string format in formats)
            {
                if(this.filmName.Contains(format))
                {
                    this.format = format;
                    break;
                }
            }
            trailerFilename = TranslateTrailerFilename();
        }

        private string TranslateTrailerFilename()
        {
            string result = filmName;
            if(!string.IsNullOrEmpty(format))
            {
                result = result.Replace(format, "");
            }
            result = result.ToLower();
            
            Regex nonAlphaNumeric = new Regex(@"\W");
            result = nonAlphaNumeric.Replace(result, " ");
            Regex tooManySpaces = new Regex(@"\s{2,}");
            result = tooManySpaces.Replace(result, " ").Trim();

            string[] rus = {
                "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и",
                "й", "к", "л", "м", "н", "о", "п", "р", "с", "т",
                "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь",
                "э", "ю", "я", " "
            };
            string[] trans = {
                "a", "b", "v", "g", "d", "e", "e", "j", "z", "i",
                "i", "k", "l", "m", "n", "o", "p", "r", "s", "t",
                "u", "f", "h", "c", "ch", "sh", "sc", "", "y", "",
                "e", "iu", "ia", "_"
            };
            for (int i = 0; i < rus.Length; i ++)
            {
                result = result.Replace(rus[i], trans[i]);
            }

            return result;
        }

        public void AddSession(string time, string price, string hall)
        {
            sessions.Add(new Session(time, price, hall));
        }

        public string FilmName
        {
            get { return filmName; }
        }

        public string Format
        {
            get { return format; }
        }

        public string TrailerFileName
        {
            get { return trailerFilename; }
        }

        public int GetRowsCount()
        {
            return (int)Math.Ceiling((double)sessions.Count / SESSIONS_IN_ROW);
        }

        public override string ToString()
        {
            string result = "";
            string pattern = "Название фильма;№ сеанса;;1 сеанс;2 сеанс;3 сеанс;4 сеанс;5 сеанс;6 сеанс;7 сеанс;8 сеанс;9 сеанс;;\r\n";
            pattern += filmName + ";Начало сеанса;;{0}Путь к трейлеру;" + trailerPath + "\r\n";
            pattern += ";Цена;;{1}Продолжительность;" + duration + "\r\n";
            pattern += ";Зал;;{2}Возраст;" + ageRestriction + "\r\n";
            pattern += ";;;;;;;;;;;;;\r\n;;;;;;;;;;;;;\r\n";

            sessions.Sort();
            while (sessions.Count > 0)
            {
                string times = "";
                string prices = "";
                string halls = "";

                for (int i = 0; i < SESSIONS_IN_ROW; i++)
                {
                    if(sessions.Count > 0)
                    {
                        Session current = sessions[0];
                        times += string.Format("{0:HH:mm};", current.Time);
                        prices += string.Format("{0};", current.Price);
                        halls += string.Format("{0};", current.Hall);
                        sessions.RemoveAt(0);
                    }
                    else
                    {
                        times += ";";
                        prices += ";";
                        halls += ";";
                    }
                }
                result += string.Format(pattern, times, prices, halls);
            }
            return result;
        }

        public int CompareTo(object obj)
        {
            Film anotherFilm = (Film)obj;
            return filmName.CompareTo(anotherFilm.filmName);
        }
    }
}
