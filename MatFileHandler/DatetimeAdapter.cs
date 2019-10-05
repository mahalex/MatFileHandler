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

            switch (matObject["data", 0])
            {
                case IArrayOf<double> dataArray:
                    data = dataArray.ConvertToDoubleArray()
                        ?? throw new HandlerException("Cannot extract data for the datetime adapter.");
                    data2 = new double[data.Length];
                    dimensions = dataArray.Dimensions;
                    break;
                case IArrayOf<Complex> dataComplex:
                    var complexData = dataComplex.ConvertToComplexArray()
                        ?? throw new HandlerException("Cannot extract data for the datetime adapter.");
                    data = complexData.Select(c => c.Real).ToArray();
                    data2 = complexData.Select(c => c.Imaginary).ToArray();
                    dimensions = dataComplex.Dimensions;
                    break;
                default:
                    throw new HandlerException("Datetime data not found.");
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
        /// <returns>Value converted to <see cref="DateTimeOffset"/>; null if the resulting value is unrepresentable.</returns>
        public DateTimeOffset? this[params int[] list]
        {
            get
            {
                var milliseconds = data[Dimensions.DimFlatten(list)];
                if (milliseconds < -62_135_596_800_000.0 || milliseconds > 253_402_300_799_999.0)
                {
                    return null;
                }
                return epoch.AddMilliseconds(milliseconds);
            }
        }
    }
}