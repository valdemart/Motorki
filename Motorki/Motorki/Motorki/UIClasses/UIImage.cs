using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public class UIImage : UIControl
    {
        /// <summary>
        /// size parameters determine scaling
        /// </summary>
        public override Rectangle PositionAndSize
        {
            get { return base.PositionAndSize; }
            set
            {
                if (value.Width < 1)
                    value.Width = 1;
                if (value.Height < 1)
                    value.Height = 1;
                base.PositionAndSize = value;
            }
        }

        public Rectangle NormalTexture = new Rectangle(0, 0, 1, 1);

        public UIImage(MotorkiGame game)
            : base(game)
        {
            ControlType = UIControlType.UIImage;
        }

        public override void LoadAndInitialize()
        {
            base.LoadAndInitialize();

            if (Textures == null)
                Textures = UIParent.defaultTextures;
        }

        public override void Draw(ref List<UIDrawRequest> UIDrawRequests, GameTime gameTime)
        {
            if (Visible)
            {
                Rectangle texRect = NormalTexture;

                //do drawing

                Draw(ref UIDrawRequests, PositionAndSize, Textures, new Rectangle(0, 0, PositionAndSize.Width, PositionAndSize.Height), new Rectangle(texRect.X, texRect.Y, texRect.Width, texRect.Height), Color.White);
                
                //end drawing

                //draw child controls
                base.Draw(ref UIDrawRequests, gameTime);
            }
        }
    }
}
