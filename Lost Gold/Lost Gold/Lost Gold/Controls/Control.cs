using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lost_Gold.Input;
using Microsoft.Xna.Framework.Input;

namespace Lost_Gold.Controls
{
    public abstract class Control
    {       
        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Color _color = Color.White;
        public Color Color
        {
            set { _color = value; }
        }

        public enum textSizeOptions { Small, Medium, Large };
        public textSizeOptions textSize = textSizeOptions.Medium;

        protected Vector2 _pos;
        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public int offsetX;
        public int offsetY;

        public virtual void Update(GameTime gameTime) {}

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, GameTime gameTime)
        {
            _pos = centerText(spriteBatch, spriteFont, _name);
            _pos.X += offsetX;
            _pos.Y += offsetY;
            spriteBatch.DrawString(spriteFont, _name, _pos, _color);            
        }

        public virtual Vector2 centerText(SpriteBatch spriteBatch, SpriteFont spriteFont, string text)
        {
            Vector2 textSize = spriteFont.MeasureString(text);
            return new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), (spriteBatch.GraphicsDevice.Viewport.Height / 2) - (textSize.Y / 2));
        }
    }
}
