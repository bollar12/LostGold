using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Engine
{
    public class Character
    {
        public int X;
        public int Y;
        public int _frameWidth;
        public int _frameHeight;
        private Texture2D _art;
        private int _currentFrame;
        private int _maxFrames;
        private int _currentDirection;
        private int _lastFrameUpdate;
        private Game _game;

        public Character(Game game)
        {
            _frameWidth = 32;
            _frameHeight = 48;
            _game = game;
            X = 0;
            Y = 0;
            _art = game.Content.Load<Texture2D>(@"Other\squire_m");

            _currentFrame = 1;
            _maxFrames = 3;
            _currentDirection = 0;
        }

        public Texture2D getCurrentFrame(int direction, GameTime gameTime)
        {
            if (direction != _currentDirection)
            {
                _currentFrame = 0;
                _currentDirection = direction;
            }            
            else
            {
                _lastFrameUpdate += gameTime.ElapsedGameTime.Milliseconds;
                if (_lastFrameUpdate > 150)
                {
                    _currentFrame++;
                    _lastFrameUpdate = 0;
                }                
            }
            if (_currentFrame > _maxFrames)
            {
                _currentFrame = 0;
            }
            Texture2D _frame = getTexture2DAtPos(_currentFrame, _currentDirection);            
            return _frame;
        }

        public Rectangle getCollisionRectangle(int x, int y) {
            Rectangle rect = new Rectangle(x, y, _frameWidth, _frameHeight);
            rect.Y += 20;
            rect.Height -= 25;
            return rect;
        }

        private Texture2D getTexture2DAtPos(int x, int y)
        {
            Rectangle sourceRectangle = new Rectangle(x * _frameWidth, y * _frameHeight, _frameWidth, _frameHeight);

            Texture2D cropTexture = new Texture2D(_art.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            _art.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }
    }
}
