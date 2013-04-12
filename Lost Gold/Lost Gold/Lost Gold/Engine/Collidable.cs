using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// Colliable class
    /// </summary>
    public class Collidable
    {
        // Collidable rectangle
        private Rectangle _rect;
        public Rectangle Rectangle
        {
            get { return _rect; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect"></param>
        public Collidable(Rectangle rect)
            : this(rect.X, rect.Y, rect.Width, rect.Height) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Collidable(Point pos, int width, int height)
            : this(pos.X, pos.Y, width, height) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Collidable(int x, int y, int width, int height)
        {
            _rect = new Rectangle(x, y, width, height);
            _rect.Inflate(-10, 0);
        }

        /// <summary>
        /// Checks if collidable rectangle intersects with provided rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Boolean intersects(Rectangle rect)
        {            
            return _rect.Intersects(rect);
        }
    }
}
