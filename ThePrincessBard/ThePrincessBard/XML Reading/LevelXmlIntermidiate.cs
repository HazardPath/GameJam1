﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePrincessBard
{
	/// <summary>
	/// An intermediate class for constructing a level from an xml file.
	/// </summary>
	public class LevelXmlIntermidiate
	{
		/// <summary>
		/// Number of columns of tiles in the level.
		/// </summary>
		private int width;
		/// <summary>
		/// Number of rows of tiles in the level.
		/// </summary>
		private int height;
		/// <summary>
		/// List of blocks of tiles.
		/// </summary>
		private List<TileBlock> tileBlocks;
		/// <summary>
		/// List of clumps of tiles.
		/// </summary>
		private List<TileClump> tileClumps;

		/// <summary>
		/// The number of columns of tiles in the level.
		/// </summary>
		public int Width
		{
			get { return width; }
			set { width = value; }
		}
		/// <summary>
		/// The number of rows of tiles in the level.
		/// </summary>
		public int Height
		{
			get { return height; }
			set { height = value; }
		}
		/// <summary>
		/// The list of TileBlocks, rectangular groups of tiles.
		/// </summary>
		public List<TileBlock> TileBlocks
		{
			get { return tileBlocks; }
			set { tileBlocks = value; }
		}
		/// <summary>
		/// The list of TileClumps, shapeless groups of tiles.
		/// </summary>
		public List<TileClump> TileClumps
		{
			get { return tileClumps; }
			set { tileClumps = value; }
		}

		/// <summary>
		/// Constructs a new LevelXmlIntermidiate, with no data.
		/// </summary>
		public LevelXmlIntermidiate()
		{
			tileBlocks = new List<TileBlock>();
			tileClumps = new List<TileClump>();
		}

		/// <summary>
		/// Adds a TileBlock to the list of TileBlocks.
		/// </summary>
		/// <param name="tb">the TileBlock to add</param>
		public void Add(TileBlock tb)
		{
			this.tileBlocks.Add(tb);
		}

		/// <summary>
		/// Adds a TileClump to the list of TileClumps.
		/// </summary>
		/// <param name="tc">the TileClump to add</param>
		public void Add(TileClump tc)
		{
			this.tileClumps.Add(tc);
		}
	}
}
