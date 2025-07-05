using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemVisibilitySystem :
    IUpdateSystem
{
    private readonly IEntityView _gemEntityView;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemVisibilitySystem(
        IEntityContext entityContext)
    {
        _gemEntityView = entityContext
           .UseQuery()
           .With<Gem>()
           .With<GemPlayBehavior>()
           .AsView();

        _rectTransformStore = entityContext.UseStore<RectTransform>();
        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    public void Update(GameTime gameTime)
    {
        foreach (var gemEntity in _gemEntityView.AsEnumerable())
        {
            var gemPlayBehavior = _gemPlayBehaviorStore.Get(gemEntity);

            if (gemPlayBehavior.IsVisible)
            {
                continue;
            }

            var gemRectTransform = _rectTransformStore.Get(gemEntity);

            var gameBoardRectTransform = _rectTransformStore.Get(
                gemPlayBehavior.GameBoardEntity);

            TryUpdateGemVisibility(
                gemEntity,
                gemPlayBehavior,
                gemRectTransform,
                gameBoardRectTransform);
        }
    }

    private void TryUpdateGemVisibility(
        Entity gemEntity,
        GemPlayBehavior gemPlayBehavior,
        RectTransform gemRectTransform,
        RectTransform gameBoardRectTransform)
    {
        if (gemRectTransform.IsInside(gameBoardRectTransform))
        {
            _gemPlayBehaviorStore.Set(
                gemEntity,
                gemPlayBehavior.SetVisibility(true));
        }
    }
}
