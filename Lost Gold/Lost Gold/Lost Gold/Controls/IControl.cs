using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Controls
{
    public interface IControl
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, GameTime gameTime);
    }
}
