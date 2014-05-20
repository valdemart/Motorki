using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Motorki
{
    public abstract class Motorek
    {
        public const float motorSpeedPerSecond = 400.0f; //units (pixels?) per second
        public const float motorTurnPerSecond = 360.0f; //degrees per second

        //internals
        protected Game game;
        protected SpriteBatch sb;

        //physics
        protected Vector2 position;
        protected float rotation { get; set; }
        protected Rectangle framingRect { get; private set; }

        //graphics
        private Texture2D Textures = null;
        private Rectangle[] BackTexture = new Rectangle[] { new Rectangle(0, 33, 29, 40), new Rectangle(30, 33, 29, 40) };
        protected int BackSelector { get; set; }
        private Rectangle FrontTexture = new Rectangle(60, 33, 29, 40);
        private Color motorColor = Color.White;
        private RenderTarget2D motorRenderTarget = null;
        //todo: track should be a class
        private Rectangle TrackTexture = new Rectangle(65, 17, 79, 9);
        private Color trackColor = Color.Yellow;
        //todo: animations

        //todo: bonuses

        public Motorek(Game game, Vector2 position, float rotation, Color motorColor, Color trackColor, Rectangle framingRect)
        {
            this.game = game;
            sb = new SpriteBatch(game.GraphicsDevice);

            this.motorColor = motorColor;
            this.trackColor = trackColor;

            this.position = position;
            this.rotation = rotation;
            this.framingRect = framingRect;

            BackSelector = 0;
        }

        public void LoadAndInitialize()
        {
            Textures = game.Content.Load<Texture2D>("common");

            motorRenderTarget = new RenderTarget2D(game.GraphicsDevice, BackTexture[0].Width, BackTexture[0].Height);
        }

        public void Update(GameTime gameTime)
        {
            MindProc(gameTime);

            //change tires picture
            BackSelector = (int)((position.Length() / 10) % 2);

            //do some coord corrections (map bounds)
            Vector2[] v = new Vector2[4];
            v[0] = new Vector2(-BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[1] = new Vector2(+BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[2] = new Vector2(-BackTexture[0].Width / 4, +BackTexture[0].Height / 2);
            v[3] = new Vector2(+BackTexture[0].Width / 4, +BackTexture[0].Height / 2);

            Vector2[] v_prim = new Vector2[4];
            Matrix matRotZ = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));
            Vector2.Transform(v, ref matRotZ, v_prim);
            for (int i = 0; i < 4; i++)
            {
                v_prim[i] += position;

                Vector2 _ = new Vector2(MathHelper.Clamp(v_prim[i].X, framingRect.Left, framingRect.Right), MathHelper.Clamp(v_prim[i].Y, framingRect.Top, framingRect.Bottom));
                position += _ - v_prim[i];
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (motorRenderTarget != null)
            {
                game.GraphicsDevice.SetRenderTarget(motorRenderTarget);
                game.GraphicsDevice.Clear(Color.Transparent);

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Textures, new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), BackTexture[BackSelector], Color.White);
                sb.Draw(Textures, new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), FrontTexture, motorColor);
                sb.End();

                game.GraphicsDevice.SetRenderTarget(null);
            }
        }

        public void DrawToSB(ref SpriteBatch sb, int cameraX, int cameraY)
        {
            if (motorRenderTarget != null)
            {
                sb.Draw(motorRenderTarget, new Vector2(position.X - cameraX, position.Y - cameraY), new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), Color.White, MathHelper.ToRadians(rotation), new Vector2(BackTexture[0].Width / 2, BackTexture[0].Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            }
        }

        protected abstract void MindProc(GameTime gameTime);
    }
}
