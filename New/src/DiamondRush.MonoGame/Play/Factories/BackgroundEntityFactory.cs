using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Factories;

internal sealed class BackgroundEntityFactory
{
    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    private readonly GraphicsDevice _graphicsDevice;

    public BackgroundEntityFactory(
        IPlaySceneContentProvider playSceneContentProvider,
        GraphicsDevice graphicsDevice)
    {
        _playSceneContentProvider = playSceneContentProvider;
        _graphicsDevice = graphicsDevice;
    }

    public Entity Create(
        IEntityContext entityContext)
    {
        var entity = entityContext.CreateEntity();

        var identity = new Identity
        {
            EntityType = EntityType.Background,
        };

        var sprite = new Sprite
        {
            TextureRegion = new TextureRegion(
                _playSceneContentProvider.BackgroundTexture)
        };

        var rectTransform = new RectTransform
        {
            Position = Vector2.Zero,
            Width = _graphicsDevice.Viewport.Width,
            Height = _graphicsDevice.Viewport.Height
        };


        entityContext.Set(entity, identity);
        entityContext.Set(entity, sprite);
        entityContext.Set(entity, rectTransform);

        return entity;
    }
}
