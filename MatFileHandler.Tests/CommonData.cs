// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Data used in reading/writing tests.
    /// </summary>
    public static class CommonData
    {
        /// <summary>
        /// Limits of Int8.
        /// </summary>
        public static readonly sbyte[] Int8Limits = { -128, 127 };

        /// <summary>
        /// Limits of UInt8.
        /// </summary>
        public static readonly byte[] UInt8Limits = { 0, 255 };

        /// <summary>
        /// Limits of Int16.
        /// </summary>
        public static readonly short[] Int16Limits = { -32768, 32767 };

        /// <summary>
        /// Limits of UInt16.
        /// </summary>
        public static readonly ushort[] UInt16Limits = { 0, 65535 };

        /// <summary>
        /// Limits of Int32.
        /// </summary>
        public static readonly int[] Int32Limits = { -2147483648, 2147483647 };

        /// <summary>
        /// Limits of UInt32.
        /// </summary>
        public static readonly uint[] UInt32Limits = { 0U, 4294967295U };

        /// <summary>
        /// Limits of Int64.
        /// </summary>
        public static readonly long[] Int64Limits = { -9223372036854775808L, 9223372036854775807L };

        /// <summary>
        /// Limits of UInt64.
        /// </summary>
        public static readonly ulong[] UInt64Limits = { 0UL, 18446744073709551615UL };
    }
}