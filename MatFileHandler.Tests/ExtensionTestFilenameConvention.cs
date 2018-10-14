// Copyright 2017-2018 Alexander Luzgarev

using System.IO;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// A filename convention based on file extensions.
    /// </summary>
    internal class ExtensionTestFilenameConvention : ITestFilenameConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionTestFilenameConvention"/> class.
        /// </summary>
        /// <param name="extension">File extension.</param>
        public ExtensionTestFilenameConvention(string extension)
        {
            Extension = extension;
        }

        private string Extension { get; }

        /// <summary>
        /// Convert test name to filename by adding the extension.
        /// </summary>
        /// <param name="testName">Test name.</param>
        /// <returns>The corresponding filename.</returns>
        public string ConvertTestNameToFilename(string testName)
        {
            return Path.ChangeExtension(testName, Extension);
        }

        /// <summary>
        /// Compare file's extension to the one specified during initialization.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <returns>True iff the file has the extension stored in the class.</returns>
        public bool FilterFile(string filename)
        {
            return Path.GetExtension(filename) == "." + Extension;
        }
    }
}