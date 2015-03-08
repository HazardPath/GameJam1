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
        /// Constructors a new player.
        /// </summary>
        public Ghost(Level level, Vector2 position)
            : base(level, position)
        {
            MoveAcceleration = 6000.0f;
            MaxMoveSpeed = 400.0f;
            isActive = true;
        }

        protected override void SetIsActive(bool value)
        {
            if (!isActive && value)
            {//wasn't active, but is now
                //TODO spawn yourslef
            }
            else if (isActive && !value)
            {//was active, but isn't now
                //TODO despawn yourself
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
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, true, 32);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, false, 32);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/princess/princess"), 0.1f, false, 32);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            //killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            //jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            //fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        protected override void GetIdleInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation)
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

        override protected float DoJump(float velocityY, GameTime gameTime)
        {
            return 0;
        }
    }
}
