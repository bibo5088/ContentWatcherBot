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
        
    }
}