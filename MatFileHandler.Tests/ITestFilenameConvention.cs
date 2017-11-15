// Copyright 2017 Alexander Luzgarev

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Interface for handling filtering tests based on filenames.
    /// </summary>
    public interface ITestFilenameConvention
    {
        /// <summary>
        /// Convert test name to a filename (e.g., by adding an appropriate extension).
        /// </summary>
        /// <param name="testName">Name of a test.</param>
        /// <returns>Filename.</returns>
        string ConvertTestNameToFilename(string testName);

        /// <summary>
        /// Decide if a file contains a test based on the filename.
        /// </summary>
        /// <param name="filename">A filename.</param>
        /// <returns>True iff the file should contain a test.</returns>
        bool FilterFile(string filename);
    }
}