using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _2DPlatformer.States
{
    public abstract class State // Abstrakt klass då varje State ska ärva samma egenskaper (ContentManager, GraphicsDevice, Game1, Konstruktor)
    {
        protected ContentManager _content; // ContentManager för att kunna ladda in och använda innehåll
        protected GraphicsDevice _graphics; // GraphicsDevice för att kunna rita saker på skärmen
        protected Game1 _game; // Tillgång till Game1 för att kunna byta GameState mm.
        
        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _content = content;
            _graphics = graphicsDevice;
            _game = game;
        }

        public abstract void LoadContent();
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }
}
