
namespace Motorki.UIClasses
{
    public class UITaggedValue
    {
        public string Text { get; set; }
        public object Tag { get; set; }

        public UITaggedValue(string text = "", object tag = null)
        {
            Text = text;
            Tag = tag;
        }

        public static implicit operator UITaggedValue(string text)
        {
            return new UITaggedValue(text);
        }

        public override string ToString()
        {
            return "{Text=" + Text + "; Tag=" + (Tag == null ? "null" : Tag.ToString()) + "}";
        }
    }
}
