using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using UbStudyHelp.Classes;
using System.Text;

namespace UbStudyHelpCoreUnitTests
{
    [TestClass]
    public class TextWorkTests
    {
        private const string par1 = "1. &lt;em&gt;Estático —&lt;/em&gt; A Deidade contida em si própria e existente em si.";

        private const string par2 = "Deus é eterna e infinitamente perfeito, Ele não pode conhecer a imperfeição como uma experiência Sua, propriamente, no entanto, Ele compartilha da consciência de toda a experiência de imperfeição, de todas as criaturas em luta, em todos universos evolucionários, dos Filhos Criadores do Paraíso. O toque pessoal e libertador do Deus da perfeição paira sobre os corações e, no Seu circuito, abrange as naturezas de todas as criaturas mortais que ascenderam no universo até o nível do discernimento moral. Desse modo, tanto quanto por meio dos contatos da divina presença, o Pai Universal participa efetivamente da experiência &lt;em&gt;com&lt;/em&gt; a imaturidade e com a imperfeição, na carreira evolutiva de todos os seres mortais no universo inteiro.";
        private const string par3 = "1. A Infinitude de Deus";

        public void Setup()
        {
        }

        [TestMethod]
        public void DecodeParagraph1()
        {
            TextWork work = new TextWork(par1);
            string text = work.DecodedText;
            Assert.IsTrue(text.IndexOf("&lt;") == -1);
            Assert.IsTrue(text.IndexOf("&gt;") == -1);
        }

        [TestMethod]
        public void DecodeParagraph2()
        {
            TextWork work = new TextWork(par2);
            string text = work.DecodedText;
            Assert.IsTrue(text.IndexOf("&lt;") == -1);
            Assert.IsTrue(text.IndexOf("&gt;") == -1);
        }

        [TestMethod]
        public void GetReducedTextFromLarge()
        {
            TextWork work = new TextWork(par2);
            string text = work.GetReducedText();
            Assert.IsTrue(text.Length < par2.Length);
        }

        [TestMethod]
        public void GetReducedTextFromSmall()
        {
            TextWork work = new TextWork(par3);
            string text = work.GetReducedText();
            Assert.IsTrue(text == par3);
        }


        // decided to &lt;span class="Colored"&gt;“talk with my Father who is in heaven”;&lt;/span&gt; and while

        /*
         "<span class=\"SCaps\">", "</span>"
          "<b>", "</b>"
          "<em>", "</em>"
          "<br />"
          "<sup>", "</sup>"
          &lt;span class="Colored"&gt;
          &lt;/span&gt;

    Because of this difference of opinion between Joseph and Mary, Nahor requested permission to lay the whole matter before Jesus. 
        Jesus listened attentively, talked with Joseph, Mary, and a neighbor, Jacob the stone mason, whose son was his favorite playmate, 
        and then, two days later, reported that since there was such a difference of opinion among his parents and advisers, 
        and since he did not feel competent to assume the responsibility for such a decision, not feeling strongly one way or the other, 
        in view of the whole situation, he had finally decided to &lt;span class="Colored"&gt;“talk with my Father who is in heaven”;
        &lt;/span&gt; and while he was not perfectly sure about the answer, he rather felt he should remain at 
        home &lt;span class="Colored"&gt;“with my father and mother,”&lt;/span&gt; adding, 
        &lt;span class="Colored"&gt;“they who love me so much should be able to do more for me and guide me more safely than strangers 
        who can only view my body and observe my mind but can hardly truly know me.”&lt;/span&gt; They all marveled, and Nahor went his way, 
        back to Jerusalem. And it was many years before the subject of Jesus’ going away from home again came up for consideration.


         * */


        [TestMethod]
        public void GetHtmlRemoveColored()
        {
            TextWork work = new TextWork("home &lt;span class=\"Colored\"&gt;“with my father and mother,”&lt;/span&gt; adding");
            string text = work.GetHtml();
            Assert.IsTrue(text == "home “with my father and mother,” adding");
        }

        [TestMethod]
        public void GetHtmlRemoveColoredInTheBeginning()
        {
            TextWork work = new TextWork("&lt;span class=\"Colored\"&gt;“with my father and mother,”&lt;/span&gt; adding");
            string text = work.GetHtml();
            Assert.IsTrue(text == "“with my father and mother,” adding");
        }

        [TestMethod]
        public void GetHtmlRemoveColoredAtEnd()
        {
            TextWork work = new TextWork("home &lt;span class=\"Colored\"&gt;“with my father and mother,”&lt;/span&gt;");
            string text = work.GetHtml();
            Assert.IsTrue(text == "home “with my father and mother,”");
        }

        [TestMethod]
        public void GetTags()
        {
            StringBuilder sb = new StringBuilder("home &lt;span class=\"Colored\"&gt;“with my father and mother,”&lt;/span&gt;");
            sb.Append("<b>Bold</b>");
            sb.Append("<em>Em</em>");
            //sb.Append("<br />NewLine");
            //sb.Append("<sup>Sup</sup>");
            //sb.Append("<span class=\"SCaps\">SpanClassScaps</span>");
            TextWork work = new TextWork(sb.ToString());
            List<UbTextTag> list = work.Tags();
            Assert.IsTrue(list.Count == 3);
            Assert.IsTrue(list[0].Tag == TextTag.Normal);
            Assert.IsTrue(list[1].Tag == TextTag.Bold);
            Assert.IsTrue(list[2].Tag == TextTag.Italic);

            Assert.IsTrue(list[0].Text == "home “with my father and mother,”");
            Assert.IsTrue(list[1].Text == "Bold");
            Assert.IsTrue(list[2].Text == "Em");
        }

        [TestMethod]
        public void GetTagsAtStart()
        {
            StringBuilder sb = new StringBuilder("<b>Bold</b>NoBold");
            TextWork work = new TextWork(sb.ToString());
            List<UbTextTag> list = work.Tags();
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].Tag == TextTag.Bold);
            Assert.IsTrue(list[1].Tag == TextTag.Normal);

            Assert.IsTrue(list[0].Text == "Bold");
            Assert.IsTrue(list[1].Text == "NoBold");
        }

        [TestMethod]
        public void GetTagsAtEnd()
        {
            StringBuilder sb = new StringBuilder("NoBold<b>Bold</b>");
            TextWork work = new TextWork(sb.ToString());
            List<UbTextTag> list = work.Tags();
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].Tag == TextTag.Normal);
            Assert.IsTrue(list[1].Tag == TextTag.Bold);

            Assert.IsTrue(list[0].Text == "NoBold");
            Assert.IsTrue(list[1].Text == "Bold");
        }

        [TestMethod]
        public void GetTagsWithSCaps()
        {
            StringBuilder sb = new StringBuilder("start");
            sb.Append("<span class=\"SCaps\">SpanClassScaps</span>");
            TextWork work = new TextWork(sb.ToString());
            List<UbTextTag> list = work.Tags();
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list[0].Tag == TextTag.Normal);
            Assert.IsTrue(list[0].Text == "startSpanClassScaps");
        }

        [TestMethod]
        public void GetTagsWithSCapsInStart()
        {
            StringBuilder sb = new StringBuilder("<span class=\"SCaps\">SpanClassScaps</span>end");
            TextWork work = new TextWork(sb.ToString());
            List<UbTextTag> list = work.Tags();
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list[0].Tag == TextTag.Normal);
            Assert.IsTrue(list[0].Text == "SpanClassScapsend");
        }

        [TestMethod]
        public void GetReducedTextFromLargeWithHtmlTags()
        {
            string text = "Deus é eterna e infinitamente <b>perfeito</b>, Ele não pode conhecer a imperfeição como uma experiência Sua, propriamente, no entanto, Ele compartilha da consciência de toda a experiência de imperfeição, de todas as criaturas em luta, em todos universos evolucionários, dos Filhos Criadores do Paraíso. O toque pessoal e libertador do Deus da perfeição paira sobre os corações e, no Seu circuito, abrange as naturezas de todas as criaturas mortais que ascenderam no universo até o nível do discernimento moral. Desse modo, tanto quanto por meio dos contatos da divina presença, o Pai Universal participa efetivamente da experiência &lt;em&gt;com&lt;/em&gt; a imaturidade e com a imperfeição, na carreira evolutiva de todos os seres mortais no universo inteiro.";
            TextWork work = new TextWork(text);
            string textReduced = work.GetReducedText();
            Assert.IsTrue(textReduced.Length < text.Length);
            List<UbTextTag> list = work.Tags(null, true);
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list[0].Tag == TextTag.Normal);
        }
    }
}
