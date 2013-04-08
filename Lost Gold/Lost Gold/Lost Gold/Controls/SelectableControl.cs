using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lost_Gold.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Lost_Gold.Controls
{
    /// <summary>
    /// Extension of Control that allows control to be selected
    /// Has an event that actions can be bound to
    /// </summary>
    public class SelectableControl : Control
    {
        // EventHandler for when control is "activated"
        public event EventHandler OnSelect;

        // Customizable color for when control is selected
        public Color OriginalColor;
        public Color SelectColor = Color.Yellow;

        public SelectableControl(string name, TextSizeOptions size)
            : base(name, size)
        {
            OriginalColor = Color;
        }

        /// <summary>
        /// Updates control, triggers event if "activated"
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="selected"></param>
        public virtual void Update(GameTime gameTime, Boolean selected)
        {
            if (selected)
            {
                Color = SelectColor;
            }
            else
            {
                Color = OriginalColor;
            }

            if (selected && (InputManager.KeyReleased(Keys.Enter) || InputManager.ButtonPressed(Buttons.A, 0)))
            {
                if (this.OnSelect != null)
                    this.OnSelect(this, new EventArgs());
            }            
        }
    }
}
