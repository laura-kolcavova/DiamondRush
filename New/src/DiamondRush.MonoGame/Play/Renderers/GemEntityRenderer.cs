using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Renderers;

internal sealed class GemEntityRenderer
{
    private readonly IEntityContext _entityContext;

    private readonly SpriteBatch _spriteBatch;

    private readonly IComponentStore<Sprite> _spriteStore;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemEntityRenderer(
        IEntityContext entityContext,
        SpriteBatch spriteBatch)
    {
        _entityContext = entityContext;
        _spriteBatch = spriteBatch;

        _spriteStore = _entityContext.UseStore<Sprite>();
        _rectTransformStore = _entityContext.UseStore<RectTransform>();
        _gemPlayBehaviorStore = _entityContext.UseStore<GemPlayBehavior>();
    }

    public void Render(
        Entity entity)
    {
        var gemPlayBehavior = _gemPlayBehaviorStore.Get(entity);

        if (!gemPlayBehavior.IsVisible)
        {
            return;
        }

        var sprite = _spriteStore.Get(entity);
        var rectTransform = _rectTransformStore.Get(entity);

        Rectangle destinationRectangle;

        if (gemPlayBehavior.CollectAnimationEnabled)
        {
            var collectAnimationProgress = gemPlayBehavior.CollectAnimationProgress;

            var scale = 1f * (1f - collectAnimationProgress);

            var newWidth = rectTransform.Width * scale;
            var newHeight = rectTransform.Height * scale;
            var newPositionX = rectTransform.Position.X + ((rectTransform.Width - newWidth) / 2);
            var newPositionY = rectTransform.Position.Y + ((rectTransform.Height - newHeight) / 2);

            destinationRectangle = new Rectangle(
                (int)newPositionX,
                (int)newPositionY,
                (int)newWidth,
                (int)newHeight);
        }

        else
        {
            destinationRectangle = new Rectangle(
                (int)rectTransform.Position.X,
                (int)rectTransform.Position.Y,
                (int)rectTransform.Width,
                (int)rectTransform.Height);
        }

        _spriteBatch.DrawSprite(
            sprite,
            destinationRectangle);
    }
}
