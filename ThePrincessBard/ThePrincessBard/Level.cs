using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrincessBard
{
	/// <summary>
	/// Represents one level of our gamejam game, The Princess Bard.
	/// </summary>
	class Level
	{
		/// <summary>
		/// So, the idea is that this is a grid that holds the level data. We'll see if this is ok.
		/// </summary>
		private GameObj[,] levelContent;

		public enum Terrain { grasL, grasM, grasR, dirtL, dirtM, dirtR, platL, platM, platR, platS};

		public Level(int xSize, int ySize) {
			levelContent = new GameObj[xSize, ySize];
		}
	}
}
