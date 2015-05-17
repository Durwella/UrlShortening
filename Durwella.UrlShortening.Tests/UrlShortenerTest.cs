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

        [Test]
        public void ShouldReturnExistingHash()
        {
            var mockHashScheme = new Mock<IHashScheme>();
            var url = "http://example.com/a/b/c";
            var hash = "abc";
            var mockRepository = new Mock<IAliasRepository>();
            mockRepository.Setup(r => r.ContainsValue(url)).Returns(true);
            mockRepository.Setup(r => r.GetKey(url)).Returns(hash);
            var baseUrl = "http://go2";
            var subject = new UrlShortener(mockHashScheme.Object, mockRepository.Object, baseUrl);

            var shortened = subject.Shorten(url);

            shortened.Should().Be("http://go2/abc");
            mockRepository.Verify(r => r.Add(hash, url), Times.Never());
            mockHashScheme.Verify(h => h.GetKey(url), Times.Never());
        }

        [Test]
        public void HandleHashCollision()
        {
            var mockHashScheme = new Mock<IHashScheme>();
            var url = "http://example.com/hash/collision";
            var hash1 = "aaaa";
            var hash2 = "bbbb";
            var hash3 = "cccc";
            mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash1);
            mockHashScheme.Setup(h => h.GetKey(url, 0)).Returns(hash1);
            mockHashScheme.Setup(h => h.GetKey(url, 1)).Returns(hash2);
            mockHashScheme.Setup(h => h.GetKey(url, 2)).Returns(hash3);
            var mockRepository = new Mock<IAliasRepository>();
            mockRepository.Setup(r => r.ContainsValue(url)).Returns(false);
            mockRepository.Setup(r => r.ContainsKey(hash1)).Returns(true);
            mockRepository.Setup(r => r.ContainsKey(hash2)).Returns(true);
            mockRepository.Setup(r => r.ContainsKey(hash3)).Returns(false);
            var baseUrl = "http://go";
            var subject = new UrlShortener(mockHashScheme.Object, mockRepository.Object, baseUrl);

            var shortened = subject.Shorten(url);

            shortened.Should().Be("http://go/cccc");
            mockRepository.Verify(r => r.Add(hash3, url));
        }

        // TODO: Redirection unwrapping (avoid multiple redirects and redirect loops)
    }
}
