using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _2DPlatformer.Sprites
{
    public class Animation
    {
        Texture2D texture; // Spritesheet
        List<Rectangle> sourceRectangles = new List<Rectangle>(); // Lista av enskilda frames i spritesheet
        int frames; // Antal frames i spritesheet
        int frame; // Aktiva framen
        float frameTime; // Hur länge varje frame ska visas
        float frameTimeLeft; 
        public Animation(Texture2D newTexture, int framesX, float newFrameTime)
        {
            texture = newTexture;
            frameTime = newFrameTime;
            frameTimeLeft = frameTime;
            frames = framesX;

            var frameWidth = texture.Width / framesX;
            var frameHeight = texture.Height;

            for (int i = 0; i < frames; i++) // Lägger till en rektangel för varje frame i spritesheet
            {
                sourceRectangles.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
            }
        }

        public void Update()
        {
            frameTimeLeft -= Game1.TotalSeconds;
            if (frameTimeLeft <= 0)
            {
                frameTimeLeft += frameTime;
                frame = (frame + 1) % frames; // Flyttar aktiva framen ett steg framåt i spritesheeten
            }
        }

        public void Draw(Vector2 position, SpriteBatch spriteBatch, SpriteEffects s)
        {
            spriteBatch.Draw(texture, position, sourceRectangles[frame], Color.White, 0, Vector2.Zero, Vector2.One, s, 1); // Ritar enbart ut aktiva framen (sourceRectangles[frame])
        }

    }
}
