using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lost_Gold.Sprites
{
    interface IDrawSprites
    {
        void AddDrawable(DrawData drawable);
        void RemoveDrawable(DrawData drawable);
    }
}
