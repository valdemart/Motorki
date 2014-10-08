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
        public int layerDepth;

        public UIDrawRequest(Rectangle? Viewport, Texture2D texture, Rectangle destination, Rectangle? source, Color color, int layer)
        {
            type = UIDrawRequestTypes.Texture;
            this.Viewport = Viewport;
            this.texture = texture;
            this.destination = destination;
            this.source = source;
            this.filterColor = color;
            this.layerDepth = layer;
        }

        public UIDrawRequest(Rectangle? Viewport, SpriteFont font, string text, Vector2 position, Color color, int layer)
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

    public delegate void UIParent_ESCHook();

    public class UIParent : UIControlContainer
    {
        public static UIParent UI { get; private set; }
        private List<UIDrawRequest> drawRequests;

        private MotorkiGame game;

        public static Texture2D defaultTextures;
        public static SpriteFont defaultFont;

        public static event UIParent_ESCHook ESCHook;

        public UIParent(MotorkiGame game)
        {
            if (UI != null)
                throw new UIParentExistsException();
            UI = this;
            this.game = game;
            drawRequests = new List<UIDrawRequest>();
            InputEvents.KeyPressed += InputEvents_KeyPressed;
            defaultTextures = game.Content.Load<Texture2D>("UIdefaulttextures");
            defaultFont = game.Content.Load<SpriteFont>("UIdefaultfont");
            ESCHook = null;
        }

        public void LoadAndInitialize()
        {
            foreach (UIControl c in ChildControls)
                if (!c.isLoaded)
                {
                    c.LoadAndInitialize();
                    c.MarkLoaded();
                }
        }

        void InputEvents_KeyPressed(Keys key, bool state, int modifiers)
        {
            if (key == Keys.Escape)
            {
                if (ESCHook != null)
                    ESCHook();
                else
                {
                    bool exit = true;
                    foreach (UIControl child in ChildControls)
                        exit &= (child.ControlType != UIControlType.UITextBox ? true : !((UITextBox)child).DuringEdition);
                    if (exit)
                        game.Exit();
                }
            }
        }

        public static void ClearESCHook()
        {
            ESCHook = null;
        }

        //left for UI animations???
        public void Update(GameTime gameTime)
        {
        }

        /// <param name="sb">not-began sprite batch to use during drawing</param>
        public void Draw(ref SpriteBatch sb, GameTime gameTime)
        {
            drawRequests.Clear();
            foreach (UIControl child in ChildControls)
                child.Draw(ref drawRequests, gameTime);

            //draw requests
            int last_layer = -1;
            sb.GraphicsDevice.SetRenderTarget(null);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Viewport vp = sb.GraphicsDevice.Viewport;
            bool vpChanged = false;
            foreach (UIDrawRequest dr in drawRequests)
            {
                //set layer
                if (dr.layerDepth != last_layer)
                {
                    sb.End();
                    sb.GraphicsDevice.SetRenderTarget(game.layerTargets[dr.layerDepth]);
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    last_layer = dr.layerDepth;
                }
                //set viewport
                if (dr.Viewport != null)
                {
                    sb.End();
                    int l = (int)MathHelper.Clamp(((Rectangle)dr.Viewport).Left, 0, game.layerTargets[last_layer].Bounds.Width - 1);
                    int r = (int)MathHelper.Clamp(((Rectangle)dr.Viewport).Right, 0, game.layerTargets[last_layer].Bounds.Width - 1);
                    int t = (int)MathHelper.Clamp(((Rectangle)dr.Viewport).Top, 0, game.layerTargets[last_layer].Bounds.Height - 1);
                    int b = (int)MathHelper.Clamp(((Rectangle)dr.Viewport).Bottom, 0, game.layerTargets[last_layer].Bounds.Height - 1);
                    sb.GraphicsDevice.Viewport = new Viewport(new Rectangle(l, t, r - l + 1, b - t + 1));
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    vpChanged = true;
                }
                //draw
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
            drawRequests.Clear();
            sb.End();
            sb.GraphicsDevice.SetRenderTarget(null);
            if (vpChanged)
                sb.GraphicsDevice.Viewport = vp;
        }
    }
}
