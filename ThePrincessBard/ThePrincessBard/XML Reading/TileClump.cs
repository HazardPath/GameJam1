using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrincessBard
{
	/// <summary>
	/// Unlike TileBlock, which is a nice rectangle of tiles, TileClump is a
	/// group of disorganized, odds-and-ends tiles.
	/// </summary>
	public class TileClump
	{
		/// <summary>
		/// List of coordinates of the tiles in the TileClump.
		/// </summary>
		private List<List<int>> coords;
		/// <summary>
		/// Type of tiles that make up the TileGroup.
		/// </summary>
		private string tileType;

		/// <summary>
		/// The list of coordinates of the tiles in the TileClump.
		/// </summary>
		public List<List<int>> Coords
		{
			get { return coords; }
			set { coords = value; }
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
		/// Creates a new TileClump. Parameterless constructor is required
		/// by the xml serializer.
		/// </summary>
		public TileClump()
		{
			this.coords = new List<List<int>>();
			this.tileType = null;
		}

		/// <summary>
		/// Creates a new TileClump with the specified tile type and no data.
		/// </summary>
		/// <param name="tileType">the type of tile (like grass, dirt, etc.)</param>
		public TileClump(string tileType)
		{
			this.coords = new List<List<int>>();
			this.tileType = tileType;
		}

		/// <summary>
		/// Add a set of coordinates to the list of coordinates.
		/// </summary>
		/// <param name="x">the x coordinate</param>
		/// <param name="y">the y coordinate</param>
		public void Add(int x, int y)
		{
			List<int> temp = new List<int>();
			temp.Add(x);
			temp.Add(y);
			this.coords.Add(temp);
		}
	}
}
