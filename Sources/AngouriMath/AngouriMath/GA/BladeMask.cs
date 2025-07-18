//
// Copyright (c) 2019-2022 Angouri.
// AngouriMath is licensed under MIT.
// Details: https://github.com/asc-community/AngouriMath/blob/master/LICENSE.md.
// Website: https://am.angouri.org.
//
namespace AngouriMath.GA
{
    /// <summary>
    /// Represents a bitmask identifying a blade within a geometric algebra.
    /// </summary>
    public readonly struct BladeMask : System.IEquatable<BladeMask>
    {
        /// <summary>
        /// Underlying mask value where each bit corresponds to a basis vector.
        /// </summary>
        public readonly ulong Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BladeMask"/> struct.
        /// </summary>
        /// <param name="value">The bit mask.</param>
        public BladeMask(ulong value) => Value = value;

        /// <summary>
        /// Gets the grade of the blade represented by this mask.
        /// </summary>
        public int Grade => CountBits(Value);

        private static int CountBits(ulong value)
        {
            int count = 0;
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }
            return count;
        }

        /// <inheritdoc/>
        public bool Equals(BladeMask other) => Value == other.Value;
        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is BladeMask mask && Equals(mask);
        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();
    }
}
