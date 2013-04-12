using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// A tile within a layer
    /// </summary>
    public class LayerTile
    {
        // X and Y position of tile within a layer
        private int _x;
        public int X
        {
            get { return _x; }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
        }

        // This tiles tilesettile
        private TileSetTile _tileSetTile;
        public TileSetTile TileSetTile
        {
            get { return _tileSetTile; }
        }

        // Constructor
        public LayerTile(TileSetTile tileSetTile, int x, int y)
        {
            _tileSetTile = tileSetTile;
            _x = x;
            _y = y;
        }
    }
}
