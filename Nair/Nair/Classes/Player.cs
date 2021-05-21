using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
using Nair.Classes.Managers;
using Microsoft.Xna.Framework.Audio;

enum PlayerState
{
    stand,
    dash,
    run,
    squat,
    aerial,
    attack,
    land,
    hit,
    dodgeStartup,
    dodge,
    skid,
    crouch,
    dead,
    wallJumpSquat
}

enum Direction
{
    left,
    right
}

namespace Nair.Classes
{
    class Player
    {
        private Vector2 position;
        private PlayerState state;
        private Vector2 velocity;
        private Rectangle defaultHurtbox;
        private Rectangle hurtbox;
        private Direction direction;
        private List<Rectangle> hitbox;
        private Rectangle hurtboxPosition;
        private SoundEffectInstance skidEffect;

        private int playerIndex;
        public int controllerProfile;
        private int lag;
        private bool fastfall = false;
        private bool fall = false;
        private bool turnaround = false;
        private bool rehit = false;
        private int health;
        private int attackDamage;

        //Stats
        private int maxAirSpeed = 15;
        private int maxGroundSpeed = 19;
        private int dashSpeed = 16;
        private float airAcceleration = 0.68f;
        private float fallspeed = 1.2f;
        private float ffspeed = 40f;
        private int dashLength = 17;
        private int jumpSquat = 4;
        private int attackStartup = 4; //3
        private int attackActive = 4; //7
        private int attackLag = 12; //9
        private int airdodgeStartup = 4;
        private int airdodge = 17;
        private int crouchHeight = 150;
        private int landHeight = 200;
        private int wallJumpSquat = 4;

        List<Actions> buffer;
        List<DustCloud> dust;

        public Rectangle Hurtbox { get => hurtbox; }
        public List<Rectangle> Hitbox { get => hitbox; }
        public PlayerState State { get => state; }
        public Direction Facing { get => direction; }
        //Prevents hitboxes from connecting multiple times
        public bool Rehit { get => rehit; set => rehit = value; }
        public int Health { get => health; set => health = value; }
        public int AttackDamage { get => attackDamage; }

        public Player(Vector2 position, Rectangle hurtbox, Direction direction, int playerIndex, int controllerProfile)
        {
            this.position = position;
            this.defaultHurtbox = hurtbox;
            this.hurtbox = hurtbox;
            this.playerIndex = playerIndex;
            this.controllerProfile = controllerProfile;
            this.direction = direction;

            this.health = 100;
            this.attackDamage = 0;

            hitbox = new List<Rectangle>();
            buffer = new List<Actions>();
            velocity = new Vector2(0, 0);
            dust = new List<DustCloud>();
        }

        public void Update()
        {
            hitbox.Clear();

            for(int i = 0; i < dust.Count; i++)
            {
                if(dust[i].UpAge() > 10)
                {
                    dust.RemoveAt(i);
                    i--;
                }
            }

            //Accept input and change states accordingly

            //Jump button
            if (ControllerManager.GetAction(controllerProfile, Actions.jump, false) && !ControllerManager.GetAction(controllerProfile, Actions.jump, true))
            {
                if(state == PlayerState.crouch || state == PlayerState.stand || state == PlayerState.dash || state == PlayerState.run || state == PlayerState.skid)
                {
                    lag = jumpSquat;
                    hurtbox = defaultHurtbox;
                    if(state == PlayerState.crouch)
                        position = new Vector2(position.X, position.Y - (defaultHurtbox.Height - crouchHeight));

                    if (state == PlayerState.skid)
                        skidEffect.Stop();

                    state = PlayerState.squat;
                    dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                    dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.right));

                }
                else if(state == PlayerState.land)
                {
                    buffer.Add(Actions.jump);
                }
                else if((state == PlayerState.aerial && !fall))
                {
                    if(position.X == 0)
                    {
                        direction = Direction.right;
                        state = PlayerState.wallJumpSquat;
                        dust.Add(new VerticalDustCloud(new Point((int)position.X, (int)position.Y), Direction.right));
                        lag = wallJumpSquat;
                    }
                    else if(position.X == 1770)
                    {
                        direction = Direction.left;
                        state = PlayerState.wallJumpSquat;
                        dust.Add(new VerticalDustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                        lag = wallJumpSquat;
                    }
                }
                else if (state == PlayerState.attack)
                {
                    buffer.Add(Actions.jump);
                }
            }
            //Attack button
            if(ControllerManager.GetAction(controllerProfile, Actions.attack, false) && !ControllerManager.GetAction(controllerProfile, Actions.attack, true))
            {
                if(state == PlayerState.aerial && !fall)
                {
                    lag = 0;
                    state = PlayerState.attack;
                    PlayerManager.Instance.PlaySound("swordSwing");
                }
                else if(state == PlayerState.attack || state == PlayerState.squat || state == PlayerState.wallJumpSquat)
                {
                    buffer.Add(Actions.attack);
                }
            }
            //Dodge button
            else if(ControllerManager.GetAction(controllerProfile, Actions.dodge, false))
            {
                if(state == PlayerState.aerial && !fall)
                {
                    state = PlayerState.dodgeStartup;
                    velocity = new Vector2();
                    lag = airdodgeStartup;
                    fall = true;
                }
                else if(state == PlayerState.attack || state == PlayerState.squat || state == PlayerState.wallJumpSquat)
                {
                    buffer.Add(Actions.dodge);
                }
            }

            if (state == PlayerState.aerial || state == PlayerState.attack)
            {
                //Adjust air velocity
                if (!ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, false))
                {
                    //Current air speed
                    float speed = velocity.X;
                    //Add  speed up to max value
                    speed += ControllerManager.GetDrift(controllerProfile) * airAcceleration;
                    if (speed >= maxAirSpeed)
                        speed = maxAirSpeed;
                    if (speed <= -1 * maxAirSpeed)
                        speed = -1 * maxAirSpeed;

                    velocity = new Vector2(speed, velocity.Y);
                }

                //Check for fastfall
                if (!fall && !fastfall && velocity.Y >= 0 && ControllerManager.GetAction(controllerProfile, Actions.hardDown, false) && !ControllerManager.GetAction(controllerProfile, Actions.hardDown, true))
                {
                    fastfall = true;
                    velocity = new Vector2(velocity.X, ffspeed);
                }

                //Change position, adjust velocity
                position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
                velocity = new Vector2(velocity.X, velocity.Y + fallspeed);
            }




            //Control the player based on current status
            switch (state)
            {
                case PlayerState.stand:
                    //Turnaround right
                    if(ControllerManager.GetAction(controllerProfile, Actions.softRight, false) && !ControllerManager.GetAction(controllerProfile, Actions.softRight, true))
                    {
                        direction = Direction.right;
                    }
                    //Turnaround left
                    else if (ControllerManager.GetAction(controllerProfile, Actions.softLeft, false) && !ControllerManager.GetAction(controllerProfile, Actions.softLeft, true))
                    {
                        direction = Direction.left;
                    }
                    //Dash
                    if(ControllerManager.GetAction(controllerProfile, Actions.hardLeft, false))
                    {
                        state = PlayerState.dash;
                        direction = Direction.left;
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                        PlayerManager.Instance.PlaySound("dust1");
                        lag = dashLength;
                    }
                    else if (ControllerManager.GetAction(controllerProfile, Actions.hardRight, false))
                    {
                        state = PlayerState.dash;
                        direction = Direction.right;
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                        PlayerManager.Instance.PlaySound("dust1");
                        lag = dashLength;
                    }
                    //Crouch
                    else if (ControllerManager.GetAction(controllerProfile, Actions.hardDown, false))
                    {
                        state = PlayerState.crouch;
                    }
                    break;
                case PlayerState.crouch:
                    hurtbox = new Rectangle(hurtbox.X, hurtbox.Y, hurtbox.Width, crouchHeight);
                    position = new Vector2(position.X, 900 - crouchHeight);

                    if (!ControllerManager.GetAction(controllerProfile, Actions.hardDown, false))
                    {
                        hurtbox = defaultHurtbox;
                        position = new Vector2(position.X, position.Y - (defaultHurtbox.Height - crouchHeight));
                        state = PlayerState.stand;
                    }
                    break;
                case PlayerState.dash:
                    lag--;
                    if(direction == Direction.right)
                        velocity = new Vector2(dashSpeed, 0);
                    else
                        velocity = new Vector2(-1 * dashSpeed, 0);
                    position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);

                    //Hop animation
                    if(lag > dashLength/2)
                    {
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, hurtbox.Height - 2);
                    }
                    else
                    {
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, hurtbox.Height + 2);
                    }

                    //DashDance
                    if(direction == Direction.right && ControllerManager.GetAction(controllerProfile, Actions.hardLeft, false) && !ControllerManager.GetAction(controllerProfile, Actions.hardLeft, true))
                    {
                        lag = dashLength;
                        direction = Direction.left;
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                        PlayerManager.Instance.PlaySound("dust1");
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, 250);
                    }
                    else if (direction == Direction.left && ControllerManager.GetAction(controllerProfile, Actions.hardRight, false) && !ControllerManager.GetAction(controllerProfile, Actions.hardRight, true))
                    {
                        lag = dashLength;
                        direction = Direction.right;
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                        PlayerManager.Instance.PlaySound("dust1");
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, 250);
                    }
                        
                    //Dash is over
                    if(lag == 0)
                    {
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, 250);
                        velocity = new Vector2();
                        if (ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, false))
                            state = PlayerState.stand;
                        else
                            state = PlayerState.run;
                    }
                    break;
                case PlayerState.run:
                    velocity = new Vector2(maxGroundSpeed * ControllerManager.GetDrift(controllerProfile), 0);
                    //Stop running
                    if (ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, false) && ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, true))
                    {
                        turnaround = false;
                        state = PlayerState.skid;
                        if(direction == Direction.right) dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                        else if (direction == Direction.left) dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.right));
                        lag = 9;
                        skidEffect = PlayerManager.Instance.GetSound("skid1");
                        skidEffect.Play();
                    }
                    if((direction == Direction.left && ControllerManager.GetDrift(controllerProfile) > 0) || (direction == Direction.right && ControllerManager.GetDrift(controllerProfile) < 0))
                    {
                        turnaround = true;
                        //turn around
                        if (direction == Direction.left)
                        {
                            direction = Direction.right;
                            dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.right));
                        }
                        else if (direction == Direction.right)
                        {
                            direction = Direction.left;
                            dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                        }
                        state = PlayerState.skid;
                        lag = 20;
                        skidEffect = PlayerManager.Instance.GetSound("skid1");
                        skidEffect.Play();
                    }

                    position = new Vector2(position.X + velocity.X, position.Y);
                    break;


                case PlayerState.dodge:
                    lag--;
                    position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
                    if(lag == 0)
                    {
                        state = PlayerState.aerial;
                        velocity = new Vector2();
                    }
                    break;


                case PlayerState.squat:
                    lag--;

                    //Transition to aerial if we get through jumpsquat
                    if (lag == 0)
                    {
                        state = PlayerState.aerial;

                        fastfall = false;
                        fall = false;
                        //shorthop
                        if (!ControllerManager.GetAction(controllerProfile, Actions.jump, false))
                        {
                            PlayerManager.Instance.PlaySound("shorthop");
                            velocity = new Vector2((velocity.X / maxGroundSpeed) * maxAirSpeed, -22);
                        }
                        //fullhop
                        else
                        {
                            PlayerManager.Instance.PlaySound("fullhop");
                            velocity = new Vector2((velocity.X / maxGroundSpeed) * maxAirSpeed, -30);
                        }

                        //Buffer an attack or dodge
                        if (buffer.Count > 0 && buffer.Contains(Actions.attack))
                        {
                            buffer.Clear();
                            lag = 0;
                            state = PlayerState.attack;
                            PlayerManager.Instance.PlaySound("swordSwing");
                        }
                        else if (buffer.Count > 0 && buffer.Contains(Actions.dodge))
                        {
                            buffer.Clear();
                            state = PlayerState.dodgeStartup;
                            velocity = new Vector2();
                            lag = airdodgeStartup;
                            fall = true;
                        }
                    }
                    break;
                case PlayerState.wallJumpSquat:
                    lag--;

                    //When jumpsquat is finished, bounce off
                    if (lag == 0)
                    {
                        state = PlayerState.aerial;

                        fastfall = false;
                        fall = false;


                        if (direction == Direction.left)
                        {
                            velocity = new Vector2(-1 * maxAirSpeed, -22);
                        }
                        else
                            velocity = new Vector2(maxAirSpeed, -22);

                        PlayerManager.Instance.PlaySound("shorthop");
                        PlayerManager.Instance.PlaySound("dust1");

                        //Buffer an attack or dodge
                        if (buffer.Count > 0 && buffer.Contains(Actions.attack))
                        {
                            buffer.Clear();
                            lag = 0;
                            state = PlayerState.attack;
                            PlayerManager.Instance.PlaySound("swordSwing");
                        }
                        else if (buffer.Count > 0 && buffer.Contains(Actions.dodge))
                        {
                            buffer.Clear();
                            state = PlayerState.dodgeStartup;
                            velocity = new Vector2();
                            lag = airdodgeStartup;
                            fall = true;
                        }
                    }
                    break;
                case PlayerState.land:
                    lag--;
                    hurtbox = new Rectangle(hurtbox.X, hurtbox.Y, hurtbox.Width, landHeight);
                    position = new Vector2(position.X + velocity.X, 900 - landHeight);

                    //Transition to stand if we get through jumpsquat
                    if (lag <= 0)
                    {
                        hurtbox = defaultHurtbox;
                        position = new Vector2(position.X, position.Y - (defaultHurtbox.Height - landHeight));

                        //Buffer a jump
                        if (buffer.Count > 0)
                        {
                            buffer.Clear();
                            lag = jumpSquat;
                            state = PlayerState.squat;
                            dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                            dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.right));
                        }
                        else
                        {
                            velocity = new Vector2();

                            //You must hold STRAIGHT down for a buffered crouch, otherwise it stands
                            if (ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, false) && !ControllerManager.GetAction(controllerProfile, Actions.hardDown, false))
                            {
                                state = PlayerState.stand;
                            }
                            //Check for crouch
                            else if (ControllerManager.GetAction(controllerProfile, Actions.hardDown, false))
                            {
                                state = PlayerState.crouch;
                                hurtbox = new Rectangle(hurtbox.X, hurtbox.Y, hurtbox.Width, crouchHeight);
                                position = new Vector2(position.X, 900 - crouchHeight);
                            }
                            else
                            {
                                if (ControllerManager.GetAction(controllerProfile, Actions.softRight, false) || ControllerManager.GetAction(controllerProfile, Actions.hardRight, false))
                                    direction = Direction.right;
                                else
                                    direction = Direction.left;
                                state = PlayerState.dash;
                                dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                                PlayerManager.Instance.PlaySound("dust1");
                                lag = dashLength;
                            }
                        }
                    }
                    break;
                case PlayerState.dodgeStartup:
                    lag--;
                    //Transition to stand if we get through jumpsquat
                    if (lag == 0)
                    {
                        state = PlayerState.dodge;
                        fall = true;
                        lag = airdodge;
                        velocity = new Vector2(ControllerManager.GetDrift(controllerProfile) * maxAirSpeed * 1.2f, ControllerManager.GetHeight(controllerProfile) * maxAirSpeed * -1);
                    }
                    break;
                case PlayerState.hit:
                    lag--;
                    position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
                    hurtbox = defaultHurtbox;
                    if (lag == 0)
                    {
                        if (health > 0)
                            state = PlayerState.aerial;
                        else
                            state = PlayerState.dead;
                    }
                    break;
                case PlayerState.skid:
                    lag--;
                    if((direction == Direction.right && !turnaround) || (direction == Direction.left && turnaround))
                        velocity = new Vector2(maxGroundSpeed / 2, 0);
                    else if((direction == Direction.left && !turnaround) || (direction == Direction.right && turnaround))
                        velocity = new Vector2(-1 * maxGroundSpeed / 2, 0);

                    position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);

                    if(turnaround && lag % 8 == 0)
                    {
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), direction));
                    }

                    if (lag == 0)
                    {
                        velocity = new Vector2();

                        skidEffect.Stop();

                        if (ControllerManager.GetAction(controllerProfile, Actions.horizontalNeutral, false))
                            state = PlayerState.stand;
                        else
                            state = PlayerState.run;

                        turnaround = false;
                    }
                    break;

                case PlayerState.attack:
                    lag++;

                    //Activate the hitbox
                    if (lag < 4)
                    {
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width + 10, hurtbox.Height - 25);
                        if (direction == Direction.left)
                            position = new Vector2(position.X - 10, position.Y);
                    }
                    else if (lag > attackStartup && lag < attackStartup + attackActive)
                    {
                        if(direction == Direction.left)
                            hitbox.Add(new Rectangle((int)position.X - 35, (int)position.Y + 50, hurtbox.Width + 100, hurtbox.Width - 20));
                        else
                            hitbox.Add(new Rectangle((int)position.X - 65, (int)position.Y + 50, hurtbox.Width + 100, hurtbox.Width - 20));
                        hitbox.Add(new Rectangle((int)position.X, (int)position.Y + hurtbox.Height + 35, hurtbox.Width, 40));
                        attackDamage = 10;
                    }
                    else if (lag > attackStartup + attackActive && lag < attackStartup + attackActive + 4)
                    {
                        rehit = false;
                        hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width - 10, hurtbox.Height + 25);
                        if (direction == Direction.left)
                            position = new Vector2(position.X + 10, position.Y);
                    }
                    else if (lag > attackStartup + attackActive + attackLag)
                    {
                        hurtbox = defaultHurtbox;
                        lag = 0;

                        if(buffer.Count == 0)
                            state = PlayerState.aerial;

                        else if (buffer[buffer.Count - 1] == Actions.jump)
                        {
                            if (position.X < 0)
                            {
                                direction = Direction.right;
                                state = PlayerState.wallJumpSquat;
                                lag = wallJumpSquat;
                            }
                            else if (position.X > 1770)
                            {
                                direction = Direction.left;
                                state = PlayerState.wallJumpSquat;
                                lag = wallJumpSquat;
                            }
                            else
                                state = PlayerState.aerial;
                        }

                        else if (buffer[buffer.Count - 1] == Actions.dodge)
                        {
                            state = PlayerState.dodgeStartup;
                            lag = airdodgeStartup;
                        }
                        else if(buffer[buffer.Count - 1] == Actions.attack)
                        {
                            state = PlayerState.attack;
                            PlayerManager.Instance.PlaySound("swordSwing");
                        }

                        buffer.Clear();
                    }


                    break;


                case PlayerState.dead:
                    hurtbox = new Rectangle((int)position.X, (int)position.Y, 250, 150);
                    velocity = new Vector2(velocity.X, velocity.Y + fallspeed);
                    position = new Vector2(position.X + velocity.X, position.Y + velocity.Y);
                    break;
            }

            //Check ground for collision
            if (state == PlayerState.aerial || state == PlayerState.attack || state == PlayerState.dodge || state == PlayerState.dead)
            {
                int bottomPixel = (int)(position.Y + hurtbox.Height); //Get pixel position of bottom hitbox pixel
                if (bottomPixel > PlayerManager.Instance.FloorLocation)
                {
                    rehit = false;
                    if(state != PlayerState.dead)
                        hurtbox = defaultHurtbox;
                    position = new Vector2(position.X, 900 - hurtbox.Height);

                    if (state != PlayerState.dodge)
                    {
                        lag = 3;
                        if (fastfall)
                            lag += 2;
                        if (fall)
                            lag += 11;
                        if (state == PlayerState.attack)
                            lag += 5;
                    }

                    if (state != PlayerState.dead)
                    {
                        state = PlayerState.land;
                        PlayerManager.Instance.PlaySound("land1");
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.left));
                        dust.Add(new DustCloud(new Point((int)position.X, (int)position.Y), Direction.right));
                    }
                    else
                        velocity = new Vector2();
                }
            }

            //Stop player going off screen
            if (position.X < 0)
                position = new Vector2(0, position.Y);
            else if (position.X > (1920 - hurtbox.Width))
                position = new Vector2((1920 - hurtbox.Width), position.Y);

            hurtbox = new Rectangle((int)position.X, (int)position.Y, hurtbox.Width, hurtbox.Height);
        }

        /// <summary>
        /// Calling this function causes the player to get hit
        /// </summary>
        public void Hit(Vector2 velocity, int damage)
        {
            if(state == PlayerState.crouch) position = new Vector2(position.X, position.Y - (defaultHurtbox.Height - crouchHeight));
            if (state == PlayerState.land) position = new Vector2(position.X, position.Y - (defaultHurtbox.Height - landHeight));

            state = PlayerState.hit;
            //Attacker facing left
            if (velocity.X < 0)
            {
                this.direction = Direction.right;
            }
            //Attacker facing right
            else if (velocity.X > 0)
            {
                this.direction = Direction.left;
            }

            this.velocity = velocity;
            rehit = false;
            fall = false;
            this.health -= damage;
            lag = (int)(damage * 2.3f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            hurtboxPosition = new Rectangle((int)Math.Ceiling(Game1.ScaleX * hurtbox.X), (int)Math.Ceiling(Game1.ScaleY * hurtbox.Y), (int)Math.Ceiling(Game1.ScaleX * hurtbox.Width), (int)Math.Ceiling(Game1.ScaleY * hurtbox.Height));

            //Draw sprite
            AnimationManager.DrawPlayer(spriteBatch, this.playerIndex, this.direction, this.state, hurtboxPosition, this.lag, controllerProfile);

            
        }

        public void DrawAccessory(SpriteBatch spriteBatch)
        {
            AnimationManager.DrawWeapons(spriteBatch, this.playerIndex, this.direction, this.state, hurtboxPosition, this.lag, this.attackStartup, this.attackActive, this.attackLag, controllerProfile);

            foreach (DustCloud puff in dust) AnimationManager.DrawDust(puff, spriteBatch);
        }

        public void DrawHitboxes(SpriteBatch spriteBatch)
        {
            //Draw hitboxes
            if (direction == Direction.left)
                {
                    if (playerIndex == 1)
                        spriteBatch.Draw(Game1.BlankPixel, new Rectangle(hurtboxPosition.X, hurtboxPosition.Y, hurtboxPosition.Width / 3, hurtboxPosition.Width / 3), Color.Red);
                    else
                        spriteBatch.Draw(Game1.BlankPixel, new Rectangle(hurtboxPosition.X, hurtboxPosition.Y, hurtboxPosition.Width / 3, hurtboxPosition.Width / 3), Color.Indigo);
                }
                else if (direction == Direction.right)
                    if (playerIndex == 1)
                        spriteBatch.Draw(Game1.BlankPixel, new Rectangle(hurtboxPosition.X + hurtboxPosition.Width - hurtboxPosition.Width / 3, hurtboxPosition.Y, hurtboxPosition.Width / 3, hurtboxPosition.Width / 3), Color.Red);
                    else
                        spriteBatch.Draw(Game1.BlankPixel, new Rectangle(hurtboxPosition.X + hurtboxPosition.Width - hurtboxPosition.Width / 3, hurtboxPosition.Y, hurtboxPosition.Width / 3, hurtboxPosition.Width / 3), Color.Indigo);

                Color hbColor;
                switch (state)
                {
                    case PlayerState.squat:
                        hbColor = Color.Green;
                        break;
                    case PlayerState.land:
                        hbColor = Color.Green;
                        break;
                    case PlayerState.wallJumpSquat:
                        hbColor = Color.Green;
                        break;
                    case PlayerState.dodge:
                        hbColor = Color.Purple;
                        break;
                    case PlayerState.skid:
                        hbColor = Color.Olive;
                        break;
                    case PlayerState.dash:
                        hbColor = Color.DarkBlue;
                        break;
                    case PlayerState.hit:
                        hbColor = Color.Yellow;
                        break;
                    default:
                        hbColor = Color.Blue;
                        break;
                }
                //Draw hurtbox
                spriteBatch.Draw(Game1.TransBlankPixel, hurtboxPosition, hbColor);
                //Draw hitbox
                if (state == PlayerState.attack)
                    foreach (Rectangle hitbox in hitbox)
                    {
                        Rectangle hitboxPosition = new Rectangle((int)Math.Ceiling(Game1.ScaleX * hitbox.X), (int)Math.Ceiling(Game1.ScaleY * hitbox.Y), (int)Math.Ceiling(Game1.ScaleX * hitbox.Width), (int)Math.Ceiling(Game1.ScaleY * hitbox.Height));
                        spriteBatch.Draw(Game1.TransBlankPixel, hitboxPosition, Color.Red);
                    }
        }

    }
}
