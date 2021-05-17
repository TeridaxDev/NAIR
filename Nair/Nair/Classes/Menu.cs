using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nair.Classes.Managers;


namespace Nair.Classes
{
    class Menu
    {
        private int frame;
        private bool hidden;
        private List<Button> menuItems;
        private int cursorPos;
        private Rectangle position;

        private KeyboardState kbState;
        private KeyboardState kbStateOld;
        private GamePadState oneState;
        private GamePadState oneStateOld;
        private GamePadState twoState;
        private GamePadState twoStateOld;
        private GamePadState threeState;
        private GamePadState threeStateOld;
        private GamePadState fourState;
        private GamePadState fourStateOld;

        public bool Hidden { get => hidden; }

        public Menu(Rectangle position)
        {
            menuItems = new List<Button>();
            cursorPos = 0;
            hidden = true;

            this.position = position;
        }

        public void Show()
        {
            hidden = false;
            cursorPos = -1;

            kbStateOld = Keyboard.GetState();
            oneStateOld = GamePad.GetState(PlayerIndex.One);
            twoStateOld = GamePad.GetState(PlayerIndex.Two);
            threeStateOld = GamePad.GetState(PlayerIndex.Three);
            fourStateOld = GamePad.GetState(PlayerIndex.Four);
        }
        public void Hide()
        {
            hidden = true;
        }

        public void AddButton(Button button)
        {
            menuItems.Add(button);
        }

        public int GetValue(int index)
        {
            if(menuItems[index] is OptionWheel)
            {
                return ((OptionWheel)menuItems[index]).Cursor;
            }

            throw new InvalidOperationException();
        }
        public void SetValue(int index, int value)
        {
            if (menuItems[index] is OptionWheel)
            {
                ((OptionWheel)menuItems[index]).Cursor = value;
                return;
            }

            throw new InvalidOperationException();
        }

        public int Update()
        {
            frame++;
            if (frame == 25) frame = 0;

            kbState = Keyboard.GetState();
            oneState = GamePad.GetState(PlayerIndex.One);
            twoState = GamePad.GetState(PlayerIndex.Two);
            threeState = GamePad.GetState(PlayerIndex.Three);
            fourState = GamePad.GetState(PlayerIndex.Four);

            //Cursor up
            if (!hidden && 
                ((kbState.IsKeyDown(Keys.Up) && kbStateOld.IsKeyUp(Keys.Up))
                || (oneState.ThumbSticks.Left.Y > 0.5f && oneStateOld.ThumbSticks.Left.Y <= 0.5f)
                || (twoState.ThumbSticks.Left.Y > 0.5f && twoStateOld.ThumbSticks.Left.Y <= 0.5f)
                || (threeState.ThumbSticks.Left.Y > 0.5f && threeStateOld.ThumbSticks.Left.Y <= 0.5f)
                || (fourState.ThumbSticks.Left.Y > 0.5f && fourStateOld.ThumbSticks.Left.Y <= 0.5f)))
            {
                cursorPos--;
                if (cursorPos < 0) cursorPos = menuItems.Count - 1;
                PlayerManager.Instance.PlaySound("click");
            }
            //Cursor Down
            if (!hidden &&
                ((kbState.IsKeyDown(Keys.Down) && kbStateOld.IsKeyUp(Keys.Down))
                || (oneState.ThumbSticks.Left.Y < -0.5f && oneStateOld.ThumbSticks.Left.Y >= -0.5f)
                || (twoState.ThumbSticks.Left.Y < -0.5f && twoStateOld.ThumbSticks.Left.Y >= -0.5f)
                || (threeState.ThumbSticks.Left.Y < -0.5f && threeStateOld.ThumbSticks.Left.Y >= -0.5f)
                || (fourState.ThumbSticks.Left.Y < -0.5f && fourStateOld.ThumbSticks.Left.Y >= -0.5f)))
            {
                cursorPos++;
                if (cursorPos > menuItems.Count - 1) cursorPos = 0;
                PlayerManager.Instance.PlaySound("click");
            }

            int returnValue;

            //Enter
            if ((kbState.IsKeyDown(Keys.Enter) && kbStateOld.IsKeyUp(Keys.Enter))
                || (oneState.IsButtonDown(Buttons.A) && oneStateOld.IsButtonUp(Buttons.A))
                || (twoState.IsButtonDown(Buttons.A) && twoStateOld.IsButtonUp(Buttons.A))
                || (threeState.IsButtonDown(Buttons.A) && threeStateOld.IsButtonUp(Buttons.A))
                || (fourState.IsButtonDown(Buttons.A) && fourStateOld.IsButtonUp(Buttons.A)))
            {
                returnValue = cursorPos;
                PlayerManager.Instance.PlaySound("click");
            }
            else
                returnValue = -1;

            if(cursorPos >= 0 && menuItems[cursorPos] is OptionWheel)
            {
                //Right
                if (!hidden &&
                ((kbState.IsKeyDown(Keys.Right) && kbStateOld.IsKeyUp(Keys.Right))
                || (oneState.ThumbSticks.Left.X > 0.5f && oneStateOld.ThumbSticks.Left.X <= 0.5f)
                || (twoState.ThumbSticks.Left.X > 0.5f && twoStateOld.ThumbSticks.Left.X <= 0.5f)
                || (threeState.ThumbSticks.Left.X > 0.5f && threeStateOld.ThumbSticks.Left.X <= 0.5f)
                || (fourState.ThumbSticks.Left.X > 0.5f && fourStateOld.ThumbSticks.Left.X <= 0.5f)))
                {
                    ((OptionWheel)menuItems[cursorPos]).GoRight();
                    PlayerManager.Instance.PlaySound("click");
                }
                //Left
                if (!hidden &&
                ((kbState.IsKeyDown(Keys.Left) && kbStateOld.IsKeyUp(Keys.Left))
                || (oneState.ThumbSticks.Left.X < -0.5f && oneStateOld.ThumbSticks.Left.X >= -0.5f)
                || (twoState.ThumbSticks.Left.X < -0.5f && twoStateOld.ThumbSticks.Left.X >= -0.5f)
                || (threeState.ThumbSticks.Left.X < -0.5f && threeStateOld.ThumbSticks.Left.X >= -0.5f)
                || (fourState.ThumbSticks.Left.X < -0.5f && fourStateOld.ThumbSticks.Left.X >= -0.5f)))
                {
                    ((OptionWheel)menuItems[cursorPos]).GoLeft();
                    PlayerManager.Instance.PlaySound("click");
                }
            }

            kbStateOld = kbState;
            oneStateOld = oneState;
            twoStateOld = twoState;
            threeStateOld = threeState;
            fourStateOld = fourState;

            return returnValue;

        }

        public void Draw(SpriteBatch sb)
        {
            if (!hidden)
            {
                Rectangle scaled = new Rectangle((int)Math.Ceiling(Game1.ScaleX * position.X), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * position.Width), (int)Math.Ceiling(Game1.ScaleY * position.Height));

                if (frame <= 12)
                    sb.Draw(Game1.Menu1, scaled, Game1.BGColor);
                else
                    sb.Draw(Game1.Menu2, scaled, Game1.BGColor);
                //sb.Draw(Game1.BlankPixel, new Rectangle(scaled.X + 4, scaled.Y + 4, scaled.Width - 8, scaled.Height - 8), Color.White);

                //Displays an int indicating where the cursor is
                //sb.DrawString(Game1.CourierNew, cursorPos.ToString(), new Vector2(), Color.Black);

                for(int i = 0; i < menuItems.Count; i++)
                {
                    if (i == cursorPos)
                        menuItems[i].Draw(sb, true);
                    else
                        menuItems[i].Draw(sb, false);
                }
            }
        }

    }
}
