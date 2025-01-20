using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace FlappyBird
{
    public partial class MenuManager
    {
        private GraphicsDeviceManager _graphics;
        private Texture2D _pixelTexture;
        private SpriteFont _font;
        private Texture2D _snowflakeTexture;
        private SoundEffectInstance _musicInstance;

        private Rectangle _sliderBar;
        private Rectangle _sliderKnob;
        private bool _isDragging;

        private Rectangle _continueButton;
        private Rectangle _exitButton;

        private int _selectedIndex = 0;
        private string[] _menuOptions = { "CONTINUE", "EXIT" };

        private KeyboardState _currentKeyState;
        private KeyboardState _previousKeyState;

        private List<Snowflake> _snowflakes = new List<Snowflake>();
        private Random _random = new Random();

        public bool IsMenuActive { get; private set; }
        public float MusicVolume { get; private set; } = 0.5f;

        public MenuManager(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            IsMenuActive = false;
        }

        public void LoadContent(ContentManager content, Texture2D pixelTexture, SpriteFont font, SoundEffectInstance musicInstance, Texture2D snowflakeTexture)
        {
            _pixelTexture = pixelTexture;
            _font = font;
            _musicInstance = musicInstance;
            _snowflakeTexture = snowflakeTexture;

            _sliderBar = new Rectangle(300, 200, 200, 5);
            _sliderKnob = new Rectangle(_sliderBar.X + (int)(MusicVolume * _sliderBar.Width) - 10, _sliderBar.Y - 10, 20, 25);

            _continueButton = new Rectangle(300, 250, 200, 50);
            _exitButton = new Rectangle(300, 320, 200, 50);

            InitializeSnowflakes();
        }

        private void InitializeSnowflakes()
        {
            for (int i = 0; i < 50; i++)
            {
                _snowflakes.Add(new Snowflake
                {
                    Position = new Vector2(_random.Next(0, _graphics.PreferredBackBufferWidth), _random.Next(0, _graphics.PreferredBackBufferHeight)),
                    Speed = _random.Next(1, 4),
                    Size = _random.Next(10, 20)
                });
            }
        }

        public void ToggleMenu()
        {
            IsMenuActive = !IsMenuActive;
        }

        public void Update()
        {
            _currentKeyState = Keyboard.GetState();

            if (IsKeyPressed(Keys.Escape))
            {
                ToggleMenu();
            }

            if (IsMenuActive)
            {
                UpdateSnowflakes();

                MouseState mouse = Mouse.GetState();
                Point mousePosition = new Point(mouse.X, mouse.Y);

                if (mouse.LeftButton == ButtonState.Pressed && _sliderKnob.Contains(mousePosition))
                {
                    _isDragging = true;
                }

                if (mouse.LeftButton == ButtonState.Released)
                {
                    _isDragging = false;
                }

                if (_isDragging)
                {
                    int newKnobX = Math.Clamp(mouse.X, _sliderBar.X, _sliderBar.X + _sliderBar.Width);
                    _sliderKnob.X = newKnobX - _sliderKnob.Width / 2;

                    MusicVolume = (float)(_sliderKnob.X - _sliderBar.X) / _sliderBar.Width;
                    _musicInstance.Volume = MusicVolume;
                }

                if (IsKeyPressed(Keys.Left))
                {
                    MusicVolume = Math.Max(0f, MusicVolume - 0.05f);
                    UpdateSliderKnobPosition();
                }
                if (IsKeyPressed(Keys.Right))
                {
                    MusicVolume = Math.Min(1f, MusicVolume + 0.05f);
                    UpdateSliderKnobPosition();
                }

                if (IsKeyPressed(Keys.Up))
                {
                    _selectedIndex = (_selectedIndex - 1 + _menuOptions.Length) % _menuOptions.Length;
                }
                if (IsKeyPressed(Keys.Down))
                {
                    _selectedIndex = (_selectedIndex + 1) % _menuOptions.Length;
                }
                if (IsKeyPressed(Keys.Enter))
                {
                    if (_selectedIndex == 0)
                    {
                        ToggleMenu();
                    }
                    else if (_selectedIndex == 1)
                    {
                        Environment.Exit(0);
                    }
                }
            }

            _previousKeyState = _currentKeyState;
        }

        private void UpdateSliderKnobPosition()
        {
            _sliderKnob.X = _sliderBar.X + (int)(MusicVolume * _sliderBar.Width) - _sliderKnob.Width / 2;
            _musicInstance.Volume = MusicVolume;
        }

        private void UpdateSnowflakes()
        {
            foreach (var snowflake in _snowflakes)
            {
                snowflake.Position = new Vector2(snowflake.Position.X, snowflake.Position.Y + snowflake.Speed);

                if (snowflake.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    snowflake.Position = new Vector2(_random.Next(0, _graphics.PreferredBackBufferWidth), -snowflake.Size);
                }
            }
        }

        private bool IsKeyPressed(Keys key)
        {
            return _currentKeyState.IsKeyDown(key) && !_previousKeyState.IsKeyDown(key);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsMenuActive)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(200, 100, 400, 400), Color.DarkRed * 0.9f);
                spriteBatch.Draw(_pixelTexture, new Rectangle(210, 110, 380, 380), Color.White);

                foreach (var snowflake in _snowflakes)
                {
                    spriteBatch.Draw(_snowflakeTexture, new Rectangle((int)snowflake.Position.X, (int)snowflake.Position.Y, snowflake.Size, snowflake.Size), Color.White);
                }

                string volumeText = $"Volume {(int)(MusicVolume * 100)}";
                Vector2 volumeSize = _font.MeasureString(volumeText);
                spriteBatch.DrawString(_font, volumeText, new Vector2(400 - volumeSize.X / 2, 150), Color.DarkRed);

                spriteBatch.Draw(_pixelTexture, _sliderBar, Color.LightGray);
                spriteBatch.Draw(_pixelTexture, _sliderKnob, Color.DarkGreen);

                spriteBatch.Draw(_pixelTexture, _continueButton, _selectedIndex == 0 ? Color.Green : Color.White);
                Vector2 continueSize = _font.MeasureString("CONTINUE");
                spriteBatch.DrawString(_font, "CONTINUE", new Vector2(_continueButton.X + (_continueButton.Width - continueSize.X) / 2, _continueButton.Y + (_continueButton.Height - continueSize.Y) / 2), Color.Black);

                spriteBatch.Draw(_pixelTexture, _exitButton, _selectedIndex == 1 ? Color.Red : Color.White);
                Vector2 exitSize = _font.MeasureString("EXIT");
                spriteBatch.DrawString(_font, "EXIT", new Vector2(_exitButton.X + (_exitButton.Width - exitSize.X) / 2, _exitButton.Y + (_exitButton.Height - exitSize.Y) / 2), Color.Black);
            }
        }

        public void DrawOverlay(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.Black * 0.4f);
        }
    }
}
