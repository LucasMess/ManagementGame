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

        public static GraphicsDevice CurrentGraphicsDevice;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameWorld gameWorld;
        Camera camera;

        SpriteFont font;
        FrameCounter frameCounter;
        
        public ManagementGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            this.IsMouseVisible = true;
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
            CurrentGraphicsDevice = GraphicsDevice;

            ViewportWidth = GraphicsDevice.Viewport.Width;
            ViewportHeight = GraphicsDevice.Viewport.Height;


            FileLoader.Initialize();
            frameCounter = new FrameCounter();
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
            ContentLoader.LoadShaders(Content);
            ContentLoader.LoadFonts(Content);
            gameWorld = new GameWorld();
            camera = new Camera(GraphicsDevice.Viewport);

            graphics.SynchronizeWithVerticalRetrace = false;

            font = ContentLoader.GetFont("x32");  
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

            gameWorld.Update(gameTime, camera);
           // camera.PointTo(gameWorld.player);
            frameCounter.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.GetTransformMatrix());
            gameWorld.Draw(spriteBatch, camera);
            //spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, $"FPS: {frameCounter.GetFps()}", Vector2.Zero, Color.White);
            spriteBatch.End();

            frameCounter.Reset();

            base.Draw(gameTime);
        }
    }
}
