// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of file reading API.
    /// </summary>
    [TestFixture]
    public class MatFileReaderTests
    {
        private const string TestDirectory = "test-data";

        /// <summary>
        /// Test reading all files in a given test set.
        /// </summary>
        /// <param name="testSet">Name of the set.</param>
        [TestCase("good")]
        public void TestReader(string testSet)
        {
            foreach (var matFile in GetTests(testSet).GetAllTestData())
            {
                Assert.That(matFile.Variables, Is.Not.Empty);
            }
        }

        /// <summary>
        /// Test reading lower and upper limits of integer data types.
        /// </summary>
        [Test]
        public void TestLimits()
        {
            var matFile = GetTests("good")["limits"];
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

        /// <summary>
        /// Test writing lower and upper limits of integer-based complex data types.
        /// </summary>
        [Test]
        public void TestComplexLimits()
        {
            var matFile = GetTests("good")["limits_complex"];
            IArray array;
            array = matFile["int8_complex"].Value;
            CheckComplexLimits(array as IArrayOf<ComplexOf<sbyte>>, CommonData.Int8Limits);
            Assert.That(
                array.ConvertToComplexArray(),
                Is.EqualTo(new[] { -128.0 + (127.0 * Complex.ImaginaryOne), 127.0 - (128.0 * Complex.ImaginaryOne) }));

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
        [Test]
        public void TestAscii()
        {
            var matFile = GetTests("good")["ascii"];
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
            var matFile = GetTests("good")["unicode"];
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
            var matFile = GetTests("good")["unicode-wide"];
            var arrayUnicodeWide = matFile["s"].Value as ICharArray;
            Assert.That(arrayUnicodeWide, Is.Not.Null);
            Assert.That(arrayUnicodeWide.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(arrayUnicodeWide.String, Is.EqualTo("🍆"));
        }

        /// <summary>
        /// Test converting a structure array to a Double array.
        /// </summary>
        /// <returns>Should return null.</returns>
        [Test(ExpectedResult = null)]
        public double[] TestConvertToDoubleArray()
        {
            var matFile = GetTests("good")["struct"];
            var array = matFile.Variables[0].Value;
            return array.ConvertToDoubleArray();
        }

        /// <summary>
        /// Test converting a structure array to a Complex array.
        /// </summary>
        /// <returns>Should return null.</returns>
        [Test(ExpectedResult = null)]
        public Complex[] TestConvertToComplexArray()
        {
            var matFile = GetTests("good")["struct"];
            var array = matFile.Variables[0].Value;
            return array.ConvertToComplexArray();
        }

        /// <summary>
        /// Test reading a structure array.
        /// </summary>
        [Test]
        public void TestStruct()
        {
            var matFile = GetTests("good")["struct"];
            var structure = matFile["struct_"].Value as IStructureArray;
            Assert.That(structure, Is.Not.Null);
            Assert.That(structure.FieldNames, Is.EquivalentTo(new[] { "x", "y" }));
            var element = structure[0, 0];
            Assert.That(element.ContainsKey("x"), Is.True);
            Assert.That(element.Count, Is.EqualTo(2));
            Assert.That(element.TryGetValue("x", out var _), Is.True);
            Assert.That(element.TryGetValue("z", out var _), Is.False);
            Assert.That(element.Keys, Has.Exactly(2).Items);
            Assert.That(element.Values, Has.Exactly(2).Items);
            var keys = element.Select(pair => pair.Key);
            Assert.That(keys, Is.EquivalentTo(new[] { "x", "y" }));

            Assert.That((element["x"] as IArrayOf<double>)?[0], Is.EqualTo(12.345));

            Assert.That((structure["x", 0, 0] as IArrayOf<double>)?[0], Is.EqualTo(12.345));
            Assert.That((structure["y", 0, 0] as ICharArray)?.String, Is.EqualTo("abc"));
            Assert.That((structure["x", 1, 0] as ICharArray)?.String, Is.EqualTo("xyz"));
            Assert.That(structure["y", 1, 0].IsEmpty, Is.True);
            Assert.That((structure["x", 0, 1] as IArrayOf<double>)?[0], Is.EqualTo(2.0));
            Assert.That((structure["y", 0, 1] as IArrayOf<double>)?[0], Is.EqualTo(13.0));
            Assert.That(structure["x", 1, 1].IsEmpty, Is.True);
            Assert.That((structure["y", 1, 1] as ICharArray)?[0, 0], Is.EqualTo('a'));
            Assert.That(((structure["x", 0, 2] as ICellArray)?[0] as ICharArray)?.String, Is.EqualTo("x"));
            Assert.That(((structure["x", 0, 2] as ICellArray)?[1] as ICharArray)?.String, Is.EqualTo("yz"));
            Assert.That((structure["y", 0, 2] as IArrayOf<double>)?.Dimensions, Is.EqualTo(new[] { 2, 3 }));
            Assert.That((structure["y", 0, 2] as IArrayOf<double>)?[0, 2], Is.EqualTo(3.0));
            Assert.That((structure["x", 1, 2] as IArrayOf<float>)?[0], Is.EqualTo(1.5f));
            Assert.That(structure["y", 1, 2].IsEmpty, Is.True);
        }

        /// <summary>
        /// Test reading a sparse array.
        /// </summary>
        [Test]
        public void TestSparse()
        {
            var matFile = GetTests("good")["sparse"];
            var sparseArray = matFile["sparse_"].Value as ISparseArrayOf<double>;
            Assert.That(sparseArray, Is.Not.Null);
            Assert.That(sparseArray.Dimensions, Is.EqualTo(new[] { 4, 5 }));
            Assert.That(sparseArray.Data[(1, 1)], Is.EqualTo(1.0));
            Assert.That(sparseArray[1, 1], Is.EqualTo(1.0));
            Assert.That(sparseArray[1, 2], Is.EqualTo(2.0));
            Assert.That(sparseArray[2, 1], Is.EqualTo(3.0));
            Assert.That(sparseArray[2, 3], Is.EqualTo(4.0));
            Assert.That(sparseArray[0, 4], Is.EqualTo(0.0));
            Assert.That(sparseArray[3, 0], Is.EqualTo(0.0));
            Assert.That(sparseArray[3, 4], Is.EqualTo(0.0));
        }

        /// <summary>
        /// Test reading a logical array.
        /// </summary>
        [Test]
        public void TestLogical()
        {
            var matFile = GetTests("good")["logical"];
            var array = matFile["logical_"].Value;
            var logicalArray = array as IArrayOf<bool>;
            Assert.That(logicalArray, Is.Not.Null);
            Assert.That(logicalArray[0, 0], Is.True);
            Assert.That(logicalArray[0, 1], Is.True);
            Assert.That(logicalArray[0, 2], Is.False);
            Assert.That(logicalArray[1, 0], Is.False);
            Assert.That(logicalArray[1, 1], Is.True);
            Assert.That(logicalArray[1, 2], Is.True);
        }

        /// <summary>
        /// Test reading a sparse logical array.
        /// </summary>
        [Test]
        public void TestSparseLogical()
        {
            var matFile = GetTests("good")["sparse_logical"];
            var array = matFile["sparse_logical"].Value;
            var sparseArray = array as ISparseArrayOf<bool>;
            Assert.That(sparseArray, Is.Not.Null);
            Assert.That(sparseArray.Data[(0, 0)], Is.True);
            Assert.That(sparseArray[0, 0], Is.True);
            Assert.That(sparseArray[0, 1], Is.True);
            Assert.That(sparseArray[0, 2], Is.False);
            Assert.That(sparseArray[1, 0], Is.False);
            Assert.That(sparseArray[1, 1], Is.True);
            Assert.That(sparseArray[1, 2], Is.True);
        }

        /// <summary>
        /// Test reading a global variable.
        /// </summary>
        [Test]
        public void TestGlobal()
        {
            var matFile = GetTests("good")["global"];
            var variable = matFile.Variables.First();
            Assert.That(variable.IsGlobal, Is.True);
        }

        /// <summary>
        /// Test reading a sparse complex array.
        /// </summary>
        [Test]
        public void TextSparseComplex()
        {
            var matFile = GetTests("good")["sparse_complex"];
            var array = matFile["sparse_complex"].Value;
            var sparseArray = array as ISparseArrayOf<Complex>;
            Assert.That(sparseArray, Is.Not.Null);
            Assert.That(sparseArray[0, 0], Is.EqualTo(-1.5 + (2.5 * Complex.ImaginaryOne)));
            Assert.That(sparseArray[1, 0], Is.EqualTo(2 - (3 * Complex.ImaginaryOne)));
            Assert.That(sparseArray[0, 1], Is.EqualTo(Complex.Zero));
            Assert.That(sparseArray[1, 1], Is.EqualTo(0.5 + (1.0 * Complex.ImaginaryOne)));
        }

        /// <summary>
        /// Test reading an object.
        /// </summary>
        [Test]
        public void TestObject()
        {
            var matFile = GetTests("good")["object"];
            var obj = matFile["object_"].Value as IMatObject;
            Assert.IsNotNull(obj);
            Assert.That(obj.ClassName, Is.EqualTo("Point"));
            Assert.That(obj.FieldNames, Is.EquivalentTo(new[] { "x", "y" }));
            Assert.That(obj["x", 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 3.0 }));
            Assert.That(obj["y", 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 5.0 }));
            Assert.That(obj["x", 1].ConvertToDoubleArray(), Is.EqualTo(new[] { -2.0 }));
            Assert.That(obj["y", 1].ConvertToDoubleArray(), Is.EqualTo(new[] { 6.0 }));
        }

        /// <summary>
        /// Test reading another object.
        /// </summary>
        [Test]
        public void TestObject2()
        {
            var matFile = GetTests("good")["object2"];
            var obj = matFile["object2"].Value as IMatObject;
            Assert.IsNotNull(obj);
            Assert.That(obj.ClassName, Is.EqualTo("Point"));
            Assert.That(obj.FieldNames, Is.EquivalentTo(new[] { "x", "y" }));
            Assert.That(obj["x", 0, 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 3.0 }));
            Assert.That(obj["y", 0, 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 5.0 }));
            Assert.That(obj["x", 1, 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 1.0 }));
            Assert.That(obj["y", 1, 0].ConvertToDoubleArray(), Is.EqualTo(new[] { 0.0 }));
            Assert.That(obj["x", 0, 1].ConvertToDoubleArray(), Is.EqualTo(new[] { -2.0 }));
            Assert.That(obj["y", 0, 1].ConvertToDoubleArray(), Is.EqualTo(new[] { 6.0 }));
            Assert.That(obj["x", 1, 1].ConvertToDoubleArray(), Is.EqualTo(new[] { 0.0 }));
            Assert.That(obj["y", 1, 1].ConvertToDoubleArray(), Is.EqualTo(new[] { 1.0 }));
            Assert.That(obj[0, 1]["x"].ConvertToDoubleArray(), Is.EqualTo(new[] { -2.0 }));
            Assert.That(obj[2]["x"].ConvertToDoubleArray(), Is.EqualTo(new[] { -2.0 }));
        }

        /// <summary>
        /// Test reading a table.
        /// </summary>
        [Test]
        public void TestTable()
        {
            var matFile = GetTests("good")["table"];
            var obj = matFile["table_"].Value as IMatObject;
            var table = new TableAdapter(obj);
            Assert.That(table.NumberOfRows, Is.EqualTo(3));
            Assert.That(table.NumberOfVariables, Is.EqualTo(2));
            Assert.That(table.Description, Is.EqualTo("Some table"));
            Assert.That(table.VariableNames, Is.EqualTo(new[] { "variable1", "variable2" }));
            var variable1 = table["variable1"] as ICellArray;
            Assert.That((variable1[0] as ICharArray).String, Is.EqualTo("First row"));
            Assert.That((variable1[1] as ICharArray).String, Is.EqualTo("Second row"));
            Assert.That((variable1[2] as ICharArray).String, Is.EqualTo("Third row"));
            var variable2 = table["variable2"];
            Assert.That(variable2.ConvertToDoubleArray(), Is.EqualTo(new[] { 1.0, 3.0, 5.0, 2.0, 4.0, 6.0 }));
        }

        /// <summary>
        /// Test subobjects within objects.
        /// </summary>
        [Test]
        public void TestSubobjects()
        {
            var matFile = GetTests("good")["pointWithSubpoints"];
            var p = matFile["p"].Value as IMatObject;
            Assert.That(p.ClassName, Is.EqualTo("Point"));
            var x = p["x"] as IMatObject;
            Assert.That(x.ClassName, Is.EqualTo("SubPoint"));
            Assert.That(x.FieldNames, Is.EquivalentTo(new[] { "a", "b", "c" }));
            var y = p["y"] as IMatObject;
            Assert.That(y.ClassName, Is.EqualTo("SubPoint"));
            Assert.That(y.FieldNames, Is.EquivalentTo(new[] { "a", "b", "c" }));
            Assert.That(x["a"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 1.0 }));
            Assert.That(x["b"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 2.0 }));
            Assert.That(x["c"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 3.0 }));
            Assert.That(y["a"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 14.0 }));
            Assert.That(y["b"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 15.0 }));
            Assert.That(y["c"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 16.0 }));
        }

        /// <summary>
        /// Test nested objects.
        /// </summary>
        [Test]
        public void TestNestedObjects()
        {
            var matFile = GetTests("good")["subsubPoint"];
            var p = matFile["p"].Value as IMatObject;
            Assert.That(p.ClassName, Is.EqualTo("Point"));
            Assert.That(p["x"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 1.0 }));
            var pp = p["y"] as IMatObject;
            Assert.That(pp.ClassName == "Point");
            Assert.That(pp["x"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 10.0 }));
            var ppp = pp["y"] as IMatObject;
            Assert.That(ppp["x"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 100.0 }));
            Assert.That(ppp["y"].ConvertToDoubleArray(), Is.EquivalentTo(new[] { 200.0 }));
        }

        /// <summary>
        /// Test datetime objects.
        /// </summary>
        [Test]
        public void TestDatetime()
        {
            var matFile = GetTests("good")["datetime"];
            var d = matFile["d"].Value as IMatObject;
            var datetime = new DatetimeAdapter(d);
            Assert.That(datetime.Dimensions, Is.EquivalentTo(new[] { 1, 2 }));
            Assert.That(datetime[0], Is.EqualTo(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            Assert.That(datetime[1], Is.EqualTo(new DateTimeOffset(1987, 1, 2, 3, 4, 5, TimeSpan.Zero)));
        }

        /// <summary>
        /// Another test for datetime objects.
        /// </summary>
        [Test]
        public void TestDatetime2()
        {
            var matFile = GetTests("good")["datetime2"];
            var d = matFile["d"].Value as IMatObject;
            var datetime = new DatetimeAdapter(d);
            Assert.That(datetime.Dimensions, Is.EquivalentTo(new[] { 1, 1 }));
            var diff = new DateTimeOffset(2, 1, 1, 1, 1, 1, 235, TimeSpan.Zero);
            Assert.That(datetime[0] - diff < TimeSpan.FromMilliseconds(1));
            Assert.That(diff - datetime[0] < TimeSpan.FromMilliseconds(1));
        }

        /// <summary>
        /// Test string objects.
        /// </summary>
        [Test]
        public void TestString()
        {
            var matFile = GetTests("good")["string"];
            var s = matFile["s"].Value as IMatObject;
            var str = new StringAdapter(s);
            Assert.That(str.Dimensions, Is.EquivalentTo(new[] { 4, 1 }));
            Assert.That(str[0], Is.EqualTo("abc"));
            Assert.That(str[1], Is.EqualTo("defgh"));
            Assert.That(str[2], Is.EqualTo("абвгд"));
            Assert.That(str[3], Is.EqualTo("æøå"));
        }

        /// <summary>
        /// Test duration objects.
        /// </summary>
        [Test]
        public void TestDuration()
        {
            var matFile = GetTests("good")["duration"];
            var d = matFile["d"].Value as IMatObject;
            var duration = new DurationAdapter(d);
            Assert.That(duration.Dimensions, Is.EquivalentTo(new[] { 1, 3 }));
            Assert.That(duration[0], Is.EqualTo(TimeSpan.FromTicks(12345678L)));
            Assert.That(duration[1], Is.EqualTo(new TimeSpan(0, 2, 4)));
            Assert.That(duration[2], Is.EqualTo(new TimeSpan(1, 3, 5)));
        }

        private static AbstractTestDataFactory<IMatFile> GetTests(string factoryName) =>
            new MatTestDataFactory(Path.Combine(TestDirectory, factoryName));

        private static void CheckLimits<T>(IArrayOf<T> array, T[] limits)
            where T : struct
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(array.Data, Is.EqualTo(limits));
        }

        private static void CheckComplexLimits<T>(IArrayOf<ComplexOf<T>> array, T[] limits)
            where T : struct
        {
            Assert.That(array, Is.Not.Null);
            Assert.That(array.Dimensions, Is.EqualTo(new[] { 1, 2 }));
            Assert.That(array[0], Is.EqualTo(new ComplexOf<T>(limits[0], limits[1])));
            Assert.That(array[1], Is.EqualTo(new ComplexOf<T>(limits[1], limits[0])));
        }
    }
}