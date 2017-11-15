// Copyright 2017 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// A structure representing a complex number where real and imaginary parts are of type T.
    /// </summary>
    /// <typeparam name="T">Type of real and imaginary parts.</typeparam>
    public struct ComplexOf<T> : IEquatable<ComplexOf<T>>
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexOf{T}"/> struct.
        /// </summary>
        /// <param name="real">Real part.</param>
        /// <param name="imaginary">Imaginary part.</param>
        public ComplexOf(T real, T imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        /// <summary>
        /// Gets real part.
        /// </summary>
        public T Real { get; }

        /// <summary>
        /// Gets imaginary part.
        /// </summary>
        public T Imaginary { get; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">Left argument.</param>
        /// <param name="right">Right argument.</param>
        /// <returns>True iff the numbers are equal.</returns>
        public static bool operator ==(ComplexOf<T> left, ComplexOf<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">Left argument.</param>
        /// <param name="right">Right argument.</param>
        /// <returns>True iff the numbers are not equal.</returns>
        public static bool operator !=(ComplexOf<T> left, ComplexOf<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Equality check.
        /// </summary>
        /// <param name="other">Another complex number.</param>
        /// <returns>True iff the number is equal to another.</returns>
        public bool Equals(ComplexOf<T> other)
        {
            return Real.Equals(other.Real) && Imaginary.Equals(other.Imaginary);
        }

        /// <summary>
        /// Equality check.
        /// </summary>
        /// <param name="obj">Another object.</param>
        /// <returns>True iff another object is a complex number equal to this.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ComplexOf<T> other && Equals(other);
        }

        /// <summary>
        /// Gets has code of the number.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Real.GetHashCode() * 397) ^ Imaginary.GetHashCode();
            }
        }
    }
}