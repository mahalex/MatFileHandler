// Copyright 2017 Alexander Luzgarev

using System;
using System.IO;
using System.Numerics;
using NUnit.Framework;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of file writing API.
    /// </summary>
    [TestFixture]
    public class MatFileWriterTests
    {
        private const string TestDirectory = "test-data";

        /// <summary>
        /// Test writing a simple Double array.
        /// </summary>
        [Test]
        public void TestWrite()
        {
            var builder = new DataBuilder();
            var array = builder.NewArray<double>(1, 2);
            array[0] = -13.5;
            array[1] = 17.0;
            var variable = builder.NewVariable("test", array);
            var actual = builder.NewFile(new[] { variable });
            MatCompareWithTestData("good", "double-array", actual);
        }

        /// <summary>
        /// Test writing a large file.
        /// </summary>
        [Test]
        public void TestHuge()
        {
            var builder = new DataBuilder();
            var array = builder.NewArray<double>(1000, 10000);
            array[0] = -13.5;
            array[1] = 17.0;
            var variable = builder.NewVariable("test", array);
            var matFile = builder.NewFile(new[] { variable });
            using (var stream = new MemoryStream())
            {
                var writer = new MatFileWriter(stream);
                writer.Write(matFile);
            }
        }

        /// <summary>
        /// Test writing lower and upper limits of integer data types.
        /// </summary>
        [Test]
        public void TestLimits()
        {
            var builder = new DataBuilder();
            var int8 = builder.NewVariable("int8_", builder.NewArray(CommonData.Int8Limits, 1, 2));
            var uint8 = builder.NewVariable("uint8_", builder.NewArray(CommonData.UInt8Limits, 1, 2));
            var int16 = builder.NewVariable("int16_", builder.NewArray(CommonData.Int16Limits, 1, 2));
            var uint16 = builder.NewVariable("uint16_", builder.NewArray(CommonData.UInt16Limits, 1, 2));
            var int32 = builder.NewVariable("int32_", builder.NewArray(CommonData.Int32Limits, 1, 2));
            var uint32 = builder.NewVariable("uint32_", builder.NewArray(CommonData.UInt32Limits, 1, 2));
            var int64 = builder.NewVariable("int64_", builder.NewArray(CommonData.Int64Limits, 1, 2));
            var uint64 = builder.NewVariable("uint64_", builder.NewArray(CommonData.UInt64Limits, 1, 2));
            var actual = builder.NewFile(new[] { int16, int32, int64, int8, uint16, uint32, uint64, uint8 });
            MatCompareWithTestData("good", "limits", actual);
        }

        /// <summary>
        /// Test writing lower and upper limits of integer-based complex data types.
        /// </summary>
        [Test]
        public void TestLimitsComplex()
        {
            var builder = new DataBuilder();
            var int8Complex = builder.NewVariable(
                "int8_complex",
                builder.NewArray(CreateComplexLimits(CommonData.Int8Limits), 1, 2));
            var uint8Complex = builder.NewVariable(
                "uint8_complex",
                builder.NewArray(CreateComplexLimits(CommonData.UInt8Limits), 1, 2));
            var int16Complex = builder.NewVariable(
                "int16_complex",
                builder.NewArray(CreateComplexLimits(CommonData.Int16Limits), 1, 2));
            var uint16Complex = builder.NewVariable(
                "uint16_complex",
                builder.NewArray(CreateComplexLimits(CommonData.UInt16Limits), 1, 2));
            var int32Complex = builder.NewVariable(
                "int32_complex",
                builder.NewArray(CreateComplexLimits(CommonData.Int32Limits), 1, 2));
            var uint32Complex = builder.NewVariable(
                "uint32_complex",
                builder.NewArray(CreateComplexLimits(CommonData.UInt32Limits), 1, 2));
            var int64Complex = builder.NewVariable(
                "int64_complex",
                builder.NewArray(CreateComplexLimits(CommonData.Int64Limits), 1, 2));
            var uint64Complex = builder.NewVariable(
                "uint64_complex",
                builder.NewArray(CreateComplexLimits(CommonData.UInt64Limits), 1, 2));
            var actual = builder.NewFile(new[]
            {
                int16Complex, int32Complex, int64Complex, int8Complex,
                uint16Complex, uint32Complex, uint64Complex, uint8Complex,
            });
            MatCompareWithTestData("good", "limits_complex", actual);
        }

        /// <summary>
        /// Test writing a wide-Unicode symbol.
        /// </summary>
        [Test]
        public void TestUnicodeWide()
        {
            var builder = new DataBuilder();
            var s = builder.NewVariable("s", builder.NewCharArray("🍆"));
            var actual = builder.NewFile(new[] { s });
            MatCompareWithTestData("good", "unicode-wide", actual);
        }

        /// <summary>
        /// Test writing a sparse array.
        /// </summary>
        [Test]
        public void TestSparseArray()
        {
            var builder = new DataBuilder();
            var sparseArray = builder.NewSparseArray<double>(4, 5);
            sparseArray[1, 1] = 1;
            sparseArray[1, 2] = 2;
            sparseArray[2, 1] = 3;
            sparseArray[2, 3] = 4;
            var sparse = builder.NewVariable("sparse_", sparseArray);
            var actual = builder.NewFile(new[] { sparse });
            MatCompareWithTestData("good", "sparse", actual);
        }

        /// <summary>
        /// Test writing a structure array.
        /// </summary>
        [Test]
        public void TestStructure()
        {
            var builder = new DataBuilder();
            var structure = builder.NewStructureArray(new[] { "x", "y" }, 2, 3);
            structure["x", 0, 0] = builder.NewArray(new[] { 12.345 }, 1, 1);
            structure["y", 0, 0] = builder.NewCharArray("abc");
            structure["x", 1, 0] = builder.NewCharArray("xyz");
            structure["y", 1, 0] = builder.NewEmpty();
            structure["x", 0, 1] = builder.NewArray(new[] { 2.0 }, 1, 1);
            structure["y", 0, 1] = builder.NewArray(new[] { 13.0 }, 1, 1);
            structure["x", 1, 1] = builder.NewEmpty();
            structure["y", 1, 1] = builder.NewCharArray("acbd", 2, 2);
            var cellArray = builder.NewCellArray(1, 2);
            cellArray[0] = builder.NewCharArray("x");
            cellArray[1] = builder.NewCharArray("yz");
            structure["x", 0, 2] = cellArray;
            structure["y", 0, 2] = builder.NewArray(new[] { 1.0, 4.0, 2.0, 5.0, 3.0, 6.0 }, 2, 3);
            structure["x", 1, 2] = builder.NewArray(new[] { 1.5f }, 1, 1);
            structure["y", 1, 2] = builder.NewEmpty();
            var struct_ = builder.NewVariable("struct_", structure);
            var actual = builder.NewFile(new[] { struct_ });
            MatCompareWithTestData("good", "struct", actual);
        }

        /// <summary>
        /// Test writing a logical array.
        /// </summary>
        [Test]
        public void TestLogical()
        {
            var builder = new DataBuilder();
            var logical = builder.NewArray(new[] { true, false, true, true, false, true }, 2, 3);
            var logicalVariable = builder.NewVariable("logical_", logical);
            var actual = builder.NewFile(new[] { logicalVariable });
            MatCompareWithTestData("good", "logical", actual);
        }

        /// <summary>
        /// Test writing a sparse logical array.
        /// </summary>
        [Test]
        public void TestSparseLogical()
        {
            var builder = new DataBuilder();
            var array = builder.NewSparseArray<bool>(2, 3);
            array[0, 0] = true;
            array[0, 1] = true;
            array[1, 1] = true;
            array[1, 2] = true;
            var sparseLogical = builder.NewVariable("sparse_logical", array);
            var actual = builder.NewFile(new[] { sparseLogical });
            MatCompareWithTestData("good", "sparse_logical", actual);
        }

        /// <summary>
        /// Test writing a sparse complex array.
        /// </summary>
        [Test]
        public void TestSparseComplex()
        {
            var builder = new DataBuilder();
            var array = builder.NewSparseArray<Complex>(2, 2);
            array[0, 0] = -1.5 + (2.5 * Complex.ImaginaryOne);
            array[1, 0] = 2 - (3 * Complex.ImaginaryOne);
            array[1, 1] = 0.5 + Complex.ImaginaryOne;
            var sparseComplex = builder.NewVariable("sparse_complex", array);
            var actual = builder.NewFile(new[] { sparseComplex });
            MatCompareWithTestData("good", "sparse_complex", actual);
        }

        /// <summary>
        /// Test writing a global variable.
        /// </summary>
        [Test]
        public void TestGlobal()
        {
            var builder = new DataBuilder();
            var array = builder.NewArray(new double[] { 1, 3, 5 }, 1, 3);
            var global = builder.NewVariable("global_", array, true);
            var actual = builder.NewFile(new[] { global });
            MatCompareWithTestData("good", "global", actual);
        }

        private static AbstractTestDataFactory<IMatFile> GetMatTestData(string factoryName) =>
            new MatTestDataFactory(Path.Combine(TestDirectory, factoryName));

        private void CompareSparseArrays<T>(ISparseArrayOf<T> expected, ISparseArrayOf<T> actual)
          where T : struct
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.Dimensions, Is.EqualTo(actual.Dimensions));
            Assert.That(expected.Data, Is.EquivalentTo(actual.Data));
        }

        private void CompareStructureArrays(IStructureArray expected, IStructureArray actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.Dimensions, Is.EqualTo(actual.Dimensions));
            Assert.That(expected.FieldNames, Is.EquivalentTo(actual.FieldNames));
            foreach (var name in expected.FieldNames)
            {
                for (var i = 0; i < expected.NumberOfElements; i++)
                {
                    CompareMatArrays(expected[name, i], actual[name, i]);
                }
            }
        }

        private void CompareCellArrays(ICellArray expected, ICellArray actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.Dimensions, Is.EqualTo(actual.Dimensions));
            for (var i = 0; i < expected.NumberOfElements; i++)
            {
                CompareMatArrays(expected[i], actual[i]);
            }
        }

        private void CompareNumericalArrays<T>(IArrayOf<T> expected, IArrayOf<T> actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.Dimensions, Is.EqualTo(actual.Dimensions));
            Assert.That(expected.Data, Is.EqualTo(actual.Data));
        }

        private void CompareCharArrays(ICharArray expected, ICharArray actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(expected.Dimensions, Is.EqualTo(actual.Dimensions));
            Assert.That(expected.String, Is.EqualTo(actual.String));
        }

        private void CompareMatArrays(IArray expected, IArray actual)
        {
            switch (expected)
            {
                case ISparseArrayOf<double> expectedSparseArrayOfDouble:
                    CompareSparseArrays(expectedSparseArrayOfDouble, actual as ISparseArrayOf<double>);
                    return;
                case ISparseArrayOf<Complex> expectedSparseArrayOfComplex:
                    CompareSparseArrays(expectedSparseArrayOfComplex, actual as ISparseArrayOf<Complex>);
                    return;
                case IStructureArray expectedStructureArray:
                    CompareStructureArrays(expectedStructureArray, actual as IStructureArray);
                    return;
                case ICharArray expectedCharArray:
                    CompareCharArrays(expectedCharArray, actual as ICharArray);
                    return;
                case IArrayOf<byte> byteArray:
                    CompareNumericalArrays(byteArray, actual as IArrayOf<byte>);
                    return;
                case IArrayOf<sbyte> sbyteArray:
                    CompareNumericalArrays(sbyteArray, actual as IArrayOf<sbyte>);
                    return;
                case IArrayOf<short> shortArray:
                    CompareNumericalArrays(shortArray, actual as IArrayOf<short>);
                    return;
                case IArrayOf<ushort> ushortArray:
                    CompareNumericalArrays(ushortArray, actual as IArrayOf<ushort>);
                    return;
                case IArrayOf<int> intArray:
                    CompareNumericalArrays(intArray, actual as IArrayOf<int>);
                    return;
                case IArrayOf<uint> uintArray:
                    CompareNumericalArrays(uintArray, actual as IArrayOf<uint>);
                    return;
                case IArrayOf<long> longArray:
                    CompareNumericalArrays(longArray, actual as IArrayOf<long>);
                    return;
                case IArrayOf<ulong> ulongArray:
                    CompareNumericalArrays(ulongArray, actual as IArrayOf<ulong>);
                    return;
                case IArrayOf<float> floatArray:
                    CompareNumericalArrays(floatArray, actual as IArrayOf<float>);
                    return;
                case IArrayOf<double> doubleArray:
                    CompareNumericalArrays(doubleArray, actual as IArrayOf<double>);
                    return;
                case IArrayOf<ComplexOf<byte>> byteArray:
                    CompareNumericalArrays(byteArray, actual as IArrayOf<ComplexOf<byte>>);
                    return;
                case IArrayOf<ComplexOf<sbyte>> sbyteArray:
                    CompareNumericalArrays(sbyteArray, actual as IArrayOf<ComplexOf<sbyte>>);
                    return;
                case IArrayOf<ComplexOf<short>> shortArray:
                    CompareNumericalArrays(shortArray, actual as IArrayOf<ComplexOf<short>>);
                    return;
                case IArrayOf<ComplexOf<ushort>> ushortArray:
                    CompareNumericalArrays(ushortArray, actual as IArrayOf<ComplexOf<ushort>>);
                    return;
                case IArrayOf<ComplexOf<int>> intArray:
                    CompareNumericalArrays(intArray, actual as IArrayOf<ComplexOf<int>>);
                    return;
                case IArrayOf<ComplexOf<uint>> uintArray:
                    CompareNumericalArrays(uintArray, actual as IArrayOf<ComplexOf<uint>>);
                    return;
                case IArrayOf<ComplexOf<long>> longArray:
                    CompareNumericalArrays(longArray, actual as IArrayOf<ComplexOf<long>>);
                    return;
                case IArrayOf<ComplexOf<ulong>> ulongArray:
                    CompareNumericalArrays(ulongArray, actual as IArrayOf<ComplexOf<ulong>>);
                    return;
                case IArrayOf<ComplexOf<float>> floatArray:
                    CompareNumericalArrays(floatArray, actual as IArrayOf<ComplexOf<float>>);
                    return;
                case IArrayOf<Complex> doubleArray:
                    CompareNumericalArrays(doubleArray, actual as IArrayOf<Complex>);
                    return;
                case IArrayOf<bool> boolArray:
                    CompareNumericalArrays(boolArray, actual as IArrayOf<bool>);
                    return;
                case ICellArray cellArray:
                    CompareCellArrays(cellArray, actual as ICellArray);
                    return;
            }
            if (expected.IsEmpty)
            {
                Assert.That(actual.IsEmpty, Is.True);
                return;
            }
            throw new NotSupportedException();
        }

        private void CompareMatFiles(IMatFile expected, IMatFile actual)
        {
            Assert.That(expected.Variables.Length, Is.EqualTo(actual.Variables.Length));
            for (var i = 0; i < expected.Variables.Length; i++)
            {
                var expectedVariable = expected.Variables[i];
                var actualVariable = actual.Variables[i];
                Assert.That(expectedVariable.Name, Is.EqualTo(actualVariable.Name));
                Assert.That(expectedVariable.IsGlobal, Is.EqualTo(actualVariable.IsGlobal));
                CompareMatArrays(expectedVariable.Value, actualVariable.Value);
            }
        }

        private void MatCompareWithTestData(string factoryName, string testName, IMatFile actual)
        {
            var expected = GetMatTestData(factoryName)[testName];
            byte[] buffer;
            using (var stream = new MemoryStream())
            {
                var writer = new MatFileWriter(stream);
                writer.Write(actual);
                buffer = stream.ToArray();
            }
            using (var stream = new MemoryStream(buffer))
            {
                var reader = new MatFileReader(stream);
                var actualRead = reader.Read();
                CompareMatFiles(expected, actualRead);
            }
        }

        private ComplexOf<T>[] CreateComplexLimits<T>(T[] limits)
          where T : struct
        {
            return new[] { new ComplexOf<T>(limits[0], limits[1]), new ComplexOf<T>(limits[1], limits[0]) };
        }
    }
}