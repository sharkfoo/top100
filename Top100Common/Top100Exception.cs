//
// © Copyright 2017 Kevin Pearson
//
using System;

namespace Top100Common
{
    public enum ReasonType
    {
        Conflict,
        NotFound,
        Unknown
    }

    public class Top100Exception : Exception
    {
        public Top100Exception(ReasonType reason)
        {
            Reason = reason;
        }
        public ReasonType Reason { get; set; }
    }
}
