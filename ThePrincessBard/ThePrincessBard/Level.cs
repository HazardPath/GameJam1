#region File Description
//-----------------------------------------------------------------------------
// Level.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.IO;
using ThePrincessBard.Actors;

namespace ThePrincessBard
{
    /// <summary>
    /// A uniform grid of tiles with collections of gems and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    class Level : IDisposable
    {
		//###################################################################//
		//																	 //
		//                       VARIABLES & CONSTANTS                       //
		//																	 //
		//###################################################################//

		/// <summary>
		/// The ContentManager handles loading all the assets for the level.
		/// This is so you can dispose of the ContentManager at the end of the
		/// level, and all the assets will be cleaned up automatically.
		/// </summary>
        ContentManager content;
        /// <summary>
        /// Physical structure of the level.
        /// </summary>
        private Tile[,] tiles;
		/// <summary>
		/// Background layers. Currently unused.
		/// </summary>
        private Texture2D[] layers;
		/// <summary>
		/// The player character. 
		/// </summary>
		private Controllable player;
		/// <summary>
		/// Temporary storage for an actor that's been possessed; a possessed
		/// actor is temporarily removed from the list of possessable actors,
		/// and will need to be added back to the list at a later time.
		/// </summary>
		private Actor addMeLater = null;
		/// <summary>
		/// List of all the gems in the level. Currently usused, but could be
		/// handy for some kind of hidden item or something.
		/// </summary>
        private List<Gem> gems = new List<Gem>();
		/// <summary>
		/// The list of all actors on the level, including those that cannot
		/// be possessed. Though we currently don't have any of those.
		/// </summary>
		private List<Actor> actors = new List<Actor>();
		/// <summary>
		/// List of all actors that can be possessed in the level.
		/// </summary>
		private List<Controllable> controllables = new List<Controllable>();
		/// <summary>
		/// The initial position of the ghost princess within the level.
		/// </summary>
        private Vector2 start;
		/// <summary>
		/// The location in the level that you need to get to in order to win.
		/// </summary>
        private Point exit = InvalidPosition;
		/// <summary>
		/// Indicates whether or not the player has made it to the exit yet.
		/// </summary>
        bool reachedExit;
		/// <summary>
		/// This will eventually be used to store and play a unique sound
		/// effect at the end of each level. Currently unused.
		/// </summary>
        private SoundEffect exitReachedSound;
        /// <summary>
        /// The layer which entities are drawn on top of. Currently unused.
        /// </summary>
		private static readonly int EntityLayer = 2;
		/// <summary>
		/// A dummy location used as the initial value of Level.exit in order
		/// to verify whether Level.exit has been initialized yet.
		/// </summary>
        private static readonly Point InvalidPosition = new Point(-1, -1);
        /// <summary>
        /// A random number generator used to pick a random appearance for
		/// tiles with variants.
        /// </summary>
        private static readonly Random random = new Random();

		//###################################################################//
		//																	 //
		//                         GETTERS & SETTERS                         //
		//																	 //
		//###################################################################//

        /// <summary>
        /// Getter and Setter for player.
        /// </summary>
        public Controllable Player
        {
            get { return player; }
            set { player = value; }
        }
		/// <summary>
		/// Getter and setter for addMeLater.
		/// </summary>
		public Actor AddMeLater
		{
			get { return addMeLater; }
			set { addMeLater = value; }
		}
		/// <summary>
		/// Getter and setter for actors.
		/// </summary>
		public List<Actor> Actors
		{
			get { return actors; }
			set { actors = value; }
		}
		/// <summary>
		/// Getter and setter for controllables.
		/// </summary>
		public List<Controllable> Controllables
		{
			get { return controllables; }
			set { controllables = value; }
		}
		/// <summary>
		/// Getter and Setter for reachedExit.
		/// </summary>
        public bool ReachedExit
        {
            get { return reachedExit; }
        }
		/// <summary>
		/// Getter and setter for content.
		/// </summary>
        public ContentManager Content
        {
            get { return content; }
        }
		/// <summary>
		/// Width of level measured in tiles.
		/// </summary>
		public int Width
		{
			get { return tiles.GetLength(0); }
		}
		/// <summary>
		/// Height of the level measured in tiles.
		/// </summary>
		public int Height
		{
			get { return tiles.GetLength(1); }
		}

        #region Loading

		/// <summary>
		/// WIP: generate a level from xml.
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="fileStream">the xml file</param>
		/// <param name="levelIndex">the level number</param>
		/// <returns>a new Level constructed from an xml file</returns>
		public Level MakeXmlLevel(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
		{
			System.Xml.Serialization.XmlSerializer xmlLevelReader = new System.Xml.Serialization.XmlSerializer(typeof(Level));
			return (Level)xmlLevelReader.Deserialize(fileStream);
		}

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">
        /// A stream containing the tile data.
		/// </param>
		/// <param name="levelIndex">the level number</param>
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
			// Create a new content manager to load content used just by this level.
			content = new ContentManager(serviceProvider, "Content");

			// Loads in the the tiles from 
            LoadTiles(fileStream);

			// TODO: Load background textures properly.
			// Load background layer textures. For now, all levels must
			// use the same backgrounds and only use the left-most part of them.
			//layers = new Texture2D[3];
			//for (int i = 0; i < layers.Length; ++i)
			//{
				// Choose a random segment if each background layer for level variety.
				//int segmentIndex = levelIndex;
				//layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex);
			//}

			// TODO: Load level exit sound here, when it exists.
            // Load sounds.
            //exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");
        }

        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");

            Player.IsActive = true;
        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            // TODO: add code here to load other stuff into the map
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Exit
                case 'x':
                case 'X':
                    return LoadExitTile(x, y);

                // Tree Branch
                case '-':
                    return LoadTile("tree/leaves_oak", TileCollision.Platform);

                // Tree Trunk
                case '|':
                    return LoadTile("tree/log_oak", TileCollision.Climbable);

                // Platform block
                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Platform);

                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Player 1 start point
                case 'p':
                    return LoadStartTile(x, y);
                    
                // Rabbit
                case 'r':
                //Snake
                case 's':
                //Squirrel
                case 'q':
                //Mouse
                case 'm':
                //Kiwi
                case 'k':
                //Ostrich
                case 'o':
                    return LoadActor(tileType, x, y);

                // Impassable block
                case '#':
                    return LoadTile("bricks/bricks", TileCollision.Impassable);

                // Grass
                case 'g':
                    return LoadTile("grass/grass_mid_top", TileCollision.Impassable);

				// Dirt
				case 'd':
					return LoadTile("dirt/dirt_mid_mid", TileCollision.Impassable);

                // slant up
                case '/':
					return LoadTile("grass/grass_slantToUpRight", TileCollision.Impassable);

                // slant down
                case '\\':
					return LoadTile("grass/grass_slantToUpLeft", TileCollision.Impassable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Graphics/tiles/" + name), collision);
        }

        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }

        /// <summary>
        /// Instantiates a player, puts her in the level, and remembers where
		/// to put her when she is resurrected.
		/// </summary>
		/// <param name="x">the x coordinate of the player</param>
		/// <param name="y">the y coordinate of the player</param>
		/// <returns>an empty, passable tile</returns>
        private Tile LoadStartTile(int x, int y)
        {
			if (Player != null)
			{
				throw new NotSupportedException("A level may only have one starting point.");
			}

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Ghost(this, start);

            actors.Add(player);
            controllables.Add(player);

            return new Tile(null, TileCollision.Passable);
        }

		/// <summary>
		/// Instantiates an animal and puts it in the level.
		/// </summary>
		/// <param name="tileType">the type of animal to create</param>
		/// <param name="x">the x coordinate of the animal</param>
		/// <param name="y">the y coordinate of the animal</param>
		/// <returns>an empty, passable tile</returns>
        private Tile LoadActor(char tileType, int x, int y)
        {
            Vector2 here = RectangleExtensions.GetBottomCenter(GetBounds(x, y));

            Actor actor = null;
            switch (tileType)
            {
                case 'r':
                    Rabbit r = new Rabbit(this, here);
                    controllables.Add(r);
                    actor = r;
                    break;
                case 'q':
                    Squiwwel q = new Squiwwel(this, here);
                    controllables.Add(q);
                    actor = q;
                    break;
                case 's':
                    Snake s = new Snake(this, here);
                    controllables.Add(s);
                    actor = s;
                    break;
                case 'm':
                    Rodent m = new Rodent(this, here);
                    controllables.Add(m);
                    actor = m;
                    break;
                case 'o':
                    Ostrich o = new Ostrich(this, here);
                    controllables.Add(o);
                    actor = o;
                    break;
                case 'k':
                    Kiwi k = new Kiwi(this, here);
                    controllables.Add(k);
                    actor = k;
                    break;
            }

            if (actor == null)
                throw new NotSupportedException("Unknown actor type "+tileType);

            actors.Add(actor);

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
			// If the exit has already been set, throw an exception.
			if (exit != InvalidPosition)
			{
				// TODO: We had actually talked about allowing multiple exits; can we change this?
				throw new NotSupportedException("A level may only have one exit.");
			}

            exit = GetBounds(x, y).Center;

            return LoadTile("exit", TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a gem and puts it in the level.
        /// </summary>
		/// <remarks>
		/// Currently unused; leftover from starter kit. May be useful later.
		/// </remarks>
        private Tile LoadGemTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            gems.Add(new Gem(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Unloads the level content. Has the ContentManager do it.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
			if (x < 0 || x >= Width) { return TileCollision.Impassable; }
            // Allow jumping past the level top and falling through the bottom.
			if (y < 0 || y >= Height) { return TileCollision.Passable; }
			// Otherwise, use the collision of the tile at the given location.
            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>
		/// <param name="x">the x coordinate of the tile</param>
		/// <param name="x">the y coordinate of the tile</param>
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation)
        {
            // Pause while the player is dead or level has been won.
			if (!Player.IsAlive || ReachedExit)
            {
                // Still want to perform physics on the player.
                Player.ApplyPhysics(gameTime);
            }
            else
            {
				// Player Update

				if (Player is Kiwi) { // Kiwi-specific Player Update
					((Kiwi)Player).Update(gameTime, keyboardState, gamePadState, orientation); }
				else {
					Player.Update(gameTime, keyboardState, gamePadState, orientation); }

				// Gems Update

                UpdateGems(gameTime);

				// TODO: Why is this not in the actor update function? 
                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

				// Actors Update

                UpdateActors(gameTime, keyboardState, gamePadState, orientation);

                // The player has reached the exit if their bounding rectangle
				// contains the center of the exit tile.
				// TODO: I removed the requirement that the player be on the ground to win.
                if (Player.IsAlive && Player.BoundingRectangle.Contains(exit))
                { OnExitReached(); }
            }
        }

        /// <summary>
        /// Animates each gem and checks to allows the player to collect them.
        /// </summary>
        private void UpdateGems(GameTime gameTime)
        {
            for (int i = 0; i < gems.Count; ++i)
            {
                Gem gem = gems[i];

                gem.Update(gameTime);

                if (gem.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    gems.RemoveAt(i--);
                    OnGemCollected(gem, Player);
                }
            }
        }

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateActors(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation)
        {
            foreach (Actor actor in actors)
            {
                actor.Update(gameTime, keyboardState, gamePadState, orientation);

                // Touching an enemy might kill the player
                if (actor.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    bool isFatal = actor.HitPlayer(player);
					if (isFatal) { OnPlayerKilled(actor); }
                }
            }

            if (addMeLater != null)
            {
                actors.Add(addMeLater);
                addMeLater = null;
            }
        }

        /// <summary>
        /// Called when a gem is collected.
        /// </summary>
        /// <param name="gem">The gem that was collected.</param>
        /// <param name="collectedBy">The player who collected this gem.</param>
        private void OnGemCollected(Gem gem, Controllable collectedBy)
        {
            gem.OnCollected(collectedBy);
        }

        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(Actor killedBy)
        {
            Player.OnKilled(killedBy);
        }

        /// <summary>
        /// Called when the player reaches the level's exit.
		/// We are still using this method.
        /// </summary>
        private void OnExitReached()
        {
            Player.OnReachedExit();
			// TODO: Add exit sound.
            //exitReachedSound.Play();
            reachedExit = true;
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            Player.Reset(start);
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //for (int i = 0; i <= EntityLayer; ++i)
                //spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

            //Get the camera offset for everything
            Vector2 offset = GetCameraOffset(spriteBatch.GraphicsDevice.Viewport);

            DrawTiles(spriteBatch, offset);

            //foreach (Gem gem in gems)
                //gem.Draw(gameTime, spriteBatch);
            
            //Player.Draw(gameTime, spriteBatch);

            foreach (Actor actor in actors)
                actor.Draw(gameTime, spriteBatch, offset);

            //for (int i = EntityLayer + 1; i < layers.Length; ++i)
                //spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
        }

        private Vector2 oldCameraOffset = Vector2.Zero;

        private Vector2 GetCameraOffset(Viewport view)
        {
            Vector2 screenSize = new Vector2(view.Width, view.Height);
            Vector2 playerPos = player.Position;
            Vector2 endOfZeWorld = new Vector2(Tile.Size.X * Width, Tile.Size.Y * Height);

            Vector2 cameraTopLeft = playerPos - (screenSize/2);

            Vector2 cameraBox = new Vector2(view.Width / 4, view.Height / 4);

            if (Math.Abs(cameraTopLeft.X - oldCameraOffset.X) < cameraBox.X)
            {
                cameraTopLeft.X = oldCameraOffset.X;
            }
            else
            {
                if (cameraTopLeft.X < oldCameraOffset.X)
                    cameraTopLeft.X += cameraBox.X;
                else
                    cameraTopLeft.X -= cameraBox.X;
            }

            if (Math.Abs(cameraTopLeft.Y - oldCameraOffset.Y) < cameraBox.Y)
            {
                cameraTopLeft.Y = oldCameraOffset.Y;
            }
            else
            {
                if (cameraTopLeft.Y < oldCameraOffset.Y)
                    cameraTopLeft.Y += cameraBox.Y;
                else
                    cameraTopLeft.Y -= cameraBox.Y;
            }

            if (cameraTopLeft.X < 0) cameraTopLeft.X = 0;
            //if (cameraTopLeft.Y < 0) cameraTopLeft.Y = 0;
            if (cameraTopLeft.X + view.Width > endOfZeWorld.X) cameraTopLeft.X = endOfZeWorld.X - view.Width;
            if (cameraTopLeft.Y + view.Height > endOfZeWorld.Y) cameraTopLeft.Y = endOfZeWorld.Y - view.Height;

            oldCameraOffset = cameraTopLeft;

            return cameraTopLeft;
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch, Vector2 offset)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        position -= offset;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

        #endregion

		/// <summary>
		/// Search for a possessable actor near the player that is not the
		/// player herself.
		/// </summary>
		/// <param name="me">the ghost player character</param>
		/// <returns>the closest possessable actor, if there is one; if there isn't, returns null</returns>
        internal Controllable GetPosessableThing(Controllable me)
        {
			// current closest possessable thing
            Controllable possessable = null;
			// distance to the current closest possessable thing
            float distance = 999999;

            foreach (Controllable cur in controllables)
            {
                if (cur.IsActive) continue;
                if (cur.BoundingRectangle.Intersects(me.BoundingRectangle))
                {
                    float newDistance = RectangleExtensions.GetIntersectionDepth(cur.BoundingRectangle, me.BoundingRectangle).Length();
                    if(newDistance < distance){
                        distance = newDistance;
                        possessable = cur;
                    }
                }
            }

            return possessable;
        }
    }
}
