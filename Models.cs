using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ranger
{
    public class Resource
    {
        private string name;

        public Resource(string name, string id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Invalid resource name.");
            }
            Name = name;
            Active = true;

            if (id is null)
            {
                Id = Guid.NewGuid().ToString();
            }
            else
            {
                Id = id;
            }

            Skills = new ObservableCollection<Skill>();
            AvailabilityWindows = new ObservableCollection<AvailabilityWindow>();
        }

        public string Id { get; private set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public bool Active { get; set; }

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

        public Skill(string name, string id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Invalid skill name.");
            }
            Name = name;

            if (id is null)
            {
                Id = Guid.NewGuid().ToString();
            }
            else
            {
                Id = id;
            }
        }

        public string Id { get; private set; }

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

    public record ResourceWithSchedule
    {
        public string ResourceName { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public record GraphInfo
    {
        public List<string> Names { get; set; }

        public GraphInfo()
        {
            Names = new List<string>();
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