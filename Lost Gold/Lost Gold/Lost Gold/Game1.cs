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

        TitleScreen titleScreen;
        Engine.Engine engine;

        ControlManager controlManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;
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
            // Initialize engine
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = 0;

            StreamReader stream = System.IO.File.OpenText("Content/Maps/Level.tmx");
            XmlReader reader = XmlReader.Create(stream, settings);
            engine = new Engine.Engine(this, reader);
            Components.Add(engine);
            engine.Visible = false;
            engine.Enabled = false;
            Services.AddService(typeof(Engine.Engine), engine);

            // Add InputManager
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
            // TODO: use this.Content to load your game content here
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

            if (InputManager.KeyReleased(Keys.P))
            {
                if (engine.Enabled)
                {
                    engine.Enabled = false;
                    controlManager.Add(new Control("Game paused", Control.TextSizeOptions.Large));
                    Control helpTxt = new Control("Press \"P\" to unpause", Control.TextSizeOptions.Small);
                    helpTxt.offsetY = 50;
                    controlManager.Add(helpTxt);
                }
                else
                {
                    engine.Enabled = true;
                    controlManager.Clear();
                }
            }

            if (InputManager.KeyReleased(Keys.Escape) && engine.Enabled)
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

        void continueLbl_onSelect(object sender, EventArgs e)
        {
            controlManager.Clear();
            titleScreen.Enabled = true;
            titleScreen.Initialize();
            engine.Visible = false;
        }

        void resume_OnSelect(object sender, EventArgs e)
        {
            engine.Enabled = true;
            controlManager.Clear();
        }
    }
}
