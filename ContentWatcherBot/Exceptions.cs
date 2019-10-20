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


    public class InvalidWatcherArgumentException : Exception
    {
        public InvalidWatcherArgumentException() : base("Invalid argument")
        {
        }

        public InvalidWatcherArgumentException(string message) : base(message)
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

        public FetchFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}