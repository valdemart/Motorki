using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public delegate void UIListBox_Event(UIListBox listBox);
    public delegate void UIListBox_ItemEvent(UIListBox listBox, int itemIndex);

    public class UIListBox : UIControl, IList<string>
    {
        private List<string> Values; //potrzeba eventu reagujacego na zmiane listy wartosci...
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                int old_selectedIndex = selectedIndex;
                selectedIndex = -1;
                if ((value >= 0) && (value < Values.Count))
                    selectedIndex = value;
                if ((old_selectedIndex != selectedIndex) && (SelectionChanged != null))
                    SelectionChanged(this);
            }
        }
        public string SelectedValue
        {
            get { return ((SelectedIndex >= 0) && (SelectedIndex < Values.Count) ? Values[SelectedIndex] : null); }
            set { SelectedIndex = Values.IndexOf(value); }
        }
        private bool autoSizeHeight;
        public bool AutoSizeHeight
        {
            get { return autoSizeHeight; }
            set
            {
                autoSizeHeight = value;
                //force recalculations
                MaxVisibleValues = MaxVisibleValues;
                PositionAndSize = PositionAndSize;
            }
        }
        private int maxVisibleValues;
        public int MaxVisibleValues
        {
            get { return maxVisibleValues; }
            set
            {
                if (value <= 0)
                    value = 1;

                maxVisibleValues = value;

                if (AutoSizeHeight)
                {
                    PositionAndSize = new Rectangle(PositionAndSize.X, PositionAndSize.Y, PositionAndSize.Width, (int)(maxVisibleValues * (Font ?? UIParent.defaultFont).MeasureString("M").Y) + Edges[0].Height + Edges[3].Height);
                }
                else
                {
                    if (maxVisibleValues > Math.Floor((PositionAndSize.Height - Edges[0].Height - Edges[3].Height) / (double)(Font ?? UIParent.defaultFont).MeasureString("M").Y))
                        maxVisibleValues = (int)Math.Floor((PositionAndSize.Height - Edges[0].Height - Edges[3].Height) / (double)(Font ?? UIParent.defaultFont).MeasureString("M").Y);
                    if (maxVisibleValues == 0)
                        maxVisibleValues = 1;
                }

                if (Values.Count > maxVisibleValues)
                {
                    scroll.PositionAndSize = new Rectangle(PositionAndSize.Right - scroll.PositionAndSize.Width, PositionAndSize.Top, scroll.PositionAndSize.Width, PositionAndSize.Height);
                    scroll.MinimalValue = 0;
                    scroll.MaximalValue = Values.Count - maxVisibleValues;
                    scroll.ValuesOnScreen = 1;
                    scroll.Visible = true;
                }
                else
                {
                    scroll.Visible = false;
                    FirstVisibleIndex = 0;
                }

                if (FirstVisibleIndex > Values.Count - maxVisibleValues)
                    FirstVisibleIndex = Values.Count - maxVisibleValues;
            }
        }
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < Edges[1].Width + Edges[2].Width + 1)
                    value.Width = Edges[1].Width + Edges[2].Width + 1;
                if (value.Height < Edges[0].Height + Edges[3].Height + 1)
                    value.Height = Edges[0].Height + Edges[3].Height + 1;
                scroll.PositionAndSize = new Rectangle(0, 0, 0, 0);
                scroll.PositionAndSize = new Rectangle(value.Right - scroll.PositionAndSize.Width, value.Top, scroll.PositionAndSize.Width, scroll.PositionAndSize.Height);

                if (AutoSizeHeight)
                {
                    value = new Rectangle(value.X, value.Y, value.Width, (int)(maxVisibleValues * (Font ?? UIParent.defaultFont).MeasureString("M").Y) + Edges[0].Height + Edges[3].Height);
                }
                else
                {
                    if (MaxVisibleValues > Math.Floor((value.Height - Edges[0].Height - Edges[3].Height) / (double)(Font ?? UIParent.defaultFont).MeasureString("M").Y))
                        MaxVisibleValues = (int)Math.Floor((value.Height - Edges[0].Height - Edges[3].Height) / (double)(Font ?? UIParent.defaultFont).MeasureString("M").Y);
                    if (MaxVisibleValues == 0)
                        MaxVisibleValues = 1;
                }

                if (Values.Count > MaxVisibleValues)
                {
                    if (value.Width < Edges[1].Width + 1 + scroll.PositionAndSize.Width)
                        value.Width = Edges[1].Width + 1 + scroll.PositionAndSize.Width;
                    if (value.Height < scroll.PositionAndSize.Height)
                        value.Height = scroll.PositionAndSize.Height;
                    scroll.PositionAndSize = new Rectangle(scroll.PositionAndSize.X, scroll.PositionAndSize.Y, scroll.PositionAndSize.Width, value.Height);
                    scroll.MinimalValue = 0;
                    scroll.MaximalValue = Values.Count - MaxVisibleValues;
                    scroll.ValuesOnScreen = MaxVisibleValues;
                    scroll.Visible = true;
                }
                else
                {
                    scroll.Visible = false;
                    FirstVisibleIndex = 0;
                }

                base.PositionAndSize = value;

                if (FirstVisibleIndex > Values.Count - MaxVisibleValues)
                    FirstVisibleIndex = Values.Count - MaxVisibleValues;
            }
        }
        public override float LayerDepth
        {
            get { return base.LayerDepth; }
            set
            {
                base.LayerDepth = value;
                scroll.LayerDepth = value;
            }
        }
        private int firstVisibleIndex;
        public int FirstVisibleIndex
        {
            get { return firstVisibleIndex; }
            private set
            {
                firstVisibleIndex = 0;
                if ((value > 0) && (value < Values.Count))
                    firstVisibleIndex = (value > Values.Count - MaxVisibleValues ? Values.Count - MaxVisibleValues : value);
                scroll.Value = firstVisibleIndex;
            }
        }

        public event UIListBox_Event SelectionChanged;
        public event UIListBox_ItemEvent ItemClicked;
        public event UIListBox_ItemEvent ItemHover; //mouse points on item

        protected Texture2D Textures;
        protected SpriteFont Font;
        protected Rectangle NormalTexture = new Rectangle(0, 0, 37, 30);
        protected Rectangle ItemHiliteTexture = new Rectangle(0, 36, 37, 20);
        protected Rectangle DisabledTexture = new Rectangle(0, 62, 37, 30);
        protected Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 5, 5), new Rectangle(32, 0, 5, 5),
                                                          new Rectangle(0, 25, 5, 5), new Rectangle(32, 25, 5, 5) };
        protected Rectangle[] Edges = new Rectangle[] { new Rectangle(5, 0, 27, 5),
                                                        new Rectangle(0, 5, 5, 20), new Rectangle(32, 5, 5, 20),
                                                        new Rectangle(5, 25, 27, 5) };
        protected Rectangle[] ItemHiliteEdges = new Rectangle[] { new Rectangle(0, 0, 5, 20), new Rectangle(32, 0, 5, 20) };
        protected Rectangle Middle = new Rectangle(5, 5, 27, 20);
        protected Rectangle ItemHiliteMiddle = new Rectangle(5, 0, 27, 20);

        private UIListScrollBar scroll;

        public UIListBox(Game game)
            : base(game)
        {
            ControlType = UIControlType.UIListBox;
            scroll = new UIListScrollBar(game, true);
            Values = new List<string>();
            SelectedIndex = -1;
            SelectionChanged = null;
            ItemClicked = null;
            ItemHover = null;
            firstVisibleIndex = 0;
            AutoSizeHeight = false;
            MaxVisibleValues = 1;
            scroll.MinimalValue = 0;
            scroll.MaximalValue = FirstVisibleIndex;
            scroll.ValuesOnScreen = MaxVisibleValues;
            scroll.ValueChanged += (UIListScrollBar_ValueChanged)((s, oldval) =>
            {
                FirstVisibleIndex = s.Value;
            });
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChanged;
            InputEvents.MouseLeftRepeat += InputEvents_MouseLeftRepeat;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            scroll.LoadAndInitialize();

            Textures = UIParent.defaultTextures;
            Font = UIParent.defaultFont;
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            if (Visible && Enabled)
            {
                scroll.InputEvents_MouseMoved(md);
                if (scroll.Handled)
                    return;

                bool mouseOver = ((md.X >= PositionAndSize.Left + Corners[0].Width) && (md.X <= PositionAndSize.Right - Corners[1].Width) && (md.Y >= PositionAndSize.Top + Corners[0].Height) && (md.Y <= PositionAndSize.Bottom - Corners[2].Height));

                if (mouseOver)
                {
                    md.Y -= PositionAndSize.Y + Corners[0].Height;
                    int textHeight = (int)Font.MeasureString("M").Y;
                    int itemIndex = FirstVisibleIndex + (md.Y / textHeight);
                    if ((itemIndex >= 0) && (itemIndex < Values.Count))
                    {
                        if (ItemHover != null)
                            ItemHover(this, itemIndex);
                    }
                    else
                    {
                        if (ItemHover != null)
                            ItemHover(this, -1);
                    }
                }
            }
        }

        void InputEvents_MouseLeftChanged(MouseData md)
        {
            if (Visible && Enabled)
            {
                scroll.InputEvents_MouseLeftChangedOrRepeat(md);
                if (scroll.Handled)
                    return;

                bool mouseOver = ((md.X >= PositionAndSize.Left + Corners[0].Width) && (md.X <= PositionAndSize.Right - Corners[1].Width) && (md.Y >= PositionAndSize.Top + Corners[0].Height) && (md.Y <= PositionAndSize.Bottom - Corners[2].Height));
                bool mlbtnClicked = (md.Left != md.old_Left) && !md.Left;

                if (mouseOver)
                {
                    md.Y -= PositionAndSize.Y + Corners[0].Height;
                    int textHeight = (int)Font.MeasureString("M").Y;
                    int itemIndex = FirstVisibleIndex + (md.Y / textHeight);
                    if ((itemIndex >= 0) && (itemIndex < Values.Count) && mlbtnClicked)
                    {
                        if (ItemClicked != null)
                            ItemClicked(this, itemIndex);
                        SelectedIndex = itemIndex;
                    }
                }
            }
        }

        void InputEvents_MouseLeftRepeat(MouseData md)
        {
            if (Visible && Enabled)
            {
                scroll.InputEvents_MouseLeftChangedOrRepeat(md);
            }
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                scroll.Visible = (Values.Count > MaxVisibleValues);

                Rectangle texRect = DisabledTexture;
                Color fontColor = Color.Gray;

                if (Enabled)
                {
                    texRect = NormalTexture;
                    fontColor = Color.Black;
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

                if (Values.Count > 0)
                {
                    //measure text line height
                    int textHeight = (int)Font.MeasureString("M").Y;

                    //draw selected item hilite
                    if (Enabled && (SelectedIndex >= FirstVisibleIndex) && (SelectedIndex < FirstVisibleIndex + MaxVisibleValues))
                    {
                        int Y_Offset = (SelectedIndex - FirstVisibleIndex) * textHeight;
                        Draw(ref UIDrawRequests, new Rectangle(PositionAndSize.X, PositionAndSize.Y + Edges[0].Height, PositionAndSize.Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height), Textures, new Rectangle(0, Y_Offset, ItemHiliteEdges[0].Width, textHeight), new Rectangle(ItemHiliteTexture.X + ItemHiliteEdges[0].X, ItemHiliteTexture.Y + ItemHiliteEdges[0].Y, ItemHiliteEdges[0].Width, ItemHiliteEdges[0].Height), Color.White);
                        Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - ItemHiliteEdges[1].Width, Y_Offset, ItemHiliteEdges[1].Width, textHeight), new Rectangle(ItemHiliteTexture.X + ItemHiliteEdges[1].X, ItemHiliteTexture.Y + ItemHiliteEdges[1].Y, ItemHiliteEdges[1].Width, ItemHiliteEdges[1].Height), Color.White);
                        Draw(ref UIDrawRequests, null, Textures, new Rectangle(ItemHiliteEdges[0].Width, Y_Offset, PositionAndSize.Width - ItemHiliteEdges[0].Width - ItemHiliteEdges[1].Width, textHeight), new Rectangle(ItemHiliteTexture.X + ItemHiliteMiddle.X, ItemHiliteTexture.Y + ItemHiliteMiddle.Y, ItemHiliteMiddle.Width, ItemHiliteMiddle.Height), Color.White);
                    }

                    //draw item values
                    Rectangle vpText = new Rectangle(PositionAndSize.X + Edges[1].Width, PositionAndSize.Y + Edges[0].Height, PositionAndSize.Width - Edges[1].Width - Edges[2].Width, PositionAndSize.Height - Edges[0].Height - Edges[3].Height);
                    for (int i = FirstVisibleIndex; (i < FirstVisibleIndex + MaxVisibleValues) && (i < Values.Count); i++)
                        DrawString(ref UIDrawRequests, vpText, Font, Values[i], new Vector2(0, (i - FirstVisibleIndex) * textHeight), fontColor);
                }

                //draw scrollbar
                scroll.Draw(ref UIDrawRequests, gameTime);

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }

        #region IList implementations

        public int IndexOf(string item)
        {
            return Values.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            int oldCount = Values.Count;
            Values.Insert(index, item);
            if (oldCount != Values.Count)
                MaxVisibleValues = Values.Count; //recalculate scrollbar etc.
        }

        public void RemoveAt(int index)
        {
            Values.RemoveAt(index);
            MaxVisibleValues = Values.Count; //recalculate scrollbar etc.
        }

        public string this[int index]
        {
            get { return Values[index]; }
            set { Values[index] = value; }
        }

        public void Add(string item)
        {
            Values.Add(item);
            MaxVisibleValues = Values.Count; //recalculate scrollbar etc.
        }

        public void Clear()
        {
            Values.Clear();
            MaxVisibleValues = Values.Count; //recalculate scrollbar etc.
        }

        public bool Contains(string item)
        {
            return Values.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            int oldCount = Values.Count;
            bool ret = Values.Remove(item);
            if (oldCount != Values.Count)
                MaxVisibleValues = Values.Count; //recalculate scrollbar etc.
            return ret;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        #endregion

        #region UIListScrollBar class

        private delegate void UIListScrollBar_ValueChanged(UIListScrollBar scrollBar, int oldValue);

        private class UIListScrollBar : UIControl
        {
            private UIListScrollBarButton btnPlus, btnMinus;
            private UIListScrollBarThumb thumb;

            public bool Handled { get; set; }

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
                    btnPlus.PositionAndSize = new Rectangle(value.Bottom - btnPlus.NormalTexture.Width, value.Right - btnPlus.NormalTexture.Height, btnPlus.NormalTexture.Width, btnPlus.NormalTexture.Height);

                    base.PositionAndSize = value;
                }
            }
            public override float LayerDepth
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

            protected Texture2D Textures;
            protected Rectangle NormalBackground = new Rectangle(88, 94, 21, 21);
            protected Rectangle HiliteBackground = new Rectangle(88, 116, 21, 21);
            protected Rectangle DisabledBackground = new Rectangle(88, 138, 21, 21);
            protected Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 4, 4), new Rectangle(17, 0, 4, 4),
                                                          new Rectangle(0, 17, 4, 4), new Rectangle(17, 17, 4, 4) };
            protected Rectangle[] Edges = new Rectangle[] { new Rectangle(4, 0, 13, 4),
                                                        new Rectangle(0, 4, 4, 13), new Rectangle(17, 4, 4, 13),
                                                        new Rectangle(4, 17, 13, 4) };
            protected Rectangle Middle = new Rectangle(4, 4, 13, 13);

            public event UIListScrollBar_ValueChanged ValueChanged;

            public UIListScrollBar(Game game, bool vertical = true)
                : base(game)
            {
                ControlType = UIControlType.UIListScrollBar;
                orientation = vertical;
                btnMinus = new UIListScrollBarButton(game, this, false);
                btnPlus = new UIListScrollBarButton(game, this, true);
                thumb = new UIListScrollBarThumb(game, this);
                ValueChanged = null;
                PositionAndSize = new Rectangle(0, 0, 0, 0);
            }

            public override void LoadAndInitialize()
            {
                base.LoadAndInitialize();

                Textures = UIParent.defaultTextures;

                btnMinus.LoadAndInitialize();
                btnPlus.LoadAndInitialize();
                thumb.LoadAndInitialize();
            }

            public void InputEvents_MouseMoved(MouseData md)
            {
                Handled = false;

                if (Visible && Enabled)
                {
                    Hilited = false;

                    btnMinus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                    Handled |= btnMinus.Handled;
                    if (btnMinus.Handled)
                        return;

                    btnPlus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                    Handled |= btnPlus.Handled;
                    if (btnPlus.Handled)
                        return;

                    thumb.InputEvents_MouseMovedLeftChangedOrRepeat(md);
                    Handled |= thumb.Handled;
                    if (thumb.Handled)
                        return;

                    bool mouseOver = ((md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom));

                    Hilited = mouseOver;
                    Handled |= mouseOver;
                }
            }

            public void InputEvents_MouseLeftChangedOrRepeat(MouseData md)
            {
                Handled = false;

                if (Visible && Enabled)
                {
                    btnMinus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                    Handled |= btnMinus.Handled;
                    if (btnMinus.Handled)
                        return;

                    btnPlus.InputEvents_MouseMovedLeftChangedOrLeftRepeated(md);
                    Handled |= btnPlus.Handled;
                    if (btnPlus.Handled)
                        return;

                    thumb.InputEvents_MouseMovedLeftChangedOrRepeat(md);
                    Handled |= thumb.Handled;
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

                        Handled = true;
                    }

                    Hilited = mouseOver;
                    Handled |= mouseOver;
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

            #region UIListScrollBarButton class

            private class UIListScrollBarButton : UIControl
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

                private UIListScrollBar Parent;
                public bool Handled { get; private set; }

                public Texture2D Textures;
                public Rectangle NormalTexture = new Rectangle(0, 0, 21, 21);
                public Rectangle NormalPressedTexture = new Rectangle(0, 0, 21, 21);
                public Rectangle HiliteTexture = new Rectangle(0, 0, 21, 21);
                public Rectangle DisabledTexture = new Rectangle(0, 0, 21, 21);

                public UIListScrollBarButton(Game game, UIListScrollBar parent, bool plusButton)
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

            #region UIListScrollBarThumb class

            private class UIListScrollBarThumb : UIControl
            {
                public UIButtonState State { get; set; }

                private UIListScrollBar Parent;
                public bool Handled { get; private set; }

                public Texture2D Textures;
                public Rectangle NormalTexture = new Rectangle(0, 116, 21, 21);
                public Rectangle NormalPressedTexture = new Rectangle(44, 116, 21, 21);
                public Rectangle HiliteTexture = new Rectangle(22, 116, 21, 21);
                public Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 4, 4), new Rectangle(17, 0, 4, 4),
                                                           new Rectangle(0, 17, 4, 4), new Rectangle(17, 17, 4, 4) };
                public Rectangle[] Edges = new Rectangle[] { new Rectangle(4, 0, 13, 4),
                                                         new Rectangle(0, 4, 4, 13), new Rectangle(17, 4, 4, 13),
                                                         new Rectangle(4, 17, 13, 4) };
                public Rectangle Middle = new Rectangle(4, 4, 13, 13);

                public UIListScrollBarThumb(Game game, UIListScrollBar parent)
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

        #endregion
    }
}
