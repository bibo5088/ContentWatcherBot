using System;
using System.Runtime.Serialization;
using Discord.Commands;

namespace ContentWatcherBot
{
    public class ReportableExceptions : Exception
    {
        public ReportableExceptions()
        {
        }

        protected ReportableExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReportableExceptions(string message) : base(message)
        {
        }

        public ReportableExceptions(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UnknownWatcherUrl : ReportableExceptions
    {
        public UnknownWatcherUrl(Uri url) : base($"Unable to watch {url}")
        {
        }
    }
}