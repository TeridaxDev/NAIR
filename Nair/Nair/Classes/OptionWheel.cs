using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Nair.Classes.Managers;

namespace Nair.Classes
{
    class OptionWheel : Button
    {

        private List<Texture2D> options;
        private int cursor = 0;
        private bool singleImageMode = false;
        private int values;

        public int Cursor { get => cursor; set => cursor = value; }

        /// <summary>
        /// Create an option wheel with multiple textures
        /// </summary>
        /// <param name="position"></param>
        public OptionWheel(Rectangle position) : base(position, null, null)
        {
            options = new List<Texture2D>();
        }
        /// <summary>
        /// Create an option wheel with one texture
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        /// <param name="values"></param>
        public OptionWheel(Rectangle position, Texture2D sprite, int values) : base(position, null, null)
        {
            options = new List<Texture2D>();
            this.values = values;
            singleImageMode = true;
            options.Add(sprite);
        }

        public void GoLeft()
        {
            cursor--;
            if (cursor == -1 && !singleImageMode)
                cursor = options.Count - 1;
            else if (cursor == -1)
                cursor = values - 1;
        }
        public void GoRight()
        {
            cursor++;
            if (cursor > options.Count - 1 && !singleImageMode)
                cursor = 0;
            else if (singleImageMode && cursor >= values)
                cursor = 0;
        }

        public void AddOption(Texture2D texture)
        {
            options.Add(texture);
        }

        public override void Draw(SpriteBatch sb, bool highlighted)
        {
            Rectangle scaled = new Rectangle((int)Math.Ceiling(Game1.ScaleX * position.X), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * position.Width), (int)Math.Ceiling(Game1.ScaleY * position.Height));
            Rectangle arrowLeft = new Rectangle((int)Math.Ceiling(Game1.ScaleX * (position.X - 90)), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * 80), (int)Math.Ceiling(Game1.ScaleY * 80));
            Rectangle arrowRight = new Rectangle((int)Math.Ceiling(Game1.ScaleX * (position.X + position.Width + 10)), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * 80), (int)Math.Ceiling(Game1.ScaleY * 80));

            float colorMod = 0.5f;
            if (highlighted) colorMod = 1f;

            if (!singleImageMode)
            {
                sb.Draw(options[cursor], scaled, Color.White * colorMod);
            }
            else
            {
                sb.Draw(options[0], scaled, new Rectangle(0, cursor * (options[0].Height/values), options[0].Width, options[0].Height/values), Color.White * colorMod);
            }

            sb.Draw(Game1.Arrow, arrowLeft, null, Color.White * colorMod, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 0f);
            sb.Draw(Game1.Arrow, arrowRight, Color.White * colorMod);
        }

    }
}
