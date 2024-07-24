// Copyright 2017-2018 Alexander Luzgarev

using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of the ComplexOf value type.
    /// </summary>
    public class ComplexOfTests
    {
        /// <summary>
        /// Test getting real and imaginary parts.
        /// </summary>
        [Fact]
        public void TestAccessors()
        {
            var c = new ComplexOf<byte>(1, 2);
            Assert.Equal(1, c.Real);
            Assert.Equal(2, c.Imaginary);
        }

        /// <summary>
        /// Test equality operators.
        /// </summary>
        [Fact]
        public void TestEquals()
        {
            var c1 = new ComplexOf<byte>(1, 2);
            var c2 = new ComplexOf<byte>(3, 4);
            var c3 = new ComplexOf<byte>(1, 2);
            Assert.True(c1 == c3);
            Assert.True(c1 != c2);
            Assert.True(c2 != c3);
        }

        /// <summary>
        /// Test hash code calculation.
        /// </summary>
        [Fact]
        public void TestGetHashCode()
        {
            var c1 = new ComplexOf<byte>(1, 2);
            var c2 = new ComplexOf<byte>(1, 2);
            var h1 = c1.GetHashCode();
            var h2 = c2.GetHashCode();
            Assert.Equal(h1, h2);
        }
    }
}