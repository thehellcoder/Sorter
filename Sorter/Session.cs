using System;

namespace Sorter
{
    class Session: IComparable
    {
        private DateTime time;
        private string price;
        private string hall;

        public Session(string time, string price, string hall)
        {
            DateTime dt;
            if (DateTime.TryParse(time.Trim(), out dt))
            {
                if (dt.Hour < 9)
                {
                    dt = dt.AddDays(1);
                }
                this.time = dt;
            }
            this.price = price.Trim();
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
