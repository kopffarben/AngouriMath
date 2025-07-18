using System.Linq;
using Xunit;
using AngouriMath.GA;

namespace AngouriMath.Tests.GA
{
    public sealed class MultivectorEntityTest
    {
        [Fact]
        public void CombineSameBlades()
        {
            var mv = MultivectorEntity.Create(new[]
            {
                (new BladeMask(1ul), (AngouriMath.Entity)1),
                (new BladeMask(1ul), (AngouriMath.Entity)2),
                (new BladeMask(2ul), (AngouriMath.Entity)3)
            });

            Assert.Equal(2, mv.Terms.Count);
            Assert.Equal(3, mv.Terms[new BladeMask(1ul)].Evaled); // 1 + 2 = 3
        }

        [Fact]
        public void GetGradesReturnsOrdered()
        {
            var mv = MultivectorEntity.Create(new[]
            {
                (new BladeMask(3ul), (AngouriMath.Entity)1),
                (new BladeMask(4ul), (AngouriMath.Entity)2)
            });

            var grades = mv.GetGrades().ToArray();
            Assert.Equal(new[] { 1, 2 }, grades);
        }
    }
}
