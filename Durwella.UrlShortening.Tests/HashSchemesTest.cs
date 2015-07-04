using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace Durwella.UrlShortening.Tests
{
    public class HashSchemesTest
    {
        [Test]
        public void VerifyAllHashSchemes()
        {
            var interfaceType = typeof(IHashScheme);
            var hashSchemes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
            foreach (var hashScheme in hashSchemes)
            {
                var scheme = (IHashScheme) Activator.CreateInstance(hashScheme);
                var name = hashScheme.Name;
                PrintExamples(scheme, name);
                ShouldHaveDefaultLengthPreference(scheme, name);
                ShouldHonorLengthPreference(scheme, name);
                ShouldMeetBasicHashRequirements(scheme, name);
            }
        }

        private void PrintExamples(IHashScheme scheme, string name)
        {
            Console.WriteLine(name);
            Console.WriteLine(new String('=', name.Length));
            var examples = new[]
            {
                "a", "b", "c",
                "cat", "dog", "fish",
                "the quick brown fox jumped over the lazy dog",
                "http://example.com"
            };
            foreach (var example in examples)
                Console.WriteLine("{0} => {1}", example, scheme.GetKey(example));
            Console.WriteLine();
        }

        private static void ShouldHaveDefaultLengthPreference(IHashScheme scheme, string name)
        {
            scheme.LengthPreference.Should().BeGreaterThan(0,
                "{0} should have a default LengthPreference", name);
        }

        private static void ShouldHonorLengthPreference(IHashScheme scheme, string name)
        {
            for (int i = 1; i <= 32; i++)
            {
                scheme.LengthPreference = i;
                scheme.GetKey("some long rambling string that is significantly longer than the expected hash value")
                    .Should().HaveLength(i, "{0} should honor length preference", name);
            }
        }

        private static void ShouldMeetBasicHashRequirements(IHashScheme scheme, string name)
        {
            for (int i = 0; i < 2048; i++)
            {
                var s = i.ToString();
                scheme.GetKey(s)
                    .Should().HaveLength(scheme.LengthPreference)
                    .And.NotContain("/")
                    .And.NotContain("+")
                    .And.NotBe(scheme.GetKey(s, 1),
                        "{0} should permute hash", name);
                var alternates = Enumerable.Range(0, 32).Select(j => scheme.GetKey(s, j));
                alternates.Should().OnlyHaveUniqueItems(
                    "{0} should return unique hashes", name);
            }
        }
    }
}
