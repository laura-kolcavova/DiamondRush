using DiamondRush.Data;
using DiamondRush.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoECS.Engine.Scenes;
using MonoECS.Engine.ViewportAdapters;

namespace DiamondRush
{
    public class GameApp : Game
    {
        private GraphicsDeviceManager _graphics;

        public GameApp()
        {     
            IsMouseVisible = true;     
            IsFixedTimeStep = true;
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";

            _graphics = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //_graphics.PreferredBackBufferWidth = 1680;
            //_graphics.PreferredBackBufferHeight = 980;
            _graphics.IsFullScreen = false;
            //_graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            //_graphics.PreferMultiSampling = false;
            //_graphics.PreferredDepthStencilFormat = DepthFormat.None;
            //_graphics.SynchronizeWithVerticalRetrace = true;
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content.Load<SpriteFont>(Res.Fonts.DEFAULT);
            // ViewportAdapter
            var scallingViewportAdapter = new ScalingViewportAdapter(GraphicsDevice, Options.GameApp.Default_Width, Options.GameApp.Default_Height);
            Services.AddService<ScalingViewportAdapter>(scallingViewportAdapter);

            // SceneManager
            var sceneManager = new SceneManager(this);
            Components.Add(sceneManager);

            sceneManager.LoadScene(new PlayScene(this));
           
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
