// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Factory providing the parsed contents of .mat files,
    /// which start at a non-8 byte-aligned offset in the stream.
    /// </summary>
    public class UnalignedMatTestDataFactory : MatTestDataFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialReadMatTestDataFactory"/> class.
        /// </summary>
        /// <param name="testDirectory">Directory containing test files.</param>
        public UnalignedMatTestDataFactory(string testDirectory)
            : base(testDirectory)
        {
        }

        /// <inheritdoc/>
        protected override IMatFile ReadDataFromStream(Stream stream)
        {
            using (var ms =  new MemoryStream())
            {
                ms.Seek(3, SeekOrigin.Begin);

                stream.CopyTo(ms);

                ms.Seek(3, SeekOrigin.Begin);

                return base.ReadDataFromStream(ms);
            }
        }
    }
}
