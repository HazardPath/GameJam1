using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrincessBard.XML_Reading
{
	/// <summary>
	/// A class for the xml reading stuff that is more compact for one offs.
	/// </summary>
	public class SoloBlock
	{
		/// <summary>
		/// X coordinate of the SoloBlock
		/// </summary>
		private int x;
		/// <summary>
		/// Y coordinate of the SoloBlock
		/// </summary>
		private int y;
		/// <summary>
		/// Type of tile
		/// </summary>
		private string tileType;

		/// <summary>
		/// The x coordinate of the SoloBlock.
		/// </summary>
		public int X
		{
			get { return x; }
			set { x = value; }
		}
		/// <summary>
		/// The y coordinate of the SoloBlock.
		/// </summary>
		public int Y
		{
			get { return y; }
			set { y = value; }
		}
		/// <summary>
		/// The type of tile that this SoloBlock is.
		/// </summary>
		public string TileType
		{
			get { return tileType; }
			set { tileType = value; }
		}
	}
}
