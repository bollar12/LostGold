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
    public class Layer
    {
        protected string _name;
        public string Name
        {
            get { return _name; }
        }

        protected int _width;
        protected int _height;

        private List<LayerTile> _layerTiles = new List<LayerTile>();
        public List<LayerTile> LayerTiles
        {
            get { return _layerTiles; }
        }

        public Layer(XmlReader xml, Engine engine, TileSet tileSet)
        {
            xml.Read();
            _name = xml.GetAttribute("name");
            _width = int.Parse(xml.GetAttribute("width"));
            _height = int.Parse(xml.GetAttribute("height"));

            while (xml.Read())
            {
                switch (xml.Name)
                {
                    case "data":
                        //int dataSize = (int.Parse(_level.GetAttribute("width")) * int.Parse(_level.GetAttribute("height")) * 4) + 1024;
                        int dataSize = (_width * _height * 4) + 1024;
                        var buffer = new byte[dataSize];
                        if (xml.CanReadBinaryContent)
                        {
                            xml.ReadElementContentAsBase64(buffer, 0, dataSize);
                            Stream stream = new MemoryStream(buffer, false);

                            /*
                            if (_level.GetAttribute("compression") == "gzip")
                            {
                                stream = new GZipStream(stream, CompressionMode.Decompress, false);
                            }
                            else if (_level.GetAttribute("compression") == "zlib")
                            {*/
                            // Zlib specific - 2 bytes, we skip those before decompressing
                            stream.ReadByte();
                            stream.ReadByte();
                            stream = new DeflateStream(stream, CompressionMode.Decompress);
                            //}

                            using (stream)
                            using (var br = new BinaryReader(stream))
                            {
                                for (int y = 0; y < _height; y++)
                                {
                                    for (int x = 0; x < _width; x++)
                                    {
                                        // tileId represent the tile in the tileset
                                        int tileId = br.ReadInt32();
                                        if (tileId > 0)
                                        {
                                            TileSetTile t = tileSet.getTileById(tileId);
                                            if (t is TileSetTile)
                                            {
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

        public List<LayerTile> getLayerTiles(int tilesWide, int tilesHigh, int offsetX, int offsetY)
        {
            List<LayerTile> list = new List<LayerTile>();
            foreach (LayerTile lt in _layerTiles)
            {
                if (lt.X > offsetX && lt.X < (offsetX + tilesWide))
                {
                    if (lt.Y > offsetY && lt.Y < (offsetY + tilesHigh))
                    {
                        list.Add(lt);
                    }
                }
            }
            return list;
        }
    }
}
