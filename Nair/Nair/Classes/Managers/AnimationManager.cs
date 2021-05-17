using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Input;

namespace Nair.Classes.Managers
{
    class AnimationManager
    {

        private static AnimationManager instance;
        private GraphicsDeviceManager graphics;
        private ContentManager content;

        private List<Texture2D> playerSprites;
        private List<Texture2D> swordSwing;

        private Texture2D hurtFace;
        private Texture2D explosion;
        private Texture2D sword1;

        private List<Texture2D> healthBar;
        private List<Texture2D> healthBarFill;
        private List<Texture2D> penant;
        private Texture2D penantFill;
        private Texture2D timerBG;

        private Texture2D timerNumbers;

        private List<Texture2D> dustImage;

        private int frame;


        public static AnimationManager Instance { get => instance; }



        private AnimationManager(GraphicsDeviceManager graphics, ContentManager content)
        {
            frame = 0;

            this.graphics = graphics;
            this.content = content;

            playerSprites = new List<Texture2D>();
            swordSwing = new List<Texture2D>();
            healthBar = new List<Texture2D>();
            healthBarFill = new List<Texture2D>();
            penant = new List<Texture2D>();
            dustImage = new List<Texture2D>();

            playerSprites.Add(content.Load<Texture2D>("Player/PlayerFrame1"));
            playerSprites.Add(content.Load<Texture2D>("Player/PlayerFrame2"));
            playerSprites.Add(content.Load<Texture2D>("Player/PlayerFrame3"));

            hurtFace = content.Load<Texture2D>("Player/Hurt/Hurt1");
            explosion = content.Load<Texture2D>("Player/Explosion");
            sword1 = content.Load<Texture2D>("Player/Sword/Sword");

            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing1"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing2"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing3"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing4"));

            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing5"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing6"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing7"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing8"));
            swordSwing.Add(content.Load<Texture2D>("Player/Sword/SwordSwing9"));

            healthBar.Add(content.Load<Texture2D>("UI/HealthBar"));
            healthBar.Add(content.Load<Texture2D>("UI/HealthBar2"));
            healthBarFill.Add(content.Load<Texture2D>("UI/HealthBarFill"));
            healthBarFill.Add(content.Load<Texture2D>("UI/HealthBarFill2"));
            penant.Add(content.Load<Texture2D>("UI/Penant"));
            penant.Add(content.Load<Texture2D>("UI/Penant2"));
            penantFill = content.Load<Texture2D>("UI/PenantFill");
            timerBG = content.Load<Texture2D>("UI/TimerBG");
            timerNumbers = content.Load<Texture2D>("UI/TImerNumbers");

            dustImage.Add(content.Load<Texture2D>("Player/Dust/Dust1"));
            dustImage.Add(content.Load<Texture2D>("Player/Dust/Dust2"));
            dustImage.Add(content.Load<Texture2D>("Player/Dust/Dust3"));
            dustImage.Add(content.Load<Texture2D>("Player/Dust/Dust4"));
            dustImage.Add(content.Load<Texture2D>("Player/Dust/Dust5"));
        }

        public static void Update()
        {
            instance.frame++;
        }

        public static void ResetTime()
        {
            instance.frame = 0;
        }


        public static void Initialize(GraphicsDeviceManager graphics, ContentManager content)
        {
            if (instance == null)
            {
                instance = new AnimationManager(graphics, content);
            }
            else
                throw new InvalidOperationException();
        }


        //Draw the player
        public static void DrawPlayer(SpriteBatch spritebatch, int playerNumber, Direction direction, PlayerState state, Rectangle hurtbox, int actionFrame)
        {
            //Establish left or right
            SpriteEffects flip = SpriteEffects.None;
            if (direction == Direction.left) flip = SpriteEffects.FlipHorizontally;
            //Establish Player Color
            Color color = Color.Red;
            if (playerNumber == 1 && Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
            else if (playerNumber == 2 && Game1.BGColor != Color.White) color = new Color(0f, 0.6f, 0.8f);
            else if (playerNumber == 2) color = Color.Blue;
            if (Game1.GameState == GameState.tutorial) color = Color.Purple;
            if (Game1.UsingKeyboard == 1 && playerNumber == 1)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            else if (Game1.UsingKeyboard == 2 && playerNumber == 2)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            //Dodge invincibility 
            if (state == PlayerState.dodge || state == PlayerState.dead) color = Color.DarkGray;

            //Which frame of the player sprite to use
            int anim = ((instance.frame / 10) + playerNumber) % instance.playerSprites.Count;


            spritebatch.Draw(instance.playerSprites[anim], hurtbox, null, color, 0f, new Vector2(), flip, 0f);

            //Draw the "hurt" overlay
            if(state == PlayerState.hit || state == PlayerState.dodgeStartup || state == PlayerState.dodge || state == PlayerState.dead) spritebatch.Draw(instance.hurtFace, hurtbox, null, color, 0f, new Vector2(), flip, 0f);



        }

        public static void DrawWeapons(SpriteBatch spritebatch, int playerNumber, Direction direction, PlayerState state, Rectangle hurtbox, int actionFrame, int attackStartup, int attackActive, int attackLag)
        {
            //Establish left or right
            SpriteEffects flip = SpriteEffects.None;
            if (direction == Direction.left) flip = SpriteEffects.FlipHorizontally;
            //Establish Player Color
            Color color = Color.Blue;
            if (playerNumber == 1 && Game1.BGColor != Color.White) color = new Color(0, 0.6f, 0.8f);
            else if (playerNumber == 2 && Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
            else if (playerNumber == 2) color = Color.Red;
            if (Game1.GameState == GameState.tutorial) color = Color.Gray;
            if (Game1.UsingKeyboard == 1 && playerNumber == 1)
            {
                if (Game1.BGColor == Color.White)
                    color = Color.Yellow;
                else
                    color = Color.Green;
            }
            else if (Game1.UsingKeyboard == 2 && playerNumber == 2) color = Color.Yellow;

            //Draw Sword
            if (state != PlayerState.attack && state != PlayerState.dead)
                spritebatch.Draw(instance.sword1, hurtbox, null, color, 0f, new Vector2(), flip, 0f);

            //Handle Attacks
            if (state == PlayerState.attack)
            {
                Rectangle attackRectangle = new Rectangle(hurtbox.X - (int)Math.Ceiling(Game1.ScaleX * 65f), hurtbox.Y, (int)Math.Ceiling(Game1.ScaleX * 280f), (int)Math.Ceiling(Game1.ScaleY * 250f));
                if (direction == Direction.left)
                    attackRectangle = new Rectangle(hurtbox.X - (int)Math.Ceiling(Game1.ScaleX * 30f), hurtbox.Y, attackRectangle.Width, attackRectangle.Height);

                Texture2D texture = null;

                if (actionFrame <= attackStartup)
                {
                    //texture = instance.swordSwing[actionFrame % 4];
                    float frame = (((float)actionFrame / (float)attackStartup) * 4);
                    if (frame == 4f) frame -= 1;
                    texture = instance.swordSwing[(int)frame];
                }
                else if (actionFrame > attackStartup && actionFrame < (attackStartup + attackActive))
                    texture = instance.swordSwing[4];
                else if (actionFrame > attackStartup + attackActive)
                {
                    if (actionFrame < attackStartup + attackActive + (attackLag / 4))
                        texture = instance.swordSwing[5];
                    else if (actionFrame < attackStartup + attackActive + (attackLag / 4) * 2)
                        texture = instance.swordSwing[6];
                    else if (actionFrame < attackStartup + attackActive + (attackLag / 4) * 3)
                        texture = instance.swordSwing[7];
                    else
                        texture = instance.swordSwing[8];
                }

                if (texture != null)
                    spritebatch.Draw(texture, attackRectangle, null, color, 0f, new Vector2(), flip, 0.5f);
            }
        }

        //Draw an Explosion over the collision between hitboxes
        public static void DrawCollision(SpriteBatch spritebatch, List<Rectangle> hitboxes, Rectangle hurtbox)
        {

            foreach(Rectangle hitbox in hitboxes)
            {
                if(hitbox.Intersects(hurtbox))
                {
                    spritebatch.Draw(instance.explosion, 
                        Rectangle.Intersect(
                            new Rectangle((int)Math.Ceiling(Game1.ScaleX * hitbox.X), (int)Math.Ceiling(Game1.ScaleY * hitbox.Y), (int)Math.Ceiling(Game1.ScaleX * hitbox.Width), (int)Math.Ceiling(Game1.ScaleY * hitbox.Height)), 
                            new Rectangle((int)Math.Ceiling(Game1.ScaleX * hurtbox.X), (int)Math.Ceiling(Game1.ScaleY * hurtbox.Y), (int)Math.Ceiling(Game1.ScaleX * hurtbox.Width), (int)Math.Ceiling(Game1.ScaleY * hurtbox.Height))), 
                        Color.White);
                    return;
                }
            }

        }


        public static void DrawUI(SpriteBatch spriteBatch, Player player1, Player player2, int frame, int time, int player1Score, int player2Score)
        {
            //Draw Health Bar Fill
            int anim = (instance.frame / 15) % instance.healthBar.Count;

            //Player1
            Color color = Color.Red;
            if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
            if (Game1.UsingKeyboard == 1)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            float p1HealthBarPercent = player1.Health * 9;
            spriteBatch.Draw(instance.healthBarFill[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 60), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * p1HealthBarPercent), (int)Math.Ceiling(Game1.ScaleY * 100)),
                new Rectangle(0, 0, (int)p1HealthBarPercent, 100), color);
            //Player2
            color = Color.Blue;
            if (Game1.BGColor != Color.White) color = new Color(0, 0.6f, 0.8f);
            if (Game1.UsingKeyboard == 2)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            float p2HealthBarPercent = player2.Health * 9;
            spriteBatch.Draw(instance.healthBarFill[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * (1860 - p2HealthBarPercent)), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * p2HealthBarPercent), (int)Math.Ceiling(Game1.ScaleY * 100)),
                new Rectangle(0, 0, (int)p2HealthBarPercent, 100), color, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 1f);

            //Draw the health Bar borders
            //spriteBatch.Draw(instance.healthBar[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 55), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * 1810), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White);
            spriteBatch.Draw(instance.healthBar[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 60), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * p1HealthBarPercent), (int)Math.Ceiling(Game1.ScaleY * 100)),
                new Rectangle(0, 0, (int)p1HealthBarPercent, 100), Color.White);
            //Player2
            spriteBatch.Draw(instance.healthBar[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * (1860 - p2HealthBarPercent)), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * p2HealthBarPercent), (int)Math.Ceiling(Game1.ScaleY * 100)),
                new Rectangle(0, 0, (int)p2HealthBarPercent, 100), Color.White, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 1f);

            //Draw the Score
            color = Color.Red;
            if (Game1.BGColor != Color.White) color = new Color(1f, 0.6f, 0);
            if (Game1.UsingKeyboard == 1)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            if (player1Score > 0)
                spriteBatch.Draw(instance.penantFill, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 65), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), color);
            if (player1Score > 1)
                spriteBatch.Draw(instance.penantFill, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), color);
            spriteBatch.Draw(instance.penant[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 65), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), Color.Black);
            spriteBatch.Draw(instance.penant[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), Color.Black);

            color = Color.Blue;
            if (Game1.BGColor != Color.White) color = new Color(0, 0.6f, 0.8f);
            if (Game1.UsingKeyboard == 2)
            {
                if (Game1.BGColor == Color.White)
                    color = new Color(new Vector3(0, 0.9f, 0));
                else
                    color = Color.Maroon;
            }
            if (player2Score > 0)
                spriteBatch.Draw(instance.penantFill, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1810), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), color);
            if (player2Score > 1)
                spriteBatch.Draw(instance.penantFill, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1755), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), color);
            spriteBatch.Draw(instance.penant[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1810), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), Color.Black);
            spriteBatch.Draw(instance.penant[anim], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1755), (int)Math.Ceiling(Game1.ScaleY * 140), (int)Math.Ceiling(Game1.ScaleX * 45), (int)Math.Ceiling(Game1.ScaleY * 60)), Color.Black);

            //Draw the Timer
            spriteBatch.Draw(instance.timerBG, new Rectangle((int)Math.Ceiling(Game1.ScaleX * ((900 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * 120), (int)Math.Ceiling(Game1.ScaleY * 100)), Color.White);
            //Draw the numbers
            int firstdigit = (time / 10);
            int seconddigit = time - (firstdigit * 10);

            color = Color.Black;
            if (time <= 10) color = Color.Yellow;
            //First Digit
            spriteBatch.Draw(instance.timerNumbers, new Rectangle((int)Math.Ceiling(Game1.ScaleX * ((900 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * 60), (int)Math.Ceiling(Game1.ScaleY * 100)), new Rectangle(0 + (60 * firstdigit), 0, 60, 100), color);
            spriteBatch.Draw(instance.timerNumbers, new Rectangle((int)Math.Ceiling(Game1.ScaleX * ((960 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 30), (int)Math.Ceiling(Game1.ScaleX * 60), (int)Math.Ceiling(Game1.ScaleY * 100)), new Rectangle(0 + (60 * seconddigit), 0, 60, 100), color);

            /*if (time > 9)
            {
                spriteBatch.DrawString(Game1.CourierNew, (time.ToString())[0].ToString(), new Vector2((int)Math.Ceiling(Game1.ScaleX * ((910 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 40)), Color.Black, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080), SpriteEffects.None, 1f);
                spriteBatch.DrawString(Game1.CourierNew, (time.ToString())[1].ToString(), new Vector2((int)Math.Ceiling(Game1.ScaleX * ((960 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 40)), Color.Black, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080), SpriteEffects.None, 1f);

            }
            else
            {
                spriteBatch.DrawString(Game1.CourierNew, "0", new Vector2((int)Math.Ceiling(Game1.ScaleX * ((910 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 40)), Color.Yellow, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080), SpriteEffects.None, 1f);
                spriteBatch.DrawString(Game1.CourierNew, (time.ToString())[0].ToString(), new Vector2((int)Math.Ceiling(Game1.ScaleX * ((960 + p1HealthBarPercent) - 900)), (int)Math.Ceiling(Game1.ScaleY * 40)), Color.Yellow, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080), SpriteEffects.None, 1f);
            }*/


        }

        public static void DrawDust(DustCloud puff, SpriteBatch spritebatch)
        {
            Rectangle dustLocale = new Rectangle((int)Math.Ceiling(Game1.ScaleX * (puff.Location.X - 110)), (int)Math.Ceiling(Game1.ScaleY * (PlayerManager.Instance.FloorLocation - 70)), (int)Math.Ceiling(Game1.ScaleX * 110), (int)Math.Ceiling(Game1.ScaleY * 70));
            SpriteEffects flip = SpriteEffects.None;
            if (puff.Facing == Direction.left)
            {
                flip = SpriteEffects.FlipHorizontally;
                dustLocale = new Rectangle((int)Math.Ceiling(Game1.ScaleX * (puff.Location.X + 150)), dustLocale.Y, dustLocale.Width, dustLocale.Height);
            }

            if(puff is VerticalDustCloud && puff.Facing == Direction.right)
            {
                dustLocale = new Rectangle((int)Math.Ceiling(Game1.ScaleX * (puff.Location.X + 70)), (int)Math.Ceiling(Game1.ScaleY * (puff.Location.Y + 250)), dustLocale.Width, dustLocale.Height);
                flip = SpriteEffects.FlipHorizontally;
            }
            else if(puff is VerticalDustCloud && puff.Facing == Direction.left)
            {
                dustLocale = new Rectangle(dustLocale.X, (int)Math.Ceiling(Game1.ScaleY * (puff.Location.Y + 250)), dustLocale.Width, dustLocale.Height);
                flip = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
            }

            int anim = puff.Age / 2;
            if (anim >= 5) anim -= 1;

            if(puff is VerticalDustCloud)
                spritebatch.Draw(instance.dustImage[anim], dustLocale, null, Color.White * 0.75f, (float)Math.PI/2f, new Vector2(), flip, 0f);
            else
                spritebatch.Draw(instance.dustImage[anim], dustLocale, null, Color.White * 0.75f, 0f, new Vector2(), flip, 0f);
        }

    }
}
