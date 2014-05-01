using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        UIScrollBar, UIListScrollBar
    }

    public abstract class UIControl
    {
        public UIControlType ControlType
        {
            get;
            protected set;
        }
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
        private float layerDepth;
        public virtual float LayerDepth
        {
            get { return layerDepth; }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                if (value > 1.0f)
                    value = 1.0f;
                layerDepth = value;
            }
        }

        public Texture2D Textures;
        public SpriteFont Font;

        protected Game game;

        public UIControl(Game game)
        {
            this.game = game;
            if (UIParent.UI == null)
                throw new UIParentNotExistsException();
            ControlType = UIControlType.UIControl;
            enabled = true;
            visible = true;
            name = "";
            text = "";
            layerDepth = 1.0f;
            PosAndSize = new Rectangle(0, 0, 0, 0);
        }

        public virtual void LoadAndInitialize() { }
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
