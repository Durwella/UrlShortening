using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Durwella.UrlShortening.Tests
{
    public class UrlShortenerTest
    {
        private UrlShortener _subject;
        private const string BaseUrl = "http://goto";
        private Mock<IUrlUnwrapper> _mockUnwrapper;
        private Mock<IHashScheme> _mockHashScheme;
        private Mock<IAliasRepository> _mockRepository;

        [SetUp]
        public void SetupSubject()
        {
            _mockHashScheme = new Mock<IHashScheme>();
            _mockRepository = new Mock<IAliasRepository>();
            _mockUnwrapper = new Mock<IUrlUnwrapper>();
            _mockUnwrapper.Setup(u => u.GetDirectUrl(It.IsAny<string>())).Returns<string>(s => s);
            _subject = new UrlShortener(BaseUrl, _mockRepository.Object, _mockHashScheme.Object, _mockUnwrapper.Object);
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

        [Test]
        public void CanShortenWithCustomUrl()
        {
            _subject = new UrlShortener(BaseUrl, new MemoryAliasRepository(), _mockHashScheme.Object, _mockUnwrapper.Object);
            var url = "http://example.com/1/2/3";
            var hash = "asdf";
            _mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash);
            _subject.Shorten(url);
            var customHash = "123";

            string customShortened = _subject.ShortenWithCustomHash(url, customHash);

            _subject.Repository.ContainsKey(hash).Should().BeFalse();
            _subject.Repository.GetValue(customHash).Should().Be(url);
            customShortened.Should().Be("http://goto/123");
        }

        [Test]
        public void ShouldThrowIfCustomAlreadyInUse()
        {
            _subject = new UrlShortener(BaseUrl, new MemoryAliasRepository(), _mockHashScheme.Object, _mockUnwrapper.Object);
            var url = "http://example.com/1/2/3";
            var hash = "asdf";
            _mockHashScheme.Setup(h => h.GetKey(url)).Returns(hash);
            _subject.Shorten(url);
            var customHash = "123";
            _subject.Repository.Add(customHash, "existing");
            var thrown = false;

            try
            {
                _subject.ShortenWithCustomHash(url, customHash);
            }
            catch (ArgumentException exception)
            {
                exception.Message.Should().Contain("already").And.Contain("use");
                thrown = true;
            }

            thrown.Should().BeTrue("Should throw exception for existing key");
            _subject.Repository.GetValue(customHash).Should().Be("existing");
            _subject.Repository.GetValue(hash).Should().Be(url);
        }

        [Test]
        public void UseUnwrappedUrlForSettingCustomHash()
        {
            var givenUrl = "http://t.co/123";
            var url = "http://example.com/abc";
            var hash = "custom";
            _mockUnwrapper.Setup(u => u.GetDirectUrl(givenUrl)).Returns(url);

            var shortened = _subject.ShortenWithCustomHash(givenUrl, hash);

            shortened.Should().Be("http://goto/custom");
            _mockRepository.Verify(r => r.Add(hash, url));
        }
    }
}
