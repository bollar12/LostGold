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

        /// <summary>
        /// Constructor
        /// Parses the tileset information from the TMX map
        /// </summary>
        /// <param name="xml">Assumes only XML with tileset from TMX map</param>
        /// <param name="game"></param>
        public TileSet(XmlReader xml, Game game)
        {
            // Move ahead
            xml.Read();
            // Extract tileset size
            _tileWidth = int.Parse(xml.GetAttribute("tilewidth"));
            _tileHeight = int.Parse(xml.GetAttribute("tileheight"));

            // Parse children of tileset
            while (xml.Read())
            {
                switch (xml.Name)
                {
                    // Image is the tileset image used for drawing
                    case "image":
                        // Extract tileset name and load it from game content
                        string[] path = xml.GetAttribute("source").Split('/');
                        path = path[path.Length - 1].Split('.');
                        string image = path[0];
                        _tileSet = game.Content.Load<Texture2D>(@"TileSets\" + image);
                        
                        // Set tileset dimensions and calculate number of tiles wide and high
                        _tileSetWidth = int.Parse(xml.GetAttribute("width"));
                        _tileSetHeight = int.Parse(xml.GetAttribute("height"));
                        _tilesWide  = _tileSetWidth / _tileWidth;
                        _tilesHigh  = _tileSetHeight / _tileHeight;

                        // As we now know the bounds of the tileset we cut it into tiles (TileSetTile)
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
                    // Extract tileset tile properties and add them to the TileSetTile
                    case "tile":
                        // Get id - Because id's in tileset tile properties are ahead by one compared to layer tileset tile ids we add 1 so we can use the same method to extract tiles from both
                        int id = int.Parse(xml.GetAttribute("id"));
                        id += 1;
                        // Get TileSetTile by id
                        TileSetTile t = getTileById(id);
                        
                        // Did we find a tile by that id?
                        if (t is TileSetTile)
                        {
                            // Yep, move on and read in the properties of that tile
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

        /// <summary>
        /// Extract texture from tileset based on x amount of tiles wide and y amount of tiles high
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Texture2D getTexture2DAtPos(int x, int y)
        {
            Rectangle sourceRectangle = new Rectangle(x*_tileWidth, y*_tileHeight, _tileWidth, _tileHeight);

            Texture2D cropTexture = new Texture2D(_tileSet.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            _tileSet.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }

        /// <summary>
        /// Returns tile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
