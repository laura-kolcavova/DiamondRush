using Microsoft.Xna.Framework.Graphics;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Models
{
    public static class Gem
    {
        public static void Render(SpriteComponent sprite, Transform2DComponent transform2D, SpriteBatch sb)
        {
            if (!sprite.IsVisible)
                return;

            var texture = sprite.TextureRegion.Texture;
            var bounding = sprite.GetBoundingRectangle(transform2D);

            sb.Draw(texture, bounding, sprite.TextureRegion.Bounds, sprite.Color, transform2D.Rotation, sprite.Origin, sprite.Effects, sprite.Depth);
        }
    }
}
