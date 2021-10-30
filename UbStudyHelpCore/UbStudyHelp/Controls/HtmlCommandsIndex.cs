using System;
using System.Collections.Generic;
using System.Text;
using UbStudyHelp.Classes;

namespace UbStudyHelp.Controls
{
    public class HtmlCommandsIndex : HtmlCommands
    {



        public override string HtmlLine(TOC_Entry entry, bool shouldHighlightText)
        {
            throw new NotImplementedException();
        }

        protected override void HtmlSingleLine(StringBuilder sb, string LeftText, string RightText, bool Ident = false)
        {
            throw new NotImplementedException();
        }

        protected override string IdentedLine(string text)
        {
            throw new NotImplementedException();
        }
    }
}
