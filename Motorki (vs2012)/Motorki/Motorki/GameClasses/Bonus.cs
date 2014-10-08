using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Motorki.GameClasses
{
    public enum BonusType
    {
        SpeedUp, TraceWiden, RandomizationBomb
    }

    public class Bonus
    {
        public BonusType Type { get; private set; }
        public Vector2 Position { get; private set; }

        public Bonus(BonusType type, Vector2 position)
        {
            Type = type;
            Position = position;
        }
    }
}
