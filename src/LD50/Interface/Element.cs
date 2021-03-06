using LD50.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD50.Interface {
    public record Element {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public string Label { get; set; } = "";
        public Texture2D? Image { get; set; } = null;
        public Vector2 ImageScale { get; set; } = Vector2.One;
        public bool IsTextBlock { get; set; }
        public float Margin { get; set; }
        public Action? OnClick { get; set; }
        public Func<bool> IsHighlighted { get; set; } = () => false;
        public Func<bool> IsVisible { get; set; } = () => true;
        public BindingId? Binding { get; set; }
    }
}
