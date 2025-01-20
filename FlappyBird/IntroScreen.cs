using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyBird
{
    public class IntroScreen
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private bool _startGame = false;

        public bool StartGame => _startGame; // Indikátor, zda má hra začít

        public IntroScreen()
        {
        }

        public void LoadContent(Texture2D backgroundTexture, SpriteFont font)
        {
            _backgroundTexture = backgroundTexture;
            _font = font;
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                _startGame = true; // Zaznamená, že hráč chce začít hru
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Vykreslení pozadí
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, 800, 600), Color.White);

            // Vykreslení nadpisu
            string titleText = "FLAPPY BIRD";
            Vector2 titleSize = _font.MeasureString(titleText);
            Vector2 titlePosition = new Vector2((800 - titleSize.X) / 2, 200);
            spriteBatch.DrawString(_font, titleText, titlePosition, Color.Yellow);

            // Vykreslení tlačítka "PLAY"
            string playText = "Press ENTER to PLAY";
            Vector2 playSize = _font.MeasureString(playText);
            Vector2 playPosition = new Vector2((800 - playSize.X) / 2, 300);
            spriteBatch.DrawString(_font, playText, playPosition, Color.White);
        }
    }
}