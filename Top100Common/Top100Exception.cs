using System;
using System.Collections.Generic;
using System.Text;

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
