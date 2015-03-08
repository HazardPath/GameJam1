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
        // Animations
        protected Animation idleAnimation;
        protected Animation runAnimation;
        protected Animation jumpAnimation;
        protected Animation dieAnimation;
        protected Animation climbUpAnimation;
        protected Animation climbDownAnimation;
        protected SpriteEffects flip = SpriteEffects.None;
        protected AnimationPlayer sprite;

        // Sounds
        protected SoundEffect killedSound;
        protected SoundEffect jumpSound;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f; 



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
        bool isClimbing;

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

        private void Reset(Vector2 position)
        {
            throw new NotImplementedException();
        }

        private void LoadContent()
        {
            throw new NotImplementedException();
        }
    }
}
