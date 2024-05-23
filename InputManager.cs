namespace Pacman
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class InputManager
    {
        private static InputManager instance;
        public static InputManager Instance {
            get {
                if (instance == null)
                    instance = new InputManager();
                return instance;
            }
        }

        MouseState currentMouseState, prevMouseState;
        KeyboardState currentKeyState, prevKeyState;

        public void Update() {
            prevKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public bool KeyPressed(params Keys[] keys) {
            foreach (Keys key in keys) {
                if (currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                    return true;
            }
            return false;
        }

        public bool KeyReleased(params Keys[] keys) {
            foreach (Keys key in keys) {
                if (currentKeyState.IsKeyUp(key) && prevKeyState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public bool LeftClicked(Rectangle area) {
            if (currentMouseState.LeftButton == ButtonState.Pressed
                && prevMouseState.LeftButton == ButtonState.Released) {
                if (area.Contains(currentMouseState.Position))
                    return true;
            }
            return false;
        }

        public bool RightClicked(Rectangle area) {
            if (currentMouseState.RightButton == ButtonState.Pressed
                && prevMouseState.RightButton == ButtonState.Released) {
                if (area.Contains(currentMouseState.Position))
                    return true;
            }
            return false;
        }
    }
}

