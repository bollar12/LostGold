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
using Microsoft.Xna.Framework.Audio;
using Lost_Gold.Controls;

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
        
        private Character _character;
        public Character Character
        {
            get { return _character; }
        }
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
        
        protected double _timeInSecondsTotal;
        protected double _timePassed;
        protected double _timeInSecondsLeft;
        public int timeLeft
        {
            get { return (int)_timeInSecondsLeft; }
        }

        protected float _maxCameraX;
        protected float _maxCameraY;

        protected Rectangle _winRect;
        public Rectangle WinArea
        {
            get { return _winRect; }
        }

        protected SoundEffect _ambientSound;
        protected SoundEffectInstance _ambientPlayer;

        protected ControlManager _controlManager;
        protected Control _timer;

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
                                            _winRect = new Rectangle(
                                                int.Parse(_level.GetAttribute("x")),
                                                int.Parse(_level.GetAttribute("y")),
                                                int.Parse(_level.GetAttribute("width")),
                                                int.Parse(_level.GetAttribute("height"))
                                            );
                                        }
                                        break;
                                    case "Timer":
                                        XmlReader properties = _level.ReadSubtree();                                        
                                        while (properties.Read())
                                        {
                                            if (properties.Name == "property" && properties.GetAttribute("name") == "Timer")
                                            {
                                                _timeInSecondsTotal = double.Parse(properties.GetAttribute("value"));
                                            }
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
            _characterSpeed = 2;
            _camera2d = new Camera2d();
            _camera2d.Pos = centerCameraOnCharacter();
            _camera2d.Zoom = 1.5f;

            _ambientSound = Game.Content.Load<SoundEffect>(@"Sounds\AfternoonAmbienceSimple_01");
            _ambientPlayer = _ambientSound.CreateInstance();
            _ambientPlayer.IsLooped = true;

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

            _controlManager = (ControlManager)Game.Services.GetService(typeof(ControlManager));
            _timer = new Control(timeLeftToString(), Control.TextSizeOptions.Large);
            _timer.Id = "timer";
            _timer.CenterText = false;
            _timer.Position = Vector2.Zero;
            _timer.Color = Color.Goldenrod;
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
                    return true;
                }
            }
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Calculate max camera x and y
            _maxCameraX = _width * _tileWidth;
            _maxCameraY = _height * _tileHeight;

            float viewportX = GraphicsDevice.Viewport.Width / 2;
            float viewportY = GraphicsDevice.Viewport.Height / 2;

            _maxCameraX -= viewportX + (viewportX - (viewportX / _camera2d.Zoom));
            _maxCameraY -= viewportY + (viewportY - (viewportY / _camera2d.Zoom));
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle _charRect = _character.Rectangle();
            if (InputManager.KeyDown(Keys.Up) || InputManager.gamePadThumbStickLeftState(0).Y > 0.0f)
            {
                _charRect.Y -= _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(3, gameTime);
                    _character.Y -= _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Down) || InputManager.gamePadThumbStickLeftState(0).Y < 0.0f)
            {
                _charRect.Y += _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(0, gameTime);
                    _character.Y += _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Left) || InputManager.gamePadThumbStickLeftState(0).X < 0.0f)
            {
                _charRect.X -= _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(1, gameTime);
                    _character.X -= _characterSpeed;
                }
            }
            else if (InputManager.KeyDown(Keys.Right) || InputManager.gamePadThumbStickLeftState(0).X > 0.0f)
            {
                _charRect.X += _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(2, gameTime);
                    _character.X += _characterSpeed;
                }
            }            

            _camera2d.Pos = centerCameraOnCharacter();

            _timePassed += gameTime.ElapsedGameTime.Milliseconds;
            _timer.Name = timeLeftToString();

            // Add timer / check if timer is in controls
            if (!_controlManager.isControl("timer"))
            {
                _controlManager.Add(_timer);
            }                
            
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

        /// <summary>
        /// Calculates center position for camera based on character position.
        /// Map bounds are also taken into consideration so that camera doesn't move outside map.
        /// </summary>
        /// <returns>X and Y coordinates for camera</returns>
        private Vector2 centerCameraOnCharacter()
        {
            // Calculate center position based on character position
            float x = (_character.X - ((GraphicsDevice.Viewport.Width / 2) / _camera2d.Zoom)) + _characterFrameArt.Bounds.Width / 2;
            float y = (_character.Y - ((GraphicsDevice.Viewport.Height / 2) / _camera2d.Zoom)) + _characterFrameArt.Bounds.Height / 2;

            // Make sure camera doesn't go outside map bounds
            x = x <= 0 ? 0 : x;
            x = (x > _maxCameraX) ? _maxCameraX : x;

            y = y <= 0 ? 0 : y;
            y = (y > _maxCameraY) ? _maxCameraY : y;

            // Return camera position
            return new Vector2(x, y);
        }

        /// <summary>
        /// Formats time left into a readable string
        /// </summary>
        /// <returns>Time left formatted</returns>
        private string timeLeftToString()
        {
            _timeInSecondsLeft = _timeInSecondsTotal - (_timePassed/1000);
            int minutes = (int)Math.Floor(_timeInSecondsLeft / 60);
            int seconds = (int)_timeInSecondsLeft - (minutes * 60);
            return ((minutes < 10) ? "0" : "") + minutes + ":" + ((seconds < 10) ? "0" : "") + seconds;
        }

        /// <summary>
        /// Resets engine so that map can be played again
        /// </summary>
        public void Reset()
        {            
            _ambientPlayer.Play();            

            _timePassed = 0;
            _character.X = _startX;
            _character.Y = _startY;
        }
    }
}
