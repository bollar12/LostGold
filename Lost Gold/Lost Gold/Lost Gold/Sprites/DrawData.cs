using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Sprites
{
    public class DrawData
    {
        public Texture2D Art { get; set; }

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

        public Rectangle Source { get; set; }
        public Color BlendColor { get; set; }

        private Rectangle _destination;
        private Point _position;

        public DrawData(Texture2D art)
            : this(art, art.Bounds) {}

        public DrawData(Texture2D art, Rectangle destination)
        {
            Art = art;
            Destination = destination;
            Source = art.Bounds;
            BlendColor = Color.White;
        }
    }
}
