using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Factories;

internal sealed class BackgroundEntityFactory
{
    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    public BackgroundEntityFactory(
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _playSceneContentProvider = playSceneContentProvider;
    }

    public Entity Create(IEntityContext entityContext)
    {
        var entity = entityContext.CreateEntity();

        var identity = new Identity
        {
            EntityType = EntityType.Background,
        };

        var backgroundTexture = _playSceneContentProvider.BackgroundTexture;

        var textureRegion = new TextureRegion
        {
            Texture = backgroundTexture,
            SourceBounds = new Rectangle(
                0,
                0,
                backgroundTexture.Width,
                backgroundTexture.Height)
        };

        var sprite = new Sprite
        {
            TextureRegion = textureRegion,
        };

        var position = Vector2.Zero;

        entityContext.Set(entity, identity);
        entityContext.Set(entity, sprite);
        entityContext.Set(entity, position);

        return entity;
    }
}
