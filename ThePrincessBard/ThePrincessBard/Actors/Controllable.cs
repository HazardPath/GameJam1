using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ThePrincessBard.Actors
{
    /// <summary>
    /// Anything that can be controlled by the player.
    /// </summary>
    abstract class Controllable : Actor
    {
        public bool IsActive
        {
            get { return isActive; }
            abstract set;
        }
        private bool isActive;

        public Controllable(Level level, Vector2 position)
            : base(level, position)
        {
        }
    }
}
