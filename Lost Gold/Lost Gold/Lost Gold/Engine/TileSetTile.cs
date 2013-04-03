using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Engine
{
    public class TileSetTile
    {
        // Tile id
        private int _id;
        // Tile x
        private int _x;
        // Tile y
        private int _y;
        // Tile art
        private Texture2D _art;
        public Texture2D Art {
            get { return _art; }
        }
        // Tile properties
        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        public int id
        {
            get { return _id; }
        }

        public Boolean setProperty(string key, string value)
        {
            if (!_properties.ContainsKey(key))
            {
                _properties.Add(key, value);
                return true;
            }
            return false;
        }

        public String getProperty(string key)
        {
            if (_properties.ContainsKey(key))
            {
                return _properties[key];
            }
            return null;
        }

        public TileSetTile(Texture2D tileArt, int tileId, int x, int y)
        {
            _art = tileArt;
            _id = tileId;
            _x = x;
            _y = y;
        }        
    }
}
