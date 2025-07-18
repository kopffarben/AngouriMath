//
// Copyright (c) 2019-2022 Angouri.
// AngouriMath is licensed under MIT.
// Details: https://github.com/asc-community/AngouriMath/blob/master/LICENSE.md.
// Website: https://am.angouri.org.
//

using System;
using System.Collections.Generic;

namespace AngouriMath.GA
{
    /// <summary>
    /// Describes a geometric algebra by specifying its dimension, metric signature, and basis names.
    /// </summary>
    public sealed record AlgebraDescriptor
    {
        /// <summary>
        /// Gets the number of basis vectors.
        /// </summary>
        public int Dimension { get; init; }

        /// <summary>
        /// Gets the diagonal metric entries (e_i · e_i) for each basis vector.
        /// </summary>
        public IReadOnlyList<int> Signature { get; init; }

        /// <summary>
        /// Gets the human-readable names for each basis vector.
        /// </summary>
        public IReadOnlyList<string> BasisNames { get; init; }

        /// <summary>
        /// Gets the off-diagonal metric entries for specific pairs of basis vectors.
        /// </summary>
        public IReadOnlyDictionary<(int, int), int> OffDiagonal { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgebraDescriptor"/> class with the specified dimension, signature, basis names, and optional off-diagonal metric.
        /// </summary>
        /// <param name="dimension">The number of basis vectors in the algebra.</param>
        /// <param name="signature">The diagonal metric entries (e_i · e_i) for each basis vector.</param>
        /// <param name="basisNames">The human-readable names for each basis vector.</param>
        /// <param name="offDiagonal">
        /// Optional parameter. A dictionary representing off-diagonal metric entries for specific pairs of basis vectors. 
        /// If not provided, an empty dictionary is created.
        /// </param>
        public AlgebraDescriptor(
            int dimension,
            IReadOnlyList<int> signature,
            IReadOnlyList<string> basisNames,
            IReadOnlyDictionary<(int, int), int>? offDiagonal = null)
        {
            Dimension = dimension;
            Signature = signature;
            BasisNames = basisNames;
            OffDiagonal = offDiagonal ?? new Dictionary<(int, int), int>();
        }

        /// <summary>
        /// Gets the Projective Geometric Algebra for 3D.
        /// </summary>
        [ConstantField]
        public static readonly AlgebraDescriptor PGA3D =
            new(
                4,
                new[] { 1, 1, 1, 0 },
                new[] { "e1", "e2", "e3", "eInf" });

        /// <summary>
        /// Gets the Conformal Geometric Algebra for 3D.
        /// </summary>
        [ConstantField]
        public static readonly AlgebraDescriptor CGA3D =
            new(
                5,
                new[] { 1, 1, 1, 0, 0 },
                new[] { "e1", "e2", "e3", "ePlus", "eMinus" },
                new Dictionary<(int, int), int>
                {
                        { (3, 4), 1 },
                        { (4, 3), 1 }
                });
    }
}
