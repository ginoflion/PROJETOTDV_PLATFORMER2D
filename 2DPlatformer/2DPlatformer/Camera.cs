using Microsoft.Xna.Framework;

namespace _2DPlatformer
{
    internal class Camera
    {
        public Matrix Transform { get; private set; }
        public void Follow(Player target)
        {
            var position = Matrix.CreateTranslation( MathHelper.Clamp(-target.position.X - (target.rectangle.Width / 2), -3420, -650), MathHelper.Clamp(-target.position.Y - (target.rectangle.Height / 2), -300, -3475738), 0);
            var offset = Matrix.CreateTranslation(Game1.screenWidth / 2, Game1.screenHeight / 2 - 100, 0);
            Transform = position * offset;
        }
    }
}
