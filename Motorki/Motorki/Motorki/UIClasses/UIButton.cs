using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public enum UIButtonState
    {
        Normal,
        NormalPressed,
        Hilite
    }

    public delegate void UIButton_Action(UIButton button);

    public class UIButton : UIControl
    {
        public UIButtonState State
        {
            get;
            private set;
        }

        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < Edges[1].Width + 2 + Edges[2].Width)
                    value.Width = Edges[1].Width + 2 + Edges[2].Width;
                if (value.Height < Edges[0].Height + 2 + Edges[3].Height)
                    value.Height = Edges[0].Height + 2 + Edges[3].Height;
                base.PositionAndSize = value;
            }
        }

        protected Texture2D Textures;
        protected SpriteFont Font;
        protected Rectangle NormalTexture = new Rectangle(100, 0, 37, 30);
        protected Rectangle HiliteTexture = new Rectangle(100, 31, 37, 30);
        protected Rectangle DisabledTexture = new Rectangle(100, 62, 37, 30);
        protected Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 5, 5), new Rectangle(32, 0, 5, 5),
                                                          new Rectangle(0, 25, 5, 5), new Rectangle(32, 25, 5, 5) };
        protected Rectangle[] Edges = new Rectangle[] { new Rectangle(5, 0, 27, 5),
                                                        new Rectangle(0, 5, 5, 20), new Rectangle(32, 5, 5, 20),
                                                        new Rectangle(5, 25, 27, 5) };
        protected Rectangle Middle = new Rectangle(5, 5, 27, 20);

        public event UIButton_Action Action;

        public UIButton(Game game)
            : base(game)
        {
            ControlType = UIControlType.UIButton;
            State = UIButtonState.Normal;
            Action = null;
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChanged;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Textures = UIParent.defaultTextures;
            Font = UIParent.defaultFont;
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);

                State = (mouseOver ? (md.Left ? UIButtonState.NormalPressed : UIButtonState.Hilite) : UIButtonState.Normal);
            }
        }

        void InputEvents_MouseLeftChanged(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);
                bool mlbtnClicked = (md.Left != md.old_Left) && !md.Left;

                State = (mouseOver ? (md.Left ? UIButtonState.NormalPressed : UIButtonState.Hilite) : UIButtonState.Normal);
                if (mouseOver && mlbtnClicked && (Action != null))
                    Action(this);
            }
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                Rectangle texRect = DisabledTexture;
                Color fontColor = Color.Gray;

                if (Enabled)
                {
                    switch (State)
                    {
                        case UIButtonState.Normal:
                            texRect = NormalTexture;
                            fontColor = Color.Black;
                            break;
                        case UIButtonState.NormalPressed:
                            texRect = NormalTexture;
                            fontColor = Color.White;
                            break;
                        case UIButtonState.Hilite:
                            texRect = HiliteTexture;
                            fontColor = Color.Black;
                            break;
                    }
                }
                else
                {
                    texRect = DisabledTexture;
                    fontColor = Color.Gray;
                }

                //do drawing

                //contour
                //corners
                Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, Corners[0].Width, Corners[0].Height), new Rectangle(texRect.X + Corners[0].X, texRect.Y + Corners[0].Y, Corners[0].Width, Corners[0].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[1].Width, 0, Corners[1].Width, Corners[1].Height), new Rectangle(texRect.X + Corners[1].X, texRect.Y + Corners[1].Y, Corners[1].Width, Corners[1].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, PositionAndSize.Height - Corners[2].Height, Corners[2].Width, Corners[2].Height), new Rectangle(texRect.X + Corners[2].X, texRect.Y + Corners[2].Y, Corners[2].Width, Corners[2].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[3].Width, PositionAndSize.Height - Corners[3].Height, Corners[3].Width, Corners[3].Height), new Rectangle(texRect.X + Corners[3].X, texRect.Y + Corners[3].Y, Corners[3].Width, Corners[3].Height), Color.White);
                //edges
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Edges[1].Width, 0, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, Edges[0].Height), new Rectangle(texRect.X + Edges[0].X, texRect.Y + Edges[0].Y, Edges[0].Width, Edges[0].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, Edges[0].Height, Edges[1].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), new Rectangle(texRect.X + Edges[1].X, texRect.Y + Edges[1].Y, Edges[1].Width, Edges[1].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Edges[2].Width, Edges[0].Height, Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), new Rectangle(texRect.X + Edges[2].X, texRect.Y + Edges[2].Y, Edges[2].Width, Edges[2].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Edges[1].Width, PositionAndSize.Height - Edges[3].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, Edges[3].Height), new Rectangle(texRect.X + Edges[3].X, texRect.Y + Edges[3].Y, Edges[3].Width, Edges[3].Height), Color.White);
                //middle
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Edges[1].Width, Edges[0].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), new Rectangle(texRect.X + Middle.X, texRect.Y + Middle.Y, Middle.Width, Middle.Height), Color.White);

                //text
                Rectangle vpText = new Rectangle(PositionAndSize.Left + Edges[1].Width, PositionAndSize.Top + Edges[0].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height);
                Vector2 textSize = Font.MeasureString(Text);
                Vector2 textPos = new Vector2((vpText.Width - textSize.X) / 2, (vpText.Height - textSize.Y) / 2);
                DrawString(ref UIDrawRequests, vpText, Font, Text, textPos, fontColor);

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
