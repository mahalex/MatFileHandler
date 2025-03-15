// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// A better interface for using enum adapter.
    /// </summary>
    public class EnumAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAdapter"/> class.
        /// </summary>
        /// <param name="array">Source enum object.</param>
        public EnumAdapter(IArray array)
        {
            var matObject = array as Opaque;
            if (matObject?.RawData is not IStructureArray rawData)
            {
                throw new HandlerException("Cannot extract data for the enum adapter.");
            }

            if (rawData["ValueNames"] is not IArrayOf<uint> valueNamesData)
            {
                throw new HandlerException("Cannot extract data for the enum adapter.");
            }

            var numberOfNames = valueNamesData.Count;
            var valueNames = new string[numberOfNames];
            var names = matObject.SubsystemData.FieldNames;
            for (var i = 0; i < numberOfNames; i++)
            {
                valueNames[i] = names[valueNamesData[i] - 1];
            }

            if (rawData["ValueIndices"] is not IArrayOf<uint> valueIndices)
            {
                throw new HandlerException("Cannot extract data for the enum adapter.");
            }

            ClassName = matObject.ClassName;
            ValueNames = valueNames;
            Values = valueIndices;
        }

        /// <summary>
        /// Gets name of the enumeration class.
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// Gets names of the enumeration values.
        /// </summary>
        public string[] ValueNames { get; }

        /// <summary>
        /// Gets indices of values stored in the variable.
        /// </summary>
        public IArrayOf<uint> Values { get; }
    }
}