using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace Durwella.UrlShortening.Tests
{
    public class DefaultHashSchemeTest
    {
        [Test]
        public void ExamplesOfDefaultHash()
        {
            var scheme = new DefaultHashScheme();
            Console.WriteLine(scheme.GetKey("foo"));
            Console.WriteLine(scheme.GetKey("foo", 1));
            Console.WriteLine(scheme.GetKey("goo"));
            Console.WriteLine(scheme.GetKey("http://faslokdfjasf.asdfj.asdf.sa.fs.df.asdfa.vvdhh.aeh/efiab/esbaseieia/vea"));
        }

        [Test]
        public void BasicHashRequirements()
        {
            IHashScheme scheme = new DefaultHashScheme();
            scheme.LengthPreference.Should().BeGreaterThan(0);
            for (int i = 0; i < 2048; i++)
            {
                var s = i.ToString();
                scheme.GetKey(s)
                    .Should().HaveLength(scheme.LengthPreference)
                    .And.NotContain("/")
                    .And.NotContain("+")
                    .And.NotBe(scheme.GetKey(s, 1));
                var alternates = Enumerable.Range(0, 32).Select(j => scheme.GetKey(s, j));
                alternates.Should().OnlyHaveUniqueItems();
            }
        }

        [Test]
        public void ShouldRespectPreferredLength()
        {
            var scheme = new DefaultHashScheme {LengthPreference = 3};
            scheme.GetKey("some long rambling string that is significantly longer than the expected hash value")
                .Should().HaveLength(3);
        }
    }
}
