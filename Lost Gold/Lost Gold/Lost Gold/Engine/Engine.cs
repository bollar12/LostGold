using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Lost_Gold.Sprites;
using Lost_Gold.Input;

namespace Lost_Gold.Engine
{
    public class Engine : DrawableGameComponent, IDrawSprites
    {
        private SpriteBatch _spriteBatch;
        protected List<DrawData> _toDraw = new List<DrawData>();
        private Point _cameraPosition;

        protected int _width;
        protected int _height;
        protected int _tileWidth;
        protected int _tileHeight;
        protected XmlReader _level;
        protected List<Layer> _layers = new List<Layer>();
        protected TileSet _tileSet;

        public Engine(Game game, XmlReader level)
            : base(game)
        {
            _level = level;
            while (_level.Read()) {
                var name = _level.Name;

                switch (_level.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "tileset":
                                _tileWidth = int.Parse(_level.GetAttribute("tilewidth"));
                                _tileHeight = int.Parse(_level.GetAttribute("tileheight"));                               
                                break;
                            case "image":
                                string[] path = _level.GetAttribute("source").Split('/');
                                path = path[path.Length - 1].Split('.');
                                string image = path[0];
                                _tileSet = new TileSet(Game.Content.Load<Texture2D>(@"TileSets\" + image), int.Parse(_level.GetAttribute("width")), int.Parse(_level.GetAttribute("height")), _tileWidth, _tileHeight);
                                break;
                            case "layer":
                                _layers.Add(
                                    new Layer(
                                        (string)_level.GetAttribute("name"),
                                        int.Parse(_level.GetAttribute("width")),
                                        int.Parse(_level.GetAttribute("height"))
                                    )
                                );
                                break;
                            case "data":
                                //int dataSize = (int.Parse(_level.GetAttribute("width")) * int.Parse(_level.GetAttribute("height")) * 4) + 1024;
                                int dataSize = (30 * 30 * 4) + 1024;
                                var buffer = new byte[dataSize];
                                if (_level.CanReadBinaryContent)
                                {                                   
                                    _level.ReadElementContentAsBase64(buffer, 0, dataSize);                                
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
                                        for (int y = 0; y < 30; y++)
                                        {
                                            for (int x = 0; x < 30; x++)
                                            {
                                                // b represent the tile in the tileset
                                                int b = br.ReadInt32();
                                                if (b > 0)
                                                {
                                                    this.AddDrawable(
                                                        new DrawData(
                                                            _tileSet.getTexture2DAtTile(b),
                                                            new Rectangle(
                                                                x * _tileWidth,
                                                                y * _tileHeight,
                                                                _tileWidth,
                                                                _tileHeight
                                                            )
                                                        )
                                                    );
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case "objectgroup":
                                break;
                        }
                        Console.WriteLine(name);
                        break;
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public void AddDrawable(DrawData drawable)
        {
            if (drawable == null || _toDraw.Contains(drawable))
            {
                Console.WriteLine("Drawable null or already added");
                return;
            }
            _toDraw.Add(drawable);
        }

        public void RemoveDrawable(DrawData toRemove)
        {
            _toDraw.Remove(toRemove);
        }

        public override void Initialize()
        {
            base.Initialize();
            _cameraPosition = new Point(0, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                _cameraPosition.Y -= 32;
            }
            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                _cameraPosition.Y += 32;
            }
            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                _cameraPosition.X -= 32;
            }
            if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                _cameraPosition.X += 32;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach (DrawData d in _toDraw)
            {
                drawElement(d);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void drawElement(DrawData drawable)
        {
            Rectangle worldDestination = drawable.Destination;
            worldDestination.X -= _cameraPosition.X;
            worldDestination.Y -= _cameraPosition.Y;

            _spriteBatch.Draw(drawable.Art, worldDestination, drawable.Source, Color.White);
        }
    }
}
