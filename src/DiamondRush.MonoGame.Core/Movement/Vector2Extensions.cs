using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Core.Movement;

public static class Vector2Extensions
{
    public static Vector2 MoveTowards(
        this Vector2 current,
        Vector2 target,
        float stepLength)
    {
        var toTarget = target - current;
        var distanceToTarget = toTarget.Length();

        if (distanceToTarget <= stepLength || distanceToTarget == 0f)
        {
            return target;
        }

        var direction = toTarget / distanceToTarget;

        return current + stepLength * direction;
    }
}
