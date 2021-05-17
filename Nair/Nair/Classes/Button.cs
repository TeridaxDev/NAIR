using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nair.Classes
{
    class Button
    {

        protected Rectangle position;
        private Texture2D texture;
        private Texture2D highlightTexture;

        public Button(Rectangle position, Texture2D texture, Texture2D highlightTexture)
        {
            this.position = position;
            this.texture = texture;
            this.highlightTexture = highlightTexture;
        }

        public virtual void Draw(SpriteBatch sb, bool highlighted)
        {
            Rectangle scaled = new Rectangle((int)Math.Ceiling(Game1.ScaleX * position.X), (int)Math.Ceiling(Game1.ScaleY * position.Y), (int)Math.Ceiling(Game1.ScaleX * position.Width), (int)Math.Ceiling(Game1.ScaleY * position.Height));

            Color color = Game1.BGColor;

            if (!highlighted)
                sb.Draw(texture, scaled, color);
            else
                sb.Draw(highlightTexture, scaled, color);

        }

    }
}
