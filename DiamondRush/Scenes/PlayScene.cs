using DiamondRush.Services;
using DiamondRush.Systems;
using Microsoft.Xna.Framework;
using MonoECS.Ecs;
using MonoECS.Engine.Scenes;
using MonoECS.Engine.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Scenes
{
    public class PlayScene : IScene
    {
        private readonly GameApp _gameApp;
        private readonly ScalingViewportAdapter _viewportAdapter;
       
        public PlayScene(GameApp gameApp)
        {
            _gameApp = gameApp;
            _viewportAdapter = _gameApp.Services.GetService<ScalingViewportAdapter>();
        }

        private World _world;

        public void LoadContent()
        {
            // EntityFactory
            var entityFactory = new EntityFactory(_gameApp);
            _gameApp.Services.AddService<EntityFactory>(entityFactory);

            // GameBoardService
            var gameBoardService = new GameBoardService(_gameApp);
            _gameApp.Services.AddService<GameBoardService>(gameBoardService);

            // GameBuilder
            var gameBuilder = new GameBuilder(_gameApp);    

            // World
            _world = new WorldBuilder()
                .AddSystem(new PlayerControlSystem(_gameApp))
                .AddSystem(new GameBoardControlSystem(_gameApp))
                .AddSystem(new GemMovementSystem(_gameApp))
                .AddSystem(new AnimationSystem(_gameApp))

                .AddSystem(new RenderSystem(_gameApp))
                .Build();

            // Initialize Services
            entityFactory.Initialize(_world);
            gameBoardService.Initialize(_world);

            // Build Game
            gameBuilder.Build();
        }

        public void UnloadContent()
        {
            
        }

        public void Dispose()
        {
            _world.Dispose();

            _gameApp.Services.RemoveService(typeof(EntityFactory));
            _gameApp.Services.RemoveService(typeof(GameBoardService));
        }

        public void Update(GameTime gameTime)
        {           
            _world.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {

            _world.Draw(gameTime);
        }      
    }
}
