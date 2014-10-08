using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public delegate void UIScrollBar_ValueChanged(UIScrollBar scrollBar, int oldValue);

    public class UIScrollBar : UIControl
    {
        private UIScrollBarButton btnPlus, btnMinus;
        private UIScrollBarThumb thumb;

        private bool orientation; //true - vertical, false - horizontal
        public bool IsVertical
        {
            get { return orientation; }
            set
            {
                orientation = value;
                btnMinus.IsMinusButton = true;
                btnPlus.IsPlusButton = true;
            }
        }
        public bool IsHorizontal { get { return !orientation; } set { IsVertical = !value; } }
        private int value;
        public int Value
        {
            get { return value; }
            set
            {
                if (value < MinimalValue)
                    value = MinimalValue;
                if (value > MaximalValue)
                    value = MaximalValue;
                if (this.value != value)
                {
                    int oldValue = this.value;
                    this.value = value;
                    if (ValueChanged != null)
                        ValueChanged(this, oldValue);
                }
            }
        }
        private int valuesOnScreen;
        /// <summary>
        /// number of elements that fit on screen
        /// </summary>
        public int ValuesOnScreen
        {
            get { return valuesOnScreen; }
            set
            {
                if ((value < 1) || (value >= MaximalValue - MinimalValue + 1))
                    valuesOnScreen = MaximalValue - MinimalValue;
                else
                    valuesOnScreen = value;
            }
        }
        private int minval, maxval;
        public int MinimalValue
        {
            get { return minval; }
            set
            {
                if (value > maxval)
                {
                    minval = maxval;
                    maxval = value;
                }
                else
                    minval = value;
                Value = Value;
                ValuesOnScreen = ValuesOnScreen;
            }
        }
        public int MaximalValue
        {
            get { return maxval; }
            set
            {
                if (value < minval)
                {
                    maxval = minval;
                    minval = value;
                }
                else
                    maxval = value;
                Value = Value;
                ValuesOnScreen = ValuesOnScreen;
            }
        }
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                btnMinus.Enabled = value;
                btnPlus.Enabled = value;
                thumb.Enabled = value;
            }
        }
        private bool Hilited { get; set; }
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (IsVertical)
                {
                    if (value.Height < btnMinus.NormalTexture.Height + btnPlus.NormalTexture.Height + thumb.Edges[0].Height + thumb.Edges[3].Height + 1)
                        value.Height = btnMinus.NormalTexture.Height + btnPlus.NormalTexture.Height + thumb.Edges[0].Height + thumb.Edges[3].Height + 1;
                    value.Width = btnMinus.NormalTexture.Width;
                }
                else
                {
                    if (value.Width < btnMinus.NormalTexture.Width + btnPlus.NormalTexture.Width + thumb.Edges[1].Width + thumb.Edges[2].Width + 1)
                        value.Width = btnMinus.NormalTexture.Width + btnPlus.NormalTexture.Width + thumb.Edges[1].Width + thumb.Edges[2].Width + 1;
                    value.Height = btnMinus.NormalTexture.Height;
                }

                btnMinus.PositionAndSize = new Rectangle(value.X, value.Y, btnMinus.NormalTexture.Width, btnMinus.NormalTexture.Height);
                btnPlus.PositionAndSize = new Rectangle((IsVertical ? value.X : value.Right - btnPlus.NormalTexture.Width), (IsVertical ? value.Bottom - btnPlus.NormalTexture.Height : value.Y), btnPlus.NormalTexture.Width, btnPlus.NormalTexture.Height);

                base.PositionAndSize = value;
            }
        }
        public override int LayerDepth
        {
            get { return base.LayerDepth; }
            set
            {
                base.LayerDepth = value;
                btnMinus.LayerDepth = value;
                btnPlus.LayerDepth = value;
                thumb.LayerDepth = value;
            }
        }

        public Rectangle NormalBackground = new Rectangle(88, 94, 21, 21);
        public Rectangle HiliteBackground = new Rectangle(88, 116, 21, 21);
        public Rectangle DisabledBackground = new Rectangle(88, 138, 21, 21);
        public Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 4, 4), new Rectangle(17, 0, 4, 4),
                                                          new Rectangle(0, 17, 4, 4), new Rectangle(17, 17, 4, 4) };
        public Rectangle[] Edges = new Rectangle[] { new Rectangle(4, 0, 13, 4),
                                                        new Rectangle(0, 4, 4, 13), new Rectangle(17, 4, 4, 13),
                                                        new Rectangle(4, 17, 13, 4) };
        public Rectangle Middle = new Rectangle(4, 4, 13, 13);

        public event UIScrollBar_ValueChanged ValueChanged;

        public UIScrollBar(MotorkiGame game, bool vertical = true)
            : base(game)
        {
            ControlType = UIControlType.UIScrollBar;
            orientation = vertical;
            btnMinus = new UIScrollBarButton(game, this, false);
            btnPlus = new UIScrollBarButton(game, this, true);
            thumb = new UIScrollBarThumb(game, this);
            ValueChanged = null;
            PositionAndSize = new Rectangle(0, 0, 0, 0);
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChangedOrRepeat;
            InputEvents.MouseLeftRepeat += InputEvents_MouseLeftChangedOrRepeat;
        }

        public override void Destroy()
        {
            InputEvents.MouseMoved -= InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged -= InputEvents_MouseLeftChangedOrRepeat;
            InputEvents.MouseLeftRepeat -= InputEvents_MouseLeftChangedOrRepeat;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Textures = UIParent.defaultTextures;

            btnMinus.LoadAndInitialize();
            btnPlus.LoadAndInitialize();
            thumb.LoadAndInitialize();
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            if (Visible && Enabled)
            {
                Hilited = false;

                btnMinus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                btnPlus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                thumb.InputEvents_MouseMovedLeftChangedOrRepeat(md);

                if (btnMinus.Handled || btnPlus.Handled || thumb.Handled)
                    return;

                bool mouseOver = ((md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom));

                Hilited = mouseOver;
            }
        }

        void InputEvents_MouseLeftChangedOrRepeat(MouseData md)
        {
            if (Visible && Enabled)
            {
                btnMinus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                if (btnMinus.Handled)
                    return;

                btnPlus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                if (btnPlus.Handled)
                    return;

                thumb.InputEvents_MouseMovedLeftChangedOrRepeat(md);
                if (thumb.Handled)
                    return;

                //background handling
                bool mouseOver = ((md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom));

                if (mouseOver && md.Left)
                {
                    int scrollSpaceSize = (IsVertical ? PositionAndSize.Height - btnMinus.PositionAndSize.Height - btnPlus.PositionAndSize.Height
                                                      : PositionAndSize.Width - btnMinus.PositionAndSize.Width - btnPlus.PositionAndSize.Width);
                    int thumbLength = (int)(scrollSpaceSize * ((ValuesOnScreen + 0.5) / (double)(MaximalValue - MinimalValue + 1)));
                    if (thumbLength < (IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1))
                        thumbLength = (IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1);
                    int scrollPositionsSize = scrollSpaceSize - thumbLength;
                    double scrollPositionSize = scrollPositionsSize / (double)(MaximalValue - MinimalValue);
                    thumb.PositionAndSize = (IsVertical ? new Rectangle(PositionAndSize.X, btnMinus.PositionAndSize.Bottom + (int)((Value - MinimalValue) * scrollPositionSize), PositionAndSize.Width, thumbLength)
                                                        : new Rectangle(btnMinus.PositionAndSize.Right + (int)((Value - MinimalValue) * scrollPositionSize), PositionAndSize.Y, thumbLength, PositionAndSize.Height));
                    bool minusSide = (IsVertical ? (md.Y < thumb.PositionAndSize.Top) : (md.X < thumb.PositionAndSize.Left));
                    Value += (int)(minusSide ? Math.Floor(-(MaximalValue - MinimalValue + 1) / 10.0) : Math.Ceiling((MaximalValue - MinimalValue + 1) / 10.0));
                }

                Hilited = mouseOver;
            }
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                Rectangle texRect = DisabledBackground;

                if (Enabled)
                {
                    if (Hilited)
                        texRect = HiliteBackground;
                    else
                        texRect = NormalBackground;
                }

                //do drawing

                //background
                //corners
                Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, Corners[0].Width, Corners[0].Height), new Rectangle(texRect.X + Corners[0].X, texRect.Y + Corners[0].Y, Corners[0].Width, Corners[0].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[1].Width, 0, Corners[1].Width, Corners[1].Height), new Rectangle(texRect.X + Corners[1].X, texRect.Y + Corners[1].Y, Corners[1].Width, Corners[1].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, PositionAndSize.Height - Corners[2].Height, Corners[2].Width, Corners[2].Height), new Rectangle(texRect.X + Corners[2].X, texRect.Y + Corners[2].Y, Corners[2].Width, Corners[2].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[3].Width, PositionAndSize.Height - Corners[3].Height, Corners[3].Width, Corners[3].Height), new Rectangle(texRect.X + Corners[3].X, texRect.Y + Corners[3].Y, Corners[3].Width, Corners[3].Height), Color.White);
                //edges
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Corners[0].Width, 0, PositionAndSize.Width - Corners[0].Width - Corners[1].Width, Edges[0].Height), new Rectangle(texRect.X + Edges[0].X, texRect.Y + Edges[0].Y, Edges[0].Width, Edges[0].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, Corners[0].Height, Edges[1].Width, PositionAndSize.Height - Corners[0].Height - Corners[2].Height), new Rectangle(texRect.X + Edges[1].X, texRect.Y + Edges[1].Y, Edges[1].Width, Edges[1].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[1].Width, Corners[1].Height, Edges[2].Width, PositionAndSize.Height - Corners[1].Height - Corners[3].Height), new Rectangle(texRect.X + Edges[2].X, texRect.Y + Edges[2].Y, Edges[2].Width, Edges[2].Height), Color.White);
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Corners[2].Width, PositionAndSize.Height - Corners[2].Height, PositionAndSize.Width - Corners[2].Width - Corners[3].Width, Edges[3].Height), new Rectangle(texRect.X + Edges[3].X, texRect.Y + Edges[3].Y, Edges[3].Width, Edges[3].Height), Color.White);
                //middle
                Draw(ref UIDrawRequests, null, Textures, new Rectangle(Edges[1].Width, Edges[0].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), new Rectangle(texRect.X + Middle.X, texRect.Y + Middle.Y, Middle.Width, Middle.Height), Color.White);

                //buttons
                btnMinus.Draw(ref UIDrawRequests, gameTime);
                btnPlus.Draw(ref UIDrawRequests, gameTime);

                //thumb
                thumb.Draw(ref UIDrawRequests, gameTime);

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }

        #region UIScrollBarButton class

        private class UIScrollBarButton : UIControl
        {
            public UIButtonState State { get; set; }
            private bool plusButton; //true - plus button, false - minus button
            public bool IsPlusButton
            {
                get { return plusButton; }
                set
                {
                    plusButton = value;
                    if (Parent.IsVertical)
                    {
                        NormalTexture = (value ? new Rectangle(0, 138, 21, 21) : new Rectangle(0, 94, 21, 21));
                        NormalPressedTexture = (value ? new Rectangle(44, 138, 21, 21) : new Rectangle(44, 94, 21, 21));
                        HiliteTexture = (value ? new Rectangle(22, 138, 21, 21) : new Rectangle(22, 94, 21, 21));
                        DisabledTexture = (value ? new Rectangle(66, 138, 21, 21) : new Rectangle(66, 94, 21, 21));
                    }
                    else
                    {
                        NormalTexture = (value ? new Rectangle(0, 182, 21, 21) : new Rectangle(0, 160, 21, 21));
                        NormalPressedTexture = (value ? new Rectangle(44, 182, 21, 21) : new Rectangle(44, 160, 21, 21));
                        HiliteTexture = (value ? new Rectangle(22, 182, 21, 21) : new Rectangle(22, 160, 21, 21));
                        DisabledTexture = (value ? new Rectangle(66, 182, 21, 21) : new Rectangle(66, 160, 21, 21));
                    }
                }
            }
            public bool IsMinusButton { get { return !plusButton; } set { IsPlusButton = !value; } }

            private UIScrollBar Parent;
            public bool Handled { get; private set; }

            public Rectangle NormalTexture = new Rectangle(0, 0, 21, 21);
            public Rectangle NormalPressedTexture = new Rectangle(0, 0, 21, 21);
            public Rectangle HiliteTexture = new Rectangle(0, 0, 21, 21);
            public Rectangle DisabledTexture = new Rectangle(0, 0, 21, 21);

            public UIScrollBarButton(MotorkiGame game, UIScrollBar parent, bool plusButton)
                : base(game)
            {
                ControlType = UIControlType.UIScrollBarButton;
                Parent = parent;
                State = UIButtonState.Normal;
                IsPlusButton = plusButton;
            }

            public override void LoadAndInitialize()
            {
                base.LoadAndInitialize();

                Textures = UIParent.defaultTextures;
            }

            public void InputEvents_MouseMovedLeftChangedOrLeftRepeated(MouseData md)
            {
                Handled = false;

                if (Visible && Enabled)
                {
                    //check mouse
                    bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);

                    State = (mouseOver ? (md.Left ? UIButtonState.NormalPressed
                                                  : UIButtonState.Hilite)
                                       : UIButtonState.Normal);
                    Handled = mouseOver;
                    if (State == UIButtonState.NormalPressed)
                        Parent.Value = Parent.Value + (IsPlusButton ? 1 : -1);
                }
            }

            public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
            {
                if (Visible)
                {
                    Rectangle texRect = DisabledTexture;

                    if (Enabled)
                    {
                        switch (State)
                        {
                            case UIButtonState.Normal:
                                texRect = NormalTexture;
                                break;
                            case UIButtonState.NormalPressed:
                                texRect = NormalPressedTexture;
                                break;
                            case UIButtonState.Hilite:
                                texRect = HiliteTexture;
                                break;
                        }
                    }

                    //do drawing

                    Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, PositionAndSize.Width, PositionAndSize.Height), texRect, Color.White);

                    //end drawing

                    //draw child controls
                    base.Draw(ref UIDrawRequests, gameTime);
                }
            }
        }

        #endregion

        #region UIScrollBarThumb class

        private class UIScrollBarThumb : UIControl
        {
            public UIButtonState State { get; set; }

            private UIScrollBar Parent;
            public bool Handled { get; private set; }

            public Rectangle NormalTexture = new Rectangle(0, 116, 21, 21);
            public Rectangle NormalPressedTexture = new Rectangle(44, 116, 21, 21);
            public Rectangle HiliteTexture = new Rectangle(22, 116, 21, 21);
            public Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 4, 4), new Rectangle(17, 0, 4, 4),
                                                           new Rectangle(0, 17, 4, 4), new Rectangle(17, 17, 4, 4) };
            public Rectangle[] Edges = new Rectangle[] { new Rectangle(4, 0, 13, 4),
                                                         new Rectangle(0, 4, 4, 13), new Rectangle(17, 4, 4, 13),
                                                         new Rectangle(4, 17, 13, 4) };
            public Rectangle Middle = new Rectangle(4, 4, 13, 13);

            public UIScrollBarThumb(MotorkiGame game, UIScrollBar parent)
                : base(game)
            {
                ControlType = UIControlType.UIScrollBarThumb;
                Parent = parent;
                State = UIButtonState.Normal;
            }

            public override void LoadAndInitialize()
            {
                base.LoadAndInitialize();

                Textures = UIParent.defaultTextures;
            }

            public void InputEvents_MouseMovedLeftChangedOrRepeat(MouseData md)
            {
                Handled = false;

                if (Visible && Enabled)
                {
                    //check mouse
                    bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);

                    UIButtonState old_State = State;
                    State = (mouseOver ? (md.Left ? UIButtonState.NormalPressed
                                                  : UIButtonState.Hilite)
                                       : UIButtonState.Normal);
                    Handled = mouseOver;
                    if ((State == UIButtonState.NormalPressed) && (old_State == State) && (Parent.IsVertical ? (md.Y != md.old_Y) : (md.X != md.old_X)))
                    {
                        int scrollSpaceSize = (Parent.IsVertical ? Parent.PositionAndSize.Height - Parent.btnMinus.PositionAndSize.Height - Parent.btnPlus.PositionAndSize.Height
                                                             : Parent.PositionAndSize.Width - Parent.btnMinus.PositionAndSize.Width - Parent.btnPlus.PositionAndSize.Width);
                        int thumbLength = (int)(scrollSpaceSize * ((Parent.ValuesOnScreen + 0.5) / (double)(Parent.MaximalValue - Parent.MinimalValue + 1)));
                        if (thumbLength < (Parent.IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1))
                            thumbLength = (Parent.IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1);
                        int scrollPositionsSize = scrollSpaceSize - thumbLength;
                        double scrollPositionSize = scrollPositionsSize / (double)(Parent.MaximalValue - Parent.MinimalValue);
                        Vector2 DragDistance = new Vector2(md.X - Parent.btnMinus.PositionAndSize.Right - thumbLength / 4, md.Y - Parent.btnMinus.PositionAndSize.Bottom - thumbLength / 4);
                        Parent.Value = (int)(Parent.IsVertical ? Math.Truncate(DragDistance.Y / scrollPositionSize)
                                                               : Math.Truncate(DragDistance.X / scrollPositionSize));
                    }
                }
            }

            public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
            {
                if (Visible && Enabled)
                {
                    int scrollSpaceSize = (Parent.IsVertical ? Parent.PositionAndSize.Height - Parent.btnMinus.PositionAndSize.Height - Parent.btnPlus.PositionAndSize.Height
                                                             : Parent.PositionAndSize.Width - Parent.btnMinus.PositionAndSize.Width - Parent.btnPlus.PositionAndSize.Width);
                    int thumbLength = (int)(scrollSpaceSize * ((Parent.ValuesOnScreen + 0.5) / (double)(Parent.MaximalValue - Parent.MinimalValue + 1)));
                    if (thumbLength < (Parent.IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1))
                        thumbLength = (Parent.IsVertical ? Edges[0].Height + Edges[3].Height + 1 : Edges[1].Width + Edges[2].Width + 1);
                    int scrollPositionsSize = scrollSpaceSize - thumbLength;
                    double scrollPositionSize = scrollPositionsSize / (double)(Parent.MaximalValue - Parent.MinimalValue);
                    PositionAndSize = (Parent.IsVertical ? new Rectangle(Parent.PositionAndSize.X, Parent.btnMinus.PositionAndSize.Bottom + (int)((Parent.Value - Parent.MinimalValue) * scrollPositionSize), Parent.PositionAndSize.Width, thumbLength)
                                                         : new Rectangle(Parent.btnMinus.PositionAndSize.Right + (int)((Parent.Value - Parent.MinimalValue) * scrollPositionSize), Parent.PositionAndSize.Y, thumbLength, Parent.PositionAndSize.Height));

                    Rectangle texRect = NormalTexture;

                    switch (State)
                    {
                        case UIButtonState.Normal:
                            texRect = NormalTexture;
                            break;
                        case UIButtonState.NormalPressed:
                            texRect = NormalPressedTexture;
                            break;
                        case UIButtonState.Hilite:
                            texRect = HiliteTexture;
                            break;
                    }

                    //do drawing

                    //corners
                    Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, Corners[0].Width, Corners[0].Height), new Rectangle(texRect.X + Corners[0].X, texRect.Y + Corners[0].Y, Corners[0].Width, Corners[0].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[1].Width, 0, Corners[1].Width, Corners[1].Height), new Rectangle(texRect.X + Corners[1].X, texRect.Y + Corners[1].Y, Corners[1].Width, Corners[1].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, PositionAndSize.Height - Corners[2].Height, Corners[2].Width, Corners[2].Height), new Rectangle(texRect.X + Corners[2].X, texRect.Y + Corners[2].Y, Corners[2].Width, Corners[2].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[3].Width, PositionAndSize.Height - Corners[3].Height, Corners[3].Width, Corners[3].Height), new Rectangle(texRect.X + Corners[3].X, texRect.Y + Corners[3].Y, Corners[3].Width, Corners[3].Height), Color.White);
                    //edges
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(Corners[0].Width, 0, PositionAndSize.Width - Corners[0].Width - Corners[1].Width, Edges[0].Height), new Rectangle(texRect.X + Edges[0].X, texRect.Y + Edges[0].Y, Edges[0].Width, Edges[0].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, Corners[0].Height, Edges[1].Width, PositionAndSize.Height - Corners[0].Height - Corners[2].Height), new Rectangle(texRect.X + Edges[1].X, texRect.Y + Edges[1].Y, Edges[1].Width, Edges[1].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - Corners[1].Width, Corners[1].Height, Edges[2].Width, PositionAndSize.Height - Corners[1].Height - Corners[3].Height), new Rectangle(texRect.X + Edges[2].X, texRect.Y + Edges[2].Y, Edges[2].Width, Edges[2].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(Corners[2].Width, PositionAndSize.Height - Corners[2].Height, PositionAndSize.Width - Corners[2].Width - Corners[3].Width, Edges[3].Height), new Rectangle(texRect.X + Edges[3].X, texRect.Y + Edges[3].Y, Edges[3].Width, Edges[3].Height), Color.White);
                    //middle
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(Edges[1].Width, Edges[0].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), new Rectangle(texRect.X + Middle.X, texRect.Y + Middle.Y, Middle.Width, Middle.Height), Color.White);

                    //end drawing

                    //draw child controls
                    base.Draw(ref UIDrawRequests, gameTime);
                }
            }
        }

        #endregion
    }
}
