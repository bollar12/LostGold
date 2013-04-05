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
        void Update(GameTime gameTime, Boolean selected);
        void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, GameTime gameTime, Boolean selected);
    }
}
