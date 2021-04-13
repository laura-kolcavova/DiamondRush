using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Models
{
    public static class Background
    {
        public static void Render(RendererComponent renderer, Transform2DComponent transform2D, SpriteBatch sb)
        {
            if (!renderer.IsVisible)
                return;

            var texture = renderer.MainTexture;
            var bounding = renderer.GetBoundingRectangle(transform2D);

            sb.Draw(texture, bounding, Color.White);
        }
    }
}
