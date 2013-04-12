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
    /// <summary>
    /// Tiled orthogonal 2d engine - mapeditor.org
    /// See readme dir for details
    /// Implements the IDrawSprites and ICollidable interfaces
    /// </summary>
    public class Engine : DrawableGameComponent, IDrawSprites, ICollidable
    {
        // SpriteBatch
        private SpriteBatch _spriteBatch;
        // List of DrawData to draw
        protected List<DrawData> _toDraw = new List<DrawData>();
        
        // Player character
        private Character _character;
        public Character Character
        {
            get { return _character; }
        }
        // Placeholder for current character animation frame texture
        private Texture2D _characterFrameArt;
        // Character speed
        private int _characterSpeed;
        // Camera
        private Camera2d _camera2d;

        // Character start position
        protected int _startX;
        protected int _startY;

        // Map width / height
        protected int _width;
        protected int _height;

        // Tile width / height
        protected int _tileWidth;
        protected int _tileHeight;

        // Xml data - tmx file
        protected XmlReader _level;
        
        // List of layers
        protected List<Layer> _layers = new List<Layer>();

        // TileSet - As we only support one at this point. In a full implementation of the Tiled editor this would have to be a list.
        protected TileSet _tileSet;

        // Collection of collidables for collision detection
        protected List<Collidable> _collidables = new List<Collidable>();
        
        // Timer data. Because of menus, pause and more we need our own timer that only updates when the engine is running.
        // Time set in map.
        protected double _timeInSecondsTotal;
        // How much time has passed, i.e, how long has engine been running (enabled).
        protected double _timePassed;
        // Time left - Time set minus time passed.
        protected double _timeInSecondsLeft;
        public int timeLeft
        {
            get { return (int)_timeInSecondsLeft; }
        }

        // These float values will tell us how far we can move the camera on the x and y axis before we hit the edges of the map. Used to "lock" camera within the map boundaries.
        protected float _maxCameraX;
        protected float _maxCameraY;

        // Win area. This rectangle is checked for collision against player position.
        protected Rectangle _winRect;
        public Rectangle WinArea
        {
            get { return _winRect; }
        }

        // Ambient sound effect
        protected SoundEffect _ambientSound;
        protected SoundEffectInstance _ambientPlayer;

        // Control Manager
        protected ControlManager _controlManager;
        // Control that shows how much time is left
        protected Control _timer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level">TMX level</param>
        public Engine(Game game, XmlReader level)
            : base(game)
        {
            _level = level;
            // Parse XML data
            while (_level.Read()) {
                var name = _level.Name;

                switch (_level.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (name)
                        {
                            // Map - Root level
                            case "map":
                                // Extract map width, height, tilewidth and tileheight
                                _width = int.Parse(_level.GetAttribute("width"));
                                _height = int.Parse(_level.GetAttribute("height"));
                                _tileWidth = int.Parse(_level.GetAttribute("tilewidth"));
                                _tileHeight = int.Parse(_level.GetAttribute("tileheight"));
                                break;
                            // TileSet
                            case "tileset":
                                // Create a new tileset
                                _tileSet = new TileSet(_level.ReadSubtree(), game);
                                break;
                            // Layer
                            case "layer":
                                // Create a new layer
                                _layers.Add(new Layer(_level.ReadSubtree(), this, _tileSet));
                                break;
                            // Objectgroups (Object layers, Start, Win and Timer)
                            case "objectgroup":
                                switch (_level.GetAttribute("name"))
                                {
                                    // Start object layer
                                    case "Start":
                                        _level.ReadToDescendant("object");
                                        if (_level.Name == "object")
                                        {
                                            // Extract start position (x,y)
                                            _startX = int.Parse(_level.GetAttribute("x"));
                                            _startY = int.Parse(_level.GetAttribute("y"));
                                        }
                                        break;
                                    // Win object layer
                                    case "Win":
                                        _level.ReadToDescendant("object");
                                        if (_level.Name == "object")
                                        {
                                            // Create win rectangle based on object size
                                            _winRect = new Rectangle(
                                                int.Parse(_level.GetAttribute("x")),
                                                int.Parse(_level.GetAttribute("y")),
                                                int.Parse(_level.GetAttribute("width")),
                                                int.Parse(_level.GetAttribute("height"))
                                            );
                                        }
                                        break;
                                    // Timer object layer
                                    case "Timer":
                                        XmlReader properties = _level.ReadSubtree();                                        
                                        while (properties.Read())
                                        {
                                            // Read timer properties and extract time value
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

        /// <summary>
        /// Load content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            
            // SpriteBatch
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            
            // Character
            _character = new Character(Game);
            // Set character position
            _character.X = _startX;
            _character.Y = _startY;
            // Get starting animation frame art for character
            _characterFrameArt = _character.getCurrentFrame(0, new GameTime());
            // Set character speed
            _characterSpeed = 2;

            // Camera2d
            _camera2d = new Camera2d();
            // Center camera on player
            _camera2d.Pos = centerCameraOnCharacter();
            // Camera zoom level
            _camera2d.Zoom = 1.5f;

            // Ambient sound - Load effect
            _ambientSound = Game.Content.Load<SoundEffect>(@"Sounds\AfternoonAmbienceSimple_01");
            _ambientPlayer = _ambientSound.CreateInstance();
            // Loop soundeffect
            _ambientPlayer.IsLooped = true;

            // Add each tile in each layer as a drawable
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

            // Get Control Manager
            _controlManager = (ControlManager)Game.Services.GetService(typeof(ControlManager));

            // Create timer control
            _timer = new Control(timeLeftToString(), Control.TextSizeOptions.Large);
            _timer.Id = "timer";
            _timer.CenterText = false;
            _timer.Position = Vector2.Zero;
            _timer.Color = Color.Goldenrod;
        }

        /// <summary>
        /// Add drawable
        /// </summary>
        /// <param name="drawable"></param>
        public void AddDrawable(DrawData drawable)
        {
            if (drawable == null || _toDraw.Contains(drawable))
            {
                Console.WriteLine("Drawable null or already added");
                return;
            }
            _toDraw.Add(drawable);
        }

        /// <summary>
        /// Remove drawable
        /// </summary>
        /// <param name="toRemove"></param>
        public void RemoveDrawable(DrawData toRemove)
        {
            _toDraw.Remove(toRemove);
        }

        /// <summary>
        /// Add collidable
        /// </summary>
        /// <param name="collidable"></param>
        public void AddCollidable(Collidable collidable)
        {
            if (collidable == null || _collidables.Contains(collidable))
            {
                Console.WriteLine("Collidable null or already added");
                return;
            }
            _collidables.Add(collidable);
        }

        /// <summary>
        /// Remove collidable
        /// </summary>
        /// <param name="collidable"></param>
        public void RemoveCollidable(Collidable collidable)
        {
            _collidables.Remove(collidable);
        }

        /// <summary>
        /// Collision - Loops coolidables and check against supplied rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Calculate max camera x and y
            _maxCameraX = _width * _tileWidth;
            _maxCameraY = _height * _tileHeight;

            // Calculate center of viewport
            float viewportX = GraphicsDevice.Viewport.Width / 2;
            float viewportY = GraphicsDevice.Viewport.Height / 2;

            // We need to factor in camera zoom to correctly find the max values for where the camera can be positioned on the map's x and y axis
            _maxCameraX -= viewportX + (viewportX - (viewportX / _camera2d.Zoom));
            _maxCameraY -= viewportY + (viewportY - (viewportY / _camera2d.Zoom));
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Character rectangle
            Rectangle _charRect = _character.Rectangle();

            // Movement - We check for collision based on where character would be IF moved BEFORE we move and cancel movement IF collision is detected
            // The numbers 0, 1, 2, 3 means directions. 3 = Up, 0 = Down, 1 = Left and 2 = Right
            // Move north (Up)
            if (InputManager.KeyDown(Keys.Up) || InputManager.gamePadThumbStickLeftState(0).Y > 0.0f)
            {
                _charRect.Y -= _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(3, gameTime);
                    _character.Y -= _characterSpeed;
                }
            }
            // Move south (Down)
            else if (InputManager.KeyDown(Keys.Down) || InputManager.gamePadThumbStickLeftState(0).Y < 0.0f)
            {
                _charRect.Y += _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(0, gameTime);
                    _character.Y += _characterSpeed;
                }
            }
            // Move west (Left)
            else if (InputManager.KeyDown(Keys.Left) || InputManager.gamePadThumbStickLeftState(0).X < 0.0f)
            {
                _charRect.X -= _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(1, gameTime);
                    _character.X -= _characterSpeed;
                }
            }
            // Move east (Right)
            else if (InputManager.KeyDown(Keys.Right) || InputManager.gamePadThumbStickLeftState(0).X > 0.0f)
            {
                _charRect.X += _characterSpeed;
                if (!Collision(_charRect))
                {
                    _characterFrameArt = _character.getCurrentFrame(2, gameTime);
                    _character.X += _characterSpeed;
                }
            }            

            // Make camera "follow" player by always centering on the player (map boundaries may override center position)
            _camera2d.Pos = centerCameraOnCharacter();

            // Pass time
            _timePassed += gameTime.ElapsedGameTime.Milliseconds;
            // Update timer control with time left represented as a string
            _timer.Name = timeLeftToString();

            // Add timer / check if timer is in controls
            if (!_controlManager.isControl("timer"))
            {
                _controlManager.Add(_timer);
            }                
            
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Call spritebatch with camera modes to support rotation/zoom.
            _spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                _camera2d.get_transformation(GraphicsDevice)
            );

            // Draw whatever is in the drawdata list
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

        /// <summary>
        /// Draw DrawData
        /// </summary>
        /// <param name="drawable"></param>
        protected virtual void drawElement(DrawData drawable)
        {
            // Offset whatever is drawn based on camera position
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
            _timeInSecondsLeft = _timeInSecondsTotal;            
            _character.X = _startX;
            _character.Y = _startY;
        }
    }
}
