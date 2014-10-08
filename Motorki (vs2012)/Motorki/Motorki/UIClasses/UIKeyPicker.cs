using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Motorki.UIClasses
{

    public delegate void UIKeyPicker_KeyChanged(UIKeyPicker keypicker);

    public class UIKeyPicker : UIControl
    {
        private enum UIKeyPickerState
        {
            Normal,
            Hilite,
            Selecting,
            SelectingHilite
        }

        private UIKeyPickerState State { get; set; }

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

        public Rectangle NormalTexture = new Rectangle(100, 0, 37, 30);
        public Rectangle HiliteTexture = new Rectangle(100, 31, 37, 30);
        public Rectangle DisabledTexture = new Rectangle(100, 62, 37, 30);
        public Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 5, 5), new Rectangle(32, 0, 5, 5),
                                                          new Rectangle(0, 25, 5, 5), new Rectangle(32, 25, 5, 5) };
        public Rectangle[] Edges = new Rectangle[] { new Rectangle(5, 0, 27, 5),
                                                        new Rectangle(0, 5, 5, 20), new Rectangle(32, 5, 5, 20),
                                                        new Rectangle(5, 25, 27, 5) };
        public Rectangle Middle = new Rectangle(5, 5, 27, 20);

        private Keys selectedkey = Keys.None;
        public Keys SelectedKey
        {
            get { return selectedkey; }
            set
            {
                Keys oldKey = selectedkey;
                selectedkey = value;
                if (State == UIKeyPickerState.Selecting)
                    State = UIKeyPickerState.Normal;
                if (State == UIKeyPickerState.SelectingHilite)
                    State = UIKeyPickerState.Hilite;
                base.Text = (UICharMap.ToChar(selectedkey) == '\0' ? selectedkey.ToString() : "" + UICharMap.ToChar(selectedkey));
                if ((oldKey != selectedkey) && (KeyChanged != null))
                    KeyChanged(this);
            }
        }

        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                State = UIKeyPickerState.Normal;
            }
        }

        public override string Text
        {
            get { return ""; }
            set { return; }
        }

        public event UIKeyPicker_KeyChanged KeyChanged;

        public UIKeyPicker(MotorkiGame game)
            : base(game)
        {
            ControlType = UIControlType.UIKeyPicker;
            State = UIKeyPickerState.Normal;
            KeyChanged = null;
            InputEvents.KeyPressed += InputEvents_KeyPressed;
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChanged;
        }

        public override void Destroy()
        {
            InputEvents.KeyPressed -= InputEvents_KeyPressed;
            InputEvents.MouseMoved -= InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged -= InputEvents_MouseLeftChanged;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Textures = UIParent.defaultTextures;
            Font = UIParent.defaultFont;
        }

        void InputEvents_KeyPressed(Keys key, bool state, int modifiers)
        {
            if (Visible && Enabled && ((State == UIKeyPickerState.Selecting) || (State == UIKeyPickerState.SelectingHilite)) && state)
            {
                if (State == UIKeyPickerState.Selecting)
                    State = UIKeyPickerState.Normal;
                else if (State == UIKeyPickerState.SelectingHilite)
                    State = UIKeyPickerState.SelectingHilite;
                //filter out illegal keys as Keys.None
                switch(key)
                {
                    case Keys.None:
                    case Keys.Apps: case Keys.Attn: case Keys.BrowserBack:
                    case Keys.BrowserFavorites: case Keys.BrowserForward: case Keys.BrowserHome: case Keys.BrowserRefresh: case Keys.BrowserSearch: case Keys.BrowserStop:
                    case Keys.CapsLock: case Keys.NumLock: case Keys.Scroll:
                    case Keys.ChatPadGreen: case Keys.ChatPadOrange:
                    case Keys.Crsel:
                    case Keys.Insert: case Keys.Delete: case Keys.Home: case Keys.End: case Keys.PageUp: case Keys.PageDown:
                    case Keys.EraseEof:
                    case Keys.Escape: case Keys.Execute: case Keys.Exsel: case Keys.ProcessKey:
                    case Keys.F1: case Keys.F2: case Keys.F3: case Keys.F4: case Keys.F5: case Keys.F6: case Keys.F7: case Keys.F8:
                    case Keys.F9: case Keys.F10: case Keys.F11: case Keys.F12: case Keys.F13: case Keys.F14: case Keys.F15: case Keys.F16:
                    case Keys.F17: case Keys.F18: case Keys.F19: case Keys.F20: case Keys.F21: case Keys.F22: case Keys.F23: case Keys.F24:
                    case Keys.Help:
                    case Keys.ImeConvert: case Keys.ImeNoConvert:
                    case Keys.Kana: case Keys.Kanji:
                    case Keys.LaunchApplication1: case Keys.LaunchApplication2: case Keys.LaunchMail:
                    case Keys.LeftAlt: case Keys.LeftControl: case Keys.LeftShift:
                    case Keys.LeftWindows: case Keys.RightWindows:
                    case Keys.MediaNextTrack: case Keys.MediaPlayPause: case Keys.MediaPreviousTrack: case Keys.MediaStop: case Keys.Select: case Keys.SelectMedia:
                    case Keys.Oem8: case Keys.OemAuto: case Keys.OemClear: case Keys.OemCopy: case Keys.OemEnlW:
                    case Keys.Pa1:
                    case Keys.Pause: case Keys.Play:
                    case Keys.Print: case Keys.PrintScreen:
                    case Keys.RightAlt: case Keys.RightControl: case Keys.RightShift:
                    case Keys.Separator:
                    case Keys.Sleep:
                    case Keys.VolumeDown: case Keys.VolumeMute: case Keys.VolumeUp:
                    case Keys.Zoom:
                        key = Keys.None;
                        break;
                }
                SelectedKey = key;
            }
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);

                State = (mouseOver ? (((State == UIKeyPickerState.Selecting) || (State == UIKeyPickerState.SelectingHilite)) ? UIKeyPickerState.SelectingHilite : UIKeyPickerState.Hilite)
                                   : (((State == UIKeyPickerState.Selecting) || (State == UIKeyPickerState.SelectingHilite)) ? UIKeyPickerState.Selecting : UIKeyPickerState.Normal));
            }
        }

        void InputEvents_MouseLeftChanged(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);
                bool mlbtnClicked = (md.Left != md.old_Left) && !md.Left;

                UIKeyPickerState oldState = State;
                switch(State)
                {
                    case UIKeyPickerState.Normal:
                        State = (mouseOver ? UIKeyPickerState.Hilite : UIKeyPickerState.Normal);
                        break;
                    case UIKeyPickerState.Hilite:
                        State = (mouseOver ? (mlbtnClicked ? UIKeyPickerState.SelectingHilite : UIKeyPickerState.Hilite) : UIKeyPickerState.Normal);
                        break;
                    case UIKeyPickerState.Selecting:
                        State = (mouseOver ? UIKeyPickerState.SelectingHilite : (mlbtnClicked ? UIKeyPickerState.Normal : UIKeyPickerState.Selecting));
                        break;
                    case UIKeyPickerState.SelectingHilite:
                        State = (mouseOver ? (mlbtnClicked ? UIKeyPickerState.Hilite : UIKeyPickerState.SelectingHilite) : UIKeyPickerState.Selecting);
                        break;
                }
                if (((oldState == UIKeyPickerState.Selecting) || (oldState == UIKeyPickerState.SelectingHilite)) &&
                   ((State == UIKeyPickerState.Normal) || (State == UIKeyPickerState.Hilite)))
                {
                    //key picking cancelled - refresh current key
                    SelectedKey = SelectedKey;
                }
                if (((State == UIKeyPickerState.Selecting) || (State == UIKeyPickerState.SelectingHilite)) &&
                   ((oldState == UIKeyPickerState.Normal) || (oldState == UIKeyPickerState.Hilite)))
                {
                    //key picking started - set request text
                    base.Text = "Press a key";
                }
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
                        case UIKeyPickerState.Normal:
                            texRect = NormalTexture;
                            fontColor = Color.Black;
                            break;
                        case UIKeyPickerState.Hilite:
                            texRect = HiliteTexture;
                            fontColor = Color.Black;
                            break;
                        case UIKeyPickerState.Selecting:
                            texRect = NormalTexture;
                            fontColor = Color.White;
                            break;
                        case UIKeyPickerState.SelectingHilite:
                            texRect = HiliteTexture;
                            fontColor = Color.White;
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
                Vector2 textSize = Font.MeasureString(base.Text);
                Vector2 textPos = new Vector2((vpText.Width - textSize.X) / 2, (vpText.Height - textSize.Y) / 2);
                DrawString(ref UIDrawRequests, vpText, Font, base.Text, textPos, fontColor);

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
