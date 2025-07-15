using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DiamondRush.MonoGame.Play.Factories;

internal sealed class GemEntityFactory
{
    private readonly TextureAtlas _textureAtlas;

    public GemEntityFactory(
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _textureAtlas = CreateGemTextureAtlas(
            playSceneContentProvider.GemSpriteSheet);
    }

    public Entity Create(
        IEntityContext entityContext,
        GemType gemType)
    {
        var entity = entityContext.CreateEntity();

        var identity = new Identity
        {
            EntityType = EntityType.Gem,
        };

        var gem = new Gem
        {
            GemType = gemType,
        };

        var textureRegionName = Enum.GetName(gemType)!;

        var sprite = new Sprite
        {
            TextureRegion = _textureAtlas.GetTextureRegion(textureRegionName),
        };

        var rectTransform = new RectTransform
        {
            Width = Constants.GemWidth,
            Height = Constants.GemHeight,
        };

        entityContext.Set(entity, identity);
        entityContext.Set(entity, gem);
        entityContext.Set(entity, sprite);
        entityContext.Set(entity, rectTransform);

        return entity;
    }

    private static TextureAtlas CreateGemTextureAtlas(
        Texture2D gemSpriteSheet)
    {
        var textureAtlas = new TextureAtlas(gemSpriteSheet);

        textureAtlas.SetTextureRegion(
            nameof(GemType.Orange),
            new Rectangle(0, 0, 502, 502));

        textureAtlas.SetTextureRegion(
            nameof(GemType.DarkBlue),
            new Rectangle(523, 0, 502, 502));

        textureAtlas.SetTextureRegion(
            nameof(GemType.Red),
            new Rectangle(1043, 0, 502, 502));

        textureAtlas.SetTextureRegion(
            nameof(GemType.Blue),
            new Rectangle(0, 524, 502, 502));

        textureAtlas.SetTextureRegion(
            nameof(GemType.Purple),
            new Rectangle(523, 524, 502, 502));

        textureAtlas.SetTextureRegion(
            nameof(GemType.Green),
            new Rectangle(1043, 524, 502, 502));

        return textureAtlas;
    }
}
