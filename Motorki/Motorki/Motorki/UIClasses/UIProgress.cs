using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Motorki.UIClasses
{
    public class UIProgress : UIControl
    {
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < NormalEdges[1].Width + 2 + NormalEdges[2].Width)
                    value.Width = NormalEdges[1].Width + 2 + NormalEdges[2].Width;
                if (value.Height < NormalEdges[0].Height + 2 + NormalEdges[3].Height)
                    value.Height = NormalEdges[0].Height + 2 + NormalEdges[3].Height;
                base.PositionAndSize = value;
            }
        }

        /// <summary>
        /// if true forces progress bar to contain rounded fragment on the right
        /// </summary>
        public bool Angular { get; set; }
        private double? percent;
        /// <summary>
        /// value ranges from 0 to 1. Setting Percent to null will cause marquee
        /// </summary>
        public double? Percent
        {
            get { return percent; }
            set
            {
                if (value != null)
                {
                    if ((double)value < 0.0)
                        value = 0.0;
                    if ((double)value > 1.0)
                        value = 1.0;
                }
                percent = value;
            }
        }
        /// <summary>
        /// setting color to null will cause gradient filling. Color is ignored when Angular==true
        /// </summary>
        public Color? color { get; set; }

        public Rectangle NormalFrameTexture = new Rectangle(0, 261, 262, 22);
        public Rectangle NormalGradientTexture = new Rectangle(3, 284, 256, 16);
        public Rectangle[] NormalCorners = { new Rectangle(0, 0, 3, 3), new Rectangle(259, 0, 3, 3),
                                             new Rectangle(0, 19, 3, 3), new Rectangle(259, 19, 3, 3) };
        public Rectangle[] NormalEdges = { new Rectangle(3, 0, 256, 3),
                                           new Rectangle(0, 3, 3, 16), new Rectangle(259, 3, 3, 16),
                                           new Rectangle(3, 19, 256, 3) };
        public Rectangle AngularFrameTexture = new Rectangle(0, 301, 262, 70);
        public Rectangle AngularGradientTexture = new Rectangle(3, 372, 256, 64);
        public Rectangle WhiteProbeTexture = new Rectangle(232, 0, 20, 20);
        public Rectangle TransparentTexture = new Rectangle(2, 441, 30, 30);
        private RenderTarget2D renderTarget = null;
        private SpriteBatch sb = null;
        private BlendState bs;

        public UIProgress(MotorkiGame game)
            : base(game)
        {
            ControlType = UIControlType.UIProgress;
            percent = 0;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            Textures = UIParent.defaultTextures;
            renderTarget = new RenderTarget2D(game.GraphicsDevice, AngularFrameTexture.Width, AngularFrameTexture.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents);
            sb = new SpriteBatch(game.GraphicsDevice);
            bs = new BlendState();
            bs.ColorSourceBlend = bs.AlphaSourceBlend = Blend.SourceColor;
            bs.ColorBlendFunction = bs.AlphaBlendFunction = BlendFunction.Add;
            bs.ColorDestinationBlend = bs.AlphaDestinationBlend = Blend.SourceAlpha;
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                Rectangle texFrame = (!Angular ? NormalFrameTexture : AngularFrameTexture);
                Rectangle texFill = (!Angular ? (color != null ? WhiteProbeTexture : NormalGradientTexture) : AngularGradientTexture);

                //do drawing

                if (!Angular)
                {
                    //common progress bar
                    //contour
                    //corners
                    Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, NormalCorners[0].Width, NormalCorners[0].Height), new Rectangle(texFrame.X + NormalCorners[0].X, texFrame.Y + NormalCorners[0].Y, NormalCorners[0].Width, NormalCorners[0].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - NormalCorners[1].Width, 0, NormalCorners[1].Width, NormalCorners[1].Height), new Rectangle(texFrame.X + NormalCorners[1].X, texFrame.Y + NormalCorners[1].Y, NormalCorners[1].Width, NormalCorners[1].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, PositionAndSize.Height - NormalCorners[2].Height, NormalCorners[2].Width, NormalCorners[2].Height), new Rectangle(texFrame.X + NormalCorners[2].X, texFrame.Y + NormalCorners[2].Y, NormalCorners[2].Width, NormalCorners[2].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - NormalCorners[3].Width, PositionAndSize.Height - NormalCorners[3].Height, NormalCorners[3].Width, NormalCorners[3].Height), new Rectangle(texFrame.X + NormalCorners[3].X, texFrame.Y + NormalCorners[3].Y, NormalCorners[3].Width, NormalCorners[3].Height), Color.White);
                    //edges
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(NormalEdges[1].Width, 0, PositionAndSize.Width - NormalEdges[1].Width - NormalEdges[2].Width, NormalEdges[0].Height), new Rectangle(texFrame.X + NormalEdges[0].X, texFrame.Y + NormalEdges[0].Y, NormalEdges[0].Width, NormalEdges[0].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(0, NormalEdges[0].Height, NormalEdges[1].Width, PositionAndSize.Height - NormalEdges[0].Height - NormalEdges[3].Height), new Rectangle(texFrame.X + NormalEdges[1].X, texFrame.Y + NormalEdges[1].Y, NormalEdges[1].Width, NormalEdges[1].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(PositionAndSize.Width - NormalEdges[2].Width, NormalEdges[0].Height, NormalEdges[2].Width, PositionAndSize.Height - NormalEdges[0].Height - NormalEdges[3].Height), new Rectangle(texFrame.X + NormalEdges[2].X, texFrame.Y + NormalEdges[2].Y, NormalEdges[2].Width, NormalEdges[2].Height), Color.White);
                    Draw(ref UIDrawRequests, null, Textures, new Rectangle(NormalEdges[1].Width, PositionAndSize.Height - NormalEdges[3].Height, PositionAndSize.Width - NormalEdges[1].Width - NormalEdges[2].Width, NormalEdges[3].Height), new Rectangle(texFrame.X + NormalEdges[3].X, texFrame.Y + NormalEdges[3].Y, NormalEdges[3].Width, NormalEdges[3].Height), Color.White);
                    //middle
                    Rectangle vpMiddle = new Rectangle(PositionAndSize.Left + NormalEdges[1].Width, PositionAndSize.Top + NormalEdges[0].Height, PositionAndSize.Width - NormalEdges[1].Width - NormalEdges[2].Width, PositionAndSize.Height - NormalEdges[0].Height - NormalEdges[3].Height);
                    if(percent==null)
                    {
                        Draw(ref UIDrawRequests, vpMiddle, Textures, new Rectangle((int)(vpMiddle.Width * (gameTime.TotalGameTime.Milliseconds / 1000.0)), 0, (int)(vpMiddle.Width * 0.2), vpMiddle.Height), new Rectangle(texFill.X + (int)(texFill.Width * (gameTime.TotalGameTime.Milliseconds / 1000.0)), texFill.Y, (int)(texFill.Width * 0.2), texFill.Height), (color ?? Color.White));
                    }
                    else if((double)percent>0.0)
                    {
                        Draw(ref UIDrawRequests, vpMiddle, Textures, new Rectangle(0, 0, (int)(vpMiddle.Width*percent), vpMiddle.Height), new Rectangle(texFill.X, texFill.Y, (int)(texFill.Width*percent), texFill.Height), (color??Color.White));
                    }
                }
                else
                {
                    //frame
                    Draw(ref UIDrawRequests, new Rectangle(PositionAndSize.X, PositionAndSize.Y, texFrame.Width, texFrame.Height), Textures, new Rectangle(0, 0, texFrame.Width, texFrame.Height), texFrame, Color.White);
                    //fill
                    if (percent > 0.68571)
                    {
                        sb.GraphicsDevice.SetRenderTarget(renderTarget);
                        sb.GraphicsDevice.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, bs);
                        sb.Draw(Textures, new Rectangle(0, 0, texFill.Width, texFill.Height), texFill, Color.White);
                        sb.Draw(Textures, new Rectangle(texFill.Width - texFill.Height, -2, texFill.Height + 2, texFill.Height + 2), TransparentTexture, Color.White, (float)(MathHelper.PiOver2 - MathHelper.PiOver2 * (((double)percent - 0.68571) / (1 - 0.68571))), new Vector2(0, TransparentTexture.Height - 1), SpriteEffects.None, 0);
                        sb.End();
                        sb.GraphicsDevice.SetRenderTarget(null);
                        Draw(ref UIDrawRequests, null, renderTarget, new Rectangle(3, 3, texFill.Width, texFill.Height), new Rectangle(0, 0, texFill.Width, texFill.Height), Color.White);
                    }
                    else
                    {
                        Draw(ref UIDrawRequests, null, Textures, new Rectangle(3, 3, (int)((texFill.Width - texFill.Height) * ((double)percent / 0.68571)), texFill.Height), new Rectangle(texFill.X, texFill.Y, (int)((texFill.Width - texFill.Height) * ((double)percent / 0.68571)), texFill.Height), Color.White);
                    }
                }

                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
