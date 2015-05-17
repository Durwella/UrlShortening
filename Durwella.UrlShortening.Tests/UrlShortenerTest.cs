using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Durwella.UrlShortening.Tests
{
    [TestClass]
    public class UrlShortenerTest
    {
        [TestMethod]
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
    }
}
