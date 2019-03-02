using System;

namespace Top40Modify
{
    public class Top40Util
    {
        public static void Debug(string str)
        {
            if (Top40Settings.Debug)
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

