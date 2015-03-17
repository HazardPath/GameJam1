using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrincessBard
{
	/// <summary>
	/// The idea behind this class is that it will be really easy to make this
	/// with xml, so you can fill out big chunks of terrain at the same time.
	/// </summary>
	public class TileBlock
	{
		/// <summary>
		/// Left edge of the TileGroup.
		/// </summary>
		private int startX;
		/// <summary>
		/// Right edge of the TileGroup.
		/// </summary>
		private int finalX;
		/// <summary>
		/// Bottom edge of the TileGroup.
		/// </summary>
		private int startY;
		/// <summary>
		/// Top edge of the TileGroup.
		/// </summary>
		private int finalY;
		/// <summary>
		/// Type of tiles that make up the TileGroup.
		/// </summary>
		private string tileType;

		/// <summary>
		/// The left edge of the TileGroup.
		/// Lower number.
		/// </summary>
		public int StartX
		{
			get { return startX; }
			set { startX =value; }
		}
		/// <summary>
		/// The right edge of the TileGroup.
		/// Higher number.
		/// </summary>
		public int FinalX
		{
			get { return finalX; }
			set { finalX = value; }
		}
		/// <summary>
		/// The top edge of the TileGroup.
		/// Lower number.
		/// </summary>
		public int StartY
		{
			get { return startY; }
			set { startY = value; }
		}
		/// <summary>
		/// The bottom edge of the TileGroup.
		/// Higher number.
		/// </summary>
		public int FinalY
		{
			get { return finalY; }
			set { finalY = value; }
		}
		/// <summary>
		/// The type of tile with which to fill the TileGroup.
		/// </summary>
		public string TileType
		{
			get { return tileType; }
			set { tileType = value; }
		}

		/// <summary>
		/// Creates a new TileBlock. Parameterless constructor required by
		/// xml serializer.
		/// </summary>
		public TileBlock()
		{
			this.startX = -1;
			this.finalX = -1;
			this.startY = -1;
			this.finalY = -1;
			this.tileType = null;
		}

		/// <summary>
		/// Creates a new TileBlock with the specified coordinates and tile type.
		/// </summary>
		/// <param name="StartX">left edge</param>
		/// <param name="FinalX">right edge</param>
		/// <param name="StartY">top edge</param>
		/// <param name="FinalY">bottom edge</param>
		/// <param name="TileType">type of tiles (like grass, dirt, etc.)</param>
		public TileBlock(int StartX, int FinalX, int StartY, int FinalY, string TileType)
		{
			this.startX = StartX;
			this.finalX = FinalX;
			this.startY = StartY;
			this.finalY = FinalY;
			this.tileType = TileType;
		}
	}
}
