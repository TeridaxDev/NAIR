using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using Nair.Classes;
using Nair.Classes.Managers;
using System.IO;
using System.Collections.Generic;

namespace Nair.Classes
{
    class KeyboardCustomizer
    {

        private ControlProfile profile;
        private MouseState ms;
        private MouseState oldMs;
        private KeyboardState ks;
        private KeyboardState oldKs;

        private Menu setKey;
        private int keyIndex;

        private List<KeyMouseButton> buttons;
        private List<Keys> acceptableKeys;

        private Texture2D blank1;
        private Texture2D blank2;
        private Texture2D keySelect;
        private Texture2D background;

        public ControlProfile Profile { get => profile; }

        public int Update()
        {
            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            if (setKey.Hidden)
            {
                int action = -1;
                for (int i = 0; i < buttons.Count; i++)
                {
                    action = buttons[i].Update(ms, oldMs);
                    if (action == 1)
                    {
                        keyIndex = i;
                        break;
                    }
                }

                if (ks.IsKeyDown(Keys.Escape) && oldKs.IsKeyUp(Keys.Escape))
                {
                    this.SaveKeys();
                    profile.SaveKeyProfile();
                    return 1;
                }
                else if(ks.IsKeyDown(Keys.Back) && oldKs.IsKeyUp(Keys.Back))
                {
                    return 2;
                }

                if (action == 1)
                    setKey.Show();
            }
            else
            {
                //setKey.Update();

                if(ks.IsKeyDown(Keys.Back) && oldKs.IsKeyUp(Keys.Back))
                {
                    setKey.Hide();
                    oldMs = ms;
                    oldKs = ks;
                    return 0;
                }
                else
                {
                    foreach(Keys key in acceptableKeys)
                    {
                        if (ks.IsKeyDown(key) && oldKs.IsKeyUp(key))
                            buttons[keyIndex].Button = key;
                    }
                }

            }


            oldMs = ms;
            oldKs = ks;

            return 0;
        }

        public KeyboardCustomizer(ControlProfile profile, Texture2D blank1, Texture2D blank2, Texture2D keySelect, Texture2D background)
        {
            this.profile = profile;
            this.blank1 = blank1;
            this.blank2 = blank2;
            this.keySelect = keySelect;
            this.background = background;

            buttons = new List<KeyMouseButton>();
            acceptableKeys = new List<Keys>();

            acceptableKeys.Add(Keys.A);
            acceptableKeys.Add(Keys.B);
            acceptableKeys.Add(Keys.C);
            acceptableKeys.Add(Keys.D);
            acceptableKeys.Add(Keys.E);
            acceptableKeys.Add(Keys.F);
            acceptableKeys.Add(Keys.G);
            acceptableKeys.Add(Keys.H);
            acceptableKeys.Add(Keys.I);
            acceptableKeys.Add(Keys.J);
            acceptableKeys.Add(Keys.K);
            acceptableKeys.Add(Keys.L);
            acceptableKeys.Add(Keys.M);
            acceptableKeys.Add(Keys.N);
            acceptableKeys.Add(Keys.O);
            acceptableKeys.Add(Keys.P);
            acceptableKeys.Add(Keys.Q);
            acceptableKeys.Add(Keys.R);
            acceptableKeys.Add(Keys.S);
            acceptableKeys.Add(Keys.T);
            acceptableKeys.Add(Keys.U);
            acceptableKeys.Add(Keys.V);
            acceptableKeys.Add(Keys.W);
            acceptableKeys.Add(Keys.X);
            acceptableKeys.Add(Keys.Y);
            acceptableKeys.Add(Keys.Z);
            acceptableKeys.Add(Keys.D1);
            acceptableKeys.Add(Keys.D2);
            acceptableKeys.Add(Keys.D3);
            acceptableKeys.Add(Keys.D4);
            acceptableKeys.Add(Keys.D5);
            acceptableKeys.Add(Keys.D6);
            acceptableKeys.Add(Keys.D7);
            acceptableKeys.Add(Keys.D8);
            acceptableKeys.Add(Keys.D9);
            acceptableKeys.Add(Keys.D0);
            acceptableKeys.Add(Keys.OemComma);
            acceptableKeys.Add(Keys.OemPeriod);
            acceptableKeys.Add(Keys.OemQuestion);
            acceptableKeys.Add(Keys.OemOpenBrackets);
            acceptableKeys.Add(Keys.OemCloseBrackets);
            acceptableKeys.Add(Keys.OemMinus);
            acceptableKeys.Add(Keys.OemPipe);
            acceptableKeys.Add(Keys.OemPlus);
            acceptableKeys.Add(Keys.OemSemicolon);
            acceptableKeys.Add(Keys.OemQuotes);
            acceptableKeys.Add(Keys.OemTilde);

            acceptableKeys.Add(Keys.Up);
            acceptableKeys.Add(Keys.Down);
            acceptableKeys.Add(Keys.Left);
            acceptableKeys.Add(Keys.Right);

            acceptableKeys.Add(Keys.LeftControl);
            acceptableKeys.Add(Keys.RightControl);
            acceptableKeys.Add(Keys.LeftAlt);
            acceptableKeys.Add(Keys.RightAlt);
            acceptableKeys.Add(Keys.LeftShift);
            acceptableKeys.Add(Keys.RightShift);
            acceptableKeys.Add(Keys.Space);
            acceptableKeys.Add(Keys.Enter);
            acceptableKeys.Add(Keys.Tab);


            setKey = new Menu(new Rectangle(690, 320, 540, 440));


            // Up, down, left, right | Jump, attack, dodge | hold slow | Up, down, left, right | LB, RB

            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1380, 750), profile.GetKeys(Actions.hardUp)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1380, 950), profile.GetKeys(Actions.hardDown)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1250, 850), profile.GetKeys(Actions.hardLeft)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1500, 850), profile.GetKeys(Actions.hardRight)[0]));

            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(200, 750), profile.GetKeys(Actions.jump)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(200, 850), profile.GetKeys(Actions.attack)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(200, 950), profile.GetKeys(Actions.dodge)[0]));

            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(200, 600), profile.GetKeys(Actions.holdSoft)[0]));

            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(500, 250), profile.GetKeys(Actions.softUp)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(500, 450), profile.GetKeys(Actions.softDown)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(370, 350), profile.GetKeys(Actions.softLeft)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(630, 350), profile.GetKeys(Actions.softRight)[0]));

            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1250, 350), profile.GetKeys(Actions.leftBumper)[0]));
            buttons.Add(new KeyMouseButton(blank1, blank2, new Vector2(1500, 350), profile.GetKeys(Actions.rightBumper)[0]));

        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(background, new Rectangle(0, 0, Game1.windowWidth, Game1.windowHeight), Color.White);

            foreach(KeyMouseButton button in buttons)
            {
                button.Draw(sb);
            }

            if(!setKey.Hidden)
            {
                setKey.Draw(sb);
                sb.Draw(keySelect, new Rectangle((int)Math.Ceiling(Game1.ScaleX * 710), (int)Math.Ceiling(Game1.ScaleY * 340), (int)Math.Ceiling(Game1.ScaleX * 500), (int)Math.Ceiling(Game1.ScaleY * 400)), Color.White);

                string buttonString = buttons[keyIndex].Button.ToString();
                if (buttonString.StartsWith("Oem"))
                    buttonString = buttonString.Remove(0, 3);
                else if (buttonString.StartsWith("D") && buttonString.Length == 2)
                    buttonString = buttonString.TrimStart('D');
                else if (buttonString.StartsWith("Left") && buttonString != "Left")
                {
                    buttonString = buttonString.Remove(0, 4);
                    buttonString = "L" + buttonString;
                }
                else if (buttonString.StartsWith("Right") && buttonString != "Right")
                {
                    buttonString = buttonString.Remove(0, 5);
                    buttonString = "R" + buttonString;
                }

                if (buttonString == "Comma")
                    buttonString = ",";
                if (buttonString == "Period")
                    buttonString = ".";
                if (buttonString == "OpenBrackets")
                    buttonString = "[";
                if (buttonString == "CloseBrackets")
                    buttonString = "]";
                if (buttonString == "Semicolon")
                    buttonString = ";";
                if (buttonString == "Tilde")
                    buttonString = "~";
                if (buttonString == "Quotes")
                    buttonString = "\'";
                if (buttonString == "Pipe")
                    buttonString = "\\";
                if (buttonString == "Question")
                    buttonString = "/";
                if (buttonString == "Plus")
                    buttonString = "=";
                if (buttonString == "Minus")
                    buttonString = "-";
                if (buttonString == "LControl")
                    buttonString = "LCtrl";
                if (buttonString == "RControl")
                    buttonString = "RCtrl";

                string spaces = "";
                switch (buttonString.Length)
                {
                    case 1:
                        spaces = "  ";
                        break;
                    case 2:
                        spaces = "  ";
                        break;
                    case 3:
                        spaces = " ";
                        break;
                    case 4:
                        spaces = " ";
                        break;
                    case 5:
                        spaces = "";
                        break;
                }
                sb.DrawString(Game1.CourierNew, spaces + buttonString, new Vector2((int)Math.Ceiling(Game1.ScaleX * 850), (int)Math.Ceiling(Game1.ScaleY * 500)), Color.Black, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080) * new Vector2(0.85f, 0.85f), SpriteEffects.None, 0f);
            }
        }

        private void SaveKeys()
        {
            profile.Clear();

            profile.AddButton(Actions.start, Keys.Escape);

            profile.AddButton(Actions.hardUp, buttons[0].Button);
            profile.AddButton(Actions.hardDown, buttons[1].Button);
            profile.AddButton(Actions.hardLeft, buttons[2].Button);
            profile.AddButton(Actions.hardRight, buttons[3].Button);

            profile.AddButton(Actions.jump, buttons[4].Button);
            profile.AddButton(Actions.attack, buttons[5].Button);
            profile.AddButton(Actions.dodge, buttons[6].Button);

            profile.AddButton(Actions.holdSoft, buttons[7].Button);

            profile.AddButton(Actions.softUp, buttons[8].Button);
            profile.AddButton(Actions.softDown, buttons[9].Button);
            profile.AddButton(Actions.softLeft, buttons[10].Button);
            profile.AddButton(Actions.softRight, buttons[11].Button);

            profile.AddButton(Actions.leftBumper, buttons[12].Button);
            profile.AddButton(Actions.rightBumper, buttons[13].Button);


        }

    }
}
