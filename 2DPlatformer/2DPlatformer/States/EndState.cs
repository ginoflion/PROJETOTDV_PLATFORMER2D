using _2DPlatformer.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace _2DPlatformer.States
{
    internal class EndState : State
    {
        SpriteFont font, font_larger, font_smaller;
        Button button;
        Texture2D button_texture;
        SoundEffect click_sound;
        
        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            
        }

        public override void LoadContent()
        {
            font = _content.Load<SpriteFont>("Fonts/font");
            font_larger = _content.Load<SpriteFont>("Fonts/font_larger");
            font_smaller = _content.Load<SpriteFont>("Fonts/font_smaller");
            button_texture = _content.Load<Texture2D>("Controls/button");
            button = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight - 100), font, "Menu");
            click_sound = _content.Load<SoundEffect>("Sound Effects/interface1");

            Game1.scores.Add(GameState.player.currentScore);
            Game1.scores.Sort((a, b) => b.CompareTo(a)); // Sorterar listan i storleksordning

            MediaPlayer.Stop(); // Stoppar musiken
        }

        public override void Update(GameTime gameTime)
        {
            button.Update();
            if (button.clicked)
            {
                click_sound.Play();
                Thread.Sleep(100);
                _game.ChangeState(new MenuState(_game, _graphics, _content));
                
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _game.GraphicsDevice.Clear(Color.Green);
            spriteBatch.Begin();
            spriteBatch.DrawString(font_larger, "You Win!", new Vector2(1280 / 2 - 100, 150), Color.White);
            spriteBatch.DrawString(font, $"Score: {GameState.player.currentScore} ", new Vector2(1280 / 2 - 100, 250), Color.White);

            spriteBatch.DrawString(font, "Highscores", new Vector2(900, 50), Color.White);
            for (int i = 0; i < Game1.scores.Count; i++)
            {
                spriteBatch.DrawString(font_smaller, $"{i + 1}.       " + Game1.scores[i].ToString(), new Vector2(1000, 50 * (i + 1) + 100), Color.White);
            }
            button.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
