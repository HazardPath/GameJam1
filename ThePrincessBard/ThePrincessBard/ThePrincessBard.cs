using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace ThePrincessBard
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ThePrincessBard : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        // Global content.
        private SpriteFont hudFont;

		/// <summary>
		/// Image to display when the player wins a level.
		/// </summary>
        private Texture2D winOverlay;
		/// <summary>
		/// Image to display when the player loses a level.
		/// </summary>
        private Texture2D loseOverlay;
		/// <summary>
		/// Image to display when the player dies.
		/// </summary>
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;

        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int numberOfLevels = 3;

		/// <summary>
		/// Constructor for the main class of the game.
		/// Instantiates the GraphicsDeviceManager and defines
		/// the content root directory.
		/// </summary>
        public ThePrincessBard()
        {
            graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base. Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("audio/GameMusic"));
            }
            catch { }

            LoadNextLevel();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle polling for our input and handling high-level input
            HandleInput();

            // update our level, passing down the GameTime along with all of our input states
            level.Update(gameTime, keyboardState, gamePadState, Window.CurrentOrientation);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            // Get keyboard and/or gamepad input states.
            keyboardState = Keyboard.GetState();
            gamePadState  = GamePad.GetState(PlayerIndex.One);

            // Exit the game when back is pressed.
			if (gamePadState.Buttons.Back == ButtonState.Pressed) { Exit(); }

			// TODO: this is where the 'continue' button is defined.
			// Define continue button to progess to next level.
            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                gamePadState.IsButtonDown(Buttons.A);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!wasContinuePressed && continuePressed)
            {
				/**/ if (!level.Player.IsAlive) { level.StartNewLife(); }
				else if ( level.ReachedExit   ) { LoadNextLevel(); }
            }

			// This is to make sure you don't accidentally auto-progress
			// before you're ready because you were holding the continue
			// button when you approached the exit.
            wasContinuePressed = continuePressed;
        }

		/// <summary>
		/// Loads the next game level.
		/// </summary>
		/// <remarks>
		/// Modifies ThePrincessBard.levelIndex and ThePrincessBard.level.
		/// </remarks>
        private void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
			if (level != null) { level.Dispose(); }

            // Load the level.
			// TODO: This is where level files are found and loaded.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
			using (Stream fileStream = TitleContainer.OpenStream(levelPath))
				level = new Level(Services, fileStream, levelIndex);
        }

		/// <summary>
		/// Reloads the current level of the game.
		/// </summary>
		/// <remarks>
		/// Modifies ThePrincessBard.levelIndex and calls LoadNextLevel().
		/// </remarks>
        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
			// This makes our background "sky" be blue.
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// Has the SpriteBatch start doing things.
            spriteBatch.Begin();

			// Defined in Level.cs; this is the main draw method.
            level.Draw(gameTime, spriteBatch);

			// Necessary cleanup for the SpriteBatch class.
            spriteBatch.End();

			// The 'base' in question is Microsoft.Xna.Framework.Game.
            base.Draw(gameTime);
        }
    }
}