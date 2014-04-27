using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public delegate void UICheckBox_CheckChanged(UICheckBox checkBox);

    public class UICheckBox : UIControl
    {
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                button.Enabled = value;
                label.Enabled = value;
            }
        }
        public override float LayerDepth
        {
            get { return base.LayerDepth; }
            set
            {
                base.LayerDepth = value;
                button.LayerDepth = value;
                label.LayerDepth = value;
            }
        }
        public bool AutoSize
        {
            get { return label.AutoSize; }
            set { label.AutoSize = value; }
        }
        public override Rectangle PositionAndSize
        {
            get { return new Rectangle(base.PositionAndSize.X, base.PositionAndSize.Y, label.PositionAndSize.Width, label.PositionAndSize.Height); }
            set
            {
                button.PositionAndSize = new Rectangle(0, 0, 0, 0);
                button.PositionAndSize = new Rectangle(value.X, value.Y, (int)(UIParent.defaultFont.MeasureString("X").Y) / 2 + button.PositionAndSize.Width, (int)(UIParent.defaultFont.MeasureString("X").Y) / 2 + button.PositionAndSize.Height);
                label.PositionAndSize = new Rectangle(button.PositionAndSize.Right + 2, value.Y, value.Width, value.Height);
                base.PositionAndSize = new Rectangle(value.X, value.Y, label.PositionAndSize.Right - button.PositionAndSize.Left, Math.Max(button.PositionAndSize.Height, label.PositionAndSize.Height));
            }
        }
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                label.Text = value;
            }
        }
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                base.Visible = value;
                button.Visible = value;
                label.Visible = value;
            }
        }
        private bool check;
        public bool Checked
        {
            get { return check; }
            set
            {
                bool old = check;
                check = value;
                button.Text = (value ? "X" : "");
                if ((check != old) && (CheckChanged != null))
                    CheckChanged(this);
            }
        }

        private UIButton button;
        private UILabel label;

        public event UICheckBox_CheckChanged CheckChanged;

        public UICheckBox(Game game)
            : base(game)
        {
            ControlType = UIControlType.UICheckBox;
            button = new UIButton(game);
            label = new UILabel(game);
            CheckChanged = null;
            check = false;
            button.Action += (UIButton_Action)((btn) =>
            {
                Checked = !Checked;
            });
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            button.LoadAndInitialize();
            label.LoadAndInitialize();
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                button.Draw(ref UIDrawRequests, gameTime);
                label.Draw(ref UIDrawRequests, gameTime);

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
