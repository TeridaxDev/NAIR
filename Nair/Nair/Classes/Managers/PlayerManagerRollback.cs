using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Nair.Classes.Managers
{
    class PlayerManagerRollback
    {
        Dictionary<string, SoundEffect> soundEffects;

        private static Texture2D buttonResume1;
        private static Texture2D buttonResume2;
        private Texture2D buttonRestart1;
        private Texture2D buttonRestart2;
        private static Texture2D buttonMenu1;
        private static Texture2D buttonMenu2;
        private Texture2D buttonNewGame1;
        private Texture2D buttonNewGame2;

        private static PlayerManagerRollback instance;

        private GraphicsDeviceManager graphics;
        private ContentManager Content;

        //Messages
        private Texture2D three;
        private Texture2D two;
        private Texture2D one; //120
        private Texture2D go; //400
        private Texture2D game; //700
        private Texture2D wins; //600
        private Texture2D playerText; //850
        private Texture2D draw; //700

        private Player player1;
        private int player1Score;
        private Player player2;
        private int player2Score;

        private int hitLag;
        private int floorLocation;
        private int time;
        private int frame;
        private bool gameOver;
        private bool countDown;
        private bool displayWinner;

        private SoundEffectInstance clap;
        private Random rng;

        private Menu postGameMenu;
        private Menu pauseMenu;

        public static PlayerManagerRollback Instance { get => instance; }
        public int FloorLocation { get => floorLocation; }

        public static Texture2D ButtonResume1 { get => buttonResume1; }
        public static Texture2D ButtonResume2 { get => buttonResume2; }
        public static Texture2D ButtonMenu1 { get => buttonMenu1; }
        public static Texture2D ButtonMenu2 { get => buttonMenu2; }

        private PlayerManagerRollback(GraphicsDeviceManager graphics, ContentManager content)
        {
            this.graphics = graphics;
            this.Content = content;
            soundEffects = new Dictionary<string, SoundEffect>();
            rng = new Random();

            //Messages
            three = Content.Load<Texture2D>("messages/three");
            two = Content.Load<Texture2D>("messages/two");
            one = Content.Load<Texture2D>("messages/one");
            go = Content.Load<Texture2D>("messages/go");
            game = Content.Load<Texture2D>("messages/game");
            wins = Content.Load<Texture2D>("messages/wins");
            playerText = Content.Load<Texture2D>("messages/player");
            draw = Content.Load<Texture2D>("messages/draw");
            //Buttons
            buttonResume1 = Content.Load<Texture2D>("UI/Buttons/Resume1");
            buttonResume2 = Content.Load<Texture2D>("UI/Buttons/Resume2");
            buttonRestart1 = Content.Load<Texture2D>("UI/Buttons/Restart1");
            buttonRestart2 = Content.Load<Texture2D>("UI/Buttons/Restart2");
            buttonMenu1 = Content.Load<Texture2D>("UI/Buttons/Menu1");
            buttonMenu2 = Content.Load<Texture2D>("UI/Buttons/Menu2");
            buttonNewGame1 = Content.Load<Texture2D>("UI/Buttons/NewGame1");
            buttonNewGame2 = Content.Load<Texture2D>("UI/Buttons/NewGame2");
            //SFX
            soundEffects.Add("dust1", Content.Load<SoundEffect>("audio/dust1"));
            soundEffects.Add("land1", Content.Load<SoundEffect>("audio/land1"));
            soundEffects.Add("fullhop", Content.Load<SoundEffect>("audio/fullhop"));
            soundEffects.Add("shorthop", Content.Load<SoundEffect>("audio/shorthop"));
            soundEffects.Add("skid1", Content.Load<SoundEffect>("audio/skid1"));
            soundEffects.Add("swordSwing", Content.Load<SoundEffect>("audio/swordSwing"));
            soundEffects.Add("smack", Content.Load<SoundEffect>("audio/smack"));
            soundEffects.Add("boom", Content.Load<SoundEffect>("audio/boom"));
            soundEffects.Add("clap1", Content.Load<SoundEffect>("audio/clap1"));
            soundEffects.Add("clap2", Content.Load<SoundEffect>("audio/clap2"));
            soundEffects.Add("clap3", Content.Load<SoundEffect>("audio/clap3"));
            soundEffects.Add("click", Content.Load<SoundEffect>("audio/click"));


            floorLocation = 900;
            hitLag = 0;
            gameOver = false;
            countDown = true;

            postGameMenu = new Menu(new Rectangle(700, 400, 500, 400));
            postGameMenu.AddButton(new Button(new Rectangle(845, 455, 230, 130), buttonNewGame1, buttonNewGame2));
            postGameMenu.AddButton(new Button(new Rectangle(845, 635, 230, 80), buttonMenu1, buttonMenu2));

            pauseMenu = new Menu(new Rectangle(700, 300, 500, 550));
            pauseMenu.AddButton(new Button(new Rectangle(845, 455, 230, 80), buttonResume1, buttonResume2));
            pauseMenu.AddButton(new Button(new Rectangle(845, 585, 230, 80), buttonRestart1, buttonRestart2));
            pauseMenu.AddButton(new Button(new Rectangle(845, 715, 230, 80), buttonMenu1, buttonMenu2));

            this.ResetGame(true);

        }

        public static void Initialize(GraphicsDeviceManager graphics, ContentManager content)
        {
            if (instance == null)
            {
                instance = new PlayerManagerRollback(graphics, content);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Update both players
        /// </summary>
        public void Update()
        {
            if (postGameMenu.Hidden && pauseMenu.Hidden)
            {
                if (countDown)
                {
                    if (frame == 120 || frame == 90 || frame == 60 || frame == 30)
                        this.PlaySound("boom");

                    frame--;

                    if (frame == 0)
                        countDown = false;
                }
                //Gameplay
                else if (time > 0 && (player1.Health > 0 && player2.Health > 0) && !gameOver && !countDown)
                {
                    //Count down the timer
                    frame++;
                    if (frame % 60 == 0) time--;

                    //Time out hitlag
                    if (hitLag > 0)
                        hitLag--;
                    //Update the players
                    else
                    {
                        player1.Update();
                        player2.Update();
                        HandleAttacks();
                    }
                }
                //Duration of the "game" title being shown
                else if (gameOver && hitLag != 0)
                {
                    if (hitLag == 60)
                        this.PlaySound("boom");

                    hitLag--;

                    if (hitLag == 0 && (player1Score == 2 || player2Score == 2))
                    {
                        displayWinner = true;
                        switch (rng.Next(0, 3))
                        {
                            case 0:
                                clap = this.GetSound("clap1");
                                break;
                            case 1:
                                clap = this.GetSound("clap2");
                                break;
                            case 2:
                                clap = this.GetSound("clap3");
                                break;

                        }
                        clap.Play();
                    }
                }
                //Post "game"
                else if (gameOver)
                {
                    player1.Update();
                    player2.Update();
                    frame++;

                    if (frame == 240 && player1Score < 2 && player2Score < 2)
                    {
                        this.ResetGame(false);
                    }
                    else if ((player1Score == 2 || player2Score == 2) && (ControllerManager.GetAction(1, Actions.start, false) || ControllerManager.GetAction(2, Actions.start, false) || ControllerManager.GetAction(3, Actions.start, false)))
                    {
                        postGameMenu.Show();
                    }
                }
                //Set the game to "over:
                else
                {
                    gameOver = true;
                    hitLag = 60;
                    frame = 0;

                    //Win conditions
                    if (player1.Health >= player2.Health)
                        player1Score++;
                    if (player2.Health >= player1.Health)
                        player2Score++;
                }

                if (!(gameOver && (player1Score == 2 || player2Score == 2)) && ((ControllerManager.GetAction(1, Actions.start, false) && !ControllerManager.GetAction(1, Actions.start, true))
                    || (ControllerManager.GetAction(2, Actions.start, false) && !ControllerManager.GetAction(2, Actions.start, true))
                    || (ControllerManager.GetAction(3, Actions.start, false) && !ControllerManager.GetAction(3, Actions.start, true))))
                {
                    pauseMenu.Show();
                }
            }
            else if (!postGameMenu.Hidden)
            {
                int cursor = postGameMenu.Update();

                switch (cursor)
                {
                    case -1:
                        break;
                    case 0:
                        this.ResetGame(true);
                        break;
                    case 1:
                        Game1.GameState = GameState.mainMenu;
                        break;
                }
            }
            else if (!pauseMenu.Hidden)
            {
                if ((ControllerManager.GetAction(1, Actions.start, false) && !ControllerManager.GetAction(1, Actions.start, true))
                    || (ControllerManager.GetAction(2, Actions.start, false) && !ControllerManager.GetAction(2, Actions.start, true))
                    || (ControllerManager.GetAction(3, Actions.start, false) && !ControllerManager.GetAction(3, Actions.start, true)))
                {
                    pauseMenu.Hide();
                }

                int cursor = pauseMenu.Update();

                switch (cursor)
                {
                    case -1:
                        break;
                    case 0:
                        pauseMenu.Hide();
                        break;
                    case 1:
                        this.ResetGame(true);
                        break;
                    case 2:
                        Game1.GameState = GameState.mainMenu;
                        break;
                }
            }

        }

        /// <summary>
        /// Handles collisions of hitboxes between the two players
        /// </summary>
        public void HandleAttacks()
        {
            bool p1Hit = false;
            bool p2Hit = false;
            Direction p1Facing = player1.Facing;
            Direction p2Facing = player2.Facing;

            //Check P1 for attacks
            if (player1.State == PlayerState.attack && !player1.Rehit && player2.State != PlayerState.dodge)
            {
                foreach (Rectangle hitbox in player1.Hitbox)
                {
                    if (hitbox.Intersects(player2.Hurtbox))
                    {
                        p2Hit = true;
                        player1.Rehit = true;
                    }
                }
            }
            //Check P2 for attacks
            if (player2.State == PlayerState.attack && !player2.Rehit && player1.State != PlayerState.dodge)
            {
                foreach (Rectangle hitbox in player2.Hitbox)
                {
                    if (hitbox.Intersects(player1.Hurtbox))
                    {
                        p1Hit = true;
                        player2.Rehit = true;
                    }
                }
            }

            if (p1Hit || p2Hit)
            {
                hitLag += 10;
                this.PlaySound("smack");
            }

            Vector2 velocity = new Vector2();

            //Handle attacks
            if (p1Hit)
            {
                //Facing left
                if (p2Facing == Direction.left)
                    velocity = new Vector2(-15, -6);
                //Facing right
                else if (p2Facing == Direction.right)
                    velocity = new Vector2(15, -6);

                player1.Hit(velocity, player2.AttackDamage);
                player2.Health += player2.AttackDamage;
            }
            if (p2Hit)
            {
                //Facing left
                if (p1Facing == Direction.left)
                    velocity = new Vector2(-15, -6);
                //Facing right
                else if (p1Facing == Direction.right)
                    velocity = new Vector2(15, -6);

                player2.Hit(velocity, player1.AttackDamage);
                player1.Health += player1.AttackDamage;
            }

        }

        /// <summary>
        /// Reset the timer and both players
        /// </summary>
        /// <param name="resetRound">Whether or not to set the scores to 0</param>
        public void ResetGame(bool resetRound)
        {
            int playernumber = 1;
            if (Game1.UsingKeyboard == 1) playernumber = 3;

            player1 = new Player(
                new Vector2(100, floorLocation - 250),                                                                                              //Player Position
                new Rectangle(100, floorLocation - 250, 150, 250),                                                                                  //Player hurtbox
                Direction.right,                                                                                                                    //Direction being faced
                1,                                                                                                                                  //left or right of the screen
                playernumber);                                                                                                                                 //Controller profile

            playernumber = 2;
            if (Game1.UsingKeyboard == 2) playernumber = 3;
            else if (Game1.UsingKeyboard == 1) playernumber = 1;
            player2 = new Player(
                new Vector2(1920 - player1.Hurtbox.X - player1.Hurtbox.Width, player1.Hurtbox.Y),                                                   //Player Position
                new Rectangle(1920 - player1.Hurtbox.X - player1.Hurtbox.Width, player1.Hurtbox.Y, player1.Hurtbox.Width, player1.Hurtbox.Height),  //Player hurtbox
                Direction.left,                                                                                                                     //Direction being faced
                2,                                                                                                                                  //left or right of the screen
                playernumber);                                                                                                                                 //Controller Profile

            time = 80;
            gameOver = false;
            countDown = true;
            frame = 120;
            AnimationManager.ResetTime();

            if (clap != null)
                clap.Stop();

            if (resetRound)
            {
                player1Score = 0;
                player2Score = 0;
                displayWinner = false;
                postGameMenu.Hide();
                pauseMenu.Hide();
            }

        }


        /// <summary>
        /// Draw both players
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            player1.Draw(spriteBatch);
            player2.Draw(spriteBatch);

            player1.DrawAccessory(spriteBatch);
            player2.DrawAccessory(spriteBatch);

            if (Game1.ShowHitboxes)
            {
                player1.DrawHitboxes(spriteBatch);
                player2.DrawHitboxes(spriteBatch);
            }

            if (hitLag > 0)
            {
                AnimationManager.DrawCollision(spriteBatch, player1.Hitbox, player2.Hurtbox);
                AnimationManager.DrawCollision(spriteBatch, player2.Hitbox, player1.Hurtbox);
            }


            AnimationManager.DrawUI(spriteBatch, player1, player2, frame, time, player1Score, player2Score);

            Color color = Color.Yellow;

            //Draw messages to the players
            if (countDown)
            {
                if (frame <= 120 && frame > 90)
                {
                    if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                    spriteBatch.Draw(three, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 900), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
                else if (frame <= 90 && frame > 60)
                {
                    if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                    spriteBatch.Draw(two, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 900), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
                else if (frame <= 60 && frame > 30)
                {
                    if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                    spriteBatch.Draw(one, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 900), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
                else if (frame <= 30)
                {
                    if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                    spriteBatch.Draw(go, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 760), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 400), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
            }
            else if (gameOver && hitLag != 0)
            {
                if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                spriteBatch.Draw(game, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 610), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 700), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
            }
            else if (displayWinner)
            {
                //Draw
                if (player1Score == 2 && player2Score == 2)
                {
                    color = Color.Purple;
                    if (Game1.BGColor != Color.White) color = Color.Lerp(new Color(1f, 0.6f, 0), new Color(0f, 0.6f, 0.8f), 0.5f);
                    spriteBatch.Draw(draw, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 610), (int)Math.Ceiling(Game1.ScaleY * 420), (int)Math.Ceiling(Game1.ScaleX * 700), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
                else if (player1Score == 2)
                {
                    color = Color.Red;
                    if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
                    if (Game1.UsingKeyboard == 1) color = new Color(new Vector3(0, 1f, 0));
                    spriteBatch.Draw(playerText, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 475), (int)Math.Ceiling(Game1.ScaleY * 330), (int)Math.Ceiling(Game1.ScaleX * 850), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                    spriteBatch.Draw(one, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1325), (int)Math.Ceiling(Game1.ScaleY * 330), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                    spriteBatch.Draw(wins, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 660), (int)Math.Ceiling(Game1.ScaleY * 510), (int)Math.Ceiling(Game1.ScaleX * 600), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
                else if (player2Score == 2)
                {
                    color = Color.Blue;
                    if (Game1.BGColor != Color.White) color = new Color(0, 0.6f, 0.8f);
                    if (Game1.UsingKeyboard == 2) color = new Color(new Vector3(0, 1f, 0));
                    spriteBatch.Draw(playerText, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 475), (int)Math.Ceiling(Game1.ScaleY * 330), (int)Math.Ceiling(Game1.ScaleX * 850), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                    spriteBatch.Draw(two, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1325), (int)Math.Ceiling(Game1.ScaleY * 330), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                    spriteBatch.Draw(wins, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 660), (int)Math.Ceiling(Game1.ScaleY * 510), (int)Math.Ceiling(Game1.ScaleX * 600), (int)Math.Ceiling(Game1.ScaleY * 180)), color);
                }
            }

            //Post-game menu
            if (!postGameMenu.Hidden)
            {
                postGameMenu.Draw(spriteBatch);
            }
            else if (!pauseMenu.Hidden)
            {
                pauseMenu.Draw(spriteBatch);
            }

        }

        /// <summary>
        /// Plays the requested sound
        /// </summary>
        /// <param name="effect"></param>
        public void PlaySound(string effect)
        {
            if (soundEffects.ContainsKey(effect))
            {
                soundEffects[effect].CreateInstance().Play();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        /// <summary>
        /// Returns the requested sound as an instance
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public SoundEffectInstance GetSound(string effect)
        {
            if (soundEffects.ContainsKey(effect))
            {
                return soundEffects[effect].CreateInstance();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

    }
}
