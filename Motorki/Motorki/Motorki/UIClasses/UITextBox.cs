using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public class UITextBox : UIControl
    {
        public char PasswordChar { get; set; }
        public int TextLenghtLimit { get; set; }
        private int CursorPos { get; set; }
        public bool DuringEdition { get { return CursorPos != -1; } }
        private int FirstVisibleChar { get; set; }
        private bool Hilited { get; set; }
        private bool lastMouseOver { get; set; }

        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                if (value == false)
                {
                    CursorPos = -1;
                    FirstVisibleChar = 0;
                    Hilited = lastMouseOver;
                }
            }
        }
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < Corners[0].Width + 2 + Corners[1].Width)
                    value.Width = Corners[0].Width + 2 + Corners[1].Width;
                if (value.Height < Corners[0].Height + (int)(Font ?? UIParent.defaultFont).MeasureString("M").Y + Corners[2].Height)
                    value.Height = Corners[0].Height + (int)(Font ?? UIParent.defaultFont).MeasureString("M").Y + Corners[2].Height;
                base.PositionAndSize = value;
            }
        }

        protected Texture2D Textures;
        protected SpriteFont Font;
        protected Rectangle NormalTexture = new Rectangle(0, 0, 37, 30);
        protected Rectangle HiliteTexture = new Rectangle(0, 31, 37, 30);
        protected Rectangle DisabledTexture = new Rectangle(0, 62, 37, 30);
        protected Rectangle[] Corners = new Rectangle[] { new Rectangle(0, 0, 5, 5), new Rectangle(32, 0, 5, 5),
                                                          new Rectangle(0, 25, 5, 5), new Rectangle(32, 25, 5, 5) };
        protected Rectangle[] Edges = new Rectangle[] { new Rectangle(5, 0, 27, 5),
                                                        new Rectangle(0, 5, 5, 20), new Rectangle(32, 5, 5, 20),
                                                        new Rectangle(5, 25, 27, 5) };
        protected Rectangle Middle = new Rectangle(5, 5, 27, 20);
        protected Rectangle Cursor = new Rectangle(37, 30, 1, 1);

        public UITextBox(Game game)
            : base(game)
        {
            ControlType = UIControlType.UITextBox;
            Text = "";
            PasswordChar = '\0';
            TextLenghtLimit = -1;
            CursorPos = -1;
            FirstVisibleChar = 0;
            Hilited = false;
            lastMouseOver = false;
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChanged;
            InputEvents.KeyPressed += InputEvents_KeyPressedOrRepeated;
            InputEvents.KeyRepeated += InputEvents_KeyPressedOrRepeated;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Textures = UIParent.defaultTextures;
            Font = UIParent.defaultFont;
        }

        void InputEvents_KeyPressedOrRepeated(Keys key, bool state, int modifiers)
        {
            if ((TextLenghtLimit != -1) && (Text.Length > TextLenghtLimit))
                Text = Text.Substring(0, TextLenghtLimit);

            if (Visible && Enabled && (CursorPos != -1) && state)
            {
                char input_char = '\0';
                bool accept_edit = false;

                switch (key)
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
                        if ((CursorPos != -1) && (TextLenghtLimit != -1 ? (Text.Length < TextLenghtLimit) : true))
                        {
                            input_char = key.ToString()[0];
                            if ((modifiers & (int)KeyModifiers.Shift) == 0)
                                input_char = (char)(input_char + ('a' - 'A'));
                            if (CursorPos == Text.Length)
                                Text += input_char;
                            else
                                Text = Text.Insert(CursorPos, "" + input_char);
                            CursorPos++;
                        }
                        break;
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
                        if ((CursorPos != -1) && (TextLenghtLimit != -1 ? (Text.Length < TextLenghtLimit) : true))
                        {
                            input_char = key.ToString()[1];
                            if (CursorPos == Text.Length)
                                Text += input_char;
                            else
                                Text = Text.Insert(CursorPos, "" + input_char);
                            CursorPos++;
                        }
                        break;
                    case Keys.Enter:
                        accept_edit = (CursorPos != -1);
                        break;
                    case Keys.Left:
                        if (CursorPos > 0)
                            CursorPos--;
                        break;
                    case Keys.Right:
                        if ((CursorPos != -1) && (CursorPos < Text.Length))
                            CursorPos++;
                        break;
                    case Keys.Home:
                        if (CursorPos != -1)
                        {
                            CursorPos = 0;
                            FirstVisibleChar = 0;
                        }
                        break;
                    case Keys.End:
                        if (CursorPos != -1)
                            CursorPos = Text.Length;
                        break;
                    case Keys.Delete:
                        if ((CursorPos != -1) && (CursorPos < Text.Length))
                            Text = Text.Remove(CursorPos, 1);
                        break;
                    case Keys.Back:
                        if (CursorPos > 0)
                        {
                            Text = Text.Remove(CursorPos - 1, 1);
                            CursorPos--;
                        }
                        break;
                }
                if (accept_edit)
                {
                    CursorPos = -1;
                    FirstVisibleChar = 0;
                }

                Hilited = (lastMouseOver || (CursorPos != -1));
            }
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);

                Hilited = (mouseOver || (CursorPos != -1));
                lastMouseOver = mouseOver;
            }
        }

        void InputEvents_MouseLeftChanged(MouseData md)
        {
            if (Visible && Enabled)
            {
                //check mouse
                bool mouseOver = (md.X >= PositionAndSize.Left) && (md.X <= PositionAndSize.Right) && (md.Y >= PositionAndSize.Top) && (md.Y <= PositionAndSize.Bottom);
                bool mlbtnClicked = (md.Left != md.old_Left) && !md.Left;

                if (mouseOver && mlbtnClicked && (CursorPos == -1))
                    CursorPos = Text.Length;
                if (!mouseOver && mlbtnClicked && (CursorPos != -1))
                {
                    CursorPos = -1;
                    FirstVisibleChar = 0;
                }
                Hilited = (mouseOver || (CursorPos != -1));
                lastMouseOver = mouseOver;
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
                    texRect = NormalTexture;
                    fontColor = Color.Black;

                    if (Hilited || (CursorPos != -1))
                    {
                        texRect = HiliteTexture;
                        fontColor = Color.Black;
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
                string _Text = Text;
                //replace with password char if needed
                if (PasswordChar != '\0')
                {
                    for (int i = 0; i < Text.Length; i++)
                        if (Text[i] != PasswordChar)
                            Text = Text.Replace(Text[i], PasswordChar);
                }
                //correct text visibility
                string text = Text;
                int visibleCursorPos = CursorPos;
                Vector2 textSize = Font.MeasureString(text);
                if (CursorPos == -1)
                    FirstVisibleChar = 0;
                else
                {
                    if (CursorPos < FirstVisibleChar)
                        FirstVisibleChar = CursorPos;
                    do
                    {
                        text = Text.Substring(FirstVisibleChar);
                        visibleCursorPos = CursorPos - FirstVisibleChar;
                        textSize = Font.MeasureString((visibleCursorPos == text.Length ? text : text.Substring(0, visibleCursorPos + 1)));
                        if (textSize.X >= vpText.Width)
                            FirstVisibleChar++;
                        else
                            break;
                    } while (true);
                }

                text = Text.Substring(FirstVisibleChar);
                textSize = Font.MeasureString(text);
                int textHeight = (int)Font.MeasureString("M").Y;
                Vector2 textPos = new Vector2(0, (vpText.Height - textHeight) / 2);
                if ((CursorPos != -1) && ((gameTime.TotalGameTime.Milliseconds / 500) > 0))
                {
                    textSize = Font.MeasureString((visibleCursorPos == text.Length ? text : text.Substring(0, visibleCursorPos)));
                    Draw(ref UIDrawRequests, vpText, Textures, new Rectangle((int)textSize.X, (int)textPos.Y, 1, textHeight), Cursor, Color.Black);
                }
                DrawString(ref UIDrawRequests, vpText, Font, text, textPos, fontColor);
                Text = _Text;

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
