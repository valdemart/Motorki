using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Motorki.UIClasses
{
    public enum UIDrawRequestTypes
    {
        Texture, Text
    }

    public class UIDrawRequest
    {
        public UIDrawRequestTypes type;
        public Rectangle? Viewport;
        public Texture2D texture;
        public SpriteFont font;
        public string text;
        public Rectangle destination;
        public Rectangle? source;
        public Vector2 position;
        public Color filterColor;
        public float layerDepth;

        public UIDrawRequest(Rectangle? Viewport, Texture2D texture, Rectangle destination, Rectangle? source, Color color, float layer)
        {
            type = UIDrawRequestTypes.Texture;
            this.Viewport = Viewport;
            this.texture = texture;
            this.destination = destination;
            this.source = source;
            this.filterColor = color;
            this.layerDepth = layer;
        }

        public UIDrawRequest(Rectangle? Viewport, SpriteFont font, string text, Vector2 position, Color color, float layer)
        {
            type = UIDrawRequestTypes.Text;
            this.Viewport = Viewport;
            this.font = font;
            this.text = text;
            this.position = position;
            this.filterColor = color;
            this.layerDepth = layer;
        }
    }

    public class UIParent : UIControlContainer
    {
        public static UIParent UI { get; private set; }
        private List<UIDrawRequest> drawRequests;

        private Game game;

        public static Texture2D defaultTextures;
        public static SpriteFont defaultFont;

        public UIParent(Game game)
        {
            if (UI != null)
                throw new UIParentExistsException();
            UI = this;
            this.game = game;
            drawRequests = new List<UIDrawRequest>();
            InputEvents.KeyPressed += InputEvents_KeyPressed;
            defaultTextures = game.Content.Load<Texture2D>("UIdefaulttextures");
            defaultFont = game.Content.Load<SpriteFont>("UIdefaultfont");
        }

        public void LoadAndInitialize()
        {

            foreach (UIControl c in ChildControls)
                c.LoadAndInitialize();
        }

        void InputEvents_KeyPressed(Keys key, bool state, int modifiers)
        {
            if (key == Keys.Escape)
            {
                bool exit = true;
                foreach (UIControl child in ChildControls)
                    exit &= (child.ControlType != UIControlType.UITextBox ? true : !((UITextBox)child).DuringEdition);
                game.Exit();
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        /// <param name="sb">not-began sprite batch to use during drawing</param>
        public void Draw(ref SpriteBatch sb, GameTime gameTime)
        {
            drawRequests.Clear();
            foreach (UIControl child in ChildControls)
                child.Draw(ref drawRequests, gameTime);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            while (drawRequests.Count > 0)
            {
                //detect greatest depth
                float maxDepth = 0;
                foreach (UIDrawRequest dr in drawRequests)
                    if (dr.layerDepth > maxDepth)
                        maxDepth = dr.layerDepth;
                //draw requests with greatest depth
                Viewport vp = sb.GraphicsDevice.Viewport;
                bool vpChanged = false;
                foreach (UIDrawRequest dr in (from d in drawRequests where d.layerDepth == maxDepth select d))
                {
                    if (dr.Viewport != null)
                    {
                        sb.End();
                        sb.GraphicsDevice.Viewport = new Viewport((Rectangle)dr.Viewport);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                        vpChanged = true;
                    }

                    switch (dr.type)
                    {
                        case UIDrawRequestTypes.Texture:
                            sb.Draw(dr.texture, dr.destination, dr.source, dr.filterColor);
                            break;
                        case UIDrawRequestTypes.Text:
                            sb.DrawString(dr.font, dr.text, dr.position, dr.filterColor);
                            break;
                    }
                }
                if (vpChanged)
                {
                    sb.End();
                    sb.GraphicsDevice.Viewport = vp;
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                }
                //remove drawn requests from list
                drawRequests.RemoveAll((dr) => dr.layerDepth == maxDepth);
            }
            sb.End();
        }
    }
}
