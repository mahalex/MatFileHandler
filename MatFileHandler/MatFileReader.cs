// Copyright 2017-2018 Alexander Luzgarev

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

        private static void ReadHeader(BinaryReader reader)
        {
            Header.Read(reader);
        }

        private static IMatFile Read(BinaryReader reader)
        {
            ReadHeader(reader);
            var variables = new List<IVariable>();
            while (true)
            {
                try
                {
                    var dataElement = DataElementReader.Read(reader) as MatArray;
                    if (dataElement == null)
                    {
                        continue;
                    }
                    variables.Add(new MatVariable(
                        dataElement,
                        dataElement.Name,
                        dataElement.Flags.Variable.HasFlag(Variable.IsGlobal)));
                }
                catch (EndOfStreamException)
                {
                    break;
                }
            }
            return new MatFile(variables);
        }
    }
}