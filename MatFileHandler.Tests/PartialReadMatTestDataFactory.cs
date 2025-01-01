// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Factory providing the parsed contents of .mat files,
    /// wrapped in a <see cref="PartialReadStream"/>.
    /// </summary>
    public class PartialReadMatTestDataFactory : MatTestDataFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialReadMatTestDataFactory"/> class.
        /// </summary>
        /// <param name="testDirectory">Directory containing test files.</param>
        public PartialReadMatTestDataFactory(string testDirectory)
            : base(testDirectory)
        {
        }

        /// <summary>
        /// Read and parse data from a .mat file.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>Parsed contents of the file.</returns>
        protected override IMatFile ReadDataFromStream(Stream stream)
        {
            using (var wrapper = new PartialReadStream(stream))
            {
                return base.ReadDataFromStream(wrapper);
            }
        }
    }
}
