using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Greentube.Messaging.Kafka.Tests
{
    public class KafkaPropertiesTests
    {
        // KafkaProperties extends dictionary of string,object to have a reference on some commonly used fields
        // This test ensures the map is correctly on get and set accessors
        [Theory]
        [MemberData(nameof(KafkaPropertiesProperties))]
        public void PropertyGetSetMapCorrectly(PropertyInfo propertyInfo)
        {
            // Arrange
            var sut = new KafkaProperties();
            var expected = Convert.ChangeType(1, propertyInfo.PropertyType);
            // Act
            propertyInfo.SetValue(sut, expected);
            var actual = propertyInfo.GetValue(sut);
            // Assert
            Assert.Equal(expected, actual);
        }

        // [xUnit1016] MemberData must reference a public member
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> KafkaPropertiesProperties()
            => typeof(KafkaProperties).GetProperties(
                BindingFlags.DeclaredOnly
                | BindingFlags.Instance
                | BindingFlags.Public)
                .Select(p => new object[] { p });
    }
}