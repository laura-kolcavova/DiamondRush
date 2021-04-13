using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Components
{
    public class AnimationComponent : IEntityComponent
    {
        private event Action _completed;

        public string Name { get; private set; }

        public bool IsPaused { get; private set; }

        public bool IsComplete { get; set; }

        public bool IsPlaying => !IsPaused && !IsComplete;

        public AnimationComponent()
        {
            Name = string.Empty;
            IsPaused = true;
            IsComplete = false;
        }

        public void Play(string name, Action onComplete)
        {
            Name = name;

            if(onComplete != null)
                _completed += onComplete;

            IsPaused = false;
            IsComplete = false;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Stop()
        {
            IsPaused = true;
            IsComplete = false;
        }

        public void TriggerEventComplete()
        {
            _completed?.Invoke();
        }
    }
}
