using DiamondRush.Data.Classes;
using Microsoft.Xna.Framework;
using MonoECS.Ecs;
using System;
using System.Text;

namespace DiamondRush.Components
{
    public class GemMovementComponent : IEntityComponent
    {
        public float SwitchingSpeed { get; set; }

        public float FallingSpeed { get; set; }

        public bool IsSwitching { get;  set; }

        public bool IsFalling { get; set; }

        public Vector2 TargetPosition { get; set; }

        public GemField TargetField { get; set; }

        public void SwitchTo(Vector2 targetPosition, GemField targetField)
        {
            TargetPosition = targetPosition;
            TargetField = targetField;
            IsSwitching = true;
        }

        public void FinishSwitching()
        {
            TargetPosition = default;
            TargetField = GemField.NONE;
            IsSwitching = false;
        }

        public void FallTo(Vector2 targetPosition, GemField targetField)
        {
            TargetPosition = targetPosition;
            TargetField = targetField;
            IsFalling = true;
        }

        public void FinishFalling()
        {
            TargetPosition = default;
            TargetField = GemField.NONE;
            IsFalling = false;
        }
    }
}
