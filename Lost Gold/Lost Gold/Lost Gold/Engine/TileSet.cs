using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;

namespace Lost_Gold.Engine
{
    public class TileSet
    {
        // Width of a single tile in tileset
        protected int _tileWidth;
        public int tileWidth
        {
            get { return _tileWidth; }
        }
        // Height of a single tile in tileset
        protected int _tileHeight;
        public int tileHeight
        {
            get { return _tileHeight; }
        }
        // Number of tiles on x axis
        protected int _tilesWide;
        // Number of tiles on y axis
        protected int _tilesHigh;
        // Width of tileset
        protected int _tileSetWidth;
        // Height of tileset
        protected int _tileSetHeight;
        // Tileset texture
        protected Texture2D _tileSet;
        // Collection of tiles
        protected List<TileSetTile> _tiles = new List<TileSetTile>();

        public TileSet(XmlReader xml, Game game)
        {
            xml.Read();
            _tileWidth = int.Parse(xml.GetAttribute("tilewidth"));
            _tileHeight = int.Parse(xml.GetAttribute("tileheight"));

            while (xml.Read())
            {
                switch (xml.Name)
                {
                    case "image":
                        string[] path = xml.GetAttribute("source").Split('/');
                        path = path[path.Length - 1].Split('.');
                        string image = path[0];
                        _tileSet = game.Content.Load<Texture2D>(@"TileSets\" + image);
                        
                        _tileSetWidth = int.Parse(xml.GetAttribute("width"));
                        _tileSetHeight = int.Parse(xml.GetAttribute("height"));
                        _tilesWide  = _tileSetWidth / _tileWidth;
                        _tilesHigh  = _tileSetHeight / _tileHeight;

                        // As we now know the bounds of the tileset we cut it into tiles
                        int i = 1;
                        for (int y = 0; y < _tilesHigh; y++)
                        {
                            for (int x = 0; x < _tilesWide; x++)
                            {
                                _tiles.Add(new TileSetTile(
                                        getTexture2DAtPos(x, y),
                                        i,
                                        x,
                                        y
                                    )
                                );
                                i++;
                            }
                        }
                        break;
                    case "tile":
                        int id = int.Parse(xml.GetAttribute("id"));
                        id += 1;
                        TileSetTile t = getTileById(id);
                        
                        if (t is TileSetTile)
                        {
                            XmlReader properties = xml.ReadSubtree();
                            while (properties.Read())
                            {
                                switch (properties.Name)
                                {
                                    case "property":
                                        t.setProperty(properties.GetAttribute("name"), properties.GetAttribute("value"));
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private Texture2D getTexture2DAtPos(int x, int y)
        {
            Rectangle sourceRectangle = new Rectangle(x*_tileWidth, y*_tileHeight, _tileWidth, _tileHeight);

            Texture2D cropTexture = new Texture2D(_tileSet.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            _tileSet.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }

        public TileSetTile getTileById(int id)
        {
            foreach (TileSetTile t in _tiles)
            {
                if (t.id == id)
                {
                    return t;
                }
            }
            return null;
        }
    }
}
