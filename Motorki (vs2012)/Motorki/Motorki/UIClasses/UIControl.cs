using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public enum UIControlType
    {
        UIControl,
        UIButton, UIComboButton, UIScrollBarButton, UIScrollBarThumb,
        UITextBox, UIComboTextBox,
        UICheckBox,
        UIComboBox,
        UILabel,
        UIListBox, UIComboListBox,
        UIScrollBar, UIListScrollBar,
        UIImage,
        UIProgress,
        UIKeyPicker
    }

    public class UICharMap
    {
        public static char ToChar(Keys k)
        {
            switch (k)
            {
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    return k.ToString()[0];
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    return k.ToString()[k.ToString().Length - 1];
                case Keys.OemPipe:
                case Keys.OemBackslash: return '\\';
                case Keys.OemCloseBrackets: return ']';
                case Keys.Decimal:
                case Keys.OemComma: return ',';
                case Keys.OemMinus:
                case Keys.Subtract: return '-';
                case Keys.OemOpenBrackets: return '[';
                case Keys.OemPeriod: return '.';
                case Keys.OemPlus:
                case Keys.Add: return '+';
                case Keys.OemQuestion:
                case Keys.Divide: return '/';
                case Keys.Multiply: return '*';
                case Keys.OemQuotes: return '\'';
                case Keys.OemSemicolon: return ';';
                case Keys.OemTilde: return '~';
                default: return '\0';
            }
        }
    }

    public abstract class UIControl
    {
        public UIControlType ControlType
        {
            get;
            protected set;
        }
        public bool isLoaded { get; private set; }
        private bool visible;
        public virtual bool Visible { get { return visible; } set { visible = value; } }
        private bool enabled;
        public virtual bool Enabled { get { return enabled; } set { enabled = value; } }
        private Rectangle PosAndSize;
        public virtual Rectangle PositionAndSize { get { return PosAndSize; } set { PosAndSize = value; } }
        private string text;
        public virtual string Text { get { return text; } set { text = value; } }
        private string name;
        public string Name { get { return name; } set { name = value; } }
        private int layerDepth;
        public virtual int LayerDepth
        {
            get { return layerDepth; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > game.layerTargets.Length - 1)
                    value = game.layerTargets.Length - 1;
                layerDepth = value;
            }
        }

        public Texture2D Textures;
        private SpriteFont font;
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                if (value != null)
                {
                    if (font == null)
                        font = value;
                }
                else
                    font = value;
            }
        }

        protected MotorkiGame game;

        public UIControl(MotorkiGame game)
        {
            this.game = game;
            if (UIParent.UI == null)
                throw new UIParentNotExistsException();
            ControlType = UIControlType.UIControl;
            isLoaded = false;
            enabled = true;
            visible = true;
            name = "";
            text = "";
            layerDepth = 1;
            PosAndSize = new Rectangle(0, 0, 0, 0);
        }

        public virtual void Destroy() { }
        public virtual void LoadAndInitialize() { }
        public void MarkLoaded() { isLoaded = true; }
        public virtual void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime) { }

        protected void Draw(ref List<UIDrawRequest> UIDrawRequests, Rectangle? Viewport, Texture2D texture, Rectangle destination, Rectangle? source, Color filterColor)
        {
            UIDrawRequests.Add(new UIDrawRequest(Viewport, texture, destination, source, filterColor, LayerDepth));
        }

        protected void DrawString(ref List<UIDrawRequest> UIDrawRequests, Rectangle? Viewport, SpriteFont font, string text, Vector2 position, Color fontColor)
        {
            UIDrawRequests.Add(new UIDrawRequest(Viewport, font, text, position, fontColor, layerDepth));
        }
    }
}
