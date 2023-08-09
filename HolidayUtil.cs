namespace Net_core_7.Utils
{
    public class HolidayUtil
    {
        public int Year { get; set; }
        private int EasterMonth { get; set; }
        private int EasterDay { get; set; }
        private readonly DayOfWeek[] WEEKEND_DAYS = new DayOfWeek[]{ DayOfWeek.Saturday, DayOfWeek.Sunday
    };
    private List<string> holidays = new List<string>();

        public HolidayUtil()
        {
            ProcessYear(DateTime.UtcNow.Year);
        }
        public HolidayUtil(int year)
        {
            ProcessYear(year);
        }
        private void ProcessYear(int year)
        {
            Year = year;            
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int g = (8 * b + 13) / 25;
            int h = (19 * a + b - d - g + 15) % 30;
            int j = c / 4;
            int k = c % 4;
            int m = (a + 11 * h) / 319;
            int r = (2 * e + 2 * j - k - h + m + 32) % 7;
            EasterMonth = (h - m + r + 90) / 25;
            EasterDay = (h - m + r + this.EasterMonth + 19) % 32;
            this.EasterMonth--;

            AddOrdinaryHolidays();
            AddEmilianiDays();
            AddOtherHolliday();
        }

        private void AddOrdinaryHolidays()
        {
            holidays.Add("1:1");               // Primero de Enero
            holidays.Add("5:1");               // Dia del trabajo 1 de mayo
            holidays.Add("7:20");              //Independencia 20 de Julio
            holidays.Add("8:7");               //Batalla de boyaca 7 de agosto
            holidays.Add("12:8");              //Maria inmaculada 8 de diciembre
            holidays.Add("12:25");             //Navidad 25 de diciembre
        }

        private void AddEmilianiDays()
        {
            calculateEmiliani(1, 6);           // Reyes magos 6 de enero
            calculateEmiliani(3, 19);          //San jose 19 de marzo
            calculateEmiliani(6, 29);          //San pedro y san pablo 29 de junio
            calculateEmiliani(8, 15);          //Asuncion 15 de agosto
            calculateEmiliani(10, 12);          //Descubrimiento de america 12 de octubre
            calculateEmiliani(11, 1);          //Todos los santos 1 de noviembre
            calculateEmiliani(11, 11);         //Independencia de cartagena 11 de noviembre
        }

        private void AddOtherHolliday()
        {
            calculateOtherHoliday(-3, false);  //jueves santos
            calculateOtherHoliday(-2, false);  //viernes santo
            calculateOtherHoliday(40, true);   //Asención del señor de pascua 
            calculateOtherHoliday(60, true);   //Corpus cristi
            calculateOtherHoliday(68, true);   //Sagrado corazon
        }

        /**
        * Move a holiday to monday by emiliani law
        * @param month Original month of the holiday
        * @param day Original day of the holiday
        */
        private void calculateEmiliani(int month, int day)
        {
            var date = new DateOnly(Year, month, day);            
            switch (date.DayOfWeek)
            {                
                case DayOfWeek.Tuesday: //martes 2
                    date = date.AddDays(6);
                    break;
                case DayOfWeek.Wednesday: //miercoles 3
                    date = date.AddDays(5);
                    break;
                case DayOfWeek.Thursday: //jueves 4
                    date = date.AddDays(4);
                    break;
                case DayOfWeek.Friday: //viernes 5
                    date = date.AddDays(3);
                    break;
                case DayOfWeek.Saturday: //sabado 6
                    date = date.AddDays(2);
                    break;
                case DayOfWeek.Sunday: //domingo 7
                    date = date.AddDays(1);
                    break;
                default:
                    break;
            }
            holidays.Add($"{date.Month}:{date.Day}");
        }

        /**
        * Calculate holidays according to easter day
        * @param days Number of days after (+) or before (-) of easter day
        * @param emiliani Indicates whether the emiliani law affects
        */
        private void calculateOtherHoliday(int days, bool emiliani)
        {            
            var date = new DateOnly(Year, EasterMonth + 1, EasterDay);            
            date = date.AddDays(days);
            if (emiliani)
            {
                calculateEmiliani(date.Month, date.Day);
            }
            else
            {
                holidays.Add($"{date.Month}:{date.Day}");
            }
        }

        /**
        * Indicates if a day is a holiday
        * @param month Month in which the day is contained
        * @param day Day to evaluate
        * @return true if the day is a holiday, false otherwise
        */
        public bool IsHoliday(int month, int day)
        {
            return this.holidays.Contains($"{month}:{day}");
        }

        /**
        * Get the previous available business day from a given date
        * @param date Start date to count from        
        * @return Date representation of the next business day
        */
        public DateOnly GetPreviousBusinessDay(DateOnly date)
        {
            var dateToCheck = date;                     
            var businessDayFound = false;
            while (!businessDayFound)
            {
                var dayOfWeek = dateToCheck.DayOfWeek;
                if (!WEEKEND_DAYS.Contains(dayOfWeek) && !IsHoliday(dateToCheck.Month, dateToCheck.Day))
                {
                    businessDayFound = true;
                    continue;
                }
                dateToCheck = dateToCheck.AddDays(-1);
            }

            return dateToCheck;
        }

        /**
        * Get the next available business day from a given date
        * @param date Start date to count from        
        * @return Date representation of the next business day
        */
        public DateOnly GetNextBusinessDay(DateOnly date)
        {
            var dateToCheck = date;               
            var businessDayFound = false;
            while (!businessDayFound)
            {
                var dayOfWeek = dateToCheck.DayOfWeek;
                if (!WEEKEND_DAYS.Contains(dayOfWeek) && !IsHoliday(dateToCheck.Month, dateToCheck.Day))
                {
                    businessDayFound = true;
                    continue;
                }                
                dateToCheck = dateToCheck.AddDays(1);                
            }
            
            return dateToCheck;
        }

        public DateOnly GetNextBusinessDay(DateOnly date, int days)
        {
            var calendar = date;            
            while (days > 0)
            {
                calendar = calendar.AddDays(1);                
                var dayOfWeek = (int)calendar.DayOfWeek;
                if (dayOfWeek != 1 && dayOfWeek != 7 && !IsHoliday(calendar.Month, calendar.Day))
                {
                    days--;
                }
            }
            return calendar;
        }

        /**
        * Get the amount of business days between two dates
        * @param dateInit Start date
        * @param dateEnd End date
        * @return	Amount for business days
        */
        public static int CountBusinessDays(DateOnly dateInit, DateOnly? dateEnd)
        {
            DateOnly limitDay = DateOnly.FromDateTime(DateTime.Now);
            int days = 0;
            if (dateEnd != null)
            {
                limitDay = dateEnd.Value;
            }
            DateOnly startDay = dateInit;            
            HolidayUtil lobHolidayUtil = new HolidayUtil(startDay.Year);
            while (startDay < limitDay)
            {
                startDay = startDay.AddDays(1);
                if (startDay.Year != lobHolidayUtil.Year)
                {
                    lobHolidayUtil = new HolidayUtil(startDay.Year);
                }
                var dayOfWeek = (int)startDay.DayOfWeek;
                if (dayOfWeek != 1 && dayOfWeek != 7 && !lobHolidayUtil.IsHoliday(startDay.Month, startDay.Day))
                {
                    days++;
                }
            }
            return days;
        }
    }
}
