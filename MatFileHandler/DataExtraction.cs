// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Functions for extracting values from data elements.
    /// </summary>
    internal static class DataExtraction
    {
        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Double values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Double.</returns>
        public static double[] GetDataAsDouble(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToDouble(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToDouble(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToDouble(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToDouble(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToDouble(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToDouble(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToDouble(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToDouble(ulongElement.Data);
                case MiNum<float> floatElement:
                    return FloatToDouble(floatElement.Data);
                case MiNum<double> doubleElement:
                    return doubleElement.Data;
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to double, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Single values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Single.</returns>
        public static float[] GetDataAsSingle(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToSingle(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToSingle(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToSingle(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToSingle(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToSingle(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToSingle(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToSingle(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToSingle(ulongElement.Data);
                case MiNum<float> floatElement:
                    return floatElement.Data;
                case MiNum<double> doubleElement:
                    return DoubleToSingle(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to float, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int8.</returns>
        public static sbyte[] GetDataAsInt8(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data;
                case MiNum<byte> byteElement:
                    return ByteToSByte(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToSByte(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToSByte(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToSByte(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToSByte(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToSByte(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToSByte(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToSByte(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToSByte(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int8, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Uint8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt8.</returns>
        public static byte[] GetDataAsUInt8(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToByte(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return byteElement.Data;
                case MiNum<int> intElement:
                    return IntToByte(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToByte(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToByte(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToByte(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToByte(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToByte(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToByte(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToByte(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint8, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int16.</returns>
        public static short[] GetDataAsInt16(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToInt16(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToInt16(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToInt16(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToInt16(uintElement.Data);
                case MiNum<short> shortElement:
                    return shortElement.Data;
                case MiNum<ushort> ushortElement:
                    return UshortToInt16(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToInt16(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToInt16(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToInt16(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToInt16(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int16, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt16.</returns>
        public static ushort[] GetDataAsUInt16(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToUInt16(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToUInt16(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToUInt16(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToUInt16(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToUInt16(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data;
                case MiNum<long> longElement:
                    return LongToUInt16(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToUInt16(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToUInt16(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToUInt16(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint16, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int32.</returns>
        public static int[] GetDataAsInt32(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToInt32(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToInt32(byteElement.Data);
                case MiNum<int> intElement:
                    return intElement.Data;
                case MiNum<uint> uintElement:
                    return UintToInt32(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToInt32(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToInt32(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToInt32(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToInt32(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToInt32(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToInt32(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int32, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt32.</returns>
        public static uint[] GetDataAsUInt32(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToUInt32(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToUInt32(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToUInt32(intElement.Data);
                case MiNum<uint> uintElement:
                    return uintElement.Data;
                case MiNum<short> shortElement:
                    return ShortToUInt32(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToUInt32(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToUInt32(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return UlongToUInt32(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToUInt32(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToUInt32(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint32, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int64.</returns>
        public static long[] GetDataAsInt64(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToInt64(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToInt64(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToInt64(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToInt64(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToInt64(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToInt64(ushortElement.Data);
                case MiNum<long> longElement:
                    return longElement.Data;
                case MiNum<ulong> ulongElement:
                    return UlongToInt64(ulongElement.Data);
                case MiNum<float> floatElement:
                    return SingleToInt64(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToInt64(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int64, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt64.</returns>
        public static ulong[] GetDataAsUInt64(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return SbyteToUInt64(sbyteElement.Data);
                case MiNum<byte> byteElement:
                    return ByteToUInt64(byteElement.Data);
                case MiNum<int> intElement:
                    return IntToUInt64(intElement.Data);
                case MiNum<uint> uintElement:
                    return UintToUInt64(uintElement.Data);
                case MiNum<short> shortElement:
                    return ShortToUInt64(shortElement.Data);
                case MiNum<ushort> ushortElement:
                    return UshortToUInt64(ushortElement.Data);
                case MiNum<long> longElement:
                    return LongToUInt64(longElement.Data);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data;
                case MiNum<float> floatElement:
                    return SingleToUInt64(floatElement.Data);
                case MiNum<double> doubleElement:
                    return DoubleToUInt64(doubleElement.Data);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint64, found {element.GetType()}.");
        }

        // * to double

        /// <summary>
        /// Convert an array of signed bytes to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] SbyteToDouble(sbyte[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of bytes to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] ByteToDouble(byte[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of shorts to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] ShortToDouble(short[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned shorts to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UshortToDouble(ushort[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of integers to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] IntToDouble(int[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned integers to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UintToDouble(uint[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of longs to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] LongToDouble(long[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned longs to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UlongToDouble(ulong[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of floats to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] FloatToDouble(float[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        // * to single
        private static float[] SbyteToSingle(sbyte[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] ByteToSingle(byte[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] ShortToSingle(short[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UshortToSingle(ushort[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] IntToSingle(int[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UintToSingle(uint[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] LongToSingle(long[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UlongToSingle(ulong[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] DoubleToSingle(double[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        // * to sbyte
        private static sbyte[] ByteToSByte(byte[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] ShortToSByte(short[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UshortToSByte(ushort[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] IntToSByte(int[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UintToSByte(uint[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] LongToSByte(long[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UlongToSByte(ulong[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] SingleToSByte(float[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] DoubleToSByte(double[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        // * to byte
        private static byte[] SbyteToByte(sbyte[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] SingleToByte(float[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] ShortToByte(short[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UshortToByte(ushort[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] IntToByte(int[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UintToByte(uint[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] LongToByte(long[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UlongToByte(ulong[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] DoubleToByte(double[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        // * to int16
        private static short[] SbyteToInt16(sbyte[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] ByteToInt16(byte[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UshortToInt16(ushort[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] IntToInt16(int[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UintToInt16(uint[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] LongToInt16(long[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UlongToInt16(ulong[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] SingleToInt16(float[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] DoubleToInt16(double[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        // * to uint16
        private static ushort[] SbyteToUInt16(sbyte[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] ByteToUInt16(byte[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] ShortToUInt16(short[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] IntToUInt16(int[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] UintToUInt16(uint[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] LongToUInt16(long[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] UlongToUInt16(ulong[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] SingleToUInt16(float[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] DoubleToUInt16(double[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        // * to int32
        private static int[] SbyteToInt32(sbyte[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] ByteToInt32(byte[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] ShortToInt32(short[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UshortToInt32(ushort[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UintToInt32(uint[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] LongToInt32(long[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UlongToInt32(ulong[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] SingleToInt32(float[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] DoubleToInt32(double[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        // * to uint32
        private static uint[] SbyteToUInt32(sbyte[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] ByteToUInt32(byte[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] ShortToUInt32(short[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] UshortToUInt32(ushort[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] IntToUInt32(int[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] LongToUInt32(long[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] UlongToUInt32(ulong[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] SingleToUInt32(float[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] DoubleToUInt32(double[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        // * to int64
        private static long[] SbyteToInt64(sbyte[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] ByteToInt64(byte[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] ShortToInt64(short[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UshortToInt64(ushort[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] IntToInt64(int[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UintToInt64(uint[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UlongToInt64(ulong[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] SingleToInt64(float[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] DoubleToInt64(double[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        // * to uint64
        private static ulong[] SbyteToUInt64(sbyte[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] ByteToUInt64(byte[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] ShortToUInt64(short[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] UshortToUInt64(ushort[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] IntToUInt64(int[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] UintToUInt64(uint[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] LongToUInt64(long[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] SingleToUInt64(float[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] DoubleToUInt64(double[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }
    }
}