using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Nair.Classes;
using Nair.Classes.Managers;
using System.IO;
using System.Collections.Generic;

namespace Nair.Classes
{
    class ControllerCustomizer
    {

        private Texture2D background;
        private List<HoldBar> options;
        private ControlProfile profile;
        private int controller;
        private Buttons highlightedButton;
        private int highlighted;

        private GamePadState state;
        private GamePadState oldState;

        public ControlProfile Profile { get => profile; }
        public int Controller { get => controller; }

        public int Update()
        {
            //Exit for free
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                return -1;

            state = GamePad.GetState(controller);

            if(state.IsButtonDown(Buttons.Back))
            {
                return 2;
            }
            else if(state.IsButtonDown(Buttons.Start))
            {
                this.SaveButtons();
                profile.SaveControllerProfile(controller + 1);
                return 1;
            }
            else if(highlightedButton == Buttons.BigButton)
            {
                if(state.IsButtonDown(Buttons.A))
                {
                    highlightedButton = Buttons.A;
                    highlighted = 7;
                }
                else if (state.IsButtonDown(Buttons.B))
                {
                    highlightedButton = Buttons.B;
                    highlighted = 6;
                }
                else if (state.IsButtonDown(Buttons.X))
                {
                    highlightedButton = Buttons.X;
                    highlighted = 5;
                }
                else if (state.IsButtonDown(Buttons.Y))
                {
                    highlightedButton = Buttons.Y;
                    highlighted = 4;
                }
                else if (state.IsButtonDown(Buttons.RightShoulder))
                {
                    highlightedButton = Buttons.RightShoulder;
                    highlighted = 3;
                }
                else if (state.IsButtonDown(Buttons.RightTrigger))
                {
                    highlightedButton = Buttons.RightTrigger;
                    highlighted = 2;
                }
                else if (state.IsButtonDown(Buttons.LeftShoulder))
                {
                    highlightedButton = Buttons.LeftShoulder;
                    highlighted = 1;
                }
                else if (state.IsButtonDown(Buttons.LeftTrigger))
                {
                    highlightedButton = Buttons.LeftTrigger;
                    highlighted = 0;
                }

            }
            else
            {
                if(state.IsButtonUp(highlightedButton))
                {
                    highlightedButton = Buttons.BigButton;
                    highlighted = -1;
                }
                else if(state.IsButtonDown(Buttons.LeftThumbstickUp) && oldState.IsButtonUp(Buttons.LeftThumbstickUp))
                {
                    options[highlighted].Cursor--;
                }
                else if(state.IsButtonDown(Buttons.LeftThumbstickDown) && oldState.IsButtonUp(Buttons.LeftThumbstickDown))
                {
                    options[highlighted].Cursor++;
                }
            }


            oldState = state;
            return 0;
        }

        public ControllerCustomizer(Texture2D background, ControlProfile profile, Texture2D barOptions1, Texture2D barOptions2, int controller)
        {
            this.background = background;
            this.profile = profile;
            this.controller = controller;
            highlighted = -1;
            highlightedButton = Buttons.BigButton;

            options = new List<HoldBar>();

            // 0 - 7
            // LT, LB, RT, RB, Y, X, B, A

            options.Add(new HoldBar(new Vector2(101, 67), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(101, 161), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 67), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 161), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 388), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 482), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 574), barOptions1, barOptions2));
            options.Add(new HoldBar(new Vector2(1621, 668), barOptions1, barOptions2));

            //Attack
            List<Buttons> buttons = profile.GetButtons(Actions.attack);
            if (buttons.Contains(Buttons.LeftTrigger)) options[0].Cursor = 0;
            if (buttons.Contains(Buttons.LeftShoulder)) options[1].Cursor = 0;
            if (buttons.Contains(Buttons.RightTrigger)) options[2].Cursor = 0;
            if (buttons.Contains(Buttons.RightShoulder)) options[3].Cursor = 0;
            if (buttons.Contains(Buttons.Y)) options[4].Cursor = 0;
            if (buttons.Contains(Buttons.X)) options[5].Cursor = 0;
            if (buttons.Contains(Buttons.B)) options[6].Cursor = 0;
            if (buttons.Contains(Buttons.A)) options[7].Cursor = 0;

            //Jump
            buttons = profile.GetButtons(Actions.jump);
            if (buttons.Contains(Buttons.LeftTrigger)) options[0].Cursor = 1;
            if (buttons.Contains(Buttons.LeftShoulder)) options[1].Cursor = 1;
            if (buttons.Contains(Buttons.RightTrigger)) options[2].Cursor = 1;
            if (buttons.Contains(Buttons.RightShoulder)) options[3].Cursor = 1;
            if (buttons.Contains(Buttons.Y)) options[4].Cursor = 1;
            if (buttons.Contains(Buttons.X)) options[5].Cursor = 1;
            if (buttons.Contains(Buttons.B)) options[6].Cursor = 1;
            if (buttons.Contains(Buttons.A)) options[7].Cursor = 1;

            //Dodge
            buttons = profile.GetButtons(Actions.dodge);
            if (buttons.Contains(Buttons.LeftTrigger)) options[0].Cursor = 2;
            if (buttons.Contains(Buttons.LeftShoulder)) options[1].Cursor = 2;
            if (buttons.Contains(Buttons.RightTrigger)) options[2].Cursor = 2;
            if (buttons.Contains(Buttons.RightShoulder)) options[3].Cursor = 2;
            if (buttons.Contains(Buttons.Y)) options[4].Cursor = 2;
            if (buttons.Contains(Buttons.X)) options[5].Cursor = 2;
            if (buttons.Contains(Buttons.B)) options[6].Cursor = 2;
            if (buttons.Contains(Buttons.A)) options[7].Cursor = 2;

        }


        public void Draw(SpriteBatch sb)
        {
            sb.Draw(background, new Rectangle(0, 0, Game1.windowWidth, Game1.windowHeight), Color.White);

            foreach(HoldBar bar in options)
            {
                bar.Draw(sb);
            }

            if (highlighted != -1)
            {
                if(highlighted != 0 && highlighted != 2)
                    options[highlighted].DrawExtended(sb, false);
                else
                    options[highlighted].DrawExtended(sb, true);
            }
        }

        private void SaveButtons()
        {
            profile.Clear();
            // 0 - 7
            // LT, LB, RT, RB, Y, X, B, A
            // 0-3
            //Attack, Jump, Dodge, Null

            profile.AddButton(Actions.start, Buttons.Start);
            profile.AddButton(options[0].Cursor, Buttons.LeftTrigger);
            profile.AddButton(options[1].Cursor, Buttons.LeftShoulder);
            profile.AddButton(options[2].Cursor, Buttons.RightTrigger);
            profile.AddButton(options[3].Cursor, Buttons.RightShoulder);
            profile.AddButton(options[4].Cursor, Buttons.Y);
            profile.AddButton(options[5].Cursor, Buttons.X);
            profile.AddButton(options[6].Cursor, Buttons.B);
            profile.AddButton(options[7].Cursor, Buttons.A);

        }

    }
}
