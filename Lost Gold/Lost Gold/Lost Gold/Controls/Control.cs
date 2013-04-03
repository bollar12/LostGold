using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Controls
{
    public abstract class Control : IControl
    {
        protected string _name;
        protected object _value;
        protected Vector2 _pos;

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, GameTime gameTime)
        {
            spriteBatch.DrawString(spriteFont, _name, _pos, Color.White);
        }
    }
}
