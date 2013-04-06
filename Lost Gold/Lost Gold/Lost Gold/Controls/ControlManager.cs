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
using Lost_Gold.Input;


namespace Lost_Gold.Controls
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ControlManager : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFontSmall;
        private SpriteFont _spriteFontMedium;
        private SpriteFont _spriteFontLarge;
        private List<Control> _controls = new List<Control>();
        private int _selectedIndex;
        private int _selectableControls;

        private SoundEffect _itemSelect;

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
            _selectedIndex = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _spriteFontLarge = Game.Content.Load<SpriteFont>(@"Fonts\ControlsLarge");
            _spriteFontMedium = Game.Content.Load<SpriteFont>(@"Fonts\ControlsMedium");
            _spriteFontSmall = Game.Content.Load<SpriteFont>(@"Fonts\ControlsSmall");

            _itemSelect = Game.Content.Load<SoundEffect>(@"Sounds\UI_Clicks01");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {            
            if (InputManager.KeyReleased(Keys.Up))
            {
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    playEffect();
                }                    
            }
            else if (InputManager.KeyReleased(Keys.Down))
            {
                if (_selectedIndex < (_selectableControls - 1))
                {
                    _selectedIndex++;
                    playEffect();
                }
            }

            int x = 0;
            for (int i = 0; i < _controls.Count(); i++ )
            {
                if (_controls[i] is SelectableControl)
                {
                    SelectableControl c = (SelectableControl)_controls[i];
                    c.Update(gameTime, x == _selectedIndex ? true : false);
                    x++;
                }
                else
                {
                    _controls[i].Update(gameTime);
                }                
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
            int x = 0;
            for (int i = 0; i < _controls.Count(); i++)
            {
                SpriteFont _spriteFont = _spriteFontMedium;
                switch (_controls[i].textSize)
                {
                    case Control.textSizeOptions.Small:
                        _spriteFont = _spriteFontSmall;
                        break;
                    case Control.textSizeOptions.Medium:
                        _spriteFont = _spriteFontMedium;
                        break;
                    case Control.textSizeOptions.Large:
                        _spriteFont = _spriteFontLarge;
                        break;
                }
                if (_controls[i] is SelectableControl)
                {
                    SelectableControl c = (SelectableControl)_controls[i];
                    c.Draw(_spriteBatch, _spriteFont, gameTime, x == _selectedIndex ? true : false);
                    x++;
                }
                else
                {
                    _controls[i].Draw(_spriteBatch, _spriteFont, gameTime);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Add(Control control)
        {
            _controls.Add(control);
            if (control is SelectableControl)
            {
                _selectableControls++;
            }
        }

        public void Remove(Control control)
        {
            _controls.Remove(control);
        }

        public void Clear()
        {
            _controls.Clear();
            _selectedIndex = 0;
            _selectableControls = 0;
        }

        private void playEffect()
        {
            _itemSelect.Play();
        }
    }
}
