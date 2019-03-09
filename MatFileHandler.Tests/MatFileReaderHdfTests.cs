﻿using NUnit.Framework;
using System.IO;

namespace MatFileHandler.Tests
{
    [TestFixture]
    public class MatFileReaderHdfTests
    {
        private const string TestDirectory = "test-data";

        /// <summary>
        /// Test reading an ASCII-encoded string.
        /// </summary>
        [Test]
        public void TestAscii()
        {
            var matFile = ReadHdfTestFile("ascii");
            var arrayAscii = matFile["s"].Value as ICharArray;
            Assert.That(arrayAscii, Is.Not.Null);
            Assert.That(arrayAscii.Dimensions, Is.EqualTo(new[] { 1, 3 }));
            Assert.That(arrayAscii.String, Is.EqualTo("abc"));
            Assert.That(arrayAscii[2], Is.EqualTo('c'));
        }

        /// <summary>
        /// Test reading a Unicode string.
        /// </summary>
        [Test]
        public void TestUnicode()
        {
            var matFile = ReadHdfTestFile("unicode");
            var arrayUnicode = matFile["s"].Value as ICharArray;
            Assert.That(arrayUnicode, Is.Not.Null);
            Assert.That(arrayUnicode.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(arrayUnicode.String, Is.EqualTo("必フ"));
            Assert.That(arrayUnicode[0], Is.EqualTo('必'));
            Assert.That(arrayUnicode[1], Is.EqualTo('フ'));
        }

        /// <summary>
        /// Test reading a wide Unicode string.
        /// </summary>
        [Test]
        public void TestUnicodeWide()
        {
            var matFile = ReadHdfTestFile("unicode-wide");
            var arrayUnicodeWide = matFile["s"].Value as ICharArray;
            Assert.That(arrayUnicodeWide, Is.Not.Null);
            Assert.That(arrayUnicodeWide.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(arrayUnicodeWide.String, Is.EqualTo("🍆"));
        }

        /// <summary>
        /// Test reading a two-dimensional double array.
        /// </summary>
        [Test]
        public void TestMatrix()
        {
            var matFile = ReadHdfTestFile("matrix");
            var matrix = matFile["matrix"].Value as IArrayOf<double>;
            Assert.That(matrix.Dimensions, Is.EqualTo(new[] { 3, 2 }));
            Assert.That(matrix.ConvertToDoubleArray(), Is.EqualTo(new[] { 1.0, 3.0, 5.0, 2.0, 4.0, 6.0 }));
            Assert.That(matrix[0, 0], Is.EqualTo(1.0));
            Assert.That(matrix[0, 1], Is.EqualTo(2.0));
            Assert.That(matrix[1, 0], Is.EqualTo(3.0));
            Assert.That(matrix[1, 1], Is.EqualTo(4.0));
            Assert.That(matrix[2, 0], Is.EqualTo(5.0));
            Assert.That(matrix[2, 1], Is.EqualTo(6.0));
        }

        /// <summary>
        /// Test reading lower and upper limits of integer data types.
        /// </summary>
        [Test]
        public void TestLimits()
        {
            var matFile = ReadHdfTestFile("limits");
            IArray array;
            array = matFile["int8_"].Value;
            CheckLimits(array as IArrayOf<sbyte>, CommonData.Int8Limits);
            Assert.That(array.ConvertToDoubleArray(), Is.EqualTo(new[] { -128.0, 127.0 }));

            array = matFile["uint8_"].Value;
            CheckLimits(array as IArrayOf<byte>, CommonData.UInt8Limits);

            array = matFile["int16_"].Value;
            CheckLimits(array as IArrayOf<short>, CommonData.Int16Limits);

            array = matFile["uint16_"].Value;
            CheckLimits(array as IArrayOf<ushort>, CommonData.UInt16Limits);

            array = matFile["int32_"].Value;
            CheckLimits(array as IArrayOf<int>, CommonData.Int32Limits);

            array = matFile["uint32_"].Value;
            CheckLimits(array as IArrayOf<uint>, CommonData.UInt32Limits);

            array = matFile["int64_"].Value;
            CheckLimits(array as IArrayOf<long>, CommonData.Int64Limits);

            array = matFile["uint64_"].Value;
            CheckLimits(array as IArrayOf<ulong>, CommonData.UInt64Limits);
        }

        private static void CheckLimits<T>(IArrayOf<T> array, T[] limits)
            where T : struct
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(array.Data, Is.EqualTo(limits));
        }

        private static AbstractTestDataFactory<IMatFile> GetTests(string factoryName) =>
            new MatTestDataFactory(Path.Combine(TestDirectory, factoryName));

        private IMatFile ReadHdfTestFile(string testName)
        {
            return GetTests("hdf")[testName];
        }
    }
}