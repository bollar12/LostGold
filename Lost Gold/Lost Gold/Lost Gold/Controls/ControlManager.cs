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
        // List of controls based on IControl
        private List<IControl> _controls = new List<IControl>();

        // Selected index for SelectableControls
        private int _selectedIndex;

        // Counter for controls that are of type SelectableControl
        private int _selectableControls;

        // Soundeffect played when SelectableControls get focus
        private SoundEffect _itemSelect;

        // Constructor
        public ControlManager(Game game)
            : base(game) { }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            _selectedIndex = 0;

            base.Initialize();
        }

        /// <summary>
        /// Load sounds
        /// </summary>
        protected override void LoadContent()
        {            
            _itemSelect = Game.Content.Load<SoundEffect>(@"Sounds\UI_Clicks01");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Check input and move selectablecontrol focus up/down - Supports keyboard and gamepad - Assumes gamepad index 0, although there can be 4 (0,1,2,3)
            if (InputManager.KeyReleased(Keys.Up) || InputManager.ButtonPressed(Buttons.DPadUp, 0) || InputManager.ButtonPressed(Buttons.LeftThumbstickUp, 0))
            {
                if (_selectedIndex > 0)
                {
                    _selectedIndex--;
                    playEffect();
                }                    
            }
            else if (InputManager.KeyReleased(Keys.Down) || InputManager.ButtonPressed(Buttons.DPadDown, 0) || InputManager.ButtonPressed(Buttons.LeftThumbstickDown, 0))
            {
                if (_selectedIndex < (_selectableControls - 1))
                {
                    _selectedIndex++;
                    playEffect();
                }
            }

            // Update controls and check if SelectableControls are selected
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
        /// Draw controls
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < _controls.Count(); i++)
            {                                              
                _controls[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Adds control
        /// </summary>
        /// <param name="control"></param>
        public void Add(IControl control)
        {
            control.LoadContent(Game);
            _controls.Add(control);            
            if (control is SelectableControl)
            {
                _selectableControls++;
            }
        }

        /// <summary>
        /// Removes control
        /// </summary>
        /// <param name="control"></param>
        public void Remove(IControl control)
        {
            _controls.Remove(control);
        }

        /// <summary>
        /// Clears list of controls and resets counters
        /// </summary>
        public void Clear()
        {
            _controls.Clear();
            _selectedIndex = 0;
            _selectableControls = 0;
        }


        /// <summary>
        /// Plays soundeffect for item
        /// </summary>
        private void playEffect()
        {
            _itemSelect.Play();
        }

        /// <summary>
        /// Checks if a control is in the list of controls, based on controlId
        /// </summary>
        /// <param name="ControlId"></param>
        /// <returns></returns>
        public Boolean isControl(string ControlId)
        {
            for (int i = 0; i < _controls.Count(); i++)
            {
                if (_controls[i] is Control) {
                    Control c = (Control)_controls[i];
                    if (c.Id == ControlId)
                    {
                        return true;
                    }
                }                
            }
            return false;
        }
    }
}
