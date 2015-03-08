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
        // Physical structure of the level.
        private Tile[,] tiles;
        private Texture2D[] layers;
        // The layer which entities are drawn on top of.
        private const int EntityLayer = 2;

        // Entities in the level.
        public Controllable Player
        {
            get { return player; }
            set { player = value; }
        }
        Controllable player;

        public List<Gem> gems = new List<Gem>();
        public List<Actor> actors = new List<Actor>();
        public List<Controllable> controllables = new List<Controllable>();

        // Key locations in the level.        
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Level game state.
        private Random random = new Random();

        public int Score
        {
            get { return score; }
        }
        int score;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;

        private const int PointsPerSecond = 5;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect exitReachedSound;

        #region Loading

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");

            timeRemaining = TimeSpan.FromMinutes(2.0);

            LoadTiles(fileStream);

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            //TODO this right
            //layers = new Texture2D[3];
            //for (int i = 0; i < layers.Length; ++i)
            //{
                // Choose a random segment if each background layer for level variety.
            //    int segmentIndex = levelIndex;
            //    layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex);
            //}

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
                    return LoadTile("dirt/dirt_mid_mid", TileCollision.Impassable);

                // slant up
                case '/':
                    return LoadTile("dirt/dirt_mid_mid", TileCollision.Impassable);

                // slant down
                case '\\':
                    return LoadTile("dirt/dirt_mid_mid", TileCollision.Impassable);

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
        /// Instantiates a player, puts her in the level, and remembers where to put her when she is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Ghost(this, start);

            actors.Add(player);
            controllables.Add(player);

            return new Tile(null, TileCollision.Passable);
        }

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
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("exit", TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a gem and puts it in the level.
        /// </summary>
        private Tile LoadGemTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            gems.Add(new Gem(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Unloads the level content.
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
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
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
            // Pause while the player is dead or time is expired.
            if (!Player.IsAlive || TimeRemaining == TimeSpan.Zero)
            {
                // Still want to perform physics on the player.
                Player.ApplyPhysics(gameTime);
            }
            else if (ReachedExit)
            {
                // Animate the time being converted into points.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score += seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;
                if (Player is Kiwi)
                    ((Kiwi)Player).Update(gameTime, keyboardState, gamePadState, orientation);
                else
                    Player.Update(gameTime, keyboardState, gamePadState, orientation);
                UpdateGems(gameTime);

                // Falling off the bottom of the level kills the player.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

                UpdateActors(gameTime, keyboardState, gamePadState, orientation);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the gems.
                if (Player.IsAlive &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }

            // Clamp the time remaining at zero.
            if (timeRemaining < TimeSpan.Zero)
                timeRemaining = TimeSpan.Zero;
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

        public Actor addMeLater = null;

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
                    if(isFatal)
                        OnPlayerKilled(actor);
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
            score += Gem.PointValue;

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
        /// </summary>
        private void OnExitReached()
        {
            Player.OnReachedExit();
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
            if (cameraTopLeft.Y < 0) cameraTopLeft.Y = 0;
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

        internal Controllable GetPosessableThing(Controllable me)
        {
            Controllable possessable = null;
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
