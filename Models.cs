using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        private DaysOfWeek dayOfWeek;

        public AvailabilityWindow(DaysOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
        {
            DayOfWeek = dayOfWeek;
            StartTime = startTime;
            EndTime = endTime;
            Active = true;
        }

        public string Id { get; } = Guid.NewGuid().ToString();

        public DaysOfWeek DayOfWeek
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

        public bool Active { get; set; }

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
}