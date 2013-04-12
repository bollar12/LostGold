using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Lost_Gold.Engine;
using Lost_Gold.Input;
using Lost_Gold.GameScreens;
using Lost_Gold.Controls;

namespace Lost_Gold
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Titlescreen
        TitleScreen titleScreen;
        // Tiled TMX 2d Engine
        Engine.Engine engine;
        // Control Manager (menus, text on screen etc)
        ControlManager controlManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Fullscreen based on users current display mode
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Skip XML Dtd processing
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = 0;
            // Read in TMX level
            StreamReader stream = System.IO.File.OpenText("Content/Maps/Level.tmx");
            // Create XmlReader based on level stream
            XmlReader reader = XmlReader.Create(stream, settings);
            // Initialize engine with level xml
            engine = new Engine.Engine(this, reader);
            // Add engine to components
            Components.Add(engine);
            // Disable engine (Titlescreen is startup screen)
            engine.Visible = false;
            engine.Enabled = false;
            // Add engine to services
            Services.AddService(typeof(Engine.Engine), engine);

            // Add InputManager (Keyboard, GamePad)
            Components.Add(new InputManager(this));
            
            // Add titlescreen
            titleScreen = new TitleScreen(this);
            Components.Add(titleScreen);          
  
            // Add ControlManager
            controlManager = new ControlManager(this);
            Components.Add(controlManager);
            Services.AddService(typeof(ControlManager), controlManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Game over, you ran out of time
            if (engine.Enabled && engine.timeLeft <= 0)
            {
                Control loseText = new Control("You are out of time!", Control.TextSizeOptions.Large);
                loseText.Color = Color.Red;
                controlManager.Add(loseText);

                SelectableControl continueLbl = new SelectableControl("Press \"Enter\" or GamePad \"A\" to return to menu", Control.TextSizeOptions.Medium);
                continueLbl.offsetY = 50;
                continueLbl.OnSelect += new EventHandler(continueLbl_onSelect);
                controlManager.Add(continueLbl);
                
                engine.Enabled = false;
            }

            // Game won, you found the gold area
            if (engine.WinArea.Intersects(engine.Character.Rectangle()) && engine.Enabled)
            {
                Control winText = new Control("You found the gold!", Control.TextSizeOptions.Large);
                winText.Color = Color.Gold;                
                controlManager.Add(winText);

                SelectableControl continueLbl = new SelectableControl("Press \"Enter\" or GamePad \"A\" to return to menu", Control.TextSizeOptions.Medium);             
                continueLbl.offsetY = 50;
                continueLbl.OnSelect +=new EventHandler(continueLbl_onSelect);
                controlManager.Add(continueLbl);

                engine.Enabled = false;
            }

            // Game pause
            if (InputManager.KeyReleased(Keys.P) || InputManager.ButtonPressed(Buttons.Back, 0))
            {
                // If game is running, pause it
                if (engine.Enabled)
                {
                    engine.Enabled = false;
                    controlManager.Add(new Control("Game paused", Control.TextSizeOptions.Large));
                    Control helpTxt = new Control("Press \"P\" or GamePad \"Back\" to unpause", Control.TextSizeOptions.Small);
                    helpTxt.offsetY = 50;
                    controlManager.Add(helpTxt);
                }
                // Game is paused, start it
                else
                {
                    engine.Enabled = true;
                    controlManager.Clear();
                }
            }

            // Bring up Resume/exit menu during gameplay
            if ((InputManager.KeyReleased(Keys.Escape) || InputManager.ButtonPressed(Buttons.Start, 0)) && engine.Enabled)
            {
                engine.Enabled = false;

                Control menuTxt = new Control("Game menu", Control.TextSizeOptions.Large);
                menuTxt.offsetY = -40;
                controlManager.Add(menuTxt);
                
                SelectableControl resume = new SelectableControl("Resume", Control.TextSizeOptions.Medium);
                resume.OnSelect += new EventHandler(resume_OnSelect);
                controlManager.Add(resume);

                SelectableControl exit = new SelectableControl("Exit", Control.TextSizeOptions.Medium);
                exit.offsetY = 20;
                exit.OnSelect += new EventHandler(continueLbl_onSelect);
                controlManager.Add(exit);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Exit game back to titlescreen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void continueLbl_onSelect(object sender, EventArgs e)
        {
            controlManager.Clear();
            titleScreen.Enabled = true;
            titleScreen.Initialize();
            engine.Visible = false;
        }

        /// <summary>
        /// Resume game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void resume_OnSelect(object sender, EventArgs e)
        {
            engine.Enabled = true;
            controlManager.Clear();
        }
    }
}
