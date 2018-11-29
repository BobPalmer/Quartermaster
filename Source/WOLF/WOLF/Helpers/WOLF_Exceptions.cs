using System;

namespace WOLF
{
    public class StreamAlreadyCreatedException : Exception
    {
        public StreamAlreadyCreatedException(string resourceName)
            : base("A stream for " + resourceName + " has already been created.")
        {
        }
    }
}
