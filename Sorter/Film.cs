using System;
using System.Collections.Generic;

namespace Sorter
{
    public class Film: IComparable
    {
        private const decimal SESSIONS_IN_ROW = 9;
        private string filmName;
        private string trailerPath;
        private string duration;
        private string ageRestriction;
        private List<Session> sessions = new List<Session>();

        public Film(string filmName, string trailerPath, string duration, string ageRestriction)
        {
            this.filmName = filmName.Trim();
            this.trailerPath = trailerPath.Trim();
            this.duration = duration.Trim();
            this.ageRestriction = ageRestriction.Trim();
        }

        public void AddSession(string time, string price, string hall)
        {
            sessions.Add(new Session(time, price, hall));
        }

        public string FilmName
        {
            get { return filmName; }
        }

        public decimal getRowsCount()
        {
            return Math.Ceiling(sessions.Count / SESSIONS_IN_ROW);
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
