using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LD50.Input {
    public class InputBindings {
        private readonly Dictionary<BindingId, Keys> _bindings = new();

        private KeyboardState _previousState;
        private KeyboardState _currentState;

        public void CreateBinding(BindingId binding, Keys key) {
            _bindings[binding] = key;
        }
        
        public void Update() {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }

        public bool IsPressed(BindingId binding) {
            return _currentState.IsKeyDown(_bindings[binding]);
        }

        public bool IsReleased(BindingId binding) {
            return _currentState.IsKeyUp(_bindings[binding]);
        }

        public bool JustPressed(BindingId binding) {
            return _currentState.IsKeyDown(_bindings[binding])
                && _previousState.IsKeyUp(_bindings[binding]);
        }

        public bool JustReleased(BindingId binding) {
            return _currentState.IsKeyUp(_bindings[binding])
                && _previousState.IsKeyDown(_bindings[binding]);
        }
    }
}
