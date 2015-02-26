using Ploeh.AutoFixture.Xunit;

using Xunit.Extensions;

namespace Pilipala.Tests.AutoFixture
{
    public class InlineAutoNSubstituteDataAttribute : CompositeDataAttribute
    {
        public InlineAutoNSubstituteDataAttribute(params object[] values)
            : base(new DataAttribute[]
                   {
                       new InlineDataAttribute(values), new AutoNSubstituteDataAttribute()
                   })
        {
        }
    }
}
