// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// A matrix of type T.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    internal class MiNum<T> : DataElement
      where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiNum{T}"/> class.
        /// </summary>
        /// <param name="data">Contents of the matrix.</param>
        public MiNum(T[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the contents of the matrix.
        /// </summary>
        public T[] Data { get; }
    }
}