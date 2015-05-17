using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Durwella.UrlShortening.Tests
{
    public class UrlShortenerTest
    {
        [Test]
        public void ShouldSaveAndReturnHash()
        {
            var mockHashScheme = new Mock<IHashScheme>();
            var url = "http://example.com/foo/bar";
            var hash = "123abc";
            mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash);
            var mockRepository = new Mock<IAliasRepository>();
            var baseUrl = "http://goto";
            var subject = new UrlShortener(mockHashScheme.Object, mockRepository.Object, baseUrl);

            var shortened = subject.Shorten(url);

            shortened.Should().Be("http://goto/123abc");
            mockRepository.Verify(r => r.Add(hash, url));
        }

        // TODO: Already been hashed
        // TODO: Hash collision handling
        // TODO: Redirection unwrapping (avoid multiple redirects and redirect loops)
    }
}
