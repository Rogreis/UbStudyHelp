using System.Xml.Linq;
using UbStandardObjects.Objects;

namespace UbStudyHelp.Classes
{
    public class ParagraphCore : UbStandardObjects.Objects.Paragraph
    {

        private TextWork TextWork = new TextWork();

        public override string Text 
        { 
            get
            {
                return base.Text;
            }
            set
            {
                TextWork.LoadText(value);
                base.Text = TextWork.GetHtml();
            }
        }

        public ParagraphCore(short translationId) : base(translationId)
        {
        }

    }
}
