using DiamondRush.Data.Classes;
using DiamondRush.Data.Enums;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoECS;
using MonoECS.Ecs;
using MonoECS.Ecs.Systems;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Physics;
using MonoECS.Engine.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DiamondRush.Systems
{
    public class PlayerControlSystem : EntitySystem, IUpdateSystem
    {
        private readonly GameApp _gameApp;
        private readonly ScalingViewportAdapter _viewportAdapter;

        public PlayerControlSystem(GameApp gameApp) : base (Aspect
            .All(typeof(BoardPlayComponent)))
        {
            _gameApp = gameApp;
            _viewportAdapter = _gameApp.Services.GetService<ScalingViewportAdapter>();
        }

        private ComponentMapper<Transform2DComponent> _transform2DMapper;
        private ComponentMapper<BoardPlayComponent> _boardPlayMapper;
        private ComponentMapper<BoardFieldComponent> _boardFieldMapper;
        private ComponentMapper<GemComponent> _gemMapper;
        private ComponentMapper<RendererComponent> _rendererMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;

        private MouseState _mouseState_current;
        private MouseState _mouseState_prev;

        private int _draggingGemId = -1;
        private bool _isDragging;

        public override void Initialize(IComponentMapperService componentService)
        {
            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
            _rendererMapper = componentService.GetMapper<RendererComponent>();
            _spriteMapper = componentService.GetMapper<SpriteComponent>();
            _boardPlayMapper = componentService.GetMapper<BoardPlayComponent>();
            _boardFieldMapper = componentService.GetMapper<BoardFieldComponent>();
            _gemMapper = componentService.GetMapper<GemComponent>();        
        }

        public void Update(GameTime gameTime)
        {
            Begin();

            foreach(var entityId in ActiveEntities)
            {
                Proccess(gameTime, entityId);
            }

            End();
        }

        public void Begin()
        {
            _mouseState_current = Mouse.GetState();
            Mouse.SetCursor(MouseCursor.Arrow);
        }

        public void End()
        {
            _mouseState_prev = _mouseState_current;
        }

        public void Proccess(GameTime gameTime, int entityId)
        {
            var boardPlay = _boardPlayMapper.Get(entityId);

            if (boardPlay.State != BoardStates.PLAY)
                return;

            var boardTransform = _transform2DMapper.Get(entityId);
            var boardRenderer = _rendererMapper.Get(entityId);

            var boardBounding = boardRenderer.GetBoundingRectangle(boardTransform);

            var mousePosition_current = _viewportAdapter.PointToScreen(_mouseState_current.Position);
            var mousePosition_prev = _viewportAdapter.PointToScreen(_mouseState_prev.Position);

            // Mouse Intersects Board
            if (boardBounding.Intersects(mousePosition_current))
            {
                var boardField = _boardFieldMapper.Get(entityId);

                // Iterate Gems
                foreach(var gemField in boardField.Field)
                {
                    if(gemField != GemField.NONE && !gemField.IsEmpty)
                    {
                        var gemBounding = GetGemBounding(gemField.GemId);

                        // Gem Hover
                        if(gemBounding.Intersects(mousePosition_current))
                        {
                            // Set Mouse Cursor - Hand
                            Mouse.SetCursor(MouseCursor.Hand);

                            // Drag Start
                            if (!_isDragging && _mouseState_current.LeftButton == ButtonState.Pressed && _mouseState_prev.LeftButton == ButtonState.Released)
                            {
                                // Gem Was Pressed
                                _draggingGemId = gemField.GemId;
                                _isDragging = true;
                                return;
                            }
                        }
                    }
                }   

                // Drag Move
                if(_isDragging && mousePosition_current != mousePosition_prev)
                {
                    var gemBounding = GetGemBounding(_draggingGemId);

                    if(!gemBounding.Intersects(mousePosition_current))
                    {
                        int gemId = _draggingGemId;
              
                        // Get GemField
                        var gemField = _gemMapper.Get(gemId).GemField;
                        var asideGemField = GetAsideGemField(gemBounding, mousePosition_current, boardField, gemField);

                        // Try Switch Gems
                        if(asideGemField != GemField.NONE && !asideGemField.IsEmpty)
                        {
                            boardPlay.ActiveGemId = gemId;
                            boardPlay.PassiveGemId = asideGemField.GemId;
                            boardPlay.State = BoardStates.TRY_SWITCH_GEMS;
                        }

                        // Drag End
                        _draggingGemId = -1;
                        _isDragging = false;
                    }
                }

                // Drag End
                if(_isDragging && _mouseState_current.LeftButton == ButtonState.Released)
                {
                    _draggingGemId = -1;
                    _isDragging = false;
                }
            }
        }

        private Rectangle GetGemBounding(int gemId)
        {
            var gemTrnasform = _transform2DMapper.Get(gemId);
            var gemSprite = _spriteMapper.Get(gemId);

            return gemSprite.GetBoundingRectangle(gemTrnasform);
        }

        private GemField GetAsideGemField(Rectangle gemBounding, Point mousePosition, BoardFieldComponent boardField, GemField gemField)
        {
            GemField asideGemField;

            if (mousePosition.X <= gemBounding.Left)
            {
                //LEFT
                asideGemField = boardField.GetFieldLeftOf(gemField);
            }
            else if (mousePosition.X >= gemBounding.Right)
            {
                //RIGHT
                asideGemField = boardField.GetFieldRightOf(gemField);
            }
            else if (mousePosition.Y >= gemBounding.Bottom)
            {
                //Bottom
                asideGemField = boardField.GetFieldBottomOf(gemField);
            }
            else// if (e.MouseState.Position.Y <= gem.Transform.Bounds.Top) //UP
            {
                //UP
                asideGemField = boardField.GetFieldUpOf(gemField);
            }

            return asideGemField;
        }
    }
}
