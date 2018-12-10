﻿using System;

namespace WOLF
{
    public class DepotDoesNotExistException : Exception
    {
        private static readonly string EXCEPTION_MESSAGE = "A WOLF depot has not yet been established for {0} in {1}.";

        public DepotDoesNotExistException(string body, string biome)
            : base(string.Format(EXCEPTION_MESSAGE, body, biome))
        {
        }
    }
}
