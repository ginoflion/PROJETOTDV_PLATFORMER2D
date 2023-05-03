using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2DPlatformer.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DPlatformer
{
    public class Coin
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle;
        Animation animation;
        public Coin(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width / 6, texture.Height);
            animation = new Animation(texture, 6, 0.1f);
        }

        public void Update()
        {
            animation.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(position, spriteBatch, SpriteEffects.None);
        }


    }
}
