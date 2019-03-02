using System;

namespace Top40Modify
{
    public class Top40Timer
    {
        public DateTime StartTime { get; set; }
        public string Title { get; set; }

        public Top40Timer()
        {
        }

        public static Top40Timer Start(string title)
        {
            Top40Timer timer = new Top40Timer();
            timer.StartTime = DateTime.Now;
            timer.Title = title;
            Top40Util.Debug(title);
            return timer;
        }

        public void End()
        {
            TimeSpan duration = DateTime.Now - StartTime;
            Top40Util.Debug(Title + ": elapsed=" + duration.TotalSeconds);
        }
    }
}

