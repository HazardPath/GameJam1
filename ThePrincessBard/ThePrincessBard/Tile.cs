#region File Description
//-----------------------------------------------------------------------------
// Tile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ThePrincessBard
{
    /// <summary>
    /// Controls the collision detection and response behavior of a tile.
    /// </summary>
    enum TileCollision
    {
        /// <summary>
        /// A passable tile is one which does not hinder player motion at all.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// An impassable tile is one which does not allow the player to move through
        /// it at all. It is completely solid.
        /// </summary>
        Impassable = 1,

        /// <summary>
        /// A platform tile is one which behaves like a passable tile except when the
        /// player is above it. A player can jump up through a platform as well as move
        /// past it to the left and right, but can not fall down through the top of it.
        /// </summary>
        Platform = 2,

        /// <summary>
        /// A Climbable tile is one which behaves like a passable tile except when the
        /// player is adjacent to the left or right and climbs up or down.
        /// </summary>
        Climbable = 3,
    }

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    struct Tile
    {
		/// <summary>
		/// The texture that this tile will display. This variable is public.
		/// </summary>
        public Texture2D Texture;
		/// <summary>
		/// Indicates whether this tile acts like air, an impassable obstacle,
		/// a platform that can be passed through in one direction, or a
		/// climbable surface. This variable is public.
		/// </summary>
        public TileCollision Collision;

		/// <summary>
		/// The width of a tile in pixels. Hardcoded to 32.
		/// </summary>
        public const int Width = 32;
		/// <summary>
		/// The height of a tile in pixels. Hardcoded to 32.
		/// </summary>
        public const int Height = 32;
		/// <summary>
		/// The center of a tile, along both x and y axis, measured in pixels.
		/// Width is hardcoded, so this will always evaluate to 16.
		/// </summary>
        public const int Center = Width / 2;

		/// <summary>
		/// A vector containing the width and height of a tile. Since
		/// <see cref="Width"/> and <see cref="Height"/> are hardcoded, this
		/// will always evaluate to (32, 32).
		/// </summary>
        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
		/// <param name="collision">indicates if this tile is solid</param>
		/// <param name="texture">the art for this tile</param>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
