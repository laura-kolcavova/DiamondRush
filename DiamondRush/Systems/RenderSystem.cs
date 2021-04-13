using DiamondRush.Models;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoECS.Ecs;
using MonoECS.Ecs.Systems;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Physics;
using MonoECS.Engine.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Systems
{
    public class RenderSystem : EntitySystem, IDrawSystem
    {
        private readonly GameApp _gameApp;
        private readonly ScalingViewportAdapter _viewportAdapter;
        private readonly SpriteBatch _sb;

        public RenderSystem(GameApp gameApp) : base(Aspect
            .All(typeof(ModelComponent), typeof(Transform2DComponent)))
        {
            _gameApp = gameApp;
            _viewportAdapter = _gameApp.Services.GetService<ScalingViewportAdapter>();
            _sb = new SpriteBatch(_gameApp.GraphicsDevice);
        }

        private ComponentMapper<ModelComponent> _modelMapper;
        private ComponentMapper<Transform2DComponent> _transform2DMapper;
        private ComponentMapper<RendererComponent> _rendererMapper;
        private ComponentMapper<BoardFieldComponent> _boardFieldMapper;
        private ComponentMapper<BoardAppearanceComponent> _boardAppearanceMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;

        public override void Initialize(IComponentMapperService componentService)
        {
            _modelMapper = componentService.GetMapper<ModelComponent>();
            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
            _rendererMapper = componentService.GetMapper<RendererComponent>();
            _boardFieldMapper = componentService.GetMapper<BoardFieldComponent>();
            _boardAppearanceMapper = componentService.GetMapper<BoardAppearanceComponent>();
            _spriteMapper = componentService.GetMapper<SpriteComponent>();
        }

        public void Draw(GameTime gameTime)
        {
            // Begin
            _sb.Begin(samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend, transformMatrix: _viewportAdapter.GetScaleMatrix());

            // Draw Entities
            foreach (int entityId in ActiveEntities)
            {
                var model = _modelMapper.Get(entityId);
                var transform2D = _transform2DMapper.Get(entityId);

                if (string.Equals("Background", model.Name))
                {
                    var renderer = _rendererMapper.Get(entityId);

                    Background.Render(renderer, transform2D, _sb);
                }
                else if(string.Equals("GameBoard", model.Name))
                {
                    var renderer = _rendererMapper.Get(entityId);
                    var boardAppearance = _boardAppearanceMapper.Get(entityId);
                    var boardField = _boardFieldMapper.Get(entityId);

                    GameBoard.Render(boardAppearance, boardField, renderer, transform2D, _sb);
                }
                else if(string.Equals("Gem", model.Name))
                {
                    var sprite = _spriteMapper.Get(entityId);

                    Gem.Render(sprite, transform2D, _sb);
                }
            }
            // End
            _sb.End();
        }

       
    }
}
