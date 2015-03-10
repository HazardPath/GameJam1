#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThePrincessBard.Actors
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Ghost : Controllable
    {
        /// <summary>
        /// Constructors a new player. Noteworthy: she is two tiles tall;
		/// coordinates are for her lower half.
        /// </summary>
		/// <param name="level">the level on which to spawn her</param>
		/// <param name="position">the x and y coordinates of her lower half in the level</param>
        public Ghost(Level level, Vector2 position)
            : base(level, position)
        {
            MoveAcceleration = 6000.0f;
            MaxMoveSpeed = 400.0f;
            isActive = true;
        }

		/// <summary>
		/// Handles cleanup when the player possesses another creature.
		/// </summary>
		/// <param name="value">'true' to make the ghost active; 'false' to make her inactive</param>
        protected override void SetIsActive(bool value)
        {
            if (!isActive && value)
            { // wasn't active, but is now
                // I don't care, I'll be set active by someone else
            }
			else if (isActive && !value)
			{ // was active, but isn't now
				level.Controllables.Remove(this);
				level.Actors.Remove(this);
			}
			else
			{ // either already active and still is, or inactive and still is
				Console.Write("Warning: attempted to set player active state to its current value. Could be a sign of a logic error.");
			}
            isActive = value;
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public override void LoadContent()
        {
            // TODO: This is kinda wrong use the right thing once we have it
            // Load animated textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, true, 32);
            runAnimation  = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess_walk_full"), 0.5f, true, 32);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, false, 32);
            dieAnimation  = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, false, 32);

            // Calculate bounds within texture size.
            int width  = (int)(idleAnimation.FrameWidth * 0.4); // 'width' is 2/5 of the actual frame width --> 12
			int left   = (int)(idleAnimation.FrameWidth - width) / 2; // 'left' is 3/10 of the actual frame width --> 10
            int height = (int)(idleAnimation.FrameHeight * 0.8); // 'height' is 4/5 of the actual frame height --> 51
			int top    = (int)(idleAnimation.FrameHeight - height); // 'top' is 1/5 of the actual frame height --> 13
            localBounds = new Rectangle(left, top, width, height);
			// TODO: What the fuck?

            // Load sounds.            
            //killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            //jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            //fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

		/// <summary>
		/// This override prevents the ghost from trying to perform idle
		/// animations and actions.
		/// </summary>
        protected override void GetIdleInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime)
        {
            //ghost doesn't idle, it derezzes
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public new void OnKilled(Actor killedBy)
        {
            isAlive = false;

            killedSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

		/// <summary>
		/// This override prevents the ghost from jumping, because white
		/// ghosts can't jump.
		/// </summary>
		/// <param name="velocityY">this would normally be the velocity at which to jump, but is irrelevant here</param>
        override protected float DoJump(float velocityY, GameTime gameTime)
        {
            return 0;
        }
    }
}
