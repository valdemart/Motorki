using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Motorki.GameClasses;

namespace Motorki
{
    public enum GameType
    {
        DeathMatch, /*frag limit*/
        TeamDeathMatch, /*team frag limit*/
        PointMatch, /*point limit*/
        TeamPointMatch, /*team point limit*/
        Demolition, /*all must be destroyed - no respawn*/
        TeamDemolition, /*at least one team representative must survive*/
        TimeMatch, /*game time limit, gathered points determine winner*/
        TeamTimeMatch,
    }

    public enum Default_BotNames
    {
        Eric, Ralf, Peter, Robert, Julian, John, Sylvie, George, Sam, Walter
    }

    public enum GameKeyNames
    {
        GoLeft, GoRight, //relative and absolutive steering
        GoUp, GoDown, //absolutive steering only
        OpenChat, //open chat box (warning: bike rides forward while writing)
        BotMenu, //open bot menu box
        //bonuses
        SpeedUp, //speeds up players bike
        TraceWiden, //widens players trace (*1.5)
        RandomizationBomb, //forces enemy bikes to move randomly for specified amount of time
    }

    public enum Default_playerKeys
    {
        GoLeft = Keys.A, GoRight = Keys.D,
        GoUp = Keys.W, GoDown = Keys.S,
        OpenChat = Keys.T,
        BotMenu = Keys.V,
        //bonuses
        SpeedUp = Keys.D1,
        TraceWiden = Keys.D2,
        RandomizationBomb = Keys.D3,
    }

    public class MotorShortDescription
    {
        public Type type { get; private set; }
        public string name { get; private set; }
        public int playerID { get; private set; }

        /// <param name="playerID">for bot motors this value is (int)BotMotor.BotSophistication</param>
        public MotorShortDescription(Type type, string name, int playerID)
        {
            this.type = type;
            this.name = name;
            this.playerID = playerID;
        }
    }

    /// <summary>
    /// class that stores game settings and state (state probably will be put into GameState or GameMain class)
    /// </summary>
    public class GameSettings
    {
        //game settings
        public static string gameName { get; set; }
        public static Map gameMap { get; private set; }
        public static GameType gameType { get; set; }
        public static int gamePointLimit { get; set; }
        public static int gameFragLimit { get; set; }
        /// <summary>
        /// in minutes
        /// </summary>
        public static int gameTimeLimit { get; set; }
        public static string gameServerIP { get; set; } //gameServerName discarded because identical with gameName
        /// <summary>
        /// game slots settings.
        /// team is determined by index: 0-4 - red team, 5-9 - blue team
        /// </summary>
        public static MotorShortDescription[] gameSlots { get; set; }
        /// <summary>
        /// this table is filled only during game - contains server data about motors
        /// </summary>
        public static Motorek[] gameMotors { get; set; }
        public static GamePlay gamePlayScreen { get; set; }
        public static AgentController agentController { get; set; }

        //options: player
        public static string playerName { get; set; }
        public static Keys[] playerKeys { get; set; }
        public static PlayerMotor.Steering playerSteering { get; set; }
        public static Color playerColor { get; set; }

        //options: video
        public static int videoGraphMode { get; set; } //index on the list of available modes - card dependent
        public static bool videoFullscreen { get; set; }

        //options: audio
        public static bool audioEnabled { get; set; }
        public static int audioVolumePercent { get; set; }
        public static int audioSFXVolumePercent { get; set; }
        public static int audioMusicVolumePercent { get; set; }
        public static int audioUIVolumePercent { get; set; }

        //options: common
        public static bool gameoptNames { get; set; } //show bikes names
        public static bool gameoptMinimap { get; set; } //show minimaps
        public static bool gameoptPointers { get; set; } //show player pointers

        public GameSettings()
        {
            gameName = "NowaGra000";
            gameMap = null;
            gameType = GameType.DeathMatch;
            gamePointLimit = 0;
            gameFragLimit = 10;
            gameTimeLimit = 0;
            gameServerIP = "127.0.0.1";
            gameSlots = new MotorShortDescription[10];
            gameMotors = new Motorek[10];
            for (int i = 0; i < 10; i++)
            {
                gameSlots[i] = null;
                gameMotors[i] = null;
            }
            agentController = null;

            playerName = "Gracz1";
            playerKeys = new Keys[Enum.GetNames(typeof(GameKeyNames)).Count()];
            for (int i = 0; i < Enum.GetNames(typeof(GameKeyNames)).Count(); i++)
            {
                playerKeys[i] = (Keys)Enum.Parse(typeof(Default_playerKeys), Enum.GetNames(typeof(GameKeyNames))[i]);
            }
            playerSteering = PlayerMotor.Steering.Relative;
            playerColor = Color.Yellow;
            
            videoGraphMode = 0; //default graph mode
            videoFullscreen = false; //windowed by default

            audioEnabled = false;
            audioVolumePercent = 100;
            audioSFXVolumePercent = 75;
            audioMusicVolumePercent = 50;
            audioUIVolumePercent = 75;

            gameoptNames = true;
            gameoptMinimap = true;
            gameoptPointers = true;
        }

        public void SaveToFile(string filename)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("MotorkiSettingsFile");

            //player
            XElement player = new XElement("player");
            player.Add(new XElement("name", playerName));
            player.Add(new XElement("color", new XAttribute("r", playerColor.R), new XAttribute("g", playerColor.G), new XAttribute("b", playerColor.B)));
            player.Add(new XElement("keys"));
            for (int i = 0; i < Enum.GetNames(typeof(GameKeyNames)).Count(); i++)
                player.Element("keys").Add(new XElement(Enum.GetNames(typeof(GameKeyNames))[i], playerKeys[i].ToString()));
            player.Add(new XElement("steering", playerSteering));
            root.Add(player);

            //video
            XElement video = new XElement("video");
            video.Add(new XElement("mode", videoGraphMode));
            video.Add(new XElement("fullscreen", videoFullscreen));
            root.Add(video);

            //audio
            XElement audio = new XElement("audio");
            audio.Add(new XElement("enabled", audioEnabled));
            audio.Add(new XElement("volume", audioVolumePercent));
            audio.Add(new XElement("sfxvolume", audioSFXVolumePercent));
            audio.Add(new XElement("musicvolume", audioMusicVolumePercent));
            audio.Add(new XElement("uivolume", audioUIVolumePercent));
            root.Add(audio);

            //common options
            XElement common = new XElement("common");
            common.Add(new XElement("names", gameoptNames));
            common.Add(new XElement("minimap", gameoptMinimap));
            common.Add(new XElement("pointers", gameoptPointers));
            root.Add(common);

            doc.Add(root);
            doc.Save(filename);
        }

        public void LoadFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    XDocument doc = XDocument.Load(filename);

                    XElement root = doc.Element("MotorkiSettingsFile");
                    if (root == null)
                        return;

                    //player
                    XElement player = root.Element("player");
                    playerName = player.Element("name").Value;
                    playerColor = new Color(int.Parse(player.Element("color").Attribute("r").Value), int.Parse(player.Element("color").Attribute("g").Value), int.Parse(player.Element("color").Attribute("b").Value));
                    for (int i = 0; i < Enum.GetNames(typeof(GameKeyNames)).Count(); i++)
                        playerKeys[i] = (Keys)Enum.Parse(typeof(Keys), player.Element("keys").Element(Enum.GetNames(typeof(GameKeyNames))[i]).Value);
                    playerSteering = (PlayerMotor.Steering)Enum.Parse(typeof(PlayerMotor.Steering), player.Element("steering").Value);

                    //video
                    XElement video = root.Element("video");
                    videoGraphMode = int.Parse(video.Element("mode").Value);
                    videoFullscreen = bool.Parse(video.Element("fullscreen").Value);

                    //audio
                    XElement audio = root.Element("audio");
                    audioEnabled = bool.Parse(audio.Element("enabled").Value);
                    audioVolumePercent = int.Parse(audio.Element("volume").Value);
                    audioSFXVolumePercent = int.Parse(audio.Element("sfxvolume").Value);
                    audioMusicVolumePercent = int.Parse(audio.Element("musicvolume").Value);
                    audioUIVolumePercent = int.Parse(audio.Element("uivolume").Value);

                    //common options
                    XElement common = root.Element("common");
                    gameoptNames = bool.Parse(common.Element("names").Value);
                    gameoptMinimap = bool.Parse(common.Element("minimap").Value);
                    gameoptPointers = bool.Parse(common.Element("pointers").Value);
                }
                catch (Exception) { }
            }
        }

        public static void SelectMap(string gameMapPath)
        {
            if (gameMap != null)
                gameMap.Destroy();
            if (!File.Exists(gameMapPath))
                gameMap = null;
            else
                gameMap = new Map(MotorkiGame.game, gameMapPath);
        }
    }
}
