using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lost_Gold.Controls;


namespace Lost_Gold.GameScreens
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TitleScreen : Microsoft.Xna.Framework.GameComponent
    {
        ControlManager _controlManager;

        public TitleScreen(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            _controlManager = (ControlManager)Game.Services.GetService(typeof(ControlManager));

            Text title = new Text();
            title.Name = "Lost Gold";
            title.textSize = Text.textSizeOptions.Large;
            title.offsetY = -150;
            _controlManager.Add(title);

            Text byLine = new Text();
            byLine.Name = "by Lars Boldt";
            byLine.textSize = Text.textSizeOptions.Small;
            byLine.offsetY = -120;
            _controlManager.Add(byLine);

            Label newGameLbl = new Label();
            newGameLbl.Name = "Start new game";
            newGameLbl.onSelect += new EventHandler(newGameLbl_onSelect);
            _controlManager.Add(newGameLbl);

            Label optionsLbl = new Label();
            optionsLbl.Name = "Options";
            optionsLbl.offsetY = 30;
            _controlManager.Add(optionsLbl);

            Label exitLbl = new Label();
            exitLbl.Name = "Exit";
            exitLbl.offsetY = 60;
            exitLbl.onSelect += new EventHandler(exitLbl_onSelect);
            _controlManager.Add(exitLbl);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        void exitLbl_onSelect(object sender, EventArgs e)
        {
            Game.Exit();
        }

        void newGameLbl_onSelect(object sender, EventArgs e)
        {            
            this.Enabled = false;

            Engine.Engine engine = (Engine.Engine)Game.Services.GetService(typeof(Engine.Engine));
            engine.Enabled = true;
            engine.Visible = true;
            engine.Reset();

            _controlManager.Clear();            
        }
    }
}
