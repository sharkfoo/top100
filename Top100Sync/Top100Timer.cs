using System;

namespace Top100Modify
{
    public class Top100Timer
    {
        public DateTime StartTime { get; set; }
        public string Title { get; set; }

        public Top100Timer()
        {
        }

        public static Top100Timer Start(string title)
        {
            Top100Timer timer = new Top100Timer();
            timer.StartTime = DateTime.Now;
            timer.Title = title;
            Top100Util.Debug(title);
            return timer;
        }

        public void End()
        {
            TimeSpan duration = DateTime.Now - StartTime;
            Top100Util.Debug(Title + ": elapsed=" + duration.TotalSeconds);
        }
    }
}

