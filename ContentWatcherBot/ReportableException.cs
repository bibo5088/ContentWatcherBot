using System;

namespace ContentWatcherBot
{
    public class ReportableException : Exception
    {
        public ReportableException()
        {
        }

        public ReportableException(string message)
            : base(message)
        {
        }

        public ReportableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}