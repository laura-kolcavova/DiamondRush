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

        var destinationRectangle = new Rectangle(
            (int)rectTransform.Position.X,
            (int)rectTransform.Position.Y,
            (int)rectTransform.Width,
            (int)rectTransform.Height);

        _spriteBatch.DrawSprite(
            sprite,
            destinationRectangle);
    }
}
