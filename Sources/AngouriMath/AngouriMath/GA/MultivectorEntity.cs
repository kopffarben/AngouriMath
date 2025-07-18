//
// Copyright (c) 2019-2022 Angouri.
// AngouriMath is licensed under MIT.
// Details: https://github.com/asc-community/AngouriMath/blob/master/LICENSE.md.
// Website: https://am.angouri.org.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AngouriMath.GA
{

    /// <summary>
    /// Entity representing a multivector as a collection of blades with coefficients.
    /// </summary>
    public sealed partial record MultivectorEntity : AngouriMath.Entity
    {
        /// <summary>
        /// Gets the dictionary of blades and their corresponding coefficients.
        /// </summary>
        public IReadOnlyDictionary<BladeMask, AngouriMath.Entity> Terms { get; init; } = new Dictionary<BladeMask, AngouriMath.Entity>();

        /// <inheritdoc/>
        public override AngouriMath.Core.Domain Codomain { get; protected init; } = AngouriMath.Core.Domain.Any;

        private MultivectorEntity(Dictionary<BladeMask, AngouriMath.Entity> terms)
            => Terms = terms;

        /// <summary>
        /// Creates a multivector by combining coefficients of equal blades.
        /// </summary>
        public static MultivectorEntity Create(IEnumerable<(BladeMask mask, AngouriMath.Entity value)> terms)
            => new(Combine(terms));

        private static Dictionary<BladeMask, AngouriMath.Entity> Combine(IEnumerable<(BladeMask mask, AngouriMath.Entity value)> terms)
        {
            var dict = new Dictionary<BladeMask, AngouriMath.Entity>();
            foreach (var (mask, value) in terms)
            {
                if (dict.TryGetValue(mask, out var existing))
                    dict[mask] = existing + value;
                else
                    dict.Add(mask, value);
            }
            return dict;
        }

        /// <summary>
        /// Returns grades present in this multivector ordered ascending.
        /// </summary>
        public IEnumerable<int> GetGrades()
            => Terms.Keys.Select(m => m.Grade).Distinct().OrderBy(x => x);

        internal override Priority Priority => Priority.Leaf;

        /// <inheritdoc/>
        private protected override string SortHashName(Functions.TreeAnalyzer.SortLevel level) => "multivector_";

        /// <inheritdoc/>
        protected override AngouriMath.Entity InnerSimplify() => this;

        /// <inheritdoc/>
        protected override AngouriMath.Entity InnerEval() => this;

        /// <inheritdoc/>
        protected override AngouriMath.Entity[] InitDirectChildren() => Terms.Values.ToArray();

        /// <inheritdoc/>
        public override AngouriMath.Entity Replace(Func<AngouriMath.Entity, AngouriMath.Entity> func)
            => func(new MultivectorEntity(Terms.ToDictionary(kv => kv.Key, kv => kv.Value.Replace(func))));

        /// <inheritdoc/>
        public override string Stringize()
            => string.Join(" + ", Terms.Select(kv => $"{kv.Value.Stringize()}e{kv.Key.Value}"));

        /// <inheritdoc/>
        public override string ToString() => Stringize();

        /// <inheritdoc/>
        public override string Latexise() => Stringize();

        /// <inheritdoc/>
        internal override string ToSymPy()
            => throw AngouriMath.Core.Exceptions.FutureReleaseException.Raised("SymPy conversion");

        /// <inheritdoc/>
        private protected override System.Collections.Generic.IEnumerable<AngouriMath.Entity> InvertNode(AngouriMath.Entity value, AngouriMath.Entity x)
            => throw new NotSupportedException("Geometric algebra inversion not supported");
    }
}
