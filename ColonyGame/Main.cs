using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColonyGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IGameScene CurrentScene;

        public Main()
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
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 800;
          // graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new System.EventHandler<System.EventArgs>(Window_ClientSizeChanged);
            IsMouseVisible = true;
            base.Initialize();
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            //graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            //graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            // graphics.ApplyChanges();


            GraphicsDevice.Viewport = new Viewport(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            CurrentScene.ScreenResized(GraphicsDevice);
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.Effects.Add("GUI",Content.Load<Effect>("GUI"));
            Assets.SpriteFonts.Add("UIFontO", Content.Load<SpriteFont>("UIFontO"));
            // TODO: use this.Content to load your game content here

            this.CurrentScene = new GameScenes.MainGame();
            this.CurrentScene.Init(GraphicsDevice);

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
            float seconds = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

            CurrentScene.HandleInput(seconds);
            CurrentScene.Update(seconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float seconds = (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

            // TODO: Add your drawing code here
            CurrentScene.Render(seconds, GraphicsDevice, spriteBatch);
            base.Draw(gameTime);
        }
    }
}
