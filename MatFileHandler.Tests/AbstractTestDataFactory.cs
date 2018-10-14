// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Abstract factory of test data.
    /// </summary>
    /// <typeparam name="TTestData">Type of test data.</typeparam>
    public abstract class AbstractTestDataFactory<TTestData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTestDataFactory{TTestData}"/> class.
        /// </summary>
        /// <param name="dataDirectory">Directory with test files.</param>
        /// <param name="testFilenameConvention">A convention used to filter test files.</param>
        protected AbstractTestDataFactory(string dataDirectory, ITestFilenameConvention testFilenameConvention)
        {
            DataDirectory = dataDirectory;
            TestFilenameConvention = testFilenameConvention;
        }

        private string DataDirectory { get; }

        private ITestFilenameConvention TestFilenameConvention { get; }

        /// <summary>
        /// Get test data set by name.
        /// </summary>
        /// <param name="dataSet">Name of the data set.</param>
        /// <returns>Test data.</returns>
        public TTestData this[string dataSet] =>
            ReadTestData(FixPath(TestFilenameConvention.ConvertTestNameToFilename(dataSet)));

        /// <summary>
        /// Get a sequence of all test data sets in the factory.
        /// </summary>
        /// <returns>A sequence of data sets.</returns>
        public IEnumerable<TTestData> GetAllTestData()
        {
            var files = Directory.EnumerateFiles(DataDirectory).Where(TestFilenameConvention.FilterFile);
            foreach (var filename in files)
            {
                yield return ReadTestData(filename);
            }
        }

        /// <summary>
        /// Read test data from a stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>Test data.</returns>
        protected abstract TTestData ReadDataFromStream(Stream stream);

        private string FixPath(string filename) => Path.Combine(DataDirectory, filename);

        private TTestData ReadTestData(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                return ReadDataFromStream(stream);
            }
        }
    }
}