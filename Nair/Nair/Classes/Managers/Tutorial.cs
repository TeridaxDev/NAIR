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
    class Tutorial
    {

        private Player player;
        private Menu pause;

        private Texture2D currentState;
        private Texture2D states;
        private List<Texture2D> tutorial;

        private int cursor;


        public Tutorial(Texture2D currentState, Texture2D states, List<Texture2D> tutorial)
        {
            this.currentState = currentState;
            this.states = states;
            this.tutorial = tutorial;

            pause = new Menu(new Rectangle(700, 300, 500, 300));
            pause.AddButton(new Button(new Rectangle(845, 335, 230, 80), PlayerManager.ButtonResume1, PlayerManager.ButtonResume2));
            pause.AddButton(new Button(new Rectangle(845, 465, 230, 80), PlayerManager.ButtonMenu1, PlayerManager.ButtonMenu2));

            player = new Player(
                new Vector2(100, PlayerManager.Instance.FloorLocation - 250),                  //Player Position
                new Rectangle(100, PlayerManager.Instance.FloorLocation - 250, 150, 250),      //Player hurtbox
                Direction.right,                                        //Direction being faced
                1,
                1);
        }


        public void Update()
        {
            if (pause.Hidden)
            {
                if (Game1.UsingKeyboard == 0)
                {
                    if (ControllerManager.GetAction(1, Actions.leftBumper, false) && !ControllerManager.GetAction(1, Actions.leftBumper, true))
                        cursor--;
                    if (ControllerManager.GetAction(1, Actions.rightBumper, false) && !ControllerManager.GetAction(1, Actions.rightBumper, true))
                        cursor++;
                }
                else if(Game1.UsingKeyboard == 1)
                {
                    if (ControllerManager.GetAction(3, Actions.leftBumper, false) && !ControllerManager.GetAction(3, Actions.leftBumper, true))
                        cursor--;
                    if (ControllerManager.GetAction(3, Actions.rightBumper, false) && !ControllerManager.GetAction(3, Actions.rightBumper, true))
                        cursor++;
                }

                if (cursor < 0)
                    cursor = 7;
                if (cursor > 7)
                    cursor = 0;

                player.Update();
                if ((ControllerManager.GetAction(1, Actions.start, false) && !ControllerManager.GetAction(1, Actions.start, true)) || (ControllerManager.GetAction(3, Actions.start, false) && !ControllerManager.GetAction(3, Actions.start, true)))
                    pause.Show();
            }
            else
            {
                switch(pause.Update())
                {
                    case 0:
                        pause.Hide();
                        break;
                    case 1:
                        Game1.GameState = GameState.mainMenu;
                        break;
                }
            }

        }

        public void Draw(SpriteBatch sb)
        {
            //Draw box for messages
            sb.Draw(tutorial[cursor], new Rectangle((int)Math.Ceiling(Game1.ScaleX * 460), (int)Math.Ceiling(Game1.ScaleY * 100), (int)Math.Ceiling(Game1.ScaleX * 1000), (int)Math.Ceiling(Game1.ScaleY * 700)), Color.White);
            sb.Draw(currentState, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1460), (int)Math.Ceiling(Game1.ScaleY * 100), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), Color.White);

            int stateNumber;
            switch(player.State)
            {
                case PlayerState.stand:
                    stateNumber = 0;
                    break;
                case PlayerState.dash:
                    stateNumber = 1;
                    break;
                case PlayerState.run:
                    stateNumber = 2;
                    break;
                case PlayerState.squat:
                    stateNumber = 3;
                    break;
                case PlayerState.wallJumpSquat:
                    stateNumber = 3;
                    break;
                case PlayerState.aerial:
                    stateNumber = 4;
                    break;
                case PlayerState.attack:
                    stateNumber = 5;
                    break;
                case PlayerState.land:
                    stateNumber = 6;
                    break;
                case PlayerState.hit:
                    stateNumber = 7;
                    break;
                case PlayerState.dodgeStartup:
                    stateNumber = 8;
                    break;
                case PlayerState.dodge:
                    stateNumber = 8;
                    break;
                case PlayerState.skid:
                    stateNumber = 9;
                    break;
                case PlayerState.crouch:
                    stateNumber = 10;
                    break;
                case PlayerState.dead:
                    stateNumber = 11;
                    break;
                default:
                    stateNumber = -1;
                    break;
            }
            
            sb.Draw(states, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 1460), (int)Math.Ceiling(Game1.ScaleY * 180), (int)Math.Ceiling(Game1.ScaleX * 230), (int)Math.Ceiling(Game1.ScaleY * 80)), new Rectangle(0, stateNumber * 80, 230, 80), Color.White);

            //Draw the player
            player.Draw(sb);
            player.DrawAccessory(sb);
            if (Game1.ShowHitboxes)
                player.DrawHitboxes(sb);

            //Pause menu
            if (!pause.Hidden)
                pause.Draw(sb);
        }

        public void Reset()
        {
            pause.Hide();
            cursor = 0;

            int playernumber = 1;
            if (Game1.UsingKeyboard == 1) playernumber = 3;
            player = new Player(
                new Vector2(100, PlayerManager.Instance.FloorLocation - 250),                  //Player Position
                new Rectangle(100, PlayerManager.Instance.FloorLocation - 250, 150, 250),      //Player hurtbox
                Direction.right,                                        //Direction being faced
                1,
                playernumber);
        }

    }
}
