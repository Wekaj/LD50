using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LD50.Input {
    public class InputBindings {
        private readonly Dictionary<BindingId, Keys> _keyboardBindings = new();
        private readonly Dictionary<BindingId, MouseButton> _mouseBindings = new();

        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;
        
        private MouseState _previousMouseState;
        private MouseState _currentMouseState;

        public void CreateBinding(BindingId binding, Keys key) {
            _keyboardBindings[binding] = key;
        }

        public void CreateBinding(BindingId binding, MouseButton button) {
            _mouseBindings[binding] = button;
        }
        
        public void Update() {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        public bool IsPressed(BindingId binding) {
            return IsBindingDownCurrently(binding);
        }

        public bool IsReleased(BindingId binding) {
            return !IsBindingDownCurrently(binding);
        }

        public bool JustPressed(BindingId binding) {
            return IsBindingDownCurrently(binding)
                && !WasBindingDownPreviously(binding);
        }

        public bool JustReleased(BindingId binding) {
            return !IsBindingDownCurrently(binding)
                && WasBindingDownPreviously(binding);
        }

        private bool IsBindingDownCurrently(BindingId binding) {
            return _keyboardBindings.TryGetValue(binding, out Keys key) && _currentKeyboardState.IsKeyDown(key)
                || _mouseBindings.TryGetValue(binding, out MouseButton button) && IsButtonDown(_currentMouseState, button);
        }

        private bool WasBindingDownPreviously(BindingId binding) {
            return _keyboardBindings.TryGetValue(binding, out Keys key) && _previousKeyboardState.IsKeyDown(key)
                || _mouseBindings.TryGetValue(binding, out MouseButton button) && IsButtonDown(_previousMouseState, button);
        }

        private bool IsButtonDown(MouseState mouseState, MouseButton button) {
            ButtonState state = button switch {
                MouseButton.Left => mouseState.LeftButton,
                MouseButton.Right => mouseState.RightButton,

                _ => ButtonState.Released,
            };

            return state == ButtonState.Pressed;
        }
    }
}
