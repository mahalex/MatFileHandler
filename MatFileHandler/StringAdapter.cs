// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// A better interface for using string objects.
    /// </summary>
    public class StringAdapter
    {
        private readonly int[] dimensions;
        private readonly string[] strings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringAdapter"/> class.
        /// </summary>
        /// <param name="array">Source string object.</param>
        public StringAdapter(IArray array)
        {
            var matObject = array as IMatObject;
            if (matObject?.ClassName != "string")
            {
                throw new ArgumentException("The object provided is not a string.");
            }

            var binaryData = matObject["any", 0] as IArrayOf<ulong>
                             ?? throw new HandlerException("Cannot extract string data.");

            (dimensions, strings) = ParseBinaryData(binaryData.Data);
        }

        /// <summary>
        /// Gets string array dimensions.
        /// </summary>
        public int[] Dimensions => dimensions;

        /// <summary>
        /// Gets string object at given position.
        /// </summary>
        /// <param name="list">Indices.</param>
        /// <returns>Value.</returns>
        public string this[params int[] list] => strings[Dimensions.DimFlatten(list)];

        private static (int[] dimensions, string[] strings) ParseBinaryData(ulong[] binaryData)
        {
            var numberOfDimensions = (int)binaryData[1];
            var d = new int[numberOfDimensions];
            for (var i = 0; i < numberOfDimensions; i++)
            {
                d[i] = (int)binaryData[i + 2];
            }

            var numberOfElements = d.NumberOfElements();
            var start = numberOfDimensions + 2;
            var lengths = new int[numberOfElements];
            for (var i = 0; i < numberOfElements; i++)
            {
                lengths[i] = (int)binaryData[start + i];
            }

            var strings = new string[numberOfElements];

            start += numberOfElements;
            var numberOfUlongsLeft = binaryData.Length - start;
            var bytes = new byte[numberOfUlongsLeft * sizeof(ulong)];
            Buffer.BlockCopy(binaryData, start * sizeof(ulong), bytes, 0, bytes.Length);
            var counter = 0;
            for (var i = 0; i < numberOfElements; i++)
            {
                strings[i] = Encoding.Unicode.GetString(bytes, counter * 2, lengths[i] * 2);
                counter += lengths[i];
            }

            return (d, strings);
        }
    }
}