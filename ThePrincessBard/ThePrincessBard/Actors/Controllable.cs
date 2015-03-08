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
    /// Anything that can be controlled by the player.
    /// </summary>
    abstract class Controllable : Actor
    {
        public bool IsActive
        {
            get { return isActive; }
            set { SetIsActive(value); }
        }
        protected bool isActive;

        protected bool isPossessHeld = false;

        protected abstract void SetIsActive(bool value);

        public Controllable(Level level, Vector2 position)
            : base(level, position)
        {
        }

        override protected void GetInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime)
        {
            if (isActive)
                GetRealInput(keyboardState, gamePadState, orientation, gameTime);
            else
                GetIdleInput(keyboardState, gamePadState, orientation, gameTime);
        }

        virtual protected void GetRealInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime)
        {
            // Get analog cardinal movement.
            movement.X = gamePadState.ThumbSticks.Left.X * Globals.MoveStickScale;
            movement.Y = gamePadState.ThumbSticks.Left.Y * Globals.MoveStickScale;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement.X) < 0.5f)
                movement.X = 0.0f;
            else
                isClimbing = false;
            if (Math.Abs(movement.Y) < 0.5f)
                movement.Y = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement.X = -1.0f;
                isClimbing = false;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement.X = 1.0f;
                isClimbing = false;
            }

            // If any digital vertical movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
                keyboardState.IsKeyDown(Keys.Down) ||
                keyboardState.IsKeyDown(Keys.S))
            {
                isClimbing = false;
                if (IsAlignedLeftToClimbable() && level.GetCollision((int) Position.X + 1, (int) Position.Y) == TileCollision.Climbable)
                {
                        isClimbing = true;
                        climbableLeft = true;
                        isJumping = false;
                        isOnGround = false;
                        movement.Y = 2.0f;
                }
                else if (IsAlignedRightToClimbable() && level.GetCollision((int) Position.X - 1, (int) Position.Y) == TileCollision.Climbable)
                {
                    isClimbing = true;
                    climbableLeft = false;
                    isJumping = false;
                    isOnGround = false;
                    movement.Y = 2.0f;
                }
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadUp) ||
                     keyboardState.IsKeyDown(Keys.Up) ||
                     keyboardState.IsKeyDown(Keys.W))
            {
                isClimbing = false;
                if (IsAlignedLeftToClimbable() && level.GetCollision((int)Position.X + 1, (int)Position.Y) == TileCollision.Climbable)
                {
                    isClimbing = true;
                    climbableLeft = true;
                    isJumping = false;
                    isOnGround = false;
                    movement.Y = -1.0f;
                }
                else if (IsAlignedRightToClimbable() && level.GetCollision((int)Position.X - 1, (int)Position.Y) == TileCollision.Climbable)
                {
                    isClimbing = true;
                    climbableLeft = false;
                    isJumping = false;
                    isOnGround = false;
                    movement.Y = -1.0f;
                }
            }

            // Check if the player wants to jump.
            isJumping =
                gamePadState.IsButtonDown(Globals.JumpButton) ||
                keyboardState.IsKeyDown(Globals.JumpKey);

            // Try and possess something.
            if (gamePadState.IsButtonDown(Globals.PossessButton) ||
                keyboardState.IsKeyDown(Globals.PossessKey))
            {
                if (!isPossessHeld)
                {
                    isPossessHeld = true;
                    Controllable posessable = level.GetPosessableThing(this);
                    if (posessable == null)
                    {
                        //TODO do something
                    }
                    else
                    {
                        //TODO do something else
                    }
                }
            }
            else
            {
                isPossessHeld = false;
            }
        }

        protected abstract void GetIdleInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            DisplayOrientation orientation,
            GameTime gameTime);
    }
}
