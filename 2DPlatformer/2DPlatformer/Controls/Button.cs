using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DPlatformer.Controls
{
    public class Button
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        SpriteFont font;
        MouseState lastMouseState, mouseState;
        public bool clicked = false;
        public string text;

        Color background_color = Color.White;
        Color text_color = Color.Black;

        public Button(Texture2D newTexture, Vector2 newPosition, SpriteFont newFont, string newText)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            font = newFont;
            text = newText;
        }

        public void Update()
        {
            mouseState = Mouse.GetState();
            Rectangle cursor = new Rectangle(mouseState.X, mouseState.Y, 10, 10); // Rektangel för muspekaren
            if (cursor.Intersects(rectangle))
            {
                background_color = Color.DarkGray;
                text_color = Color.White;
                if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) // Om man klickar
                {
                    clicked = true; // När clicked är true hanteras händelser i klassen där knappen finns
                }
            }
            else
            {
                background_color = Color.White;
                text_color = Color.Black;
            }
            lastMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, background_color);
            spriteBatch.DrawString(font, text, new Vector2(position.X + 100, position.Y + 15), text_color);
        }
    }
}
