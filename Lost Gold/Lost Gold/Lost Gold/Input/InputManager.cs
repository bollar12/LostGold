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


namespace Lost_Gold.Input
{
    /// <summary>
    /// InputManager. This is a static implementation which means we avoid having to use Services etc.
    /// </summary>
    public class InputManager : GameComponent
    {
        // Keyboard states
        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;

        // GamePad states
        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        // Get keyboard state
        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        // Get last keyboard state
        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        // Get GamePad states
        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }

        // Get last GamePad states
        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game"></param>
        public InputManager(Game game)
            : base(game)
        {
            // Set keyboard state
            keyboardState = Keyboard.GetState();

            // GamePad state array - Set length of array to the number of Enum vars available in PlayerIndex
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {              
                gamePadStates[(int)index] = GamePad.GetState(index);
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Update keyboard and gamepad states.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
            {
                gamePadStates[(int)index] = GamePad.GetState(index);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Key released check
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Key pressed check
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Key down check
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Button released check
        /// </summary>
        /// <param name="button"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
                lastGamePadStates[(int)index].IsButtonDown(button);
        }

        /// <summary>
        /// Button pressed check
        /// </summary>
        /// <param name="button"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
                lastGamePadStates[(int)index].IsButtonUp(button);
        }

        /// <summary>
        /// Button down check
        /// </summary>
        /// <param name="button"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }

        /// <summary>
        /// Get GamePad left thumbstick state
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector2 gamePadThumbStickLeftState(PlayerIndex index)
        {
            return new Vector2(gamePadStates[(int)index].ThumbSticks.Left.X, gamePadStates[(int)index].ThumbSticks.Left.Y);
        }
    }
}