// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of Matlab array manipulation.
    /// </summary>
    [TestFixture]
    public class ArrayHandlingTests
    {
        private DataBuilder Builder { get; set; }

        /// <summary>
        /// Set up a DataBuilder.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Builder = new DataBuilder();
        }

        /// <summary>
        /// Test numerical array creation.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            TestCreateArrayOf<sbyte>();
            TestCreateArrayOf<ComplexOf<sbyte>>();
            TestCreateArrayOf<byte>();
            TestCreateArrayOf<ComplexOf<byte>>();
            TestCreateArrayOf<short>();
            TestCreateArrayOf<ComplexOf<short>>();
            TestCreateArrayOf<ushort>();
            TestCreateArrayOf<ComplexOf<ushort>>();
            TestCreateArrayOf<int>();
            TestCreateArrayOf<ComplexOf<int>>();
            TestCreateArrayOf<uint>();
            TestCreateArrayOf<ComplexOf<uint>>();
            TestCreateArrayOf<long>();
            TestCreateArrayOf<ComplexOf<long>>();
            TestCreateArrayOf<ulong>();
            TestCreateArrayOf<ComplexOf<ulong>>();
            TestCreateArrayOf<float>();
            TestCreateArrayOf<ComplexOf<float>>();
            TestCreateArrayOf<double>();
            TestCreateArrayOf<Complex>();
        }

        /// <summary>
        /// Test numerical array manipulation.
        /// </summary>
        [Test]
        public void TestNumArray()
        {
            var array = Builder.NewArray<int>(2, 3);
            array[0, 1] = 2;
            Assert.That(array[0, 1], Is.EqualTo(2));
        }

        /// <summary>
        /// Test cell array manipulation.
        /// </summary>
        [Test]
        public void TestCellArray()
        {
            var array = Builder.NewCellArray(2, 3);
            Assert.That(array.Dimensions, Is.EqualTo(new[] { 2, 3 }));
            array[0, 1] = Builder.NewArray<int>(1, 2);
            Assert.That(array[1, 2].IsEmpty, Is.True);
            Assert.That(array[0, 1].IsEmpty, Is.False);
            var assigned = (IArrayOf<int>)array[0, 1];
            Assert.That(assigned, Is.Not.Null);
            Assert.That(assigned.Dimensions, Is.EqualTo(new[] { 1, 2 }));
        }

        /// <summary>
        /// Test structure array manipulation.
        /// </summary>
        [Test]
        public void TestStructureArray()
        {
            var array = Builder.NewStructureArray(new[] { "x", "y" }, 2, 3);
            array["x", 0, 1] = Builder.NewArray<int>(1, 2);
            Assert.That(array["y", 0, 1].IsEmpty, Is.True);
            Assert.That(array["x", 0, 1].IsEmpty, Is.False);
            var assigned = (IArrayOf<int>)array["x", 0, 1];
            Assert.That(assigned, Is.Not.Null);
            Assert.That(assigned.Dimensions, Is.EqualTo(new[] { 1, 2 }));
        }

        /// <summary>
        /// Test character array manipulation.
        /// </summary>
        [Test]
        public void TestString()
        {
            var array = Builder.NewCharArray("🍆");
            Assert.That(array.Dimensions, Is.EqualTo(new[] { 1, 2 }));
        }

        /// <summary>
        /// Test file creation.
        /// </summary>
        [Test]
        public void TestFile()
        {
            var file = Builder.NewFile(new List<IVariable>());
            Assert.That(file, Is.Not.Null);
        }

        private static void TestCreateArrayOf<T>()
            where T : struct
        {
            var array = new DataBuilder().NewArray<T>(2, 3);
            Assert.That(array, Is.Not.Null);
        }
    }
}