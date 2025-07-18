//
// Copyright (c) 2019-2022 Angouri.
// AngouriMath is licensed under MIT.
// Details: https://github.com/asc-community/AngouriMath/blob/master/LICENSE.md.
// Website: https://am.angouri.org.
//

using System.Collections.Generic;

namespace AngouriMath.GA
{
    /// <summary>
    /// Describes a geometric algebra by its dimension, metric signature and basis names.
    /// </summary>
    public sealed record AlgebraDescriptor
    {
        /// <summary>Number of basis vectors.</summary>
        public int Dimension { get; init; }

        /// <summary>Diagonal metric entries e_i · e_i.</summary>
        public IReadOnlyList<int> Signature { get; init; }

        /// <summary>Human readable names for each basis vector.</summary>
        public IReadOnlyList<string> BasisNames { get; init; }

        /// <summary>Off-diagonal metric entries for special pairs.</summary>
        public IReadOnlyDictionary<(int, int), int> OffDiagonal { get; init; }

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

        /// <summary>Projective Geometric Algebra for 3D.</summary>
        public static readonly AlgebraDescriptor PGA3D =
            new(
                4,
                new[] { 1, 1, 1, 0 },
                new[] { "e1", "e2", "e3", "eInf" });

        /// <summary>Conformal Geometric Algebra for 3D.</summary>
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
