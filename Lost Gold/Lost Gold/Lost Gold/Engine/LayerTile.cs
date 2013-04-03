using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lost_Gold.Engine
{
    public class LayerTile
    {
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

        private TileSetTile _tileSetTile;
        public TileSetTile TileSetTile
        {
            get { return _tileSetTile; }
        }

        public LayerTile(TileSetTile tileSetTile, int x, int y)
        {
            _tileSetTile = tileSetTile;
            _x = x;
            _y = y;
        }
    }
}
