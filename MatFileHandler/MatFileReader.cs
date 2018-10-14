// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.IO;

namespace MatFileHandler
{
    /// <summary>
    /// Class for reading .mat files.
    /// </summary>
    public class MatFileReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileReader"/> class with a stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        public MatFileReader(Stream stream)
        {
            Stream = stream;
        }

        private Stream Stream { get; }

        /// <summary>
        /// Reads the contents of a .mat file from the stream.
        /// </summary>
        /// <returns>Contents of the file.</returns>
        public IMatFile Read()
        {
            using (var reader = new BinaryReader(Stream))
            {
                return Read(reader);
            }
        }

        /// <summary>
        /// Read raw variables from a .mat file.
        /// </summary>
        /// <param name="reader">Binary reader.</param>
        /// <param name="subsystemDataOffset">Offset to the subsystem data to use (read from the file header).</param>
        /// <returns>Raw variables read.</returns>
        internal static List<RawVariable> ReadRawVariables(BinaryReader reader, long subsystemDataOffset)
        {
            var variables = new List<RawVariable>();
            var subsystemData = new SubsystemData();
            var dataElementReader = new DataElementReader(subsystemData);
            while (true)
            {
                try
                {
                    var position = reader.BaseStream.Position;
                    var dataElement = dataElementReader.Read(reader);
                    if (position == subsystemDataOffset)
                    {
                        var subsystemDataElement = dataElement as IArrayOf<byte>;
                        subsystemData.Set(ReadSubsystemData(subsystemDataElement.Data));
                    }
                    else
                    {
                        variables.Add(new RawVariable(position, dataElement));
                    }
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }

            return variables;
        }

        private static IMatFile Read(BinaryReader reader)
        {
            var header = ReadHeader(reader);
            var rawVariables = ReadRawVariables(reader, header.SubsystemDataOffset);
            var variables = new List<IVariable>();
            foreach (var variable in rawVariables)
            {
                var array = variable.DataElement as MatArray;
                if (array is null)
                {
                    continue;
                }

                variables.Add(new MatVariable(
                    array,
                    array.Name,
                    array.Flags.Variable.HasFlag(Variable.IsGlobal)));
            }

            return new MatFile(variables);
        }

        private static Header ReadHeader(BinaryReader reader)
        {
            return Header.Read(reader);
        }

        private static SubsystemData ReadSubsystemData(byte[] bytes)
        {
            return SubsystemDataReader.Read(bytes);
        }
    }
}