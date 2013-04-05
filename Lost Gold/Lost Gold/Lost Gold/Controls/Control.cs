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
    public abstract class Control : IControl
    {
        public event EventHandler onSelect;

        protected string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected object _value;
        protected Vector2 _pos;
        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public virtual void Update(GameTime gameTime, Boolean selected)
        {
            if (InputManager.KeyReleased(Keys.Enter) && selected)
            {
                if (this.onSelect != null)
                    this.onSelect(this, new EventArgs());
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, GameTime gameTime, Boolean selected)
        {
            Vector2 textCentered = centerTextHorizontally(spriteBatch, spriteFont, _name);
            _pos.X = textCentered.X;
            spriteBatch.DrawString(spriteFont, _name, _pos, selected ? Color.Yellow : Color.White);            
        }

        public virtual Vector2 centerTextHorizontally(SpriteBatch spriteBatch, SpriteFont spriteFont, string text)
        {
            Vector2 textSize = spriteFont.MeasureString(text);
            return new Vector2((spriteBatch.GraphicsDevice.Viewport.Width/2) - textSize.X/2, 0);
        }
    }
}
