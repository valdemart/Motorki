using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Motorki.UIClasses;

namespace Motorki.GameClasses
{
    public delegate void Motorek_HPChanged(Motorek motorek, float old_hp);
    public delegate void Motorek_FragPointChanged(Motorek motorek, int old_fragpoint);

    public abstract class Motorek
    {
        public float motorSpeedPerSecond = 300.0f; //units (pixels?) per second (*1.5 during bonus)
        public float motorTurnPerSecond = 360.0f; //degrees per second

        //internals
        protected MotorkiGame game;
        protected SpriteBatch sb;

        //physics
        public Vector2 position { get; set; }
        public float rotation { get; set; }
        protected Rectangle framingRect { get; private set; }
        public List<string> newCollided { get; set; }
        public List<string> oldCollided { get; set; }
        public Vector2[] boundPoints { get; private set; }

        //graphics
        private Texture2D Textures = null;
        public Rectangle[] BackTexture = new Rectangle[] { new Rectangle(0, 33, 29, 40), new Rectangle(30, 33, 29, 40) };
        protected int BackSelector { get; set; }
        private Rectangle FrontTexture = new Rectangle(60, 33, 29, 40);
        private Rectangle PointerTexture = new Rectangle(65, 0, 16, 14);
        private Color motorColor = Color.White;
        private RenderTarget2D motorRenderTarget = null;
        public MotorTrace trace { get; private set; }
        private Rectangle TrackTexture = new Rectangle(65, 17, 79, 9);
        private Color trackColor = Color.Yellow;
        //todo: animations

        //stats
        public string name { get; set; }
        private float hp { get; set; }
        public float HP
        {
            get { return hp; }
            set
            {
                if (value != hp)
                {
                    float old = hp;
                    hp = value;
                    if (hp < 0)
                        hp = 0;
                    if (HPChanged != null)
                        HPChanged(this, old);
                }
            }
        }
        public Motorek_HPChanged HPChanged;
        public bool justSpawned { get; private set; }
        //todo: bonuses
        private int fragsCount { get; set; }
        private int pointsCount { get; set; }
        public int FragsCount
        {
            get { return fragsCount; }
            set
            {
                if (value != fragsCount)
                {
                    int old = fragsCount;
                    fragsCount = value;
                    if (fragsCount < 0)
                        fragsCount = 0;
                    if (FragsChanged != null)
                        FragsChanged(this, old);
                }
            }
        }
        public int PointsCount
        {
            get { return pointsCount; }
            set
            {
                if (value != pointsCount)
                {
                    int old = pointsCount;
                    pointsCount = value;
                    if (pointsCount < 0)
                        pointsCount = 0;
                    if (PointsChanged != null)
                        PointsChanged(this, old);
                }
            }
        }
        public Motorek_FragPointChanged FragsChanged;
        public Motorek_FragPointChanged PointsChanged;


        public Motorek(MotorkiGame game, Color motorColor, Color trackColor)
        {
            this.game = game;
            sb = new SpriteBatch(game.GraphicsDevice);

            this.motorColor = motorColor;
            this.trackColor = trackColor;

            this.position = Vector2.Zero;
            this.rotation = 0.0f;
            this.framingRect = new Rectangle(0, 0, 0, 0);
            newCollided = new List<string>();
            oldCollided = new List<string>();

            trace = new MotorTrace(game, trackColor);

            name = "<template>";
            HPChanged = null;
            HP = 0;
            justSpawned = false;
            FragsChanged = null;
            PointsChanged = null;
            fragsCount = 0;
            pointsCount = 0;

            BackSelector = 0;
        }

        public void Destroy()
        {
            motorRenderTarget.Dispose();
        }

        /// <summary>
        /// prepare for play
        /// </summary>
        /// <param name="framingRect">bounding rectangle to limit movement</param>
        public void LoadAndInitialize(Rectangle framingRect)
        {
            Textures = game.Content.Load<Texture2D>("common");
            trace.LoadAndInitialize();
            motorRenderTarget = new RenderTarget2D(game.GraphicsDevice, BackTexture[0].Width, BackTexture[0].Height);

            this.framingRect = framingRect;
        }

        public void Update(GameTime gameTime)
        {
            MindProc(gameTime);

            //update trace
            trace.Add(gameTime, position, justSpawned);
            trace.Update(gameTime);

            //change tires picture
            BackSelector = (int)((position.Length() / 10) % 2);

            //do some coord corrections (map bounds)
            Vector2[] v = new Vector2[4];
            v[0] = new Vector2(-BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[1] = new Vector2(+BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[2] = new Vector2(+BackTexture[0].Width / 4, +BackTexture[0].Height / 2);
            v[3] = new Vector2(-BackTexture[0].Width / 4, +BackTexture[0].Height / 2);

            Vector2[] v_prim = new Vector2[4];
            Matrix mat = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));
            Vector2.Transform(v, ref mat, v_prim);
            for (int i = 0; i < 4; i++)
            {
                v_prim[i] += position;

                Vector2 _ = new Vector2(MathHelper.Clamp(v_prim[i].X, framingRect.Left, framingRect.Right), MathHelper.Clamp(v_prim[i].Y, framingRect.Top, framingRect.Bottom));
                position += _ - v_prim[i];
            }

            RefreshBoundingPoints();

            justSpawned = false;
        }

        public void Draw(ref SpriteBatch sb, GameTime gameTime, int cameraX, int cameraY)
        {
            cameraX -= 400;
            cameraY -= 300;

            if (motorRenderTarget != null)
            {
                game.GraphicsDevice.SetRenderTarget(motorRenderTarget);
                game.GraphicsDevice.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(Textures, new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), BackTexture[BackSelector], Color.White);
                sb.Draw(Textures, new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), FrontTexture, motorColor);
                sb.End();

                game.GraphicsDevice.SetRenderTarget(game.layerTargets[3]);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(motorRenderTarget, new Vector2(position.X - cameraX, position.Y - cameraY), new Rectangle(0, 0, BackTexture[0].Width, BackTexture[0].Height), Color.White, MathHelper.ToRadians(rotation), new Vector2(BackTexture[0].Width / 2, BackTexture[0].Height / 2), 1.0f, SpriteEffects.None, 0);
                if (GameSettings.gameoptNames)
                {
                    float width = UIParent.defaultFont.MeasureString(name).X;
                    sb.DrawString(UIParent.defaultFont, name, new Vector2(position.X - cameraX - width / 2, position.Y - cameraY + BackTexture[0].Height / 2 + 5), motorColor);
                }
                sb.End();

                if (GameSettings.gameoptPointers)
                {
                    game.GraphicsDevice.SetRenderTarget(game.layerTargets[2]);
                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    if (position.X - cameraX < 0)
                    {
                        if ((0 <= position.Y - cameraY) && (position.Y - cameraY < 600))
                            sb.Draw(Textures, new Rectangle(0, (int)(position.Y - cameraY), PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(-90), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                        else if (position.Y - cameraY < 0)
                            sb.Draw(Textures, new Rectangle(0, 0, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(-45), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                        else if (600 < position.Y - cameraY)
                            sb.Draw(Textures, new Rectangle(0, 600, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(-135), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                    }
                    else if (800 < position.X - cameraX)
                    {
                        if ((0 <= position.Y - cameraY) && (position.Y - cameraY < 600))
                            sb.Draw(Textures, new Rectangle(800, (int)(position.Y - cameraY), PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(90), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                        else if (position.Y - cameraY < 0)
                            sb.Draw(Textures, new Rectangle(800, 0, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(45), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                        else if (600 < position.Y - cameraY)
                            sb.Draw(Textures, new Rectangle(800, 600, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(135), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                    }
                    else if ((0 <= position.X - cameraX) && (position.X - cameraX < 800))
                    {
                        if (position.Y - cameraY < 0)
                            sb.Draw(Textures, new Rectangle((int)(position.X - cameraX), 0, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(0), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                        else if (600 < position.Y - cameraY)
                            sb.Draw(Textures, new Rectangle((int)(position.X - cameraX), 600, PointerTexture.Width, PointerTexture.Height), PointerTexture, motorColor, MathHelper.ToRadians(180), new Vector2(PointerTexture.Width / 2, 0), SpriteEffects.None, 0);
                    }
                    sb.End();
                }

                trace.Draw(ref sb, gameTime, cameraX, cameraY);

                game.GraphicsDevice.SetRenderTarget(null);
            }
        }

        public void RefreshBoundingPoints()
        {
            Vector2[] v = new Vector2[4];
            v[0] = new Vector2(-BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[1] = new Vector2(+BackTexture[0].Width / 4, -BackTexture[0].Height / 2);
            v[2] = new Vector2(+BackTexture[0].Width / 4, +BackTexture[0].Height / 2);
            v[3] = new Vector2(-BackTexture[0].Width / 4, +BackTexture[0].Height / 2);

            Vector2[] v_prim = new Vector2[4];
            Matrix mat = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation));
            Vector2.Transform(v, ref mat, v_prim);

            boundPoints = new Vector2[4];
            for (int i = 0; i < 4; i++)
                boundPoints[i] = v_prim[i] + position;
        }

        /// <param name="newRotation">note: ignore that value for now - rebound not implemented yet</param>
        public bool TestCollisions(ref Motorek motor, out Vector2 dispVec, out float newRotation, out float damage)
        {
            dispVec = Vector2.Zero;
            newRotation = 0;
            damage = 0;

            Vector2 motorDirVec = Utils.CalculateDirectionVector(MathHelper.ToRadians(motor.rotation));
            //for (int edge = 0; edge < 4; edge++)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        Vector2 disp;
            //        Vector2[] points = new Vector2[] { boundPoints[edge], boundPoints[(edge + 1) % 4] };
            //        if (!Utils.TestLineAndPointCollision(points, Utils.CalculateLineFacing(points[0], points[1]), motor.boundPoints[i], motorDirVec, out disp))
            //            continue;
            //        if (disp.Length() > dispVec.Length())
            //            dispVec = disp;
            //    }
            //    if (dispVec.Length() > 0)
            //    {
            //        damage = 25 * (1 - Vector2.Dot(Utils.CalculateDirectionVector(MathHelper.ToRadians(rotation)), motorDirVec)) / 2;
            //        newRotation = MathHelper.ToDegrees(-(float)Math.Atan(motorDirVec.X / (-motorDirVec.Y)));
            //        return true;
            //    }
            //}
            if (Vector2.Dot(motorDirVec, position - motor.position) < 0)
                return false;
            float radius = (BackTexture[0].Width + BackTexture[0].Height) / 2;
            float distance = (motor.position - position).Length();

            if (distance > radius)
                return false;
            if (distance < radius)
            {
                Vector2 _ = motor.position - position;
                _.Normalize();
                dispVec = _ * radius * 0.1f;
                damage = 25 * (1 - Vector2.Dot(Utils.CalculateDirectionVector(MathHelper.ToRadians(rotation)), motorDirVec)) / 2;
                newRotation = MathHelper.ToDegrees(-(float)Math.Atan(_.X / (_.Y)));
                return true;
            }

            return false;
        }

        public void Spawn(Vector2 position, float rotation)
        {
            if (HP == 0)
            {
                this.position = position;
                this.rotation = rotation;
                HP = 100;
                justSpawned = true;
            }
        }

        protected abstract void MindProc(GameTime gameTime);
    }

    public class MotorTraceSpot
    {
        public Vector2 position;
        public float rotation;
        public double creationMillis;
        public Vector2[] rectPoints;
        public Vector2 directionVec;

        public MotorTraceSpot(GameTime gameTime, Vector2 position, float rotation)
        {
            this.position = position;
            this.rotation = rotation;
            creationMillis = gameTime.TotalGameTime.TotalMilliseconds;
            Vector2[] v = new Vector2[4];
            v[0] = new Vector2(-MotorTrace.SpotTexture.Width / 2, -MotorTrace.SpotTexture.Width / 2);
            v[1] = new Vector2(+MotorTrace.SpotTexture.Width / 2, -MotorTrace.SpotTexture.Width / 2);
            v[2] = new Vector2(-MotorTrace.SpotTexture.Width / 2, +MotorTrace.SpotTexture.Height - MotorTrace.SpotTexture.Width / 2);
            v[3] = new Vector2(+MotorTrace.SpotTexture.Width / 2, +MotorTrace.SpotTexture.Height - MotorTrace.SpotTexture.Width / 2);

            rectPoints = new Vector2[4];
            Matrix matRotZ = Matrix.CreateRotationZ(rotation);
            Vector2.Transform(v, ref matRotZ, rectPoints);
            directionVec = rectPoints[0] + rectPoints[1];
            directionVec.Normalize();
        }

        public bool TestCollisions(ref Motorek motor, out Vector2 dispVec, out float newRotation, out float damage)
        {
            dispVec = Vector2.Zero;
            newRotation = 0;
            damage = 0;

            Vector2 motorDirVec = Utils.CalculateDirectionVector(MathHelper.ToRadians(motor.rotation));
            if (Vector2.Dot(motorDirVec, position - motor.position) < 0)
                return false;
            float radius = ((MotorTrace.SpotTexture.Width + MotorTrace.SpotTexture.Height) / 2 + (motor.BackTexture[0].Width + motor.BackTexture[0].Height) / 2) / 2;
            float distance = (motor.position - position).Length();

            if (distance > radius)
                return false;
            if (distance < radius)
            {
                Vector2 _ = motor.position - position;
                _.Normalize();
                dispVec = _ * radius * 0.1f;
                damage = 10 * Math.Abs(Vector2.Dot(new Vector2(directionVec.Y, -directionVec.X), motorDirVec));
                newRotation = MathHelper.ToDegrees(-(float)Math.Atan(_.X / (_.Y)));
                return true;
            }

            return false;
        }
    }

    public class MotorTrace : IList<MotorTraceSpot>
    {
        private MotorkiGame game;
        public Color color;
        public List<MotorTraceSpot> spots;
        public Texture2D Textures;
        public static Rectangle SpotTexture = new Rectangle(82, 0, 9, 18);

        public MotorTrace(MotorkiGame game, Color color)
        {
            this.game = game;
            this.color = color;
            spots = new List<MotorTraceSpot>();
        }

        public void LoadAndInitialize()
        {
            Textures = game.Content.Load<Texture2D>("common");
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < spots.Count; i++)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - spots[i].creationMillis > 1000)
                    spots.RemoveAt(i);
                else
                    i++;
            }
        }

        public void Draw(ref SpriteBatch sb, GameTime gameTime, int cameraX, int cameraY)
        {
            sb.GraphicsDevice.SetRenderTarget(game.layerTargets[4]);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (MotorTraceSpot mts in spots)
            {
                sb.Draw(Textures, new Rectangle((int)mts.position.X - cameraX, (int)mts.position.Y - cameraY, SpotTexture.Width, SpotTexture.Height), SpotTexture, color, mts.rotation, new Vector2(4, 4), SpriteEffects.None, 0);
            }
            sb.End();
        }

        public void Add(GameTime gameTime, Vector2 position, bool justSpawned)
        {
            if (justSpawned || (spots.Count == 0))
                this.Add(new MotorTraceSpot(gameTime, position, 0.0f));
            else
            {
                int newSpotsCount = (int)Math.Ceiling((double)(position - spots[spots.Count - 1].position).Length() / ((SpotTexture.Width + SpotTexture.Height) / 2));
                Vector2 baseline = (position - spots[spots.Count - 1].position) / (float)newSpotsCount;

                for (int i = 0; i < newSpotsCount; i++)
                {
                    Vector2 p = baseline;
                    p.Normalize();
                    p = position - p * (i * baseline.Length());

                    Vector2 current_direction = new Vector2((float)Math.Sin(spots[spots.Count - 1].rotation), -(float)Math.Cos(spots[spots.Count - 1].rotation));
                    Vector2 new_direction = p - spots[spots.Count - 1].position;
                    if ((new_direction.X == 0) && (new_direction.Y == 0))
                        return;
                    new_direction.Normalize();
                    new_direction = new Vector2(new_direction.Y, -new_direction.X);
                    float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product
                    this.Add(new MotorTraceSpot(gameTime, p, spots[spots.Count - 1].rotation + (float)Math.Asin(sin_alpha)));
                }
            }
        }

        //add animations later
        /// <summary>
        /// don't use it
        /// </summary>
        public void CollisionTest(ref Motorek motor)
        {
            //calculate rect points for motor
            Vector2[] v = new Vector2[4];
            v[0] = new Vector2(-motor.BackTexture[0].Width / 4, -motor.BackTexture[0].Height / 2);
            v[1] = new Vector2(+motor.BackTexture[0].Width / 4, -motor.BackTexture[0].Height / 2);
            v[2] = new Vector2(-motor.BackTexture[0].Width / 4, +motor.BackTexture[0].Height / 2);
            v[3] = new Vector2(+motor.BackTexture[0].Width / 4, +motor.BackTexture[0].Height / 2);

            Vector2[] v_prim = new Vector2[4];
            Matrix matRotZ = Matrix.CreateRotationZ(MathHelper.ToRadians(motor.rotation));
            Vector2.Transform(v, ref matRotZ, v_prim);
            Vector2 directionVec = v_prim[0] + v_prim[1];
            directionVec.Normalize();

            //test spots
            foreach (MotorTraceSpot spot in spots)
            {
                //check for collision

                //cut down hp

                /*Vector2 p1 = new Vector2((float)Math.Sin(spot.rotation), -(float)Math.Cos(spot.rotation));
                Vector2 new_direction = position - spots[spots.Count - 1].position;
                if ((new_direction.X == 0) && (new_direction.Y == 0))
                    return;
                new_direction.Normalize();
                new_direction = new Vector2(new_direction.Y, -new_direction.X);
                float sin_alpha = Vector2.Dot(new_direction, current_direction); //perpendicular dot product*/
            }

            return;
        }

        #region IList implementation

        public int IndexOf(MotorTraceSpot item)
        {
            return spots.IndexOf(item);
        }

        public void Insert(int index, MotorTraceSpot item)
        {
            spots.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            spots.RemoveAt(index);
        }

        public MotorTraceSpot this[int index]
        {
            get { return spots[index]; }
            set { spots[index] = value; }
        }

        public void Add(MotorTraceSpot item)
        {
            spots.Add(item);
        }

        public void Clear()
        {
            spots.Clear();
        }

        public bool Contains(MotorTraceSpot item)
        {
            return spots.Contains(item);
        }

        public void CopyTo(MotorTraceSpot[] array, int arrayIndex)
        {
            spots.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return spots.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(MotorTraceSpot item)
        {
            return spots.Remove(item);
        }

        public IEnumerator<MotorTraceSpot> GetEnumerator()
        {
            return spots.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return spots.GetEnumerator();
        }

        #endregion
    }
}
