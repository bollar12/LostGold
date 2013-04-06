using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lost_Gold.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Controls
{
    public abstract class SelectableControl : Control
    {
        public event EventHandler onSelect;

        protected Color _selectColor = Color.Yellow;
        public Color SelectColor
        {
            set { _selectColor = value; }
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
            _pos = centerText(spriteBatch, spriteFont, _name);
            _pos.X += offsetX;
            _pos.Y += offsetY;
            spriteBatch.DrawString(spriteFont, _name, _pos, selected ? _selectColor : _color);
        }
    }
}
