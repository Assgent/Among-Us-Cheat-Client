using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using HamsterCheese.AmongUsMemory;

namespace AmongUsModClient
{
    public class Player //This has to be a class or stuff breaks
    {
        public static readonly string[] COLOR_NAMES = {
            "Red",
            "Blue",
            "Green",
            "Pink",
            "Orange",
            "Yellow",
            "Black",
            "White",
            "Purple",
            "Brown",
            "Cyan",
            "Light Green"
        };

        public static readonly Color[] COLORS = {
            Colors.Red,
            Colors.Blue,
            Colors.Green,
            Colors.Pink,
            Colors.Orange,
            Colors.Yellow,
            Colors.Black,
            Colors.White,
            Colors.Purple,
            Colors.Brown,
            Colors.Cyan,
            Colors.LightGreen
        };

        //==========

        public byte PlayerID { get; set; }
        public string Name { get; set; }
        public PlayerColor Color { get; set; }
        public bool IsImposter { get; set; }
        public bool IsDead { get; set; }
        public bool InVent { get; set; }
        public Vector2 Position { get; set; }

        public RadioButton CreateUIPoint(Vector2 position)
        {
            return new RadioButton()
            {
                Background = new SolidColorBrush(Color.Color),
                IsChecked = false,
                Margin = new Thickness(position.x * 8.0 + 226.0, position.y * -7.0 + 96.0, 0.0, 0.0)
            };
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Player)
            {
                return PlayerID.Equals((obj as Player).PlayerID);
            }

            return base.Equals(obj);
        }
    }

    public struct PlayerColor
    {
        public Color Color;
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
