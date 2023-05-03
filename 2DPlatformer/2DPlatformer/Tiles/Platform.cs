using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2DPlatformer
{
    public class Platform
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle, rectangleTop;
        public bool isDeadly, isOnlySolidTop, isBouncy;

        /*
         * isDeadly ger en plattform som spelaren dör av att kollidera med
         * 
         * isOnlySolidTop ger en plattform som spelaren kan gå igenom i sidled, hoppa igenom och kan stå på
         * 
         * isBouncy ger en plattform som spelaren studsar på om han rör den med fötterna
         */

        public Platform(Texture2D newTexture, Vector2 newPosition, bool deadly, bool onlySolidTop, bool bouncy)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            rectangleTop = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, 25);
            isDeadly = deadly;
            isOnlySolidTop = onlySolidTop;
            isBouncy = bouncy;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

    }
}
