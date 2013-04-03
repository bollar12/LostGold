using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Lost_Gold.Sprites;
using Lost_Gold.Input;
using Microsoft.Xna.Framework.Input;

namespace Lost_Gold.Engine
{
    public class Engine : DrawableGameComponent, IDrawSprites, ICollidable
    {
        private Boolean _debug;
        public Boolean isDebugEnabled
        {
            get { return _debug; }
        }

        private SpriteBatch _spriteBatch;
        protected List<DrawData> _toDraw = new List<DrawData>();

        private SpriteFont _default;
        
        private Character _character;
        private Texture2D _characterFrameArt;
        private int _characterSpeed;
        private Camera2d _camera2d;

        protected int _startX;
        protected int _startY;

        protected int _width;
        protected int _height;
        protected int _tileWidth;
        protected int _tileHeight;
        protected XmlReader _level;
        protected List<Layer> _layers = new List<Layer>();
        protected TileSet _tileSet;

        // Collection of collidables for collision detection
        protected List<Collidable> _collidables = new List<Collidable>();

        protected DrawData _winObj;

        public Engine(Game game, XmlReader level)
            : base(game)
        {
            _debug = false;

            _level = level;
            while (_level.Read()) {
                var name = _level.Name;

                switch (_level.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            case "map":
                                _width = int.Parse(_level.GetAttribute("width"));
                                _height = int.Parse(_level.GetAttribute("height"));
                                _tileWidth = int.Parse(_level.GetAttribute("tilewidth"));
                                _tileHeight = int.Parse(_level.GetAttribute("tileheight"));
                                break;
                            case "tileset":
                                _tileSet = new TileSet(_level.ReadSubtree(), game);
                                break;
                            case "layer":
                                _layers.Add(new Layer(_level.ReadSubtree(), this, _tileSet));
                                break;
                            case "objectgroup":
                                switch (_level.GetAttribute("name"))
                                {
                                    case "Start":
                                        _level.ReadToDescendant("object");
                                        if (_level.Name == "object")
                                        {
                                            _startX = int.Parse(_level.GetAttribute("x"));
                                            _startY = int.Parse(_level.GetAttribute("y"));
                                        }
                                        break;
                                    case "Win":
                                        _level.ReadToDescendant("object");
                                        if (_level.Name == "object")
                                        {
                                            int tileId = int.Parse(_level.GetAttribute("gid"));
                                            _winObj = new DrawData(                                                
                                                        _tileSet.getTileById(tileId).Art,
                                                        new Rectangle(
                                                            int.Parse(_level.GetAttribute("x")),
                                                            int.Parse(_level.GetAttribute("y")),
                                                            _tileWidth,
                                                            _tileHeight
                                                        )
                                                    );
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }            
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _character = new Character(Game);
            _character.X = _startX;
            _character.Y = _startY;
            _characterFrameArt = _character.getCurrentFrame(0, new GameTime());
            _characterSpeed = 1;
            _camera2d = new Camera2d();
            _camera2d.Pos = centerCameraOnCharacter();
            _camera2d.Zoom = 2.0f;

            _default = Game.Content.Load<SpriteFont>(@"Fonts\Default");

            foreach (Layer l in _layers)
            {
                foreach (LayerTile lt in l.LayerTiles)
                {
                    AddDrawable(
                        new DrawData(lt.TileSetTile.Art,
                            new Rectangle(
                                lt.X * _tileWidth,
                                lt.Y * _tileHeight,
                                lt.TileSetTile.Art.Width,
                                lt.TileSetTile.Art.Height
                            )
                        )
                    );
                }
            }
            AddDrawable(_winObj);
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

        public void AddCollidable(Collidable collidable)
        {
            if (collidable == null || _collidables.Contains(collidable))
            {
                Console.WriteLine("Collidable null or already added");
                return;
            }
            _collidables.Add(collidable);
        }

        public void RemoveCollidable(Collidable collidable)
        {
            _collidables.Remove(collidable);
        }

        public Boolean Collision(Rectangle rect)
        {
            foreach (Collidable c in _collidables)
            {
                if (c.intersects(rect))
                {
                    //Console.WriteLine("Collision at " + rect.X + "x" + rect.Y + " with (" + c.Rectangle.X + "-" + c.Rectangle.Width + ")x(" + c.Rectangle.Y + "-" + c.Rectangle.Height + ")");
                    return true;
                }
            }
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.KeyDown(Keys.Up))
            {
                if (!Collision(_character.getCollisionRectangle(_character.X, _character.Y - _characterSpeed)))
                {
                    _characterFrameArt = _character.getCurrentFrame(3, gameTime);
                    _character.Y -= _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Down))
            {
                if (!Collision(_character.getCollisionRectangle(_character.X, _character.Y + _characterSpeed)))
                {
                    _characterFrameArt = _character.getCurrentFrame(0, gameTime);
                    _character.Y += _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Left))
            {
                if (!Collision(_character.getCollisionRectangle(_character.X - _characterSpeed, _character.Y)))
                {
                    _characterFrameArt = _character.getCurrentFrame(1, gameTime);
                    _character.X -= _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Right))
            {
                if (!Collision(_character.getCollisionRectangle(_character.X + _characterSpeed, _character.Y)))
                {
                    _characterFrameArt = _character.getCurrentFrame(2, gameTime);
                    _character.X += _characterSpeed;
                }
            }

            _camera2d.Pos = centerCameraOnCharacter();
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camera2d.get_transformation(GraphicsDevice)
            );

            foreach (DrawData d in _toDraw)
            {
                drawElement(d);
            }

            // Draw character
            drawElement(
                new DrawData(_characterFrameArt,
                    new Rectangle(
                        _character.X,
                        _character.Y,
                        _character._frameWidth,
                        _character._frameHeight
                    )
                )
            );
            
            // Draw text
            drawString(gameTime.TotalGameTime.Minutes.ToString() + ":" + gameTime.TotalGameTime.Seconds.ToString());

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void drawElement(DrawData drawable)
        {
            Rectangle worldDest = drawable.Destination;
            worldDest.X -= (int)_camera2d.Pos.X;
            worldDest.Y -= (int)_camera2d.Pos.Y;

            _spriteBatch.Draw(drawable.Art, worldDest, drawable.Source, Color.White);
        }

        protected virtual void drawString(string text)
        {
            _spriteBatch.DrawString(_default, text, Vector2.Zero, Color.Yellow);
        }

        private Vector2 centerCameraOnCharacter()
        {
            return new Vector2(
                (_character.X - ((GraphicsDevice.Viewport.Width / 2)) / _camera2d.Zoom) + _characterFrameArt.Bounds.Width / 2,
                (_character.Y - ((GraphicsDevice.Viewport.Height / 2)) / _camera2d.Zoom) + _characterFrameArt.Bounds.Height / 2
            );
        }
    }
}
