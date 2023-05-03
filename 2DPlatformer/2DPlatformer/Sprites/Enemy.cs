using _2DPlatformer.Sprites;
using _2DPlatformer.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DPlatformer
{
    public class Enemy
    {
        Texture2D texture;
        public Vector2 position;
        public static float speed = 2f;
        public Vector2 velocity;
        public Rectangle rectangle, turningRectangle, rectangleHead, rectangleRight, rectangleLeft;
        bool intersected, isGrounded;
        bool movingRight;
        public bool isDead = false;
        Animation animation;
        int framesX = 10;

        public Enemy(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            velocity.X = speed;
            movingRight = true;
            animation = new Animation(texture, framesX, 0.1f);
        }

        public void Update(GameTime gameTime)
        {

            #region Uppdatering

            position += velocity;
            rectangle = new Rectangle((int)position.X + 10, (int)position.Y + 20, (int)texture.Width / framesX - 10, texture.Height - 30);
            rectangleHead = new Rectangle((int)position.X + 10, (int)position.Y, (int)texture.Width / framesX - 10, 10);
            rectangleLeft = new Rectangle((int)position.X, (int)position.Y, 1, texture.Height - 10);
            rectangleRight = new Rectangle((int)position.X + texture.Width / framesX, (int)position.Y, 1, texture.Height - 10);
            animation.Update();

            if (movingRight)
            {
                // Rektangel som är fiendens bredd längre ut än fienden själv i riktningen som fienden rör sig
                turningRectangle = new Rectangle((int)position.X + texture.Width / framesX, (int)position.Y, (int)texture.Width / framesX, (int)texture.Height); 
            }
            else if (!movingRight)
            {
                turningRectangle = new Rectangle((int)position.X - texture.Width / framesX, (int)position.Y, (int)texture.Width / framesX, (int)texture.Height);
            }

            #endregion

            intersected = false;
            foreach (Platform platform in GameState.platforms)
            {
                if (rectangleRight.Intersects(platform.rectangle) || rectangleLeft.Intersects(platform.rectangle))
                {
                    Turn();
                }
                else if (turningRectangle.Intersects(platform.rectangle))
                {
                    intersected = true;
                }
            }
            isGrounded = intersected;
            
            if (isGrounded == false) // Om turningRectangle inte längre rör plattformen vänder fienden håll
            {
                Turn();
            }
        }

        public void Turn()
        {
            velocity.X = -velocity.X;
            if (movingRight)
            {
                movingRight = false;
            }
            else if (!movingRight)
            {
                movingRight = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (movingRight)
            {
                animation.Draw(position, spriteBatch, SpriteEffects.None);
            }
            else
            {
                animation.Draw(position, spriteBatch, SpriteEffects.FlipHorizontally);
            }
             
        }
    }
}
