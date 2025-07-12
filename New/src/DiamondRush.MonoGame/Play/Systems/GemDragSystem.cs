using DiamondRush.MonoGame.Core.Messages.Abstractions;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Messages;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class GemDragSystem
    : IUpdateSystem
{
    private readonly IMessenger _messenger;

    private readonly PlayContext _playContext;

    private readonly IComponentStore<GemPlayBehavior> _gemPlayBehaviorStore;

    public GemDragSystem(
        IEntityContext entityContext,
        IMessenger messenger,
        PlayContext playContext)
    {
        _messenger = messenger;

        _playContext = playContext;

        _gemPlayBehaviorStore = entityContext.UseStore<GemPlayBehavior>();
    }

    private bool _isGemBeingDragged = false;

    private Entity? _draggingGemEntity = null;

    private GameBoardField? _draggingStartGameBoardField = null;

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        if (_messenger.TryReadMessage<PlayerPressStartedMessage>(
               out var playerPressStartedMessage))
        {
            HandlePlayerPressStartedMessage(playerPressStartedMessage);
        }

        if (_messenger.TryReadMessage<PlayerPressReleasedMessage>(
              out var playerPressReleasedMessage))
        {
            HandlePlayerPressReleasedMessage(playerPressReleasedMessage);
        }

        if (_messenger.TryReadMessage<PlayerPressMovedMessage>(
               out var playerPressMovedMessage))
        {
            HandlePlayerPressMovedMessage(playerPressMovedMessage);
        }
    }

    private bool IsUpdateEnabled() =>
        _playContext.PlayState == PlayState.WaitingForInput;

    private void HandlePlayerPressStartedMessage(
        PlayerPressStartedMessage playerPressStartedMessage)
    {
        _messenger.RemoveMessage<PlayerPressStartedMessage>();

        if (_isGemBeingDragged)
        {
            return;
        }

        SearchForDraggingGemEntity(
            playerPressStartedMessage.Position);
    }

    private void HandlePlayerPressReleasedMessage(
        PlayerPressReleasedMessage playerPressMovedMessage)
    {
        _messenger.RemoveMessage<PlayerPressReleasedMessage>();

        if (!_isGemBeingDragged)
        {
            return;
        }

        StopDraggingGem();
    }

    private void HandlePlayerPressMovedMessage(
        PlayerPressMovedMessage playerPressMovedMessage)
    {
        _messenger.RemoveMessage<PlayerPressMovedMessage>();

        if (!_isGemBeingDragged)
        {
            return;
        }

        if (SearchForGemToSwap(
            playerPressMovedMessage.CurrentPosition))
        {
            StopDraggingGem();

            _playContext.SetPlayState(PlayState.SwappingGems);
        }
    }

    private void SearchForDraggingGemEntity(
        Vector2 pressStartPosition)
    {
        foreach (var gameBoardField in _playContext.GameBoardFields.AsEnumerable())
        {
            if (gameBoardField.IsEmpty)
            {
                continue;
            }

            var gameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(
                gameBoardField);

            var gameBoardFieldRect = new RectangleF(
                gameBoardFieldPosition.X,
                gameBoardFieldPosition.Y,
                Constants.GameBoardFieldSize,
                Constants.GameBoardFieldSize);

            if (!gameBoardFieldRect.Contains(
                pressStartPosition.X,
                pressStartPosition.Y))
            {
                continue;
            }

            StartDraggingGem(gameBoardField);

            return;
        }
    }

    private bool SearchForGemToSwap(
        Vector2 pressCurrentPosition)
    {
        var gameBoardFieldPosition = _playContext.GetGameBoardFieldPosition(
              _draggingStartGameBoardField!);

        var rowIndex = _draggingStartGameBoardField!.RowIndex;

        var columnIndex = _draggingStartGameBoardField!.ColumnIndex;

        var firstRowIndex = 0;

        var firstColumnIndex = 0;

        var lastRowIndex = _playContext.GameBoard.Rows - 1;

        var lastColumIndex = _playContext.GameBoard.Columns - 1;

        var gameBoardFieldRect = new RectangleF(
            gameBoardFieldPosition.X,
            gameBoardFieldPosition.Y,
            Constants.GameBoardFieldSize,
            Constants.GameBoardFieldSize);

        var isSwapping = false;

        if (
            columnIndex != firstColumnIndex &&
            pressCurrentPosition.X < gameBoardFieldRect.Left)
        {
            // Left
            isSwapping = TryToSwapGem(
                rowIndex,
                columnIndex - 1);
        }
        else if (
            columnIndex != lastColumIndex &&
            pressCurrentPosition.X > gameBoardFieldRect.Right)
        {
            // Right
            isSwapping = TryToSwapGem(
                rowIndex,
                columnIndex + 1);
        }
        else if (
            rowIndex != firstRowIndex &&
            pressCurrentPosition.Y < gameBoardFieldRect.Top)
        {
            // Top
            isSwapping = TryToSwapGem(
               rowIndex - 1,
               columnIndex);
        }
        else if (
            rowIndex != lastRowIndex &&
            pressCurrentPosition.Y > gameBoardFieldRect.Bottom)
        {
            // Bottom
            isSwapping = TryToSwapGem(
               rowIndex + 1,
               columnIndex);
        }

        return isSwapping;
    }

    private bool TryToSwapGem(
        int targetRowIndex,
        int targetColumnIndex)
    {
        var targetGameBoardField = _playContext.GameBoardFields.GetField(
            targetRowIndex,
            targetColumnIndex);

        if (targetGameBoardField.IsEmpty)
        {
            return false;
        }

        var draggingGemPlayBehavior = _gemPlayBehaviorStore.Get(
            _draggingGemEntity!.Value);

        _gemPlayBehaviorStore.Set(
            _draggingGemEntity!.Value,
            draggingGemPlayBehavior.StartSwapping(
                targetRowIndex,
                targetColumnIndex));

        var targetGemEntity = targetGameBoardField.GemEntity;

        var targetGemPlayBehavior = _gemPlayBehaviorStore.Get(
            targetGemEntity);

        _gemPlayBehaviorStore.Set(
            targetGemEntity,
            targetGemPlayBehavior.StartSwapping(
                _draggingStartGameBoardField!.RowIndex,
                _draggingStartGameBoardField!.ColumnIndex));

        return true;
    }

    private void StartDraggingGem(
        GameBoardField gameBoardField)
    {
        _isGemBeingDragged = true;

        _draggingGemEntity = gameBoardField.GemEntity;

        _draggingStartGameBoardField = gameBoardField;
    }

    private void StopDraggingGem()
    {
        _isGemBeingDragged = false;

        _draggingGemEntity = null;

        _draggingStartGameBoardField = null;
    }
}
