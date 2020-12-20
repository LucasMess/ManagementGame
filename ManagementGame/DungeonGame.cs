using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using ManagementGame.Networking;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reactive.Linq;
using ManagementGame.UI;

namespace ManagementGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DungeonGame : Game
    {
        public static int ViewportWidth = 0;
        public static int ViewportHeight = 0;

        public static GraphicsDevice CurrentGraphicsDevice;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dungeon gameWorld;
        Camera camera;
        GameClient client;

        SpriteFont font;
        FrameCounter frameCounter;

        Div uiRoot;
        
        public DungeonGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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

            client = new GameClient();
            client.Connect();

            InputManager.LeftMouseButtonState.DistinctUntilChanged().Subscribe((mouseEvent) =>
            {
                if (mouseEvent.Pressed)
                {
                    client.CreateGame();
                } 
            });

            InputManager.RightMouseButtonState.DistinctUntilChanged().Subscribe((mouseEvent) =>
            {
                if (mouseEvent.Pressed)
                {
                    client.JoinGame();
                }
            });

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
            gameWorld = new Dungeon();
            camera = new Camera(GraphicsDevice.Viewport);

            graphics.SynchronizeWithVerticalRetrace = false;

            font = ContentLoader.GetFont("x32");

            uiRoot = new Div();
            uiRoot.SetMargin(300, 0, 0, 0);
            uiRoot.LayoutDirection = LayoutDirection.Horizontal;

            var helloButton = new Button("Hello World!");
            helloButton.SetMargin(50, 50, 50, 50);
            helloButton.SetPadding(50, 50, 50, 50);

            var howdyButton = new Button("Howdy");
            howdyButton.SetMargin(50, 50, 50, 50);

            uiRoot.AppendChild(helloButton);
            uiRoot.AppendChild(howdyButton);
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
            camera.PointTo(gameWorld.player);
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
            uiRoot.Draw(spriteBatch);
            spriteBatch.End();

            //spriteBatch.Begin();
            //spriteBatch.DrawString(font, $"FPS: {frameCounter.GetFps()}", Vector2.Zero, Color.White);
            //spriteBatch.End();

            frameCounter.Reset();

            base.Draw(gameTime);
        }
    }
}
