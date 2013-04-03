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


namespace Lost_Gold.Controls
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ControlManager : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private List<IControl> _controls = new List<IControl>();

        public ControlManager(Game game)
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
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFont = Game.Content.Load<SpriteFont>(@"Fonts\Controls");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (IControl c in this._controls)
            {
                c.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            foreach (IControl c in this._controls)
            {
                c.Draw(_spriteBatch, _spriteFont, gameTime);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void AddControl(IControl control)
        {
            _controls.Add(control);
        }

        public void RemoveControl(IControl control)
        {
            _controls.Remove(control);
        }

        public void Clear()
        {
            _controls.Clear();
        }
    }
}
