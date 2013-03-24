using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lost_Gold.Engine
{
    public class TileSet
    {
        protected int _width;
        protected int _height;
        protected int _tileWidth;
        protected int _tileHeight;
        protected int _tilesWide;
        protected int _tilesHigh;

        protected Texture2D _tileSet;

        public TileSet(Texture2D tileSet, int width, int height, int tileWidth, int tileHeight)
        {
            _tileSet    = tileSet;
            _width      = width;
            _height     = height;
            _tileWidth  = tileWidth;
            _tileHeight = tileHeight;
            _tilesWide  = width / tileWidth;
            _tilesHigh  = height / tileHeight;
        }

        public Texture2D getTexture2DAtTile(int tileId)
        {
            int i = 0, x = 0, y = 0;
            for (y = 0; y < _tilesHigh; y++)
            {
                for (x = 0; x < _tilesWide; x++)
                {
                    i++;
                    if (i == tileId)
                    {                        
                        break;
                    }                    
                }
                if (i == tileId)
                {
                    break;
                }
            }
            Console.WriteLine("looking at (" + x + "," + y + ") " + i + " tileid " + tileId);
            Rectangle sourceRectangle = new Rectangle(x*_tileWidth, y*_tileHeight, _tileWidth, _tileHeight);

            Texture2D cropTexture = new Texture2D(_tileSet.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            _tileSet.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }
    }
}
