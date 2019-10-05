// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// A better interface for using duration objects.
    /// </summary>
    public class DurationAdapter
    {
        private readonly int[] dimensions;
        private readonly double[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationAdapter"/> class.
        /// </summary>
        /// <param name="array">Source duration object.</param>
        public DurationAdapter(IArray array)
        {
            var matObject = array as IMatObject;
            if (matObject?.ClassName != "duration")
            {
                throw new ArgumentException("The object provided is not a duration.");
            }

            var dataObject = matObject["millis", 0];
            data = dataObject.ConvertToDoubleArray()
                ?? throw new HandlerException("Cannot extract data for the duration adapter.");
            dimensions = dataObject.Dimensions;
        }

        /// <summary>
        /// Gets duration array dimensions.
        /// </summary>
        public int[] Dimensions => dimensions;

        /// <summary>
        /// Gets duration object at given position.
        /// </summary>
        /// <param name="list">Indices.</param>
        /// <returns>Value.</returns>
        public TimeSpan this[params int[] list] => TimeSpan.FromTicks((long)(10000.0 * data[Dimensions.DimFlatten(list)]));
    }
}