// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of file reading API.
    /// </summary>
    public class MatFileReaderTests
    {
        private const string TestDirectory = "test-data";

        /// <summary>
        /// Test reading all files in a given test set.
        /// </summary>
        /// <param name="testSet">Name of the set.</param>
        [Theory]
        [InlineData("good")]
        public void TestReader(string testSet)
        {
            foreach (var matFile in GetTests(testSet).GetAllTestData())
            {
                Assert.NotEmpty(matFile.Variables);
            }
        }

        /// <summary>
        /// Test reading lower and upper limits of integer data types.
        /// </summary>
        [Fact]
        public void TestLimits()
        {
            var matFile = GetTests("good")["limits"];
            IArray array;
            array = matFile["int8_"].Value;
            CheckLimits(array as IArrayOf<sbyte>, CommonData.Int8Limits);
            Assert.Equal(new[] { -128.0, 127.0 }, array.ConvertToDoubleArray());

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

        /// <summary>
        /// Test writing lower and upper limits of integer-based complex data types.
        /// </summary>
        [Fact]
        public void TestComplexLimits()
        {
            var matFile = GetTests("good")["limits_complex"];
            IArray array;
            array = matFile["int8_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<sbyte>>, CommonData.Int8Limits);
            Assert.Equal(
                new[] { -128.0 + (127.0 * Complex.ImaginaryOne), 127.0 - (128.0 * Complex.ImaginaryOne) },
                array.ConvertToComplexArray());

            array = matFile["uint8_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<byte>>, CommonData.UInt8Limits);

            array = matFile["int16_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<short>>, CommonData.Int16Limits);

            array = matFile["uint16_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<ushort>>, CommonData.UInt16Limits);

            array = matFile["int32_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<int>>, CommonData.Int32Limits);

            array = matFile["uint32_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<uint>>, CommonData.UInt32Limits);

            array = matFile["int64_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<long>>, CommonData.Int64Limits);

            array = matFile["uint64_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<ulong>>, CommonData.UInt64Limits);
        }

        /// <summary>
        /// Test reading an ASCII-encoded string.
        /// </summary>
        [Fact]
        public void TestAscii()
        {
            var matFile = GetTests("good")["ascii"];
            var arrayAscii = matFile["s"].Value as ICharArray;
            Assert.NotNull(arrayAscii);
            Assert.Equal(new[] { 1, 3 }, arrayAscii.Dimensions);
            Assert.Equal("abc", arrayAscii.String);
            Assert.Equal('c', arrayAscii[2]);
        }

        /// <summary>
        /// Test reading a Unicode string.
        /// </summary>
        [Fact]
        public void TestUnicode()
        {
            var matFile = GetTests("good")["unicode"];
            var arrayUnicode = matFile["s"].Value as ICharArray;
            Assert.NotNull(arrayUnicode);
            Assert.Equal(new[] { 1, 2 }, arrayUnicode.Dimensions);
            Assert.Equal("必フ", arrayUnicode.String);
            Assert.Equal('必', arrayUnicode[0]);
            Assert.Equal('フ', arrayUnicode[1]);
        }

        /// <summary>
        /// Test reading a wide Unicode string.
        /// </summary>
        [Fact]
        public void TestUnicodeWide()
        {
            var matFile = GetTests("good")["unicode-wide"];
            var arrayUnicodeWide = matFile["s"].Value as ICharArray;
            Assert.NotNull(arrayUnicodeWide);
            Assert.Equal(new[] { 1, 2 }, arrayUnicodeWide.Dimensions);
            Assert.Equal("🍆", arrayUnicodeWide.String);
        }

        /// <summary>
        /// Test converting a structure array to a Double array.
        /// </summary>
        [Fact]
        public void TestConvertToDoubleArray()
        {
            var matFile = GetTests("good")["struct"];
            var array = matFile.Variables[0].Value;
            Assert.Null(array.ConvertToDoubleArray());
        }

        /// <summary>
        /// Test converting a structure array to a Complex array.
        /// </summary>
        /// <returns>Should return null.</returns>
        [Fact]
        public void TestConvertToComplexArray()
        {
            var matFile = GetTests("good")["struct"];
            var array = matFile.Variables[0].Value;
            Assert.Null(array.ConvertToComplexArray());
        }

        /// <summary>
        /// Test reading a structure array.
        /// </summary>
        [Fact]
        public void TestStruct()
        {
            var matFile = GetTests("good")["struct"];
            var structure = matFile["struct_"].Value as IStructureArray;
            Assert.NotNull(structure);
            Assert.Equal(new[] { "x", "y" }, structure.FieldNames);
            var element = structure[0, 0];
            Assert.True(element.ContainsKey("x"));
            Assert.Equal(2, element.Count);
            Assert.True(element.TryGetValue("x", out var _));
            Assert.False(element.TryGetValue("z", out var _));
            Assert.Equal(2, element.Keys.Count());
            Assert.Equal(2, element.Values.Count());
            var keys = element.Select(pair => pair.Key);
            Assert.Equal(new[] { "x", "y" }, keys);

            Assert.Equal(12.345, (element["x"] as IArrayOf<double>)?[0]);

            Assert.Equal(12.345, (structure["x", 0, 0] as IArrayOf<double>)?[0]);
            Assert.Equal(2.0, (structure["x", 0, 1] as IArrayOf<double>)?[0]);
            Assert.Equal("x", ((structure["x", 0, 2] as ICellArray)?[0] as ICharArray)?.String);
            Assert.Equal("yz", ((structure["x", 0, 2] as ICellArray)?[1] as ICharArray)?.String);
            Assert.Equal("xyz", (structure["x", 1, 0] as ICharArray)?.String);
            Assert.True(structure["x", 1, 1].IsEmpty);
            Assert.Equal(1.5f, (structure["x", 1, 2] as IArrayOf<float>)?[0]);

            Assert.Equal("abc", (structure["y", 0, 0] as ICharArray)?.String);
            Assert.Equal(13.0, (structure["y", 0, 1] as IArrayOf<double>)?[0]);
            Assert.Equal(new[] { 2, 3 }, (structure["y", 0, 2] as IArrayOf<double>)?.Dimensions);
            Assert.Equal(3.0, (structure["y", 0, 2] as IArrayOf<double>)?[0, 2]);
            Assert.True(structure["y", 1, 0].IsEmpty);
            Assert.Equal('a', (structure["y", 1, 1] as ICharArray)?[0, 0]);
            Assert.True(structure["y", 1, 2].IsEmpty);

            Assert.Equal(
                new double[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                },
                structure["y", 0, 2].ConvertTo2dDoubleArray());
            Assert.Equal(
                new double[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 },
                },
                structure["y", 0, 2].ConvertToMultidimensionalDoubleArray());
        }

        /// <summary>
        /// Test reading a sparse array.
        /// </summary>
        [Fact]
        public void TestSparse()
        {
            var matFile = GetTests("good")["sparse"];
            var sparseArray = matFile["sparse_"].Value as ISparseArrayOf<double>;
            Assert.NotNull(sparseArray);
            Assert.Equal(new[] { 4, 5 }, sparseArray.Dimensions);
            Assert.Equal(1.0, sparseArray.Data[(1, 1)]);
            Assert.Equal(1.0, sparseArray[1, 1]);
            Assert.Equal(2.0, sparseArray[1, 2]);
            Assert.Equal(3.0, sparseArray[2, 1]);
            Assert.Equal(4.0, sparseArray[2, 3]);
            Assert.Equal(0.0, sparseArray[0, 4]);
            Assert.Equal(0.0, sparseArray[3, 0]);
            Assert.Equal(0.0, sparseArray[3, 4]);

            Assert.Equal(
                new double[,]
                {
                    { 0, 0, 0, 0, 0 },
                    { 0, 1, 2, 0, 0 },
                    { 0, 3, 0, 4, 0 },
                    { 0, 0, 0, 0, 0 },
                },
                sparseArray.ConvertTo2dDoubleArray());
        }

        /// <summary>
        /// Test reading a logical array.
        /// </summary>
        [Fact]
        public void TestLogical()
        {
            var matFile = GetTests("good")["logical"];
            var array = matFile["logical_"].Value;
            var logicalArray = array as IArrayOf<bool>;
            Assert.NotNull(logicalArray);
            Assert.True(logicalArray[0, 0]);
            Assert.True(logicalArray[0, 1]);
            Assert.False(logicalArray[0, 2]);
            Assert.False(logicalArray[1, 0]);
            Assert.True(logicalArray[1, 1]);
            Assert.True(logicalArray[1, 2]);
        }

        /// <summary>
        /// Test reading a sparse logical array.
        /// </summary>
        [Fact]
        public void TestSparseLogical()
        {
            var matFile = GetTests("good")["sparse_logical"];
            var array = matFile["sparse_logical"].Value;
            var sparseArray = array as ISparseArrayOf<bool>;
            Assert.NotNull (sparseArray);
            Assert.True(sparseArray.Data[(0, 0)]);
            Assert.True(sparseArray[0, 0]);
            Assert.True(sparseArray[0, 1]);
            Assert.False(sparseArray[0, 2]);
            Assert.False(sparseArray[1, 0]);
            Assert.True(sparseArray[1, 1]);
            Assert.True(sparseArray[1, 2]);
        }

        /// <summary>
        /// Test reading a global variable.
        /// </summary>
        [Fact]
        public void TestGlobal()
        {
            var matFile = GetTests("good")["global"];
            var variable = matFile.Variables.First();
            Assert.True(variable.IsGlobal);
        }

        /// <summary>
        /// Test reading a sparse complex array.
        /// </summary>
        [Fact]
        public void TextSparseComplex()
        {
            var matFile = GetTests("good")["sparse_complex"];
            var array = matFile["sparse_complex"].Value;
            var sparseArray = array as ISparseArrayOf<Complex>;
            Assert.NotNull(sparseArray);
            Assert.Equal(-1.5 + (2.5 * Complex.ImaginaryOne), sparseArray[0, 0]);
            Assert.Equal(2 - (3 * Complex.ImaginaryOne), sparseArray[1, 0]);
            Assert.Equal(Complex.Zero, sparseArray[0, 1]);
            Assert.Equal(0.5 + (1.0 * Complex.ImaginaryOne), sparseArray[1, 1]);
        }

        /// <summary>
        /// Test reading an object.
        /// </summary>
        [Fact]
        public void TestObject()
        {
            var matFile = GetTests("good")["object"];
            var obj = matFile["object_"].Value as IMatObject;
            Assert.NotNull(obj);
            Assert.Equal("Point", obj.ClassName);
            Assert.Equal(new[] { "x", "y" }, obj.FieldNames);
            Assert.Equal(new[] { 3.0 }, obj["x", 0].ConvertToDoubleArray());
            Assert.Equal(new[] { 5.0 }, obj["y", 0].ConvertToDoubleArray());
            Assert.Equal(new[] { -2.0 }, obj["x", 1].ConvertToDoubleArray());
            Assert.Equal(new[] { 6.0 }, obj["y", 1].ConvertToDoubleArray());
        }

        /// <summary>
        /// Test reading another object.
        /// </summary>
        [Fact]
        public void TestObject2()
        {
            var matFile = GetTests("good")["object2"];
            var obj = matFile["object2"].Value as IMatObject;
            Assert.NotNull(obj);
            Assert.Equal("Point", obj.ClassName);
            Assert.Equal(new[] { "x", "y" }, obj.FieldNames);
            Assert.Equal(new[] { 3.0 }, obj["x", 0, 0].ConvertToDoubleArray());
            Assert.Equal(new[] { -2.0 }, obj["x", 0, 1].ConvertToDoubleArray());
            Assert.Equal(new[] { 1.0 }, obj["x", 1, 0].ConvertToDoubleArray());
            Assert.Equal(new[] { 0.0 }, obj["x", 1, 1].ConvertToDoubleArray());
            Assert.Equal(new[] { 5.0 }, obj["y", 0, 0].ConvertToDoubleArray());
            Assert.Equal(new[] { 6.0 }, obj["y", 0, 1].ConvertToDoubleArray());
            Assert.Equal(new[] { 0.0 }, obj["y", 1, 0].ConvertToDoubleArray());
            Assert.Equal(new[] { 1.0 }, obj["y", 1, 1].ConvertToDoubleArray());
            Assert.Equal(new[] { -2.0 }, obj[0, 1]["x"].ConvertToDoubleArray());
            Assert.Equal(new[] { -2.0 }, obj[2]["x"].ConvertToDoubleArray());
        }

        /// <summary>
        /// Test reading a table.
        /// </summary>
        [Fact]
        public void TestTable()
        {
            var matFile = GetTests("good")["table"];
            var obj = matFile["table_"].Value as IMatObject;
            var table = new TableAdapter(obj);
            Assert.Equal(3, table.NumberOfRows);
            Assert.Equal(2, table.NumberOfVariables);
            Assert.Equal("Some table", table.Description);
            Assert.Equal(new[] { "variable1", "variable2" }, table.VariableNames);
            var variable1 = table["variable1"] as ICellArray;
            Assert.Equal("First row", (variable1[0] as ICharArray).String);
            Assert.Equal("Second row", (variable1[1] as ICharArray).String);
            Assert.Equal("Third row", (variable1[2] as ICharArray).String);
            var variable2 = table["variable2"];
            Assert.Equal(new[] { 1.0, 3.0, 5.0, 2.0, 4.0, 6.0 }, variable2.ConvertToDoubleArray());
        }

        /// <summary>
        /// Test subobjects within objects.
        /// </summary>
        [Fact]
        public void TestSubobjects()
        {
            var matFile = GetTests("good")["pointWithSubpoints"];
            var p = matFile["p"].Value as IMatObject;
            Assert.Equal("Point", p.ClassName);
            var x = p["x"] as IMatObject;
            Assert.Equal("SubPoint", x.ClassName);
            Assert.Equal(new[] { "a", "b", "c" }, x.FieldNames);
            var y = p["y"] as IMatObject;
            Assert.Equal("SubPoint", y.ClassName);
            Assert.Equal(new[] { "a", "b", "c" }, y.FieldNames);
            Assert.Equal(new[] { 1.0 }, x["a"].ConvertToDoubleArray());
            Assert.Equal(new[] { 2.0 }, x["b"].ConvertToDoubleArray());
            Assert.Equal(new[] { 3.0 }, x["c"].ConvertToDoubleArray());
            Assert.Equal(new[] { 14.0 }, y["a"].ConvertToDoubleArray());
            Assert.Equal(new[] { 15.0 }, y["b"].ConvertToDoubleArray());
            Assert.Equal(new[] { 16.0 }, y["c"].ConvertToDoubleArray());
        }

        /// <summary>
        /// Test nested objects.
        /// </summary>
        [Fact]
        public void TestNestedObjects()
        {
            var matFile = GetTests("good")["subsubPoint"];
            var p = matFile["p"].Value as IMatObject;
            Assert.Equal("Point", p.ClassName);
            Assert.Equal(new[] { 1.0 }, p["x"].ConvertToDoubleArray());
            var pp = p["y"] as IMatObject;
            Assert.True(pp.ClassName == "Point");
            Assert.Equal(new[] { 10.0 }, pp["x"].ConvertToDoubleArray());
            var ppp = pp["y"] as IMatObject;
            Assert.Equal(new[] { 100.0 }, ppp["x"].ConvertToDoubleArray());
            Assert.Equal(new[] { 200.0 }, ppp["y"].ConvertToDoubleArray());
        }

        /// <summary>
        /// Test datetime objects.
        /// </summary>
        [Fact]
        public void TestDatetime()
        {
            var matFile = GetTests("good")["datetime"];
            var d = matFile["d"].Value as IMatObject;
            var datetime = new DatetimeAdapter(d);
            Assert.Equal(new[] { 1, 2 }, datetime.Dimensions);
            Assert.Equal(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero), datetime[0]);
            Assert.Equal(new DateTimeOffset(1987, 1, 2, 3, 4, 5, TimeSpan.Zero), datetime[1]);
        }

        /// <summary>
        /// Another test for datetime objects.
        /// </summary>
        [Fact]
        public void TestDatetime2()
        {
            var matFile = GetTests("good")["datetime2"];
            var d = matFile["d"].Value as IMatObject;
            var datetime = new DatetimeAdapter(d);
            Assert.Equal(new[] { 1, 1 }, datetime.Dimensions);
            var diff = new DateTimeOffset(2, 1, 1, 1, 1, 1, 235, TimeSpan.Zero);
            Assert.True(datetime[0] - diff < TimeSpan.FromMilliseconds(1));
            Assert.True(diff - datetime[0] < TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// Test string objects.
        /// </summary>
        [Fact]
        public void TestString()
        {
            var matFile = GetTests("good")["string"];
            var s = matFile["s"].Value as IMatObject;
            var str = new StringAdapter(s);
            Assert.Equal(new[] { 4, 1 }, str.Dimensions);
            Assert.Equal("abc", str[0]);
            Assert.Equal("defgh", str[1]);
            Assert.Equal("абвгд", str[2]);
            Assert.Equal("æøå", str[3]);
        }

        /// <summary>
        /// Test duration objects.
        /// </summary>
        [Fact]
        public void TestDuration()
        {
            var matFile = GetTests("good")["duration"];
            var d = matFile["d"].Value as IMatObject;
            var duration = new DurationAdapter(d);
            Assert.Equal(new[] { 1, 3 }, duration.Dimensions);
            Assert.Equal(TimeSpan.FromTicks(12345678L), duration[0]);
            Assert.Equal(new TimeSpan(0, 2, 4), duration[1]);
            Assert.Equal(new TimeSpan(1, 3, 5), duration[2]);
        }

        /// <summary>
        /// Test unrepresentable datetime.
        /// </summary>
        [Fact]
        public void TestDatetime_Unrepresentable()
        {
            var matFile = GetTests("good")["datetime-unrepresentable"];
            var obj = matFile["d"].Value as IMatObject;
            var datetime = new DatetimeAdapter(obj);
            var d0 = datetime[0];
            Assert.Null(d0);
        }

        [Fact]
        public void Test_3DArrays()
        {
            var matFile = GetTests("good")["issue20.mat"];
            var obj = matFile["a3d"].Value;
            var values = obj.ConvertToDoubleArray();
            Assert.Equal(Enumerable.Range(1, 24).Select(x => (double)x).ToArray(), values);
            var expected = new double[3, 4, 2]
            {
                {
                    { 1, 13 },
                    { 4, 16 },
                    { 7, 19 },
                    { 10, 22 },
                },
                {
                    { 2, 14 },
                    { 5, 17 },
                    { 8, 20 },
                    { 11, 23 },
                },
                {
                    { 3, 15 },
                    { 6, 18 },
                    { 9, 21 },
                    { 12, 24 },
                },
            };
            Assert.Equal(expected, obj.ConvertToMultidimensionalDoubleArray());
            Assert.Null(obj.ConvertTo2dDoubleArray());
        }

        [Fact]
        public void Test_4DArrays()
        {
            var matFile = GetTests("good")["issue20.mat"];
            var obj = matFile["a4d"].Value;
            Assert.Equal(Enumerable.Range(1, 120).Select(x => (double)x).ToArray(), obj.ConvertToDoubleArray());
            Assert.Null(obj.ConvertTo2dDoubleArray());
        }

        private static AbstractTestDataFactory<IMatFile> GetTests(string factoryName) =>
            new PartialReadMatTestDataFactory(Path.Combine(TestDirectory, factoryName));

        private static void CheckLimits<T>(IArrayOf<T> array, T[] limits)
            where T : struct
        {
            Assert.NotNull(array);
            Assert.Equal(new[] { 1, 2 }, array.Dimensions);
            Assert.Equal(limits, array.Data);
        }

        private static void CheckComplexLimits<T>(IArrayOf<ComplexOf<T>> array, T[] limits)
            where T : struct
        {
            Assert.NotNull(array);
            Assert.Equal(new[] { 1, 2 }, array.Dimensions);
            Assert.Equal(new ComplexOf<T>(limits[0], limits[1]), array[0]);
            Assert.Equal(new ComplexOf<T>(limits[1], limits[0]), array[1]);
        }
    }
}