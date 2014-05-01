using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public class UILabel : UIControl
    {
        public bool AutoSize { get; set; }
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < 1)
                    value.Width = 1;
                if (value.Height < 1)
                    value.Height = 1;
                base.PositionAndSize = value;
            }
        }
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Vector2 textMetrics = (Font ?? UIParent.defaultFont).MeasureString(value);
                PositionAndSize = new Rectangle(PositionAndSize.X, PositionAndSize.Y, (int)textMetrics.X, (int)textMetrics.Y);
            }
        }

        public UILabel(Game game)
            : base(game)
        {
            ControlType = UIControlType.UILabel;
            AutoSize = true;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Font = UIParent.defaultFont;
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                if (Text != "")
                {
                    Color fontColor = Color.Black;
                    if (!Enabled)
                        fontColor = Color.Gray;

                    Rectangle vp = PositionAndSize;
                    if (AutoSize)
                    {
                        Vector2 textMetrics = Font.MeasureString(Text);
                        vp.Width = (int)textMetrics.X;
                        vp.Height = (int)textMetrics.Y;
                    }
                    DrawString(ref UIDrawRequests, vp, Font, Text, Vector2.Zero, fontColor);
                }

                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
