using DiamondRush.Components;
using DiamondRush.Services;
using Microsoft.Xna.Framework;
using MonoECS;
using MonoECS.Ecs;
using MonoECS.Ecs.Systems;
using MonoECS.Engine.Physics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DiamondRush.Systems
{
    public class GemMovementSystem : EntitySystem, IUpdateSystem
    {
        private readonly GameApp _gameApp;
        private readonly GameBoardService _gameService;

        public GemMovementSystem(GameApp gameApp) : base(Aspect
            .All(typeof(GemComponent), typeof(GemMovementComponent), typeof(Transform2DComponent)))
        {
            _gameApp = gameApp;
            _gameService = _gameApp.Services.GetService<GameBoardService>();
        }

        private ComponentMapper<Transform2DComponent> _transform2DMapper;
        private ComponentMapper<GemMovementComponent> _gemMovementMapper;
        private ComponentMapper<GemComponent> _gemMapper;

        public override void Initialize(IComponentMapperService componentService)
        {
            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
            _gemMovementMapper = componentService.GetMapper<GemMovementComponent>();
            _gemMapper = componentService.GetMapper<GemComponent>();
        }

        public void Update(GameTime gameTime)
        {
            var delta = gameTime.GetElapsedSeconds();

            foreach (int entityId in ActiveEntities)
            {
                var transform2D = _transform2DMapper.Get(entityId);
                var gemMovement = _gemMovementMapper.Get(entityId);
                var gem = _gemMapper.Get(entityId);

                if (gemMovement.IsSwitching || gemMovement.IsFalling)
                {
                    //Dettach Gem from Field
                    if (gem.IsAttachedToGemField)
                    {
                        _gameService.DettachGemFromField(gem.GemField);
                    }

                    // Get Speed
                    float speed = gemMovement.IsSwitching ? gemMovement.SwitchingSpeed : gemMovement.FallingSpeed;


                    // Move Gem closer To TargetField
                    transform2D.Position = Vector2Extension.MoveTowards(transform2D.Position, gemMovement.TargetPosition, speed * delta);

                    // Gem has reached the TargetField
                    if (transform2D.Position == gemMovement.TargetPosition)
                    {
                        //Attach Gem to TargetField
                        _gameService.AttachGemToField(gemMovement.TargetField, entityId);

                        if (gemMovement.IsSwitching)
                        {
                            gemMovement.FinishSwitching();
                            Debug.WriteLine($"GEM #{entityId} SWITCHED");
                        }
                        else
                        {
                            gemMovement.FinishFalling();
                            Debug.WriteLine($"GEM #{entityId} FALLEN");
                        }

                    }
                }
            }
        }
    }
}
