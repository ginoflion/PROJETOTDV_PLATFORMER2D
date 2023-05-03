using _2DPlatformer.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace _2DPlatformer.States
{
    public class MenuState : State
    {
        SpriteFont font, font_larger, font_smallest, font_smaller;
        Texture2D button_texture, background;
        SoundEffect click_sound;

        public static Button button_play;
        public static Button button_exit;
        public static List<Button> buttons;

        public MenuState(Game1 game,GraphicsDevice graphics, ContentManager content)
            : base(game, graphics, content)
        {
            font = _content.Load<SpriteFont>("Fonts/font");
            font_larger = _content.Load<SpriteFont>("Fonts/font_larger");
            font_smaller = _content.Load<SpriteFont>("Fonts/font_smaller");
            font_smallest = _content.Load<SpriteFont>("Fonts/font_smallest");

            button_texture = _content.Load<Texture2D>("Controls/button");
            background = _content.Load<Texture2D>("Sprites/menu-background");

            click_sound = _content.Load<SoundEffect>("Sound Effects/interface1");

            button_play = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2), font, "Play");
            button_exit = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2 + 150), font, "Exit");
        }
        public override void LoadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
            button_play.Update(); 
            if (button_play.clicked) // Om klickar play --> spelet startar
            {
                click_sound.Play();
                Thread.Sleep(200);
                GameState.enemies.Clear();
                GameState.coins.Clear();
                GameState.platforms.Clear();
                GameState.movingPlatforms.Clear();
                GameState.backgroundTiles.Clear(); 

                _game.ChangeState(new GameState(_game, _graphics, _content));

                DeadState.savedPlayerScore = 0;
                DeadState.savedPlayerLevel = 1;
                Enemy.speed = 2f; // Börjanhastighet för fiender

                _game.IsMouseVisible = false;
            }

            button_exit.Update();
            if (button_exit.clicked)
            {
                click_sound.Play();
                Thread.Sleep(200);
                _game.Quit();  // Om klickar exit --> spelet avslutas
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _game.GraphicsDevice.Clear(Color.Green);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            button_play.Draw(spriteBatch);
            button_exit.Draw(spriteBatch);
            spriteBatch.DrawString(font_larger, "2DPlatformer", new Vector2(1280 / 2 - 250, 150), Color.White);
            spriteBatch.DrawString(font_smaller, "Controls:", new Vector2(100, 150), Color.White);
            spriteBatch.DrawString(font_smallest, "Move Left: A / LeftArrow", new Vector2(100 , 185), Color.White);
            spriteBatch.DrawString(font_smallest, "Move Right: D / RightArrow", new Vector2(100, 205), Color.White);
            spriteBatch.DrawString(font_smallest, "Jump: Space", new Vector2(100, 225), Color.White);
            spriteBatch.DrawString(font_smallest, "Climb Ladder: Hold Space", new Vector2(100, 245), Color.White);
            spriteBatch.DrawString(font_smallest, "Exit to menu: Escape", new Vector2(100, 265), Color.White);
            spriteBatch.End();
        }
    }
}
