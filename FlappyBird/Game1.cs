using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace FlappyBird
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _birdTexture;
        private Texture2D _backgroundTexture;
        private Texture2D _pixelTexture;
        private SpriteFont _scoreFont;
        private Vector2 _birdPosition;
        private float _birdVelocity;
        private List<Rectangle> _topPipes;
        private List<Rectangle> _bottomPipes;
        private float _pipeSpawnTimer;
        private const float PipeSpawnInterval = 1500;
        private const float Gravity = 0.5f;
        private const float FlapStrength = -10f;
        private int _score;
        private int _highScore;
        private bool _gameOver;
        private const int BirdWidth = 50;
        private const int BirdHeight = 50;
        private Pipe _pipeManager;
        private SoundEffect _backgroundMusic;
        private SoundEffectInstance _backgroundMusicInstance;
        private bool _musicStarted = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            _birdPosition = new Vector2(100, 200);
            _birdVelocity = 0;
            _topPipes = new List<Rectangle>();
            _bottomPipes = new List<Rectangle>();
            _pipeSpawnTimer = 0;
            _score = 0;
            _gameOver = false;
            _pipeManager = new Pipe(_topPipes, _bottomPipes, _graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _birdTexture = Content.Load<Texture2D>("bird");
            _backgroundTexture = Content.Load<Texture2D>("background");
            _scoreFont = Content.Load<SpriteFont>("gameFont");

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _backgroundMusic = Content.Load<SoundEffect>("audio");

            if (!_musicStarted)
            {
                _backgroundMusicInstance = _backgroundMusic.CreateInstance();
                _backgroundMusicInstance.IsLooped = true;
                _backgroundMusicInstance.Play();
                _musicStarted = true;
            }
        }

        private bool CheckCollision()
        {
            Rectangle birdRect = new Rectangle((int)_birdPosition.X, (int)_birdPosition.Y, BirdWidth, BirdHeight);

            foreach (var topPipe in _topPipes)
            {
                if (birdRect.Intersects(topPipe))
                    return true;
            }

            foreach (var bottomPipe in _bottomPipes)
            {
                if (birdRect.Intersects(bottomPipe))
                    return true;
            }

            return false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (_score > _highScore)
                    {
                        _highScore = _score;
                    }
                    Initialize();
                }
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _birdVelocity = FlapStrength;
            }

            _birdVelocity += Gravity;
            _birdPosition.Y += _birdVelocity;

            if (_birdPosition.Y < 0 || _birdPosition.Y + BirdHeight > _graphics.PreferredBackBufferHeight)
            {
                _gameOver = true;
            }

            _pipeSpawnTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_pipeSpawnTimer >= PipeSpawnInterval)
            {
                _pipeSpawnTimer = 0;
                _pipeManager.GeneratePipes();
            }

            _pipeManager.UpdatePipes(ref _score);

            if (CheckCollision())
            {
                _gameOver = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            _pipeManager.DrawPipes(_spriteBatch, _pixelTexture);

            _spriteBatch.Draw(_birdTexture, new Rectangle((int)_birdPosition.X, (int)_birdPosition.Y, BirdWidth, BirdHeight), Color.White);

            _spriteBatch.DrawString(_scoreFont, "Score " + _score, new Vector2(10, 10), Color.Green);

            string highScoreText = "HighScore " + _highScore;
            Vector2 highScoreSize = _scoreFont.MeasureString(highScoreText);
            _spriteBatch.DrawString(_scoreFont, highScoreText, new Vector2(_graphics.PreferredBackBufferWidth - highScoreSize.X - 10, 10), Color.Blue);

            if (_gameOver)
            {
                string gameOverText = "You lost press Space to restart";
                Vector2 size = _scoreFont.MeasureString(gameOverText);
                Vector2 position = new Vector2((_graphics.PreferredBackBufferWidth - size.X) / 2, (_graphics.PreferredBackBufferHeight - size.Y) / 2);
                _spriteBatch.DrawString(_scoreFont, gameOverText, position, Color.OrangeRed);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
