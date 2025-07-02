using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace DiamondRush.MonoGame.Core.Textures;

public sealed class TextureAtlas
{
    private readonly Texture2D _texture;

    private readonly Dictionary<string, TextureRegion> _textureRegionsByName;

    public TextureAtlas(
        Texture2D texture)
    {
        _texture = texture;

        _textureRegionsByName = [];
    }

    public void SetTextureRegion(
        string name,
        Rectangle sourceBounds)
    {
        var textureRegion = new TextureRegion(
            _texture,
            sourceBounds);

        _textureRegionsByName[name] = textureRegion;
    }

    public TextureRegion GetTextureRegion(
        string name)
    {
        if (!_textureRegionsByName.TryGetValue(
            name,
            out var textureRegion))
        {
            throw new InvalidOperationException(
                $"TextureRegion not found for name {name}.");
        }

        return textureRegion;
    }

    public bool TryGetTextureRegion(
       string name,
       [MaybeNullWhen(false)] out TextureRegion textureRegion)
    {
        return _textureRegionsByName.TryGetValue(
            name,
            out textureRegion);
    }

    public void Clear()
    {
        _textureRegionsByName.Clear();
    }
}
