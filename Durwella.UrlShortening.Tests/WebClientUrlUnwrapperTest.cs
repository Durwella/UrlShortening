using System.Net;
using FluentAssertions;
using NUnit.Framework;

namespace Durwella.UrlShortening.Tests
{
    public class WebClientUrlUnwrapperTest
    {
        [Test]
        public void ShouldGetResourceLocation()
        {
            var wrappedUrl = "http://goo.gl/mSkqOi";
            var subject = new WebClientUrlUnwrapper();

            var directUrl = subject.GetDirectUrl(wrappedUrl);

            directUrl.Should().Be("http://example.com/");
        }

        [Test]
        public void ShouldReturnGivenLocationIfAuthenticationRequired()
        {
            var givenUrl = "http://durwella.com/testing/does-not-exist";
            var subject = new WebClientUrlUnwrapper
            {
                IgnoreErrorCodes = new[] { HttpStatusCode.NotFound }
            };

            var directUrl = subject.GetDirectUrl(givenUrl);

            directUrl.Should().Be(givenUrl);
        }
    }
}
