using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lost_Gold.Sprites
{
    /// <summary>
    /// Interface for drawing sprites
    /// </summary>
    interface IDrawSprites
    {
        void AddDrawable(DrawData drawable);
        void RemoveDrawable(DrawData drawable);
    }
}
