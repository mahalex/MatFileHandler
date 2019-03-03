// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Linq;
using System.Numerics;

namespace MatFileHandler
{
    /// <summary>
    /// A better interface for using datetime objects.
    /// </summary>
    public class DatetimeAdapter
    {
        private readonly double[] data;
        private readonly double[] data2;
        private readonly int[] dimensions;

        private readonly DateTimeOffset epoch;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatetimeAdapter"/> class.
        /// </summary>
        /// <param name="array">Source datetime object.</param>
        public DatetimeAdapter(IArray array)
        {
            var matObject = array as IMatObject;
            if (matObject?.ClassName != "datetime")
            {
                throw new ArgumentException("The object provided is not a datetime.");
            }

            epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var dataArray = matObject["data", 0] as IArrayOf<double>;
            if (dataArray is null)
            {
                var dataComplex = matObject["data", 0] as IArrayOf<Complex>;
                var complexData = dataComplex.ConvertToComplexArray();
                data = complexData.Select(c => c.Real).ToArray();
                data2 = complexData.Select(c => c.Imaginary).ToArray();
                dimensions = dataComplex.Dimensions;
            }
            else
            {
                data = dataArray.ConvertToDoubleArray();
                data2 = new double[data.Length];
                dimensions = dataArray.Dimensions;
            }
        }

        /// <summary>
        /// Gets datetime array dimensions.
        /// </summary>
        public int[] Dimensions => dimensions;

        /// <summary>
        /// Gets values of datetime object at given position in the array converted to <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="list">Indices.</param>
        /// <returns>Value converted to  <see cref="DateTimeOffset"/>.</returns>
        public DateTimeOffset this[params int[] list] => epoch.AddMilliseconds(data[Dimensions.DimFlatten(list)]);
    }
}