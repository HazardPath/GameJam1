using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ThePrincessBard
{
	/// <summary>
	/// Defines some global constants that we needed.
	/// </summary>
    static class Globals
    {
		static public readonly float MoveStickScale = 1.0f;
		static public readonly float AccelerometerScale = 1.5f;

		static public readonly Buttons JumpButton = Buttons.A;
		static public readonly Keys JumpKey = Keys.Space;

		static public readonly Buttons PossessButton = Buttons.X;
		static public readonly Keys PossessKey = Keys.Z;
    }
}
