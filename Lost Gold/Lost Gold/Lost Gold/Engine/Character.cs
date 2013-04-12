using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// Character class
    /// </summary>
    public class Character
    {
        // Character position x,y
        public int X;
        public int Y;

        // Character collision rectangle
        public Rectangle Rectangle()
        {
            Rectangle rect = new Rectangle(X, Y, _frameWidth, _frameHeight);
            // Add some "offsets" to allow character overlapping terrain somewhat
            rect.Y += 30;
            rect.Height -= 35;
            return rect;
        }

        // Size of "tiles" within character art (tileset)
        public int _frameWidth;
        public int _frameHeight;

        // Character tileset
        private Texture2D _art;
        
        // Current animation frame
        private int _currentFrame;
        // Maximum animation frames for each movement direction
        private int _maxFrames;
        // Current direction
        private int _currentDirection;
        // Time last frame was changed (to control how often animation is updated)
        private int _lastFrameUpdate;
        
        // Game object
        private Game _game;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public Character(Game game)
        {
            // Tile size width and height in tileset
            _frameWidth = 32;
            _frameHeight = 48;
            _game = game;

            // Position
            X = 0;
            Y = 0;

            // Load tileset
            _art = game.Content.Load<Texture2D>(@"Other\squire_m");

            // Set starting animation values
            _currentFrame = 1;
            _maxFrames = 3;
            _currentDirection = 0;
        }

        /// <summary>
        /// Extracts a Texture2D from the character tileset based on direction and last update
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="gameTime"></param>
        /// <returns></returns>
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
            return getTexture2DAtPos(_currentFrame, _currentDirection);            
        }        

        /// <summary>
        /// Internal function used by getCurrentFrame to actually extract texture from tileset
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
