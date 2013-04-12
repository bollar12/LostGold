using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Sprites
{
    /// <summary>
    /// DrawData class, used by 2d engine to draw layers
    /// </summary>
    public class DrawData
    {
        // 2DTexture
        public Texture2D Art { get; set; }

        // Position
        private Point _position;
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _destination.X = _position.X;
                _destination.Y = _position.Y;
            }
        }

        // Destination
        private Rectangle _destination;
        public Rectangle Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                _position.X = _destination.X;
                _position.Y = _destination.Y;
            }
        }

        // Source
        public Rectangle Source { get; set; }
        // BlendColor
        public Color BlendColor { get; set; }               

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="art"></param>
        public DrawData(Texture2D art)
            : this(art, art.Bounds) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="art"></param>
        /// <param name="destination"></param>
        public DrawData(Texture2D art, Rectangle destination)
        {
            Art = art;
            Destination = destination;
            Source = art.Bounds;
            BlendColor = Color.White;
        }
    }
}
