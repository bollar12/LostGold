using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// Collidable interface
    /// </summary>
    interface ICollidable
    {
        void AddCollidable(Collidable collidable);
        void RemoveCollidable(Collidable collidable);
        Boolean Collision(Rectangle rect);
    }
}
