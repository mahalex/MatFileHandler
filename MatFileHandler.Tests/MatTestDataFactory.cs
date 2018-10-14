// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Factory providing the parsed contents of .mat files.
    /// </summary>
    public class MatTestDataFactory : AbstractTestDataFactory<IMatFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatTestDataFactory"/> class.
        /// </summary>
        /// <param name="testDirectory">Directory containing test files.</param>
        public MatTestDataFactory(string testDirectory)
            : base(
                testDirectory,
                new ExtensionTestFilenameConvention("mat"))
        {
        }

        /// <summary>
        /// Read and parse data from a .mat file.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>Parsed contents of the file.</returns>
        protected override IMatFile ReadDataFromStream(Stream stream)
        {
            var matFileReader = new MatFileReader(stream);
            var matFile = matFileReader.Read();
            return matFile;
        }
    }
}