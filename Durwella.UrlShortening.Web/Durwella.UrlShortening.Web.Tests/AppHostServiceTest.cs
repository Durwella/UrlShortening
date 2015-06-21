using Durwella.UrlShortening.Web.ServiceInterface;
using Durwella.UrlShortening.Web.ServiceModel;
using FluentAssertions;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Testing;
using ServiceStack.Web;
using System;

namespace Durwella.UrlShortening.Web.Tests
{
    public class AppHostServiceTest
    {
        class FakeAliasRepository : MemoryAliasRepository { }
        class FakeHashScheme : IHashScheme
        {
            public int LengthPreference { get; set; }
            public string GetKey(string value) { return "123"; }
            public string GetKey(string value, int permutation) { throw new NotImplementedException(); }
        }
        class FakeUrlUnwrapper : IUrlUnwrapper
        {
            public string GetDirectUrl(string url)
            {
                return url.Replace("ex.ampl", "example.com");
            }
        }

        private readonly ServiceStackHost _appHost;

        public AppHostServiceTest()
        {
            _appHost = new BasicAppHost(typeof(UrlShorteningService).Assembly)
            {
                ConfigureContainer = container =>
                {
                    container.Register<IResolver>(container);
                    container.RegisterAs<FakeAliasRepository, IAliasRepository>();
                    container.RegisterAs<FakeHashScheme, IHashScheme>();
                    container.RegisterAs<FakeUrlUnwrapper, IUrlUnwrapper>();
                    container.Register(new UrlShortener("http://a.b.c", container.Resolve<IAliasRepository>(), container.Resolve<IHashScheme>(), container.Resolve<IUrlUnwrapper>()));
                    // TODO: Base url via owin?
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _appHost.Dispose();
        }

        [Test]
        public void HelloServiceAsCanary()
        {
            var service = _appHost.Container.Resolve<HelloService>();

            var response = (HelloResponse)service.Any(new Hello { Name = "World" });

            Assert.That(response.Result, Is.EqualTo("Hello, World!"));
        }

        class MockRequest : MockHttpRequest, IRequest
        {
            string IRequest.AbsoluteUri { get { return "http://a.b.c/shorten?Url=http%3A%2F%2Fex.ample%2fone"; } }
        }

        [Test]
        public void PostShouldCreateNewShortUrl()
        {
            var service = _appHost.Container.Resolve<UrlShorteningService>();
            service.Request = new MockRequest(); // For the Absolute Uri
            var givenUrl = "http://ex.ampl/one";

            var response = service.Post(new ShortUrlRequest { Url = givenUrl });

            response.Shortened.Should().Be("http://a.b.c/123");
        }

        [Test]
        public void PostShouldCreateCustomShortUrl()
        {
            var service = _appHost.Container.Resolve<UrlShorteningService>();
            service.Request = new MockRequest(); // For the Absolute Uri
            var givenUrl = "http://ex.ampl/two";

            var response = service.Post(new CustomShortUrlRequest { Url = givenUrl, CustomPath = "2"});

            response.Shortened.Should().Be("http://a.b.c/2");
        }
    }
}
