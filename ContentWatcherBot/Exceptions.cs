using System;
using System.Runtime.Serialization;

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

    public class UnknownWatcherUrl : Exception
    {
        public UnknownWatcherUrl(Uri url) : base($"Unable to watch {url}")
        {
        }
    }

    public class ServerOnlyCommand : ReportableExceptions
    {
        public ServerOnlyCommand() : base("This command can only be used from a server")
        {
        }
    }

    public class AdminOnlyCommand : ReportableExceptions
    {
        public AdminOnlyCommand() : base("This command can only be used by admins")
        {
        }
    }
}