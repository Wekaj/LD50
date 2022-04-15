using Microsoft.Xna.Framework;
using System;

namespace LD50.Interface {
    public record Element {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public string Label { get; set; } = "";
        public bool IsTextBlock { get; set; }
        public float Margin { get; set; }
        public Action? OnClick { get; set; }
        public Func<bool> IsHighlighted { get; set; } = () => false;
    }
}
