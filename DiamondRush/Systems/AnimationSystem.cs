using DiamondRush.Animations;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using MonoECS;
using MonoECS.Ecs;
using MonoECS.Ecs.Systems;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Systems
{
    public class AnimationSystem : EntitySystem, IUpdateSystem
    {
        private readonly GameApp _gameApp;

        public AnimationSystem(GameApp gameApp) : base(Aspect
            .All(typeof(AnimationComponent), typeof(Transform2DComponent)))
        {
            _gameApp = gameApp;
        }

        private ComponentMapper<AnimationComponent> _animationMapper;
        private ComponentMapper<Transform2DComponent> _transform2DMapper;

        public override void Initialize(IComponentMapperService componentService)
        {
            _animationMapper = componentService.GetMapper<AnimationComponent>();
            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
        }

        public void Update(GameTime gameTime)
        {
            foreach(var entityId in ActiveEntities)
            {
                var animation = _animationMapper.Get(entityId);
                var transform2D = _transform2DMapper.Get(entityId);

                if(!string.IsNullOrEmpty(animation.Name) && !animation.IsComplete)
                {
                    if(string.Equals(animation.Name, "Hide"))
                    {
                        if (Hide.Update(gameTime, transform2D))
                            CompleteAnimation(animation);
                    }
                }
            }
        }

        private void CompleteAnimation(AnimationComponent animation)
        {
            animation.IsComplete = true;
            animation.TriggerEventComplete();
        }
    }
}
