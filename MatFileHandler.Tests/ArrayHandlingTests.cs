// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Numerics;
using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of Matlab array manipulation.
    /// </summary>
    public class ArrayHandlingTests : IDisposable
    {
        public ArrayHandlingTests()
        {
            Builder = new DataBuilder();
        }

        private DataBuilder Builder { get; set; }

        /// <summary>
        /// Test numerical array creation.
        /// </summary>
        [Fact]
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
        [Fact]
        public void TestNumArray()
        {
            var array = Builder.NewArray<int>(2, 3);
            array[0, 1] = 2;
            Assert.Equal(2, array[0, 1]);
        }

        /// <summary>
        /// Test cell array manipulation.
        /// </summary>
        [Fact]
        public void TestCellArray()
        {
            var array = Builder.NewCellArray(2, 3);
            Assert.Equal(new[] { 2, 3 }, array.Dimensions);
            array[0, 1] = Builder.NewArray<int>(1, 2);
            Assert.True(array[1, 2].IsEmpty);
            Assert.False(array[0, 1].IsEmpty);
            var assigned = (IArrayOf<int>)array[0, 1];
            Assert.NotNull(assigned);
            Assert.Equal(new[] { 1, 2 }, assigned.Dimensions);
        }

        /// <summary>
        /// Test structure array manipulation.
        /// </summary>
        [Fact]
        public void TestStructureArray()
        {
            var array = Builder.NewStructureArray(new[] { "x", "y" }, 2, 3);
            array["x", 0, 1] = Builder.NewArray<int>(1, 2);
            Assert.True(array["y", 0, 1].IsEmpty);
            Assert.False(array["x", 0, 1].IsEmpty);
            var assigned = (IArrayOf<int>)array["x", 0, 1];
            Assert.NotNull(assigned);
            Assert.Equal(new[] { 1, 2 }, assigned.Dimensions);
        }

        /// <summary>
        /// Test character array manipulation.
        /// </summary>
        [Fact]
        public void TestString()
        {
            var array = Builder.NewCharArray("🍆");
            Assert.Equal(new[] { 1, 2 }, array.Dimensions);
        }

        /// <summary>
        /// Test file creation.
        /// </summary>
        [Fact]
        public void TestFile()
        {
            var file = Builder.NewFile(new List<IVariable>());
            Assert.NotNull(file);
        }
            
        private static void TestCreateArrayOf<T>()
            where T : struct
        {
            var array = new DataBuilder().NewArray<T>(2, 3);
            Assert.NotNull(array);
        }

        public void Dispose()
        {
        }
    }
}