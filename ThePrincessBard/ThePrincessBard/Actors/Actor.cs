using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThePrincessBard.Actors
{
    /// <summary>
    /// Anything that can exist in the world as a sentient thing - controllable
    /// or otherwise.
    /// </summary>
    abstract class Actor
    {
        protected float previousBottom;

        // Animations
        protected Animation idleAnimation;
        protected Animation runAnimation;
        protected Animation jumpAnimation;
        protected Animation dieAnimation;
        protected Animation climbUpAnimationL = null;
        protected Animation climbUpAnimationR = null;
        protected Animation climbDownAnimationL = null;
        protected Animation climbDownAnimationR = null;
        protected Animation climbIdleAnimationL = null;
        protected Animation climbIdleAnimationR = null;
        protected SpriteEffects flip = SpriteEffects.None;
        protected AnimationPlayer sprite;

        // Sounds
        protected SoundEffect killedSound = null;
        protected SoundEffect jumpSound = null;

        // Constants for controling horizontal movement
        protected float MoveAcceleration = 13000.0f;
        protected float MaxMoveSpeed = 1750.0f;
        protected float GroundDragFactor = 0.48f;
        protected float AirDragFactor = 0.58f;
        protected int climbReach = 12;

        // Constants for controlling vertical movement
        protected float MaxJumpTime = 0.35f;
        protected float JumpLaunchVelocity = -1500.0f;
        protected float GravityAcceleration = 3400.0f;
        protected float MaxFallSpeed = 550.0f;
        protected float JumpControlPower = 0.14f;

        // Jumping state
        protected bool isJumping;
        protected bool wasJumping;
        protected float jumpTime;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        protected Vector2 movement;

        public Level Level
        {
            get { return level; }
        }
        protected Level level;


        public bool IsAlive
        {
            get { return isAlive; }
        }
        protected bool isAlive;


        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        protected Vector2 position;


        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        protected Vector2 velocity;


        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        protected bool isOnGround;


        /// <summary>
        /// Gets whether or not the player is climbing a climbable surface
        /// </summary>
        public bool IsClimbing
        {
            get { return isClimbing; }
        }
        protected bool isClimbing;
        private bool wasClimbing;

        public bool CanClimb
        {
            get { return CanClimb; }
        }
        protected bool canClimb = false;

        /// <summary>
        /// If the player is climbing a climbable surface, gets whether the surface is on the left or not
        /// </summary>
        public bool ClimbableLeft
        {
            get { return climbableLeft; }
        }
        protected bool climbableLeft;


        protected Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }


        public Actor(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        virtual public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation)
        {
            if (keyboardState.IsKeyDown(Keys.Delete))
            {
                int debug = 1;
            }

            GetInput(keyboardState, gamePadState, orientation, gameTime);

            ApplyPhysics(gameTime);

            if (IsAlive)
            {
                if (IsOnGround)
                {
                    if (Math.Abs(Velocity.X) - 0.02f > 0)
                    {
                        sprite.PlayAnimation(runAnimation);
                    }
                    else
                    {
                        sprite.PlayAnimation(idleAnimation);
                    }
                }
                else if (IsClimbing)
                {
                    if (Velocity.Y - 0.02f > 0)
                    {
                        if (ClimbableLeft)
                            sprite.PlayAnimation(climbUpAnimationL);
                        else
                            sprite.PlayAnimation(climbUpAnimationR);
                    }
                    else if (Velocity.Y - 0.02f < 0)
                    {
                        if (ClimbableLeft)
                            sprite.PlayAnimation(climbDownAnimationL);
                        else
                            sprite.PlayAnimation(climbDownAnimationR);
                    }
                    else
                    {
                        if (ClimbableLeft)
                            sprite.PlayAnimation(climbIdleAnimationL);
                        else
                            sprite.PlayAnimation(climbIdleAnimationR);
                    }
                }
            }

            // Clear input.
            movement = Vector2.Zero;
            wasClimbing = isClimbing;
            isJumping = false;
        }

        abstract public void LoadContent();

        virtual public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        virtual public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            if (!isClimbing)
            {
                if (wasClimbing)
                    velocity.Y = 0;
                else
                {
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                }
            }
            else
            {
                velocity.Y += movement.Y * MoveAcceleration * elapsed;
            }
            velocity.X += movement.X * MoveAcceleration * elapsed;
            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else if (IsClimbing)
                velocity.Y *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        abstract protected void GetInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime);

        protected bool IsAlignedLeftToClimbable()
        {
            //TODO: stuff
            return false;
        }

        protected bool IsAlignedRightToClimbable()
        {
            //TODO: stuff
            return false;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        virtual protected float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    //if (jumpTime == 0.0f)
                        //jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        virtual protected void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY <= absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    if (collision == TileCollision.Climbable)
                                    {
                                        if (!isClimbing && !isJumping)
                                        {
                                            // When walking over a ladder
                                            isOnGround = true;
                                        }
                                    }
                                    else
                                    {
                                        isOnGround = true;
                                        isClimbing = false;
                                        isJumping = false;
                                    }
                                }

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                            else if (collision == TileCollision.Climbable && !isClimbing)
                            {
                                Position = new Vector2(Position.X, Position.Y);

                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        virtual public bool HitPlayer(Controllable player)
        {
            return false; //hits are not fatal by default, but could be
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        virtual public void OnKilled(Actor killedBy)
        {
            
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        virtual public void OnReachedExit()
        {
            
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        virtual public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 offset)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X > 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position-offset, flip);
        }
    }
}
