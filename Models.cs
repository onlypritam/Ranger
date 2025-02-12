using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ranger
{
    public class Resource
    {
        private string name;

        public Resource(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Invalid resource name.");
            }
            Name = name;
            Skills = new ObservableCollection<Skill>();
            AvailabilityWindows = new ObservableCollection<AvailabilityWindow>();
        }

        public string Id { get; } = Guid.NewGuid().ToString();

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<Skill> Skills { get; set; }

        public ObservableCollection<AvailabilityWindow> AvailabilityWindows { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            MainWindow.HasChanges = true;
        }
    }

    public class AvailabilityWindow
    {
        private TimeOnly startDate;
        private TimeOnly endDate;
        private string dayOfWeek;


        public AvailabilityWindow(string dayOfWeek, TimeOnly startTime, TimeOnly endTime)
        {
            if(!Enum.IsDefined(typeof(DaysOfWeek), dayOfWeek))
            {
                throw new ArgumentNullException("Invalid day of week.");
            }

            //if (startTime >= endTime)
            //{
            //    throw new ArgumentException("Start time must be before end time.");
            //}

            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
        }

        public string Id { get; } = Guid.NewGuid().ToString();


        public string DayOfWeek
        {
            get { return dayOfWeek; }
            set
            {
                dayOfWeek = value;
                OnPropertyChanged();
            }
        }
        public TimeOnly StartTime
        {
            get { return startDate; }
            set
            {
                startDate = value;
                OnPropertyChanged();
            }
        }
        public TimeOnly EndTime
        {
            get { return endDate; }
            set
            {
                endDate = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            MainWindow.HasChanges = true;
        }
    }

    public class Skill
    {
        private string name;

        public Skill(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Invalid skill name.");
            }
            Name = name;
        }

        public string Id { get; } = Guid.NewGuid().ToString();

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            MainWindow.HasChanges = true;
        }
    }

    public enum DaysOfWeek : ushort
    {
        Mon = 1,
        Tue = 2,
        Wed = 3,
        Thu = 4,
        Fri = 5,
        Sat = 6,
        Sun = 7
    }

    //public enum HoursOfDay : ushort
    //{
    //    H00_00 = 1,
    //    H00_30 = 2,
    //    H01_00 = 3,
    //    H01_30 = 4,
    //    H02_00 = 5,
    //    H02_30 = 6,
    //    H03_00 = 7,
    //    H03_30 = 8,
    //    H04_00 = 9,
    //    H04_30 = 10,
    //    H05_00 = 11,
    //    H05_30 = 12,
    //    H06_00 = 13,
    //    H06_30 = 14,
    //    H07_00 = 15,
    //    H07_30 = 16,
    //    H08_00 = 17,
    //    H08_30 = 18,
    //    H09_00 = 19,
    //    H09_30 = 20,
    //    H10_00 = 21,
    //    H10_30 = 22,
    //    H11_00 = 23,
    //    H11_30 = 24,
    //    H12_00 = 25,
    //    H12_30 = 26,
    //    H13_00 = 27,
    //    H13_30 = 28,
    //    H14_00 = 29,
    //    H14_30 = 30,
    //    H15_00 = 31,
    //    H15_30 = 32,
    //    H16_00 = 33,
    //    H16_30 = 34,
    //    H17_00 = 35,
    //    H17_30 = 36,
    //    H18_00 = 37,
    //    H18_30 = 38,
    //    H19_00 = 39,
    //    H19_30 = 40,
    //    H20_00 = 41,
    //    H20_30 = 42,
    //    H21_00 = 43,
    //    H21_30 = 44,
    //    H22_00 = 45,
    //    H22_30 = 46,
    //    H23_00 = 47,
    //    H23_30 = 48
    //}


}