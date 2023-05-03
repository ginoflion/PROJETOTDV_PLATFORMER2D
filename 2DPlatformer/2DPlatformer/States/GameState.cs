using _2DPlatformer.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Threading;

namespace _2DPlatformer.States
{
    public class GameState : State
    {

        // Listor som håller koll på olika objekt i spelet
        public static List<Platform> platforms = new List<Platform>();
        public static List<Enemy> enemies = new List<Enemy>();
        public static List<Coin> coins = new List<Coin>();
        public static List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();
        public static List<BackgroundTile> backgroundTiles = new List<BackgroundTile>();
        public static List<Ladder> ladders = new List<Ladder>();

        // Texturer som används i spelet
        public static Texture2D grass2_texture, dirt_texture, grass_texture, grass3_texture, grass4_texture,
                                spike_texture, spike_right_texture, spike_top_texture,
                                sign_texture, spring_texture, coin_texture, ladder_texture, box_texture,
                                player_texture, player_walking_texture, player_jumping_texture,
                                enemy_walking_texture,
                                cloud_texture, water1_texture, water2_texture, mushroom_texture, plant1_texture, plant2_texture, plant3_texture;

        // Ljud som används i spelet
        public static SoundEffect jump_sound, enemy_death_sound, coin_sound, player_death_sound, win_sound, clock_sound;
        SoundEffectInstance clock_soundInstance;


        Song music; // Musik som spelas i spelet

        SpriteFont font;
        public static string score_str = "0";
        public static string timer_str = "60s";
        public static Vector2 score_pos, timer_pos;

        public static Player player; // Spelarobjektet
        Map map; // Klass som sköter spelets Map-generering
        Camera camera; // Klass som sköter kamerarörelserna

        float timer = 100; // Timer
        bool clockSoundPlayed = false;

        public GameState(Game1 game, GraphicsDevice graphics, ContentManager content)
            : base(game, graphics, content)
        {

        }
        public override void LoadContent()
        {
            // Laddar in alla texturer
            player_walking_texture = _content.Load<Texture2D>("Sprites/walk");
            player_jumping_texture = _content.Load<Texture2D>("Sprites/jump");
            player_texture = _content.Load<Texture2D>("Sprites/player");
            dirt_texture = _content.Load<Texture2D>("Sprites/ground6");
            grass2_texture = _content.Load<Texture2D>("Sprites/ground4");
            grass_texture = _content.Load<Texture2D>("Sprites/ground11");
            grass3_texture = _content.Load<Texture2D>("Sprites/ground10");
            grass4_texture = _content.Load<Texture2D>("Sprites/ground14");

            spike_texture = _content.Load<Texture2D>("Sprites/spike2");
            spike_right_texture = _content.Load<Texture2D>("Sprites/spike-right");
            spike_top_texture = _content.Load<Texture2D>("Sprites/spike-top");

            coin_texture = _content.Load<Texture2D>("Sprites/coin4");
            enemy_walking_texture = _content.Load<Texture2D>("Sprites/zombie-walk");
            sign_texture = _content.Load<Texture2D>("Sprites/pointer2");
            spring_texture = _content.Load<Texture2D>("Sprites/spring2");
            box_texture = _content.Load<Texture2D>("Sprites/box");
            cloud_texture = _content.Load<Texture2D>("Sprites/cloud");
            water1_texture = _content.Load<Texture2D>("Sprites/water1");
            water2_texture = _content.Load<Texture2D>("Sprites/water2");
            mushroom_texture = _content.Load<Texture2D>("Sprites/mushroom");
            ladder_texture = _content.Load<Texture2D>("Sprites/ladder");
            plant1_texture = _content.Load<Texture2D>("Sprites/grass1");
            plant2_texture = _content.Load<Texture2D>("Sprites/grass2");
            plant3_texture = _content.Load<Texture2D>("Sprites/grass3");

            // Laddar in alla ljud
            jump_sound = _content.Load<SoundEffect>("Sound Effects/jump01");
            player_death_sound = _content.Load<SoundEffect>("Sound Effects/death2");
            enemy_death_sound = _content.Load<SoundEffect>("Sound Effects/death1");
            coin_sound = _content.Load<SoundEffect>("Sound Effects/coin01");
            win_sound = _content.Load<SoundEffect>("Sound Effects/win");
            clock_sound = _content.Load<SoundEffect>("Sound Effects/ticking_clock2");
            clock_soundInstance = clock_sound.CreateInstance(); // Krävs för att kunna stoppa ljudet
            music = _content.Load<Song>("Music/love");
            
            MediaPlayer.IsRepeating = true; // Musiken loopas
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(music);

            player = new Player(player_texture, player_walking_texture, player_jumping_texture, new Vector2(46, 489));
            player.level = DeadState.savedPlayerLevel;
            player.savedScore = DeadState.savedPlayerScore;
            player.currentScore = player.savedScore;

            map = new Map();
            Map.Generate();
            camera = new Camera();

            font = _content.Load<SpriteFont>("Fonts/font");
            score_str = player.currentScore.ToString();
        }
        public override void Update(GameTime gameTime)
        {
            timer_str = Math.Round(timer, 0).ToString() + " s";
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                player.Die();
                clock_soundInstance.Stop();
            }
            if (timer < 10 && clockSoundPlayed == false)
            {
                clock_soundInstance.Play();
                clockSoundPlayed = true;
            }

            // Centerar spelaren
            camera.Follow(player);

            // Uppdaterar objekt i spelet
            player.Update(gameTime);
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }
            foreach (Coin coin in coins)
            {
                coin.Update();
            }
            foreach (MovingPlatform platform in movingPlatforms)
            {
                platform.Update();
            }

            if (player.position.X < 625) // När kameran inte är centrerad på spelaren ska score-visaren inte vara baserad på spelarens position
            {
                score_pos = new Vector2(_graphics.Viewport.Width - 100, 100);
                timer_pos = new Vector2(0, 100);
            }
            else if (player.position.X > 3390)
            {
                score_pos = new Vector2(3940, 100);

            }
            else
            {
                score_pos = new Vector2(player.position.X + (_graphics.Viewport.Width / 2) - 85, 100);
                timer_pos = new Vector2(player.position.X - (_graphics.Viewport.Width / 2), 100);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) // Återgår till menyn om man klickar Escape
            {
                MediaPlayer.Stop();
                enemies.Clear();
                coins.Clear();
                platforms.Clear();
                movingPlatforms.Clear();
                backgroundTiles.Clear();
                _game.ChangeState(new MenuState(_game, _graphics, _content));
                _game.IsMouseVisible = true; // Gör muspekaren synlig för menysammanhang
            }
            if (player.win)
            {
                timer = 100;
                Enemy.speed += 1f; // Ökar fiendernas hastighet för varje level
                clockSoundPlayed = false;
                player.win = false;
                player.position.X = 46;
                player.position.Y = 489;
                player.level++;
                player.savedScore = player.currentScore; // Sparar spelarens score då spelaren ska endast ska kunna förlora poängen för den aktuella leveln då han dör
                MediaPlayer.Stop();
                win_sound.Play();
                Thread.Sleep(2500);
                enemies.Clear();
                coins.Clear();
                platforms.Clear();
                movingPlatforms.Clear();
                backgroundTiles.Clear();
                ladders.Clear();
                Map.Generate();

                if (player.level >= 4)
                {
                    _game.ChangeState(new EndState(_game, _graphics, _content));
                    _game.IsMouseVisible = true;
                }
                else
                {
                    MediaPlayer.Play(music);
                }
            }
            if (player.isDead)
            {
                Thread.Sleep(200);
                _game.ChangeState(new DeadState(_game, _graphics, _content));
                _game.IsMouseVisible = true;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: camera.Transform);
            foreach (BackgroundTile backgroundTile in backgroundTiles)
            {
                backgroundTile.Draw(spriteBatch);
            }
            foreach (Ladder ladder in ladders)
            {
                ladder.Draw(spriteBatch);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (Platform platform in platforms)
            {
                platform.Draw(spriteBatch);
            }
            foreach (Coin coin in coins)
            {
                coin.Draw(spriteBatch);
            }
            foreach (MovingPlatform platform in movingPlatforms)
            {
                platform.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);

            spriteBatch.DrawString(font, "Time:", new Vector2(timer_pos.X + 50, timer_pos.Y - 50), Color.Yellow);
            if (timer > 10)
            {
                spriteBatch.DrawString(font, timer_str, new Vector2(timer_pos.X + 50, timer_pos.Y), Color.White);
            }
            else // Röd text om timern är under 10 sekunder
            {
                spriteBatch.DrawString(font, timer_str, new Vector2(timer_pos.X + 50, timer_pos.Y), Color.Red);
            }
            spriteBatch.DrawString(font, "Score:", new Vector2(score_pos.X - 65, score_pos.Y - 50), Color.Yellow);
            spriteBatch.DrawString(font, score_str, score_pos, Color.White);
            spriteBatch.End();
        }
    }
}
