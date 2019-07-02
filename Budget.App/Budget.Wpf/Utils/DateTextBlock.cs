using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Budget.Wpf.Utils
{
    public class DateTextBlock : TextBlock
    {
        private DateTime _dateTime;

        public DateTextBlock()
        {
            Date = DateTime.Now;
        }

        public DateTime Date
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                this.Text = _dateTime.ToString("MMMM yyyy");
            }
        }

        public DateTime IncredementMonth()
        {
            Date = Date.AddMonths(1);
            return Date;
        }
        public DateTime DecrementMonth()
        {
            Date = Date.AddMonths(-1);
            return Date;
        }

        
    }
}
