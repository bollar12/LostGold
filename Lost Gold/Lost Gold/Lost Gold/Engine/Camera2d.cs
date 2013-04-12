using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// Camera2d class
    /// </summary>
    public class Camera2d
    {
        // Zoom factor
        protected float _zoom;
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < 0.1f)
                {
                    _zoom = 0.1f;
                }
            }
        }

        // Camera position
        protected Vector2 _pos;
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        // Camera rotation
        protected float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Move camera
        /// </summary>
        /// <param name="amount"></param>
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }              

        /// <summary>
        /// Constructor
        /// </summary>
        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        /// <summary>
        /// Get camera matrix for spritebatch
        /// Currently only factors in rotation and zoom
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <returns></returns>
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
        }
    }
}
