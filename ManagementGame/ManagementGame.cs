using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ManagementGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ManagementGame : Game
    {
        public static int ViewportWidth = 0;
        public static int ViewportHeight = 0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameWorld gameWorld;
        Camera camera;
        
        public ManagementGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsFixedTimeStep = false;
            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;


            FileLoader.Initialize();
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

            ContentLoader.LoadTileTextures(Content);
            gameWorld = new GameWorld();
            camera = new Camera(GraphicsDevice.Viewport);

            gameWorld.LoadChunks();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update(gameTime);

            gameWorld.Update(gameTime);
            camera.PointTo(gameWorld.player);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetTransformMatrix());
            gameWorld.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
