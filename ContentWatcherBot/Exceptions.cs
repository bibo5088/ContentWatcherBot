using System;

namespace ContentWatcherBot
{
    public class InvalidWatcherNameException : Exception
    {
    }

    public class MissingWatcherArgumentException : Exception
    {
        public MissingWatcherArgumentException(string argDesc) : base($"Missing argument : {argDesc}")
        {
        }
    }

    public class FetchFailedException : Exception
    {
        public FetchFailedException() : base("Failed to fetch content")
        {
        }

        public FetchFailedException(string message) : base(message)
        {
        }
    }
}