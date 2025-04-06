// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests for the <see cref="ChecksumCalculatingStream"/> class.
    /// </summary>
    public class ChecksumCalculatingStreamTests
    {
        /// <summary>
        /// Test writing various things.
        /// </summary>
        /// <param name="action"></param>
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test(Action<Stream> action)
        {
            using var stream = new MemoryStream();
            var sut = new ChecksumCalculatingStream(stream);
            action(sut);
            var actual = sut.GetCrc();
            var expected = ReferenceCalculation(action);
        }

        /// <summary>
        /// Test data for <see cref="Test"/>.
        /// </summary>
        /// <returns>Test data.</returns>
        public static IEnumerable<object[]> TestData()
        {
            foreach (var data in TestData_Typed())
            {
                yield return new object[] { data };
            }
        }

        private static IEnumerable<Action<Stream>> TestData_Typed()
        {
            yield return BinaryWriterAction(w => w.Write(true));
            yield return BinaryWriterAction(w => w.Write(false));
            yield return BinaryWriterAction(w => w.Write(byte.MinValue));
            yield return BinaryWriterAction(w => w.Write(byte.MaxValue));
            yield return BinaryWriterAction(w => w.Write(short.MinValue));
            yield return BinaryWriterAction(w => w.Write(short.MaxValue));
            yield return BinaryWriterAction(w => w.Write(int.MinValue));
            yield return BinaryWriterAction(w => w.Write(int.MaxValue));
            yield return BinaryWriterAction(w => w.Write(long.MinValue));
            yield return BinaryWriterAction(w => w.Write(long.MaxValue));
            yield return BinaryWriterAction(w => w.Write(decimal.MinValue));
            yield return BinaryWriterAction(w => w.Write(decimal.MaxValue));
            yield return BinaryWriterAction(w => w.Write(double.MinValue));
            yield return BinaryWriterAction(w => w.Write(double.MaxValue));
            yield return BinaryWriterAction(w => w.Write(double.PositiveInfinity));
            yield return BinaryWriterAction(w => w.Write(double.NaN));
            yield return BinaryWriterAction(w => w.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7 }));
            yield return BinaryWriterAction(w => w.Write(Enumerable.Range(0, 255).SelectMany(x => Enumerable.Range(0, 255)).Select(x => (byte)x).ToArray()));
        }

        private static Action<Stream> BinaryWriterAction(Action<BinaryWriter> action)
        {
            return stream =>
            {
                using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
                action(writer);
            };
        }

        private uint ReferenceCalculation(Action<Stream> action)
        {
            using var stream = new MemoryStream();
            action(stream);
            stream.Position = 0;
            return CalculateAdler32Checksum(stream);
        }

        private static uint CalculateAdler32Checksum(Stream stream)
        {
            uint s1 = 1;
            uint s2 = 0;
            const uint bigPrime = 0xFFF1;
            const int bufferSize = 2048;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = stream.Read(buffer, 0, bufferSize);
                for (var i = 0; i < bytesRead; i++)
                {
                    s1 = (s1 + buffer[i]) % bigPrime;
                    s2 = (s2 + s1) % bigPrime;
                }
                if (bytesRead < bufferSize)
                {
                    break;
                }
            }
            return (s2 << 16) | s1;
        }
    }
}
