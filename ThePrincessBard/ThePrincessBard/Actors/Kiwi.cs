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
    class Kiwi : Controllable
    {
        public Kiwi(Level level, Vector2 position)
            : base(level, position)
        {
            isActive = false;
        }

        protected override void SetIsActive(bool value)
        {
            if (!isActive && value)
            {//wasn't active, but is now
                //nothing to do here...?
            }
            else if (isActive && !value)
            {//was active, but isn't now
                //also nothing to do here...?
            }

            isActive = value;
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public override void LoadContent()
        {
            // Load animated textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/kiwi/kiwi"), 0.1f, true, 32);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/kiwi/kiwi_walk_full"), 0.1f, true, 32);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/kiwi/kiwi"), 0.1f, false, 32);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Graphics/kiwi/kiwi"), 0.1f, false, 32);

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

        private Random rand = new Random();
        private float timeWaiting = 0f;
        private bool idleJumping = false;

        private float timeWaitedSinceFall = 0f;

        override public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation)
        {
            base.Update(gameTime, keyboardState, gamePadState, orientation);
            if (keyboardState.IsKeyDown(Globals.JumpKey) ||
                gamePadState.IsButtonDown(Globals.JumpButton))
            {
                GravityAcceleration = 0f;
                if (!IsOnGround)
                {
                    timeWaitedSinceFall += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timeWaitedSinceFall > 0.15)
                    {
                        position.Y += 1;
                    }
                }
            }
            else
            {
                GravityAcceleration = 3400.0f;
            }
        }

        protected override void GetIdleInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime)
        {
            if (timeWaiting == 0)
            {
                if (idleJumping)
                {
                    idleJumping = false;
                    timeWaiting = (float)rand.NextDouble() * 3f + 1f;
                }
                else
                {
                    idleJumping = true;
                    timeWaiting = (float)rand.NextDouble() * 1f + 0.1f;
                }
            }
            else
            {
                timeWaiting -= (gameTime.ElapsedGameTime.Milliseconds/1000f);
                if (timeWaiting < 0) timeWaiting = 0;
            }

            isJumping = idleJumping;
        }
    }
}
