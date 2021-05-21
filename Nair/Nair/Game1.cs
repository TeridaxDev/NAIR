using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Nair.Classes;
using Nair.Classes.Managers;
using System.IO;
using System.Collections.Generic;

public enum GameState
{
    mainMenu,
    game,
    tutorial,
    intro,
    controller,
    onlineGame
}

namespace Nair
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private String versionNumber = "Version 1.2.0";

        public static int windowWidth = 800; //1280 | 2560 | 3840 | 2880
        public static int windowHeight = 600; //720 | 1440 | 2160 | 1800
        private static Color bgColor = Color.White;
        public static bool fullscreen = false;
        private static bool showHitboxes = false;
        private ControlProfile profile1;
        private ControlProfile profile2;
        private ControlProfile profile3;

        private static int usingKeyboard;

        private static double scaleX;
        private static double scaleY;

        private static GameState state;
        private Tutorial tutorial;

        private static Texture2D menu1;
        private static Texture2D menu2;
        private Texture2D buttonPlay1;
        private Texture2D buttonPlay2;
        private Texture2D buttonOptions1;
        private Texture2D buttonOptions2;
        private Texture2D buttonExit1;
        private Texture2D buttonExit2;
        private Texture2D buttonCancel1;
        private Texture2D buttonCancel2;
        private Texture2D buttonApply1;
        private Texture2D buttonApply2;
        private Texture2D buttonTutorial1;
        private Texture2D buttonTutorial2;
        private static Texture2D arrow;
        private Texture2D labelResolution;
        private Texture2D labelFullscreen;
        private Texture2D labelShowHitboxes;
        private Texture2D labelOn;
        private Texture2D labelOff;
        private Texture2D labelKeyboardIs;
        private Texture2D labelNightMode;
        private Texture2D resolutionList;
        private Texture2D pressStart;
        private Texture2D musicNotIncluded;
        private Texture2D customController;
        private Texture2D controlOptions1;
        private Texture2D controlOptions2;
        private Texture2D buttonBlank1;
        private Texture2D buttonBlank2;
        private Texture2D keySelect;
        private Texture2D keyboardBackground;

        private uint frame;
        private Texture2D[] title;
        private Texture2D[] title2;
        private Texture2D[] intro;
        private Texture2D splash;
        private int splashCount;
        private int splashNumber;

        private static Texture2D blankPixel;
        private static Texture2D transBlankPixel;
        private static SpriteFont courierNew;

        private Menu mainMenu;
        private Menu options;
        private Menu controls;
        private ControllerCustomizer controllerCustomizer;
        private KeyboardCustomizer keyboardCustomizer;

        public static OnlineGame rollbackManager;

        public static Texture2D BlankPixel { get => blankPixel; }
        public static Texture2D TransBlankPixel { get => transBlankPixel; }
        public static Color BGColor { get => bgColor; }
        public static Texture2D Menu1 { get => menu1; }
        public static Texture2D Menu2 { get => menu2; }
        public static Texture2D Arrow { get => arrow; }
        public static SpriteFont CourierNew { get => courierNew; }
        public static double ScaleX { get => scaleX; }
        public static double ScaleY { get => scaleY; }
        public static GameState GameState { get => state; set => state = value; }
        public static bool ShowHitboxes { get => showHitboxes; }
        public static int UsingKeyboard { get => usingKeyboard; }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = fullscreen;

            graphics.ApplyChanges();

            scaleX = ((double)Game1.windowWidth / 1920);
            scaleY = ((double)Game1.windowHeight / 1080);

            this.IsMouseVisible = false;

            profile1 = new ControlProfile(true);
            profile2 = new ControlProfile(true);
            profile3 = new ControlProfile(false);
            usingKeyboard = 0;
            profile1.LoadControllerProfile(1);
            profile2.LoadControllerProfile(2);
            profile3.LoadKeyProfile();

            AnimationManager.Initialize(graphics, Content);
            PlayerManager.Initialize(graphics, Content);
            PlayerManagerRollback.Initialize(graphics, Content);
            ControllerManager.Initialize(profile1, profile2, profile3);

            ControllerManager.Instance.SetPlayerIndexes(PlayerIndex.One, PlayerIndex.Two);
            ControllerManager.Update();

            state = GameState.intro;
            mainMenu = new Menu(new Rectangle(700, 530, 500, 550));
            options = new Menu(new Rectangle(500, 15, 920, 1070));
            controls = new Menu(new Rectangle(500, 120, 920, 900));

            rollbackManager = new OnlineGame();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blankPixel = Content.Load<Texture2D>("White-Pixel");
            transBlankPixel = Content.Load<Texture2D>("Trans-White-Pixel");
            courierNew = Content.Load<SpriteFont>("CourierNew");
            menu1 = Content.Load<Texture2D>("UI/Menu1");
            menu2 = Content.Load<Texture2D>("UI/Menu2");
            buttonPlay1 = Content.Load<Texture2D>("UI/Buttons/Play1");
            buttonPlay2 = Content.Load<Texture2D>("UI/Buttons/Play2");
            buttonOptions1 = Content.Load<Texture2D>("UI/Buttons/Options1");
            buttonOptions2 = Content.Load<Texture2D>("UI/Buttons/Options2");
            buttonExit1 = Content.Load<Texture2D>("UI/Buttons/Exit1");
            buttonExit2 = Content.Load<Texture2D>("UI/Buttons/Exit2");
            buttonCancel1 = Content.Load<Texture2D>("UI/Buttons/Cancel1");
            buttonCancel2 = Content.Load<Texture2D>("UI/Buttons/Cancel2");
            buttonApply1 = Content.Load<Texture2D>("UI/Buttons/Apply1");
            buttonApply2 = Content.Load<Texture2D>("UI/Buttons/Apply2");
            buttonTutorial1 = Content.Load<Texture2D>("UI/Buttons/Tutorial1");
            buttonTutorial2 = Content.Load<Texture2D>("UI/Buttons/Tutorial2");
            buttonBlank1 = Content.Load<Texture2D>("UI/Buttons/Blank1");
            buttonBlank2 = Content.Load<Texture2D>("UI/Buttons/Blank2");
            labelResolution = Content.Load<Texture2D>("UI/Resolution");
            labelFullscreen = Content.Load<Texture2D>("UI/Fullscreen");
            labelShowHitboxes = Content.Load<Texture2D>("UI/ShowHitboxes");
            arrow = Content.Load<Texture2D>("UI/Arrow");
            labelOn = Content.Load<Texture2D>("UI/On");
            labelOff = Content.Load<Texture2D>("UI/Off");
            resolutionList = Content.Load<Texture2D>("UI/Resolutions");
            pressStart = Content.Load<Texture2D>("PressStart");
            musicNotIncluded = Content.Load<Texture2D>("MusicNotIncluded");
            labelKeyboardIs = Content.Load<Texture2D>("UI/KeyboardIs");
            customController = Content.Load<Texture2D>("UI/Controller");
            controlOptions1 = Content.Load<Texture2D>("UI/Controls1");
            controlOptions2 = Content.Load<Texture2D>("UI/Controls2");
            keySelect = Content.Load<Texture2D>("UI/KeySelect");
            keyboardBackground = Content.Load<Texture2D>("UI/KeyboardBackground");
            labelNightMode = Content.Load<Texture2D>("UI/NightMode");
            splash = Content.Load<Texture2D>("Splash");
            splashCount = splash.Height / 100;
            splashNumber = new Random().Next(0, splashCount);


            title = new Texture2D[2];
            title[0] = Content.Load<Texture2D>("Title");
            title[1] = Content.Load<Texture2D>("Title2");
            intro = new Texture2D[2];
            intro[0] = Content.Load<Texture2D>("Intro1");
            intro[1] = Content.Load<Texture2D>("Intro2");
            title2 = new Texture2D[2];
            title2[0] = Content.Load<Texture2D>("Title3");
            title2[1] = Content.Load<Texture2D>("Title4");

            OptionWheel resBut = new OptionWheel(new Rectangle(935, 360, 230, 80), resolutionList, 24);
            OptionWheel fulBut = new OptionWheel(new Rectangle(935, 470, 230, 80));
            OptionWheel hitBut = new OptionWheel(new Rectangle(935, 580, 230, 80));
            OptionWheel nightMode = new OptionWheel(new Rectangle(935, 690, 230, 80));

            OptionWheel keyBut = new OptionWheel(new Rectangle(845, 640, 230, 80));

            fulBut.AddOption(labelOff);
            fulBut.AddOption(labelOn);
            hitBut.AddOption(labelOff);
            hitBut.AddOption(labelOn);
            nightMode.AddOption(labelOff);
            nightMode.AddOption(labelOn);
            keyBut.AddOption(labelOff);
            keyBut.AddOption(Content.Load<Texture2D>("UI/Player1"));
            keyBut.AddOption(Content.Load<Texture2D>("UI/Player2"));

            mainMenu.AddButton(new Button(new Rectangle(845, 580, 230, 80), buttonPlay1, buttonPlay2));
            mainMenu.AddButton(new Button(new Rectangle(845, 675, 230, 80), buttonPlay1, buttonPlay2));
            mainMenu.AddButton(new Button(new Rectangle(845, 770, 230, 80), buttonTutorial1, buttonTutorial2));
            mainMenu.AddButton(new Button(new Rectangle(845, 865, 230, 80), buttonOptions1, buttonOptions2));
            mainMenu.AddButton(new Button(new Rectangle(845, 960, 230, 80), buttonExit1, buttonExit2));

            options.AddButton(new Button(new Rectangle(845, 170, 230, 80), Content.Load<Texture2D>("UI/Buttons/Controls1"), Content.Load<Texture2D>("UI/Buttons/Controls2")));
            options.AddButton(resBut);
            options.AddButton(fulBut);
            options.AddButton(hitBut);
            options.AddButton(nightMode);
            options.AddButton(new Button(new Rectangle(845, 880, 230, 80), buttonApply1, buttonApply2));
            options.AddButton(new Button(new Rectangle(845, 970, 230, 80), buttonCancel1, buttonCancel2));

            controls.AddButton(new Button(new Rectangle(845, 250, 230, 80), Content.Load<Texture2D>("UI/Buttons/Player11"), Content.Load<Texture2D>("UI/Buttons/Player12")));
            controls.AddButton(new Button(new Rectangle(845, 340, 230, 80), Content.Load<Texture2D>("UI/Buttons/Player21"), Content.Load<Texture2D>("UI/Buttons/Player22")));
            controls.AddButton(new Button(new Rectangle(845, 430, 230, 80), Content.Load<Texture2D>("UI/Buttons/Keyboard1"), Content.Load<Texture2D>("UI/Buttons/Keyboard2")));
            controls.AddButton(keyBut);
            controls.AddButton(new Button(new Rectangle(845, 900, 230, 80), buttonCancel1, buttonCancel2));

            //ControllerManager.Update();
            ControllerManager.Update();
            List<Texture2D> tutorialPages = new List<Texture2D>();
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page1"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page2"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page3"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page4"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page5"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page6"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page7"));
            tutorialPages.Add(Content.Load<Texture2D>("UI/Tutorial/Page8"));
            tutorial = new Tutorial(Content.Load<Texture2D>("UI/CurrentState"), Content.Load<Texture2D>("UI/States"), tutorialPages);

            //Read from config.cfg
            StreamReader reader = null;
            try
            {
                Stream streamIn = File.OpenRead("Config.cfg");
                reader = new StreamReader(streamIn);

                //Resolution
                string line = reader.ReadLine();
                line = line.Substring(12);
                windowWidth = int.Parse(line.Split('x')[0]);
                windowHeight = int.Parse(line.Split('x')[1].TrimEnd(';'));

                switch (line.TrimEnd(';'))
                {
                    case "800x600":
                        options.SetValue(1, 0);
                        break;
                    case "1024x768":
                        options.SetValue(1, 1);
                        break;
                    case "1152x864":
                        options.SetValue(1, 2);
                        break;
                    case "1280x720":
                        options.SetValue(1, 3);
                        break;
                    case "1280x768":
                        options.SetValue(1, 4);
                        break;
                    case "1280x800":
                        options.SetValue(1, 5);
                        break;
                    case "1280x960":
                        options.SetValue(1, 6);
                        break;
                    case "1280x1024":
                        options.SetValue(1, 7);
                        break;
                    case "1360x768":
                        options.SetValue(1, 8);
                        break;
                    case "1366x768":
                        options.SetValue(1, 9);
                        break;
                    case "1600x900":
                        options.SetValue(1, 10);
                        break;
                    case "1600x1024":
                        options.SetValue(1, 11);
                        break;
                    case "1600x1200":
                        options.SetValue(1, 12);
                        break;
                    case "1680x1050":
                        options.SetValue(1, 13);
                        break;
                    case "1920x1080":
                        options.SetValue(1, 14);
                        break;
                    case "1920x1200":
                        options.SetValue(1, 15);
                        break;
                    case "1920x1440":
                        options.SetValue(1, 16);
                        break;
                    case "2048x1536":
                        options.SetValue(1, 17);
                        break;
                    case "2048x1800":
                        options.SetValue(1, 18);
                        break;
                    case "2560x1080":
                        options.SetValue(1, 19);
                        break;
                    case "2560x1440":
                        options.SetValue(1, 20);
                        break;
                    case "2880x1800":
                        options.SetValue(1, 21);
                        break;
                    case "3440x1440":
                        options.SetValue(1, 22);
                        break;
                    case "3840x2160":
                        options.SetValue(1, 23);
                        break;
                    default:
                        options.SetValue(1, 14);
                        break;
                }

                //Fullscreen
                line = reader.ReadLine();
                line = line.Substring(12);
                if (line == "off;")
                {
                    fullscreen = false;
                    options.SetValue(2, 0);
                }
                else
                {
                    fullscreen = true;
                    options.SetValue(2, 1);
                }

                //Hitboxes
                line = reader.ReadLine();
                line = line.Substring(15);
                if (line == "off;")
                {
                    showHitboxes = false;
                    options.SetValue(3, 0);
                }
                else
                {
                    showHitboxes = true;
                    options.SetValue(3, 1);
                }
                //Night Mode
                line = reader.ReadLine();
                line = line.Substring(12);
                if (line == "off;")
                {
                    bgColor = Color.White;
                    options.SetValue(4, 0);
                }
                else
                {
                    bgColor = new Color(65f / 255f, 65f / 255f, 65f / 255f);
                    options.SetValue(4, 1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                graphics.PreferredBackBufferWidth = windowWidth;
                graphics.PreferredBackBufferHeight = windowHeight;
                scaleX = ((double)Game1.windowWidth / 1920);
                scaleY = ((double)Game1.windowHeight / 1080);
                graphics.IsFullScreen = fullscreen;
                graphics.ApplyChanges();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // Maybe this is important
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            switch (state)
            {
                case GameState.intro:
                    frame++;
                    if (frame == 180)
                    {
                        frame = 0;
                        state = GameState.mainMenu;
                    }
                    if (mainMenu.Hidden && (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start)
                        || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start) || GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.Start)
                        || GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.Start)))
                    {
                        frame = 0;
                        state = GameState.mainMenu;
                    }
                        break;
                case GameState.game:
                    ControllerManager.Update();
                    PlayerManager.Instance.Update();
                    AnimationManager.Update();
                    break;
                case GameState.onlineGame:
                    rollbackManager.Update(gameTime);
                    break;
                case GameState.tutorial:
                    tutorial.Update();
                    ControllerManager.Update();
                    AnimationManager.Update();
                    break;
                case GameState.mainMenu:
                    frame++;
                    if (mainMenu.Hidden && (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start) 
                        || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start) || GamePad.GetState(PlayerIndex.Three).IsButtonDown(Buttons.Start) 
                        || GamePad.GetState(PlayerIndex.Four).IsButtonDown(Buttons.Start)))
                    {
                        mainMenu.Show();
                    }
                    if(!mainMenu.Hidden)
                    {
                        int action;
                        if (controllerCustomizer != null)
                            action = controllerCustomizer.Update();
                        else if (keyboardCustomizer != null)
                            action = keyboardCustomizer.Update();
                        else if (options.Hidden)
                            action = mainMenu.Update();
                        else if (controls.Hidden)
                            action = options.Update();
                        else
                            action = controls.Update();


                        //Do an action based on what menu item is selected
                        if(controllerCustomizer != null)
                        {
                            if(action == -1 || action == 1)
                            {
                                controllerCustomizer = null;
                            }
                            else if(action == 2)
                            {
                                controllerCustomizer.Profile.SetDefaults();
                                ControlProfile profile = controllerCustomizer.Profile;
                                int controller = controllerCustomizer.Controller;
                                controllerCustomizer = null;
                                controllerCustomizer = new ControllerCustomizer(customController, profile, controlOptions1, controlOptions2, controller);
                            }

                        }
                        else if(keyboardCustomizer != null)
                        {
                            if(action == 1)
                            {
                                keyboardCustomizer = null;
                                this.IsMouseVisible = false;
                            }
                            else if(action == 2)
                            {
                                keyboardCustomizer.Profile.SetDefaults();
                                keyboardCustomizer = new KeyboardCustomizer(profile3, buttonBlank1, buttonBlank2, keySelect, keyboardBackground);
                            }
                        }
                        else if(options.Hidden)
                            switch (action)
                            {
                                case -1:
                                    break;
                                case 0:
                                    mainMenu.Hide();
                                    state = GameState.game;
                                    PlayerManager.Instance.ResetGame(true);
                                    frame = 0;
                                    break;
                                case 1:
                                    mainMenu.Hide();
                                    state = GameState.onlineGame;
                                    rollbackManager.Initialize();
                                    frame = 0;
                                    break;
                                case 2:
                                    mainMenu.Hide();
                                    state = GameState.tutorial;
                                    tutorial.Reset();
                                    frame = 0;
                                    break;
                                case 3:
                                    options.Show();
                                    break;
                                case 4:
                                    this.Exit();
                                    break;
                            }
                        else if(controls.Hidden)
                            switch(action)
                            {
                                case -1:
                                    break;
                                case 0:
                                    controls.Show();
                                    break;
                                //Write a new options.txt
                                case 5:
                                    StreamWriter writer = null;
                                    try
                                    {
                                        Stream streamOut = File.OpenWrite("Config.cfg");
                                        writer = new StreamWriter(streamOut);

                                        writer.Write("resolution: ");
                                        switch(options.GetValue(1))
                                        {
                                            case 0:
                                                writer.WriteLine("800x600;");
                                                windowWidth = 800;
                                                windowHeight = 600;
                                                break;
                                            case 1:
                                                writer.WriteLine("1024x768;");
                                                windowWidth = 1024;
                                                windowHeight = 768;
                                                break;
                                            case 2:
                                                writer.WriteLine("1152x864;");
                                                windowWidth = 1152;
                                                windowHeight = 864;
                                                break;
                                            case 3:
                                                writer.WriteLine("1280x720;");
                                                windowWidth = 1280;
                                                windowHeight = 720;
                                                break;
                                            case 4:
                                                writer.WriteLine("1280x768;");
                                                windowWidth = 1280;
                                                windowHeight = 768;
                                                break;
                                            case 5:
                                                writer.WriteLine("1280x800;");
                                                windowWidth = 1280;
                                                windowHeight = 800;
                                                break;
                                            case 6:
                                                writer.WriteLine("1280x960;");
                                                windowWidth = 1280;
                                                windowHeight = 960;
                                                break;
                                            case 7:
                                                writer.WriteLine("1280x1024;");
                                                windowWidth = 1280;
                                                windowHeight = 1024;
                                                break;
                                            case 8:
                                                writer.WriteLine("1360x768;");
                                                windowWidth = 1360;
                                                windowHeight = 768;
                                                break;
                                            case 9:
                                                writer.WriteLine("1366x768;");
                                                windowWidth = 1366;
                                                windowHeight = 768;
                                                break;
                                            case 10:
                                                writer.WriteLine("1600x900;");
                                                windowWidth = 1600;
                                                windowHeight = 900;
                                                break;
                                            case 11:
                                                writer.WriteLine("1600x1024;");
                                                windowWidth = 1600;
                                                windowHeight = 1024;
                                                break;
                                            case 12:
                                                writer.WriteLine("1600x1200;");
                                                windowWidth = 1600;
                                                windowHeight = 1200;
                                                break;
                                            case 13:
                                                writer.WriteLine("1680x1050;");
                                                windowWidth = 1680;
                                                windowHeight = 1050;
                                                break;
                                            case 14:
                                                writer.WriteLine("1920x1080;");
                                                windowWidth = 1920;
                                                windowHeight = 1080;
                                                break;
                                            case 15:
                                                writer.WriteLine("1920x1200;");
                                                windowWidth = 1920;
                                                windowHeight = 1200;
                                                break;
                                            case 16:
                                                writer.WriteLine("1920x1440;");
                                                windowWidth = 1920;
                                                windowHeight = 1440;
                                                break;
                                            case 17:
                                                writer.WriteLine("2048x1536;");
                                                windowWidth = 2048;
                                                windowHeight = 1536;
                                                break;
                                            case 18:
                                                writer.WriteLine("2048x1800;");
                                                windowWidth = 2048;
                                                windowHeight = 1800;
                                                break;
                                            case 19:
                                                writer.WriteLine("2560x1080;");
                                                windowWidth = 2560;
                                                windowHeight = 1080;
                                                break;
                                            case 20:
                                                writer.WriteLine("2560x1440;");
                                                windowWidth = 2560;
                                                windowHeight = 1440;
                                                break;
                                            case 21:
                                                writer.WriteLine("2880x1800;");
                                                windowWidth = 2880;
                                                windowHeight = 1800;
                                                break;
                                            case 22:
                                                writer.WriteLine("3440x1440;");
                                                windowWidth = 3440;
                                                windowHeight = 1440;
                                                break;
                                            case 23:
                                                writer.WriteLine("3840x2160;");
                                                windowWidth = 3840;
                                                windowHeight = 2160;
                                                break;
                                        }
                                        writer.Write("fullscreen: ");
                                        switch(options.GetValue(2))
                                        {
                                            case 0:
                                                writer.WriteLine("off;");
                                                fullscreen = false;
                                                break;
                                            case 1:
                                                writer.WriteLine("on;");
                                                fullscreen = true;
                                                break;

                                        }
                                        writer.Write("show hitboxes: ");
                                        switch (options.GetValue(3))
                                        {
                                            case 0:
                                                writer.WriteLine("off;");
                                                showHitboxes = false;
                                                break;
                                            case 1:
                                                writer.WriteLine("on;");
                                                showHitboxes = true;
                                                break;

                                        }
                                        writer.Write("night mode: ");
                                        switch (options.GetValue(4))
                                        {
                                            case 0:
                                                writer.WriteLine("off;");
                                                bgColor = Color.White;
                                                break;
                                            case 1:
                                                writer.WriteLine("on;");
                                                bgColor = new Color(65f / 255f, 65f / 255f, 65f / 255f);
                                                break;
                                        }

                                    }
                                    catch(Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    finally
                                    {
                                        if(writer != null)
                                        {
                                            writer.Close();
                                        }

                                        graphics.PreferredBackBufferWidth = windowWidth;
                                        graphics.PreferredBackBufferHeight = windowHeight;
                                        scaleX = ((double)Game1.windowWidth / 1920);
                                        scaleY = ((double)Game1.windowHeight / 1080);
                                        graphics.IsFullScreen = fullscreen;
                                        graphics.ApplyChanges();
                                    }
                                    break;
                                case 6:
                                    options.Hide();
                                    break;
                            }
                        else
                        {
                            switch(action)
                            {
                                case 0:
                                    controllerCustomizer = new ControllerCustomizer(customController, profile1, controlOptions1, controlOptions2, 0);
                                    break;
                                case 1:
                                    controllerCustomizer = new ControllerCustomizer(customController, profile2, controlOptions1, controlOptions2, 1);
                                    break;
                                case 2:
                                    keyboardCustomizer = new KeyboardCustomizer(profile3, buttonBlank1, buttonBlank2, keySelect, keyboardBackground);
                                    this.IsMouseVisible = true;
                                    break;
                                case 4:
                                    int value = controls.GetValue(3);

                                    usingKeyboard = value;

                                    controls.Hide();
                                    break;
                            }
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(bgColor);

            spriteBatch.Begin();

            switch (state)
            {
                case GameState.intro:
                    if(bgColor == Color.White)
                        spriteBatch.Draw(intro[(frame / 25) % 2], new Rectangle(0, 0, windowWidth, windowHeight), Color.White * (float)(0.005 + ((0.1 - 0.005) / 3) * (frame - 1)));
                    else
                        spriteBatch.Draw(intro[(frame / 25) % 2], new Rectangle(0, 0, windowWidth, windowHeight), bgColor);
                    break;
                case GameState.game:
                    //Draw the Floor
                    spriteBatch.Draw(blankPixel, new Rectangle(0, (int)Math.Ceiling(900 * ((double)windowHeight / 1080)), windowWidth, (int)Math.Ceiling((((double)windowHeight / 1080) * 5))), Color.Black);
                    //Draw the Game
                    PlayerManager.Instance.Draw(spriteBatch);
                    break;
                case GameState.onlineGame:
                    spriteBatch.Draw(blankPixel, new Rectangle(0, (int)Math.Ceiling(900 * ((double)windowHeight / 1080)), windowWidth, (int)Math.Ceiling((((double)windowHeight / 1080) * 5))), Color.Black);
                    
                    rollbackManager.Draw(spriteBatch);
                    break;
                case GameState.tutorial:
                    //Draw the Floor
                    spriteBatch.Draw(blankPixel, new Rectangle(0, (int)Math.Ceiling(900 * ((double)windowHeight / 1080)), windowWidth, (int)Math.Ceiling((((double)windowHeight / 1080) * 5))), Color.Black);
                    //Draw the Game
                    tutorial.Draw(spriteBatch);
                    break;
                case GameState.mainMenu:
                    //Display the version Number
                    spriteBatch.DrawString(courierNew, versionNumber, new Vector2(0, (int)Math.Ceiling(Game1.ScaleY * (1020))) , Color.Black, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080) * new Vector2(0.75f, 0.75f), SpriteEffects.None, 0f);
                    //Draw title
                    if(bgColor == Color.White)
                        spriteBatch.Draw(title[(frame / 35) % 2], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 460), (int)Math.Ceiling(Game1.ScaleY * 100), (int)Math.Ceiling(Game1.ScaleX * 1000), (int)Math.Ceiling(Game1.ScaleY * 700)), Color.White); //Title
                    else
                        spriteBatch.Draw(title2[(frame / 35) % 2], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 460), (int)Math.Ceiling(Game1.ScaleY * 100), (int)Math.Ceiling(Game1.ScaleX * 1000), (int)Math.Ceiling(Game1.ScaleY * 700)), Color.White); //Title
                    if (frame <= 420)
                        spriteBatch.Draw(pressStart, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 850), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White); //Press start
                    else if(frame <= 450)
                        spriteBatch.Draw(pressStart, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 850), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White * (1f/(frame - 420))); //Press start
                    else if(frame < 480)
                    {
                        spriteBatch.Draw(musicNotIncluded, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 850), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White * ((frame - 450) / 30f)); //Music not included
                        spriteBatch.Draw(splash, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 950), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), new Rectangle(0, 100 * splashNumber, 450, 100), Color.White * ((frame - 450) / 30f));
                    }
                    else if(frame >= 480)
                    {
                        spriteBatch.Draw(musicNotIncluded, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 850), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White); //Music not included
                        spriteBatch.Draw(splash, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 725), (int)Math.Ceiling(Game1.ScaleY * 950), (int)Math.Ceiling(Game1.ScaleX * 450), (int)Math.Ceiling(Game1.ScaleY * 100)), new Rectangle(0, 100 * splashNumber, 450, 100), Color.White);
                    }

                    if (!mainMenu.Hidden)
                        mainMenu.Draw(spriteBatch);
                    if (!options.Hidden)
                    {
                        options.Draw(spriteBatch);
                        spriteBatch.Draw(labelResolution, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 605), (int)Math.Ceiling(Game1.ScaleY * 360), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);
                        spriteBatch.Draw(labelFullscreen, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 605), (int)Math.Ceiling(Game1.ScaleY * 470), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);
                        spriteBatch.Draw(labelShowHitboxes, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 605), (int)Math.Ceiling(Game1.ScaleY * 580), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);
                        spriteBatch.Draw(labelNightMode, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 605), (int)Math.Ceiling(Game1.ScaleY * 690), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);
                    }
                    if(!controls.Hidden)
                    {
                        controls.Draw(spriteBatch);
                        switch(controls.GetValue(3))
                        {
                            case 0:
                                spriteBatch.Draw(Game1.BlankPixel, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 845), (int)Math.Ceiling(Game1.ScaleY * 430), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), bgColor * 0.8f);
                                break;
                            default:
                                spriteBatch.Draw(Game1.BlankPixel, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 845), (int)Math.Ceiling(Game1.ScaleY * 340), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), bgColor * 0.8f);
                                break;
                        }
                        spriteBatch.Draw(labelKeyboardIs, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 845), (int)Math.Ceiling(Game1.ScaleY * 550), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);
                    }
                    if(controllerCustomizer != null)
                    {
                        controllerCustomizer.Draw(spriteBatch);
                    }
                    if(keyboardCustomizer != null)
                    {
                        keyboardCustomizer.Draw(spriteBatch);
                    }
                    break;
            }

            //Display the framerate
            if (showHitboxes)
            {
                Color fpsColor = Color.Black;
                float FPS = 1000 / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (Math.Ceiling(FPS) < 60) fpsColor = Color.Red;
                spriteBatch.DrawString(courierNew, FPS.ToString(), new Vector2(0, (int)Math.Ceiling(Game1.ScaleY * 960)), fpsColor, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080), SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
