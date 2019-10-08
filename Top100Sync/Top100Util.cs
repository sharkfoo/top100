using System;

namespace Top100Modify
{
    public class Top100Util
    {
        public static void Debug(string str)
        {
            if (Top100Settings.Debug)
            {
                Console.WriteLine("Debug: " + str);
            }
        }

        public static void Error(string err)
        {
            Console.WriteLine("Error: " + err);
        }
    }
}

