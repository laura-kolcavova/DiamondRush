using Microsoft.Xna.Framework;
using MonoECS;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Animations
{
    public static class Hide
    {
        public static bool Update(GameTime gameTime, Transform2DComponent transform2D)
        {
            float delta = gameTime.GetElapsedSeconds();

            var newScale = new Vector2(-2f) * delta;
            var newPos = new Vector2(64f) * delta;

           transform2D.Scale += newScale;
           transform2D.Position += newPos;

            if (transform2D.Scale.X <= 0 && transform2D.Scale.Y <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
