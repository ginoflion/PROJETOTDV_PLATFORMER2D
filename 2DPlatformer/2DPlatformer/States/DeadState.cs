using _2DPlatformer.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace _2DPlatformer.States
{
    public class DeadState : State
    {
        public static Button button_respawn;
        public static Button button_menu;
        public static int savedPlayerLevel;
        public static int savedPlayerScore = 0;

        SpriteFont font, font_larger;
        Texture2D button_texture, background;
        SoundEffect click_sound;

        public DeadState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            
        }
        public override void LoadContent()
        {
            font = _content.Load<SpriteFont>("Fonts/font");
            font_larger = _content.Load<SpriteFont>("Fonts/font_larger");
            button_texture = _content.Load<Texture2D>("Controls/button");
            click_sound = _content.Load<SoundEffect>("Sound Effects/interface1");
            background = _content.Load<Texture2D>("Sprites/deadstate-background");

            button_respawn = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2), font, "Retry");
            button_menu = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2 + 150), font, "Menu");

            MediaPlayer.Stop(); // Stannar musiken
        }

        public override void Update(GameTime gameTime)
        {
            button_respawn.Update();
            if (button_respawn.clicked)
            {
                click_sound.Play();
                Thread.Sleep(200);
                GameState.enemies.Clear();
                GameState.coins.Clear();
                GameState.platforms.Clear();
                GameState.movingPlatforms.Clear();
                GameState.backgroundTiles.Clear();
                savedPlayerLevel = GameState.player.level;
                savedPlayerScore = GameState.player.savedScore;
                _game.ChangeState(new GameState(_game, _graphics, _content));
                _game.IsMouseVisible = false;
            }

            button_menu.Update();
            if (button_menu.clicked)
            {
                click_sound.Play();
                Thread.Sleep(200);
                _game.ChangeState(new MenuState(_game, _graphics, _content));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            button_menu.Draw(spriteBatch);
            button_respawn.Draw(spriteBatch);
            spriteBatch.DrawString(font_larger, "You Died!", new Vector2(1280 / 2 - 150, 150), Color.Red);
            spriteBatch.End();
        }
    }
}
