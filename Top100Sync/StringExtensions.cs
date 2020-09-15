//
// © Copyright 2020 Kevin Pearson
//
using System;

namespace Top100Sync
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

