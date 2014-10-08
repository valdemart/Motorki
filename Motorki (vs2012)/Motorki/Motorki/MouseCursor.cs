using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Motorki
{
    public class MouseCursor
    {
        MotorkiGame game;
        int x, y;
        bool pressed;

        public bool Visible { get; set; }

        protected Texture2D Texture;
        protected Rectangle NormalRect = new Rectangle(0, 0, 32, 32);
        protected Rectangle PressedRect = new Rectangle(32, 0, 32, 32);

        public MouseCursor(MotorkiGame game)
        {
            this.game = game;
            Visible = false;
            x = y = 0;
            InputEvents.MouseMoved += InputEvents_MouseMoved;
            InputEvents.MouseLeftChanged += InputEvents_MouseLeftChanged;
        }

        void InputEvents_MouseMoved(MouseData md)
        {
            x = md.X;
            y = md.Y;
            //clipping
            //...
        }

        void InputEvents_MouseLeftChanged(MouseData md)
        {
            pressed = md.Left;
            InputEvents_MouseMoved(md);
        }

        public void LoadAndInitialize()
        {
            Texture = game.Content.Load<Texture2D>("common");
        }

        public void Draw(ref SpriteBatch sb, GameTime gameTime)
        {
            if (Visible)
            {
                Rectangle texRect = NormalRect;

                if (pressed)
                    texRect = PressedRect;
                sb.GraphicsDevice.SetRenderTarget(game.layerTargets[0]);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Texture, new Rectangle(x, y, texRect.Width, texRect.Height), new Rectangle(texRect.X, texRect.Y, texRect.Width, texRect.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                sb.End();
                sb.GraphicsDevice.SetRenderTarget(null);
            }
        }
    }
}
