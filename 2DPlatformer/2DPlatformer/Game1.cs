using _2DPlatformer.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _2DPlatformer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int screenWidth = 1280;
        public static int screenHeight = 720;
        private State _currentState;
        private State _nextState;
        public static float TotalSeconds; // Används för att animationer ska uppdateras 
        public static List<int> scores = new List<int>(); // Highscore-lista

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = screenWidth; 
            _graphics.PreferredBackBufferHeight = screenHeight;  
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentState = new MenuState(this, GraphicsDevice, Content);
        }
        protected override void Update(GameTime gameTime)
        {
            TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_nextState != null) // Om nextState har ett värde ska en ny state visas
            {
                _currentState = _nextState;
                _currentState.LoadContent(); // Laddar in nya staten
                _nextState = null;
            }
            _currentState.Update(gameTime); // Uppdaterar aktuella staten
            base.Update(gameTime);
        }
        public void ChangeState(State state)
        {
            _nextState = state;
        }
        public void Quit()
        {
            this.Exit();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            _currentState.Draw(gameTime, _spriteBatch); // Drawar aktuella staten
            base.Draw(gameTime);
        }
    }
}