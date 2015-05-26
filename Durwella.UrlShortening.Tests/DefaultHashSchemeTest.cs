using NUnit.Framework;
using System;

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

    }
}
