using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace Messaging.Tests
{
    public class AutoSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
        {
        }
    }
}