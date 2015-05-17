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
    }
}
