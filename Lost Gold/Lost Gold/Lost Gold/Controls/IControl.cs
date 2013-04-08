using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Controls
{
    /// <summary>
    /// Control interface
    /// </summary>
    public interface IControl
    {
        void LoadContent(Game game);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
