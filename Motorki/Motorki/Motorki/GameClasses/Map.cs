using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Motorki.UIClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Motorki.GameClasses
{
    public struct BonusFrequency
    {
        public BonusType type;
        public float freq;
    }

    public class MapParameters
    {
        public bool SuccessfulRead { get; private set; }

        public string FileName { get; private set; }
        public string Name { get; private set; }
        public Vector2 Size { get; private set; }
        public List<GameType> SupportedGameTypes { get; private set; }
        public Vector2 Slicing { get; private set; }
        public Color BlockingColor { get; private set; }
        public Color BackColor { get; private set; }
        public Color BackCrateColor { get; private set; }
        public List<BonusFrequency> AllowedBonuses { get; private set; }
        public int MaxPlayers { get; private set; }

        /// <summary>
        /// load map parameters from file
        /// </summary>
        public MapParameters(string filename)
        {
            FileName = filename;
            Name = "";
            Size = Vector2.Zero;
            SupportedGameTypes = new List<GameType>();
            Slicing = Vector2.Zero;
            BlockingColor = Color.Black;
            BackColor = Color.Blue;
            BackCrateColor = Color.LightBlue;
            AllowedBonuses = new List<BonusFrequency>();
            MaxPlayers = 10;

            try
            {
                SuccessfulRead = false;

                XDocument doc = XDocument.Load(filename);

                XElement root = doc.Element("MapFile");
                if (root == null)
                    return;

                XElement param_root = root.Element("Parameters");
                if (param_root == null)
                    return;

                foreach (XElement param in param_root.Elements())
                {
                    try
                    {
                        switch (param.Name.LocalName)
                        {
                            case "Name": Name = param.Attribute("value").Value; break;
                            case "Size": Size = new Vector2(float.Parse(param.Attribute("x").Value.Replace('.', ',')), float.Parse(param.Attribute("y").Value.Replace('.', ','))); break;
                            case "GameType": SupportedGameTypes.Add((GameType)Enum.Parse(typeof(GameType), param.Attribute("value").Value)); break;
                            case "Slicing": Slicing = new Vector2(float.Parse(param.Attribute("xs").Value.Replace('.', ',')), float.Parse(param.Attribute("ys").Value.Replace('.', ','))); break;
                            case "BlockingColor": BlockingColor = new Color(int.Parse(param.Attribute("r").Value), int.Parse(param.Attribute("g").Value), int.Parse(param.Attribute("b").Value)); break;
                            case "BackColor": BackColor = new Color(int.Parse(param.Attribute("r").Value), int.Parse(param.Attribute("g").Value), int.Parse(param.Attribute("b").Value)); break;
                            case "BackCrateColor": BackCrateColor = new Color(int.Parse(param.Attribute("r").Value), int.Parse(param.Attribute("g").Value), int.Parse(param.Attribute("b").Value)); break;
                            case "Bonus": AllowedBonuses.Add(new BonusFrequency { type = (BonusType)Enum.Parse(typeof(BonusType), param.Attribute("name").Value), freq = float.Parse(param.Attribute("freq").Value.Replace('.', ',')) }); break;
                            case "MaxPlayers": MaxPlayers = int.Parse(param.Attribute("value").Value); break;
                        }
                    }
                    catch { }
                }

                SuccessfulRead = true;
            }
            catch { }
        }
    }

    public class Map
    {
        #region map data classes

        public class MapPoint
        {
            public Vector2 Coords { get; private set; }
            public string Name { get; private set; }

            public MapPoint(string name, Vector2 coords)
            {
                Coords = coords;
                Name = name;
            }
        }

        public class MapEdge
        {
            public Vector2 start;
            public Vector2 end;
            /// <summary>
            /// normalized vector determining rebound direction of the edge. calculated during loading
            /// </summary>
            public Vector2 facing;
            public string Name;

            public MapEdge(string name, MapPoint start, MapPoint end)
            {
                Name = name;
                this.start = start.Coords;
                this.end = end.Coords;
                facing = Utils.CalculateLineFacing(this.start, this.end);
            }

            public bool TestCollision(Vector2 motorDirVec, Vector2[] motorBoundPoints, out Vector2 dispVec)
            {
                dispVec = Vector2.Zero;

                if (Vector2.Dot(motorDirVec, facing) >= 0)
                    return false;
                for (int i = 0; i < motorBoundPoints.Length; i++)
                {
                    Vector2 _disp;
                    if (!Utils.TestLineAndPointCollision(new Vector2[] { start, end }, facing, motorBoundPoints[i], motorDirVec, out _disp))
                        continue;
                    if (_disp.Length() > dispVec.Length())
                        dispVec = _disp;
                }
                if (dispVec.Length() > 0)
                    return true;
                return false;

            }
        }

        public class MapSpawnPoint : MapPoint
        {
            /// <summary>
            /// in degrees
            /// </summary>
            public float Rotation { get; private set; }
            public int Team { get; private set; }

            public MapSpawnPoint(string name, Vector2 coords, float rotation, int team)
                : base(name, coords)
            {
                Rotation = rotation;
                Team = team;
            }
        }

        /// <summary>
        /// circular map sector for optimizing collision detection
        /// </summary>
        public class MapSector
        {
            public string Name { get; private set; }
            public Vector2 Position { get; private set; }
            public float Range { get; private set; }
            public List<string> Edges { get; private set; }
            public List<string> Neighbours { get; private set; }
            public List<string> SpawnPoints { get; private set; }

            public MapSector(string name, Vector2 position, float range)
            {
                Name = name;
                Position = position;
                Range = range;
                Edges = new List<string>();
                Neighbours = new List<string>();
                SpawnPoints = new List<string>();
            }

            public bool IsInSector(Vector2 point)
            {
                return (point - Position).Length() <= Range;
            }
        }

        /// <summary>
        /// rectangle for drawing blocking area of map
        /// </summary>
        private class MapRect
        {
            public string Name { get; private set; }
            public Rectangle PositionAndSize { get; private set; }
            public float Rotation { get; private set; }
            public Vector2 Origin { get; private set; }

            private Texture2D Texture = UIParent.defaultTextures;
            private Rectangle NormalTexture = new Rectangle(232, 1, 23, 23);

            public MapRect(string name, Rectangle posandsize, float rot, int originX, int originY)
            {
                Name = name;
                PositionAndSize = posandsize;
                Rotation = rot;
                Origin = new Vector2(originX, originY);
            }

            public void Draw(ref SpriteBatch sb, Color blockingColor)
            {
                sb.Draw(Texture, PositionAndSize, NormalTexture, blockingColor, Rotation, Origin, SpriteEffects.None, 0.0f);
            }
        }

        private class MapSlice
        {
            public Rectangle PositionAndSize { get; private set; }
            public RenderTarget2D picture { get; private set; }

            public MapSlice(MotorkiGame game, Rectangle posandsize)
            {
                PositionAndSize = posandsize;
                picture = new RenderTarget2D(game.GraphicsDevice, PositionAndSize.Width, PositionAndSize.Height, false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
            }

            public void Draw(ref SpriteBatch sb, int cameraX, int cameraY)
            {
                Rectangle camera = new Rectangle(cameraX - 400, cameraY - 300, 800, 600);
                bool result;
                PositionAndSize.Intersects(ref camera, out result);

                if (result)
                    sb.Draw(picture, new Vector2(PositionAndSize.X - camera.Left, PositionAndSize.Y - camera.Top), Color.White);
            }
        }

        #endregion

        private MotorkiGame game;

        public MapParameters Parameters { get; private set; }
        public List<MapPoint> Points;
        public List<MapEdge> Edges;
        public List<MapSpawnPoint> SpawnPoints;
        private List<bool> SpawnInUse;
        public List<MapSector> Sectors;
        private List<MapRect> Rects;
        private MapSlice[,] Slices;

        private Texture2D Texture;
        private Rectangle BackCrateTexture = new Rectangle(267, 0, 25, 26);

        /// <summary>
        /// loads map parameters from file
        /// </summary>
        public Map(MotorkiGame game, string filename)
        {
            this.game = game;
            Parameters = new MapParameters(filename);
            Points = new List<MapPoint>();
            Edges = new List<MapEdge>();
            SpawnPoints = new List<MapSpawnPoint>();
            SpawnInUse = new List<bool>();
            Sectors = new List<MapSector>();
            Rects = new List<MapRect>();
            Slices = new MapSlice[(int)(Parameters.Size.X / Parameters.Slicing.X + (Parameters.Size.X % Parameters.Slicing.X != 0.0f ? 1.0f : 0.0f)),
                                  (int)(Parameters.Size.Y / Parameters.Slicing.Y + (Parameters.Size.Y % Parameters.Slicing.Y != 0.0f ? 1.0f : 0.0f))];
            for (int x = 0; x < Slices.GetLength(0); x++)
                for (int y = 0; y < Slices.GetLength(1); y++)
                    Slices[x, y] = null;
        }

        public void Destroy()
        {
            for (int x = 0; x < Slices.GetLength(0); x++)
                for (int y = 0; y < Slices.GetLength(1); y++)
                    if (Slices[x, y] != null)
                        Slices[x, y].picture.Dispose();
        }

        /// <summary>
        /// loads map for use in game
        /// </summary>
        public void LoadAndInitialize()
        {
            try
            {
                #region load map data

                XDocument doc = XDocument.Load(Parameters.FileName);

                XElement root = doc.Element("MapFile");
                if (root == null)
                    return;

                //sectors
                XElement sectors = root.Element("Sectors");
                if (sectors == null)
                    return;
                foreach (XElement sector in sectors.Elements("Sector"))
                {
                    try
                    {
                        Sectors.Add(new MapSector(sector.Attribute("name").Value,
                                                  new Vector2(float.Parse(sector.Attribute("x").Value.Replace('.', ',')), float.Parse(sector.Attribute("y").Value.Replace('.', ','))),
                                                  float.Parse(sector.Attribute("range").Value.Replace('.', ','))));
                        foreach (XElement item in sector.Elements())
                        {
                            try
                            {
                                switch (item.Name.LocalName)
                                {
                                    case "Edge": Sectors[Sectors.Count - 1].Edges.Add(item.Attribute("name").Value); break;
                                    case "Neighbour": Sectors[Sectors.Count - 1].Neighbours.Add(item.Attribute("name").Value); break;
                                    case "Spawn": Sectors[Sectors.Count - 1].SpawnPoints.Add(item.Attribute("name").Value); break;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                //points
                XElement points = root.Element("Points");
                if (points == null)
                    return;
                foreach (XElement point in points.Elements("Point"))
                {
                    try
                    {
                        Points.Add(new MapPoint(point.Attribute("name").Value, new Vector2(float.Parse(point.Attribute("x").Value.Replace('.', ',')), float.Parse(point.Attribute("y").Value.Replace('.', ',')))));
                    }
                    catch { }
                }

                //edges
                XElement edges = root.Element("Edges");
                if (edges == null)
                    return;
                foreach (XElement edge in edges.Elements("Edge"))
                {
                    try
                    {
                        Edges.Add(new MapEdge(edge.Attribute("name").Value, Points.Find(p => p.Name == edge.Attribute("start").Value), Points.Find(p => p.Name == edge.Attribute("end").Value)));
                    }
                    catch { }
                }

                //spawn points
                XElement spawns = root.Element("Spawns");
                if (spawns == null)
                    return;
                foreach (XElement spawn in spawns.Elements("Spawn"))
                {
                    try
                    {
                        SpawnPoints.Add(new MapSpawnPoint(spawn.Attribute("name").Value,
                                        new Vector2(float.Parse(spawn.Attribute("x").Value.Replace('.', ',')), float.Parse(spawn.Attribute("y").Value.Replace('.', ','))),
                                        float.Parse(spawn.Attribute("rot").Value.Replace('.', ',')),
                                        int.Parse(spawn.Attribute("team").Value)));
                        SpawnInUse.Add(false);
                    }
                    catch { }
                }

                //rectangles
                XElement rects = root.Element("Rects");
                if (rects == null)
                    return;
                foreach (XElement rect in rects.Elements("Rect"))
                {
                    try
                    {
                        Rects.Add(new MapRect(rect.Attribute("name").Value,
                                  new Rectangle(int.Parse(rect.Attribute("x").Value), int.Parse(rect.Attribute("y").Value), int.Parse(rect.Attribute("w").Value), int.Parse(rect.Attribute("h").Value)),
                                  float.Parse(rect.Attribute("rot").Value.Replace('.', ',')),
                                  int.Parse(rect.Attribute("originX").Value), int.Parse(rect.Attribute("originY").Value)));
                    }
                    catch { }
                }

                //clean up
                //erase edges not assigned to any sector
                for (int i = 0; i < Edges.Count; )
                {
                    if (Sectors.FindIndex(s => s.Edges.FindIndex(e => Edges[i].Name == e) != -1) == -1)
                        Edges.RemoveAt(i);
                    else
                        i++;
                }
                //erase spawns outside of every sector (spawn need to be inside of some sector for motor initialization)
                for (int i = 0; i < SpawnPoints.Count; )
                {
                    if (Sectors.FindIndex(s => s.IsInSector(SpawnPoints[i].Coords)) == -1)
                        SpawnPoints.RemoveAt(i);
                    else
                        i++;
                }

                #endregion

                #region draw map and slice it

                //draw to memory
                Texture = UIParent.defaultTextures;

                RenderTarget2D mappicture = new RenderTarget2D(game.GraphicsDevice, (int)Parameters.Size.X, (int)Parameters.Size.Y, false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
                SpriteBatch sb = new SpriteBatch(game.GraphicsDevice);
                game.GraphicsDevice.SetRenderTarget(mappicture);
                game.GraphicsDevice.Clear(Parameters.BackColor);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                for (int x = 0; x < (int)(Parameters.Size.X / BackCrateTexture.Width + (Parameters.Size.X % BackCrateTexture.Width != 0 ? 1 : 0)); x++)
                    for (int y = 0; y < (int)(Parameters.Size.Y / BackCrateTexture.Height + (Parameters.Size.Y % BackCrateTexture.Height != 0 ? 1 : 0)); y++)
                        sb.Draw(Texture, new Vector2(x * BackCrateTexture.Width, y * BackCrateTexture.Height), BackCrateTexture, Parameters.BackCrateColor);
                for (int i = 0; i < Rects.Count; i++)
                    Rects[i].Draw(ref sb, Parameters.BlockingColor);
                sb.End();

                //slice map
                for (int x = 0; x < Slices.GetLength(0); x++)
                    for (int y = 0; y < Slices.GetLength(1); y++)
                    {
                        Slices[x, y] = new MapSlice(game, new Rectangle((int)(x * Parameters.Slicing.X), (int)(y * Parameters.Slicing.Y), (int)Parameters.Slicing.X, (int)Parameters.Slicing.Y));
                        game.GraphicsDevice.SetRenderTarget(Slices[x, y].picture);
                        game.GraphicsDevice.Clear(Color.Transparent);
                        sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                        sb.Draw(mappicture, Vector2.Zero, Slices[x, y].PositionAndSize, Color.White);
                        sb.End();
                    }

                game.GraphicsDevice.SetRenderTarget(null);

                mappicture.Dispose();
                sb.Dispose();
                Rects.Clear();

                #endregion
            }
            catch { }
        }

        public void Draw(ref SpriteBatch sb, int cameraX, int cameraY)
        {
            game.GraphicsDevice.SetRenderTarget(game.layerTargets[5]);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int x = 0; x < Slices.GetLength(0); x++)
                for (int y = 0; y < Slices.GetLength(1); y++)
                    Slices[x, y].Draw(ref sb, cameraX, cameraY);
            sb.End();
        }

        /// <param name="motor">note: motor is passed by ref but is not changed - ref only prevents object copying</param>
        /// <param name="displacementVec">resulting displacement vector</param>
        /// <param name="damage">damage inflicted by collision (if any)</param>
        /// <returns>true if any collision occured</returns>
        public bool TestCollisions(ref Motorek motor, out Vector2 displacementVec, out float damage)
        {
            bool is_collided = false;
            displacementVec = Vector2.Zero;
            damage = 0;

            Vector2 motorDirVec = Utils.CalculateDirectionVector(motor.rotation.ToRadians());
            foreach (MapEdge edge in Edges)
            {
                Vector2 dispVec = Vector2.Zero;

                bool collided = edge.TestCollision(motorDirVec, motor.boundPoints, out dispVec);

                is_collided |= collided;
                if (collided)
                {
                    if (motor.oldCollided.IndexOf(edge.Name) == -1)
                        damage += 1;
                    motor.newCollided.Add(edge.Name);
                    displacementVec += dispVec;
                }
            }
            return is_collided;
        }

        /// <param name="team">-1 to ignore team number, 0 - red team, 1 - blue team</param>
        /// <returns>true if spawn point found</returns>
        public bool GetBikeSpawnData(int team, out Vector2 position, out float rotation)
        {
            position = Vector2.Zero;
            rotation = 0.0f;

            if (SpawnPoints.Count == 0)
                return false;

            List<int> SpawnIDsAllowedToUse = new List<int>();
            if (team == -1)
            {
                for (int i = 0; i < SpawnPoints.Count; i++)
                    SpawnIDsAllowedToUse.Add(i);
            }
            else
            {
                for (int i = 0; i < SpawnPoints.Count; i++)
                    if (SpawnPoints[i].Team == team)
                        SpawnIDsAllowedToUse.Add(i);
            }

            //check spawn reusal condition (allows to at least partially reduce spawn collisions)
            int count = 0;
            for (int i = 0; i < SpawnInUse.Count; i++) //for all spawns...
                if (SpawnIDsAllowedToUse.Contains(i) && SpawnInUse[i]) //...check is this spawn available to use for this particular team case...
                    count++; //...and count it in if it's already used
            if (count == SpawnInUse.Where((siu, siu_id) => SpawnIDsAllowedToUse.Contains(siu_id)).Count()) //if all spawns available for this particular team case are used...
                for (int i = 0; i < SpawnInUse.Count; i++) //...then for all spawns...
                    if(SpawnIDsAllowedToUse.Contains(i)) //...if this spawn is available for this particular team case...
                        SpawnInUse[i] = false; //...enable spawn for use

            //get actual spawn ID from allowed, not used spawns
            int spawnID;
            do
            {
                spawnID = MotorkiGame.random.Next() % SpawnIDsAllowedToUse.Count;
            } while (SpawnInUse[SpawnIDsAllowedToUse[spawnID]]);
            spawnID = SpawnIDsAllowedToUse[spawnID];

            //get spawn data and mark it as used
            position = SpawnPoints[spawnID].Coords;
            rotation = SpawnPoints[spawnID].Rotation;
            SpawnInUse[spawnID] = true;
            return true;
        }

        public static List<UITaggedValue> EnumerateMaps(GameType gameType)
        {
            List<UITaggedValue> ret = new List<UITaggedValue>();

            var filenames = Directory.EnumerateFiles("./Maps", "*.map");
            foreach (string fn in filenames)
            {
                MapParameters param = new MapParameters(fn);
                if(param.SupportedGameTypes.Contains(gameType))
                    ret.Add(new UITaggedValue(param.Name, fn));
            }

            return ret;
        }
    }
}
