// Copyright 2017 Alexander Luzgarev

using NUnit.Framework;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests of the ComplexOf value type.
    /// </summary>
    [TestFixture]
    public class ComplexOfTests
    {
        /// <summary>
        /// Test getting real and imaginary parts.
        /// </summary>
        [Test]
        public void TestAccessors()
        {
            var c = new ComplexOf<byte>(1, 2);
            Assert.That(c.Real, Is.EqualTo(1));
            Assert.That(c.Imaginary, Is.EqualTo(2));
        }

        /// <summary>
        /// Test equality operators.
        /// </summary>
        [Test]
        public void TestEquals()
        {
            var c1 = new ComplexOf<byte>(1, 2);
            var c2 = new ComplexOf<byte>(3, 4);
            var c3 = new ComplexOf<byte>(1, 2);
            Assert.That(c1 == c3, Is.True);
            Assert.That(c1 != c2, Is.True);
            Assert.That(c2 != c3, Is.True);
        }

        /// <summary>
        /// Test hash code calculation.
        /// </summary>
        [Test]
        public void TestGetHashCode()
        {
            var c1 = new ComplexOf<byte>(1, 2);
            var c2 = new ComplexOf<byte>(1, 2);
            var h1 = c1.GetHashCode();
            var h2 = c2.GetHashCode();
            Assert.That(h1, Is.EqualTo(h2));
        }
    }
}