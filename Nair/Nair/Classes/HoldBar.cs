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
    class HoldBar
    {

        private int cursor;
        private Rectangle position;
        private Texture2D options1;
        private Texture2D options2;

        public int Cursor
        {
            get => cursor;

            set
            {
                cursor = value;
                if (cursor >= 4) cursor = 0;
                if (cursor <= -1) cursor = 3;
            }
        }

        public HoldBar(Vector2 position, Texture2D options1, Texture2D options2)
        {
            this.position = new Rectangle((int)position.X, (int)position.Y, 200, 80);
            this.options1 = options1;
            this.options2 = options2;

            cursor = 3;
        }

        public void Draw(SpriteBatch sb)
        {
            Rectangle adjustedPosition = new Rectangle((int)Math.Ceiling(Game1.ScaleX * position.X), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * position.Width), (int)Math.Ceiling(Game1.ScaleY * position.Height));

            sb.Draw(options1, adjustedPosition, new Rectangle(0, cursor * 80, 200, 80), Color.White);
        }

        public void DrawExtended(SpriteBatch sb, bool down)
        {
            Rectangle adjustedPosition;

            if (!down)
            {
                adjustedPosition = new Rectangle(position.X, position.Y - 120, 200, 320);
            }
            else
            {
                adjustedPosition = new Rectangle(position.X, position.Y, 200, 320);
            }

            adjustedPosition = new Rectangle((int)Math.Ceiling(Game1.ScaleX * adjustedPosition.X), (int)Math.Ceiling(Game1.ScaleY * adjustedPosition.Y), (int)Math.Ceiling(Game1.ScaleX * adjustedPosition.Width), (int)Math.Ceiling(Game1.ScaleY * adjustedPosition.Height));

            sb.Draw(Game1.BlankPixel, new Rectangle(adjustedPosition.X - 10, adjustedPosition.Y - 6, adjustedPosition.Width + 20, adjustedPosition.Height + 21), Color.Orange);
            sb.Draw(options1, adjustedPosition, Color.White);

            sb.Draw(options2, new Rectangle(adjustedPosition.X, adjustedPosition.Y + (int)Math.Ceiling(80 * cursor * Game1.ScaleY), adjustedPosition.Width, adjustedPosition.Height/4), new Rectangle(0, cursor * 80, 200, 80), Color.White);
        }

    }
}
