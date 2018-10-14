// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Collections.Generic;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// Functions for extracting values from data elements.
    /// </summary>
    internal static class DataExtraction
    {
        /// <summary>
        /// Convert IEnumerable to array.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="somethingEnumerable">Input IEnumerable.</param>
        /// <returns>An equivalent array.</returns>
        /// <remarks>In contrast to the stanard ToArray() method, this doesn't create a copy if the input already was an array.</remarks>
        public static T[] ToArrayLazily<T>(this IEnumerable<T> somethingEnumerable)
        {
            return somethingEnumerable as T[] ?? somethingEnumerable.ToArray();
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Double values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Double.</returns>
        public static IEnumerable<double> GetDataAsDouble(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToDouble);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToDouble);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToDouble);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToDouble);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToDouble);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToDouble);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToDouble);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToDouble);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToDouble);
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
        public static IEnumerable<float> GetDataAsSingle(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToSingle);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToSingle);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToSingle);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToSingle);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToSingle);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToSingle);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToSingle);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToSingle);
                case MiNum<float> floatElement:
                    return floatElement.Data;
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToSingle);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to float, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int8.</returns>
        public static IEnumerable<sbyte> GetDataAsInt8(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data;
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToSByte);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToSByte);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToSByte);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToSByte);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToSByte);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToSByte);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToSByte);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToSByte);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToSByte);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int8, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Uint8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt8.</returns>
        public static IEnumerable<byte> GetDataAsUInt8(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToByte);
                case MiNum<byte> byteElement:
                    return byteElement.Data;
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToByte);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToByte);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToByte);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToByte);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToByte);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToByte);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToByte);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToByte);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint8, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int16.</returns>
        public static IEnumerable<short> GetDataAsInt16(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToInt16);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToInt16);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToInt16);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToInt16);
                case MiNum<short> shortElement:
                    return shortElement.Data;
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToInt16);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToInt16);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToInt16);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToInt16);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToInt16);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int16, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt16.</returns>
        public static IEnumerable<ushort> GetDataAsUInt16(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToUInt16);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToUInt16);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToUInt16);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToUInt16);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToUInt16);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data;
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToUInt16);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToUInt16);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToUInt16);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToUInt16);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint16, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int32.</returns>
        public static IEnumerable<int> GetDataAsInt32(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToInt32);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToInt32);
                case MiNum<int> intElement:
                    return intElement.Data;
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToInt32);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToInt32);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToInt32);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToInt32);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToInt32);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToInt32);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToInt32);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int32, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt32.</returns>
        public static IEnumerable<uint> GetDataAsUInt32(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToUInt32);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToUInt32);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToUInt32);
                case MiNum<uint> uintElement:
                    return uintElement.Data;
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToUInt32);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToUInt32);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToUInt32);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToUInt32);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToUInt32);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToUInt32);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint32, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int64.</returns>
        public static IEnumerable<long> GetDataAsInt64(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToInt64);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToInt64);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToInt64);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToInt64);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToInt64);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToInt64);
                case MiNum<long> longElement:
                    return longElement.Data;
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data.Select(Convert.ToInt64);
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToInt64);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToInt64);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to int64, found {element.GetType()}.");
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt64.</returns>
        public static IEnumerable<ulong> GetDataAsUInt64(DataElement element)
        {
            switch (element)
            {
                case MiNum<sbyte> sbyteElement:
                    return sbyteElement.Data.Select(Convert.ToUInt64);
                case MiNum<byte> byteElement:
                    return byteElement.Data.Select(Convert.ToUInt64);
                case MiNum<int> intElement:
                    return intElement.Data.Select(Convert.ToUInt64);
                case MiNum<uint> uintElement:
                    return uintElement.Data.Select(Convert.ToUInt64);
                case MiNum<short> shortElement:
                    return shortElement.Data.Select(Convert.ToUInt64);
                case MiNum<ushort> ushortElement:
                    return ushortElement.Data.Select(Convert.ToUInt64);
                case MiNum<long> longElement:
                    return longElement.Data.Select(Convert.ToUInt64);
                case MiNum<ulong> ulongElement:
                    return ulongElement.Data;
                case MiNum<float> floatElement:
                    return floatElement.Data.Select(Convert.ToUInt64);
                case MiNum<double> doubleElement:
                    return doubleElement.Data.Select(Convert.ToUInt64);
            }
            throw new HandlerException(
                $"Expected data element that would be convertible to uint64, found {element.GetType()}.");
        }
    }
}