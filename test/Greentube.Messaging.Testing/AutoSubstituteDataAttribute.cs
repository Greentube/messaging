using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

// ReSharper disable once CheckNamespace - discoverability
namespace Greentube.Messaging
{
    public class AutoSubstituteDataAttribute : AutoDataAttribute
    {
        public AutoSubstituteDataAttribute()
            : base(() => new Fixture().Customize(new AutoConfiguredNSubstituteCustomization()))
        {
        }
    }
}