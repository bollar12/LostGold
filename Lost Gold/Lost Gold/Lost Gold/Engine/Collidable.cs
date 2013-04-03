using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lost_Gold.Engine
{
    public class Collidable
    {
        private Rectangle _rect;

        public Rectangle Rectangle
        {
            get { return _rect; }
        }

        public Collidable(Rectangle rect)
            : this(rect.X, rect.Y, rect.Width, rect.Height) {}

        public Collidable(Point pos, int width, int height)
            : this(pos.X, pos.Y, width, height) {}

        public Collidable(int x, int y, int width, int height)
        {
            _rect = new Rectangle(x, y, width, height);
        }

        public Boolean intersects(Rectangle rect)
        {
            return _rect.Intersects(rect);
        }
    }
}
