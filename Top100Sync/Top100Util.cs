//
// © Copyright 2020 Kevin Pearson
//
using System;

namespace Top100Sync
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

