using System;

namespace Sorter
{
    class Session: IComparable
    {
        private DateTime time;
        private string price;
        private string hall;

        public Session(DateTime time, string price, string hall)
        {
            this.time = time;
            this.price = price.Trim();

            hall = hall.Replace("Зал ", "");
            this.hall = hall.Trim();
        }

        public DateTime Time
        {
            get { return time; }
        }

        public string Price
        {
            get { return price; }
        }

        public string Hall
        {
            get { return hall; }
        }

        public int CompareTo(object obj)
        {
            Session anotherSession = (Session)obj;
            return time.CompareTo(anotherSession.time);
        }
    }
}
