using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lost_Gold.Input;
using Microsoft.Xna.Framework.Input;

namespace Lost_Gold.Controls
{
    /// <summary>
    /// Generic Control
    /// </summary>
    public class Control : IControl
    {       
        // Control id
        public string Id;

        // Control name
        public string Name;

        // Control color
        public Color Color = Color.White;

        // Possible text sizes
        public enum TextSizeOptions
        {
            Small,
            Medium,
            Large
        };

        // Control text size
        public TextSizeOptions TextSize;

        // Control position
        public Vector2 Position;       

        // SpriteBatch and SpriteFonts
        protected SpriteBatch _spriteBatch;
        protected SpriteFont _spriteFontSmall;
        protected SpriteFont _spriteFontMedium;
        protected SpriteFont _spriteFontLarge;

        // Center text in viewport
        public Boolean CenterText = true;
        // Offset values are used to move control away from center if CenterText is true
        public int offsetX;
        public int offsetY;

        public Control(string name, TextSizeOptions size)
        {
            this.Name = name;
            this.TextSize = size;
        }

        public virtual void LoadContent(Game game)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _spriteFontLarge = game.Content.Load<SpriteFont>(@"Fonts\ControlsLarge");
            _spriteFontMedium = game.Content.Load<SpriteFont>(@"Fonts\ControlsMedium");
            _spriteFontSmall = game.Content.Load<SpriteFont>(@"Fonts\ControlsSmall");            
        }

        /// <summary>
        /// Update control
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) {}

        /// <summary>
        /// Draw control to screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
            if (this.CenterText)
            {
                this.Position = getCenterTextVector2();
                this.Position.X += offsetX;
                this.Position.Y += offsetY;
            }
            _spriteBatch.Begin();
            _spriteBatch.DrawString(getFont(), this.Name, this.Position, this.Color);
            _spriteBatch.End();
        }

        /// <summary>
        /// Calculate center position for control text based on viewport size
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        protected Vector2 getCenterTextVector2()
        {
            Vector2 textSize = getFont().MeasureString(this.Name);
            return new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width / 2) - (textSize.X / 2), (_spriteBatch.GraphicsDevice.Viewport.Height / 2) - (textSize.Y / 2));
        }

        protected SpriteFont getFont()
        {
            SpriteFont spriteFont = _spriteFontMedium;
            switch (TextSize)
            {
                case Control.TextSizeOptions.Small:
                    spriteFont = _spriteFontSmall;
                    break;
                case Control.TextSizeOptions.Large:
                    spriteFont = _spriteFontLarge;
                    break;
            }
            return spriteFont;
        }
    }
}
