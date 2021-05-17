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
    class KeyMouseButton
    {

        private Texture2D blank1;
        private Texture2D blank2;
        private Keys button;

        private Rectangle position;
        private Rectangle adjustedPos;

        private bool highlighted;

        public Keys Button { get => button; set => button = value; }

        public int Update(MouseState ms, MouseState old)
        {
            ms = Mouse.GetState();

            if (adjustedPos.Intersects(new Rectangle(ms.Position.X, ms.Position.Y, 1, 1)))
            {
                this.highlighted = true;
            }
            else
                this.highlighted = false;

            if (highlighted && ms.LeftButton == ButtonState.Pressed && old.LeftButton == ButtonState.Released)
                return 1;
            return 0;
        }

        public KeyMouseButton(Texture2D blank1, Texture2D blank2, Vector2 position, Keys button)
        {
            this.blank1 = blank1;
            this.blank2 = blank2;
            this.button = button;

            this.position = new Rectangle((int)position.X, (int)position.Y, 230, 80);

            adjustedPos = new Rectangle((int)Math.Ceiling(Game1.ScaleX * position.X), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * this.position.Width), (int)Math.Ceiling(Game1.ScaleY * this.position.Height));
        }

        public void Draw(SpriteBatch sb)
        {
            Texture2D texture = blank1;
            if (highlighted) texture = blank2;

            Color textColor = Color.Black;
            if (highlighted) textColor = Color.White;

            string buttonString = button.ToString();
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
            if (buttonString == "LShift")
                buttonString = "LShft";
            if (buttonString == "RShift")
                buttonString = "RShft";

            string spaces = "";
            switch(buttonString.Length)
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

            sb.Draw(texture, adjustedPos, Color.White);
            sb.DrawString(Game1.CourierNew, spaces + buttonString, new Vector2(adjustedPos.X, adjustedPos.Y), textColor, 0f, new Vector2(), new Vector2((float)Game1.windowWidth / 1920, (float)Game1.windowHeight / 1080) * new Vector2(0.85f, 0.85f), SpriteEffects.None, 0f);
        }

    }
}
