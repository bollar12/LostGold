using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;

using Lost_Gold.Sprites;

using Microsoft.Xna.Framework;

namespace Lost_Gold.Engine
{
    /// <summary>
    /// TMX layer
    /// </summary>
    public class Layer
    {
        // Layer name, mostly used when debugging
        protected string _name;
        public string Name
        {
            get { return _name; }
        }

        // Layer width / height
        protected int _width;
        protected int _height;

        // List of layertiles
        private List<LayerTile> _layerTiles = new List<LayerTile>();
        public List<LayerTile> LayerTiles
        {
            get { return _layerTiles; }
        }

        /// <summary>
        /// Constructor. Parses the LAYER part of the TMX map file. There can be many layers.
        /// </summary>
        /// <param name="xml">Assumes the LAYER part of the tmx map file</param>
        /// <param name="engine"></param>
        /// <param name="tileSet"></param>
        public Layer(XmlReader xml, Engine engine, TileSet tileSet)
        {
            // Move xml ahead
            xml.Read();
            // Read in layer name
            _name = xml.GetAttribute("name");
            // Read in layer width and height
            _width = int.Parse(xml.GetAttribute("width"));
            _height = int.Parse(xml.GetAttribute("height"));

            // Parse the layer block
            while (xml.Read())
            {
                switch (xml.Name)
                {
                    // We currently only support the data block for tile layers, properties are thus ignored
                    case "data":
                        // We create a byte buffer to read in the base64 encoded data, which may or may not be compressed
                        int dataSize = (_width * _height * 4) + 1024;
                        var buffer = new byte[dataSize];
                        if (xml.CanReadBinaryContent)
                        {
                            // Read base64 content info buffer
                            xml.ReadElementContentAsBase64(buffer, 0, dataSize);
                            // We create a memory stream of the buffer
                            Stream stream = new MemoryStream(buffer, false);

                            // At this point it would be natural to check for gzip, zlib or uncompressed xml data since these are all options in Tiled
                            // However, due to time constraints I've only implemented the default setting, zlib compressed base64 encoded data.                            
                            // The first two bytes are Zlib specific identifiers, we skip those before decompressing
                            stream.ReadByte();
                            stream.ReadByte();
                            stream = new DeflateStream(stream, CompressionMode.Decompress);
                            
                            // At this point we have a decompressed base64 stream which we need to "parse".
                            // For that we use a binary reader to read out the tile ids within the layer.
                            // In pure XML, layer data is just a long list of tile ids which represent the tile within the tileset used.
                            // So we need to loop through the layer width by height, as each tile is listed in this order.
                            using (stream)
                            using (var br = new BinaryReader(stream))
                            {
                                // Loop layer height
                                for (int y = 0; y < _height; y++)
                                {
                                    // Loop layer width
                                    for (int x = 0; x < _width; x++)
                                    {
                                        // tileId represent the tile in the tileset
                                        int tileId = br.ReadInt32();
                                        // Tile id 0 means nothing was "painted" in this tile so we skip that.
                                        if (tileId > 0)
                                        {
                                            // Get the TileSetTile using the id we extracted 
                                            TileSetTile t = tileSet.getTileById(tileId);
                                            if (t is TileSetTile)
                                            {
                                                // At this point we check if this tile has a collidable property, if so, we create a new collidable and add to the engine collidable list.
                                                if (t.getProperty("Collidable") is string && t.getProperty("Collidable").Equals("True"))
                                                {
                                                    engine.AddCollidable(
                                                        new Collidable(
                                                            new Rectangle(
                                                                x * tileSet.tileWidth,
                                                                y * tileSet.tileHeight,
                                                                tileSet.tileWidth,
                                                                tileSet.tileHeight
                                                            )
                                                        )
                                                    );                                             
                                                }
                                                // Add the new layertile to the layer list of tiles
                                                _layerTiles.Add(new LayerTile(t, x, y));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
