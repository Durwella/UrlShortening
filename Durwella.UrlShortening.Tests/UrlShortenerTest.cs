using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Durwella.UrlShortening.Tests
{
    public class UrlShortenerTest
    {
        UrlShortener _subject;
        private Mock<IUrlUnwrapper> _mockUnwrapper;
        private Mock<IHashScheme> _mockHashScheme;
        private Mock<IAliasRepository> _mockRepository;

        [SetUp]
        public void SetupSubject()
        {
            var baseUrl = "http://goto";
            _mockHashScheme = new Mock<IHashScheme>();
            _mockRepository = new Mock<IAliasRepository>();
            _mockUnwrapper = new Mock<IUrlUnwrapper>();
            _mockUnwrapper.Setup(u => u.GetDirectUrl(It.IsAny<string>())).Returns<string>(s => s);
            _subject = new UrlShortener(baseUrl, _mockRepository.Object, _mockHashScheme.Object, _mockUnwrapper.Object);
        }

        [Test]
        public void ShouldSaveAndReturnHash()
        {
            var url = "http://example.com/foo/bar";
            var hash = "123abc";
            _mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash);

            var shortened = _subject.Shorten(url);

            shortened.Should().Be("http://goto/123abc");
            _mockRepository.Verify(r => r.Add(hash, url));
        }

        [Test]
        public void ShouldReturnExistingHash()
        {
            var url = "http://example.com/a/b/c";
            var hash = "abc";
            _mockRepository.Setup(r => r.ContainsValue(url)).Returns(true);
            _mockRepository.Setup(r => r.GetKey(url)).Returns(hash);

            var shortened = _subject.Shorten(url);

            shortened.Should().Be("http://goto/abc");
            _mockRepository.Verify(r => r.Add(hash, url), Times.Never());
            _mockHashScheme.Verify(h => h.GetKey(url), Times.Never());
        }

        [Test]
        public void HandleHashCollision()
        {
            var url = "http://example.com/hash/collision";
            var hash1 = "aaaa";
            var hash2 = "bbbb";
            var hash3 = "cccc";
            _mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash1);
            _mockHashScheme.Setup(h => h.GetKey(url, 0)).Returns(hash1);
            _mockHashScheme.Setup(h => h.GetKey(url, 1)).Returns(hash2);
            _mockHashScheme.Setup(h => h.GetKey(url, 2)).Returns(hash3);
            _mockRepository.Setup(r => r.ContainsValue(url)).Returns(false);
            _mockRepository.Setup(r => r.ContainsKey(hash1)).Returns(true);
            _mockRepository.Setup(r => r.ContainsKey(hash2)).Returns(true);
            _mockRepository.Setup(r => r.ContainsKey(hash3)).Returns(false);

            var shortened = _subject.Shorten(url);

            shortened.Should().Be("http://goto/cccc");
            _mockRepository.Verify(r => r.Add(hash3, url));
        }

        [Test]
        public void ShouldUnwrapUrlBeforeShortening()
        {
            var givenUrl = "http://t.co/123";
            var url = "http://example.com/abc";
            var hash = "foo";
            _mockUnwrapper.Setup(u => u.GetDirectUrl(givenUrl)).Returns(url);
            _mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash);

            var shortened = _subject.Shorten(givenUrl);

            shortened.Should().Be("http://goto/foo");
            _mockRepository.Verify(r => r.Add(hash, url));
        }
    }
}
