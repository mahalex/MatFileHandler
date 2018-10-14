// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Exception related to Matlab data handling
    /// </summary>
    public class HandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public HandlerException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}