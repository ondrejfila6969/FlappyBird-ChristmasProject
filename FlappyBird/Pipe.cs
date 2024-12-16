using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FlappyBird
{
    public class Pipe
    {
        private List<Rectangle> _topPipes;
        private List<Rectangle> _bottomPipes;
        private GraphicsDeviceManager _graphics;

        public Pipe(List<Rectangle> topPipes, List<Rectangle> bottomPipes, GraphicsDeviceManager graphics)
        {
            _topPipes = topPipes;
            _bottomPipes = bottomPipes;
            _graphics = graphics;
        }

        public void GeneratePipes()
        {
            int gapHeight = 150;
            int minHeight = 100;
            int maxHeight = _graphics.PreferredBackBufferHeight - gapHeight - minHeight;

            Random random = new Random();
            int topHeight = random.Next(minHeight, maxHeight);
            int bottomY = topHeight + gapHeight;

            _topPipes.Add(new Rectangle(800, 0, 60, topHeight));
            _bottomPipes.Add(new Rectangle(800, bottomY, 60, _graphics.PreferredBackBufferHeight - bottomY));
        }

        public void UpdatePipes(ref int score)
        {
            for (int i = 0; i < _topPipes.Count; i++)
            {
                _topPipes[i] = new Rectangle(_topPipes[i].X - 5, _topPipes[i].Y, _topPipes[i].Width, _topPipes[i].Height);
                _bottomPipes[i] = new Rectangle(_bottomPipes[i].X - 5, _bottomPipes[i].Y, _bottomPipes[i].Width, _bottomPipes[i].Height);

                if (_topPipes[i].X + _topPipes[i].Width < 0)
                {
                    _topPipes.RemoveAt(i);
                    _bottomPipes.RemoveAt(i);
                    score++;
                    i--;
                }
            }
        }

        public void DrawPipes(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            foreach (var topPipe in _topPipes)
            {
                DrawCandyCaneStripedPipe(spriteBatch, pixelTexture, topPipe);
            }

            foreach (var bottomPipe in _bottomPipes)
            {
                DrawCandyCaneStripedPipe(spriteBatch, pixelTexture, bottomPipe);
            }
        }

        private void DrawCandyCaneStripedPipe(SpriteBatch spriteBatch, Texture2D pixelTexture, Rectangle pipe)
        {
            int stripeWidth = 10;
            int totalHeight = pipe.Height;
            int totalWidth = pipe.Width;

            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    bool isRed = ((x + y) / stripeWidth) % 2 == 0;
                    spriteBatch.Draw(
                        pixelTexture,
                        new Rectangle(pipe.X + x, pipe.Y + y, 1, 1),
                        isRed ? Color.Red : Color.White
                    );
                }
            }
        }
    }
}
