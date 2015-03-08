﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ThePrincessBard.Actors
{
    class Snake : Controllable
    {
        public Snake(Level level, Vector2 position)
            : base(level, position)
        {
            isActive = false;
        }

        protected override void SetIsActive(bool value)
        {
            if (!isActive && value)
            {//wasn't active, but is now
                //nothing to do here
            }
            else if (isActive && !value)
            {//was active, but isn't now
                //also nothing to do here
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
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/snake/snake"), 0.1f, true, 64);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/snake/snake_walk_full"), 0.1f, true, 64);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/snake/snake"), 0.1f, false, 64);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/snake/snake"), 0.1f, false, 64);

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
            DisplayOrientation orientation,
            GameTime gameTime)
        {
            
        }
    }
}
