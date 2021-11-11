using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UbStudyHelp.Classes;


namespace UbStudyHelpUnitTest
{
    [TestFixture]
    public class TextWorkTests
    {
        private const string par1 = "1. &lt;em&gt;Estático —&lt;/em&gt; A Deidade contida em si própria e existente em si.";


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DecodeParagraph()
        {
            TextWork work = new TextWork(par1);
            string text= work.DecodedText;
            Assert.IsTrue(text.IndexOf("&lt;") == -1);
            Assert.IsTrue(text.IndexOf("&gt;") == -1);
        }


    }
}
