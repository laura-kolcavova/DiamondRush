using DiamondRush.Data.Classes;
using DiamondRush.Data.Enums;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using MonoECS.Ecs;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Physics;
using MonoECS.Engine.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiamondRush.Services
{
    public class GameBoardService
    {
        private readonly GameApp _gameApp;
        private readonly EntityFactory _entityFactory;
        private readonly Random _r;

        public GameBoardService(GameApp gameApp)
        {
            _gameApp = gameApp;
            _entityFactory = _gameApp.Services.GetService<EntityFactory>();
            _r = new Random();
        }

        private World _world;

        private ComponentMapper<Transform2DComponent> _transform2DMapper;
        private ComponentMapper<BoardFieldComponent> _boardFieldMapper;
        private ComponentMapper<BoardAppearanceComponent> _boardAppearanceMapper;
        private ComponentMapper<GemComponent> _gemMapper;
        private ComponentMapper<GemMovementComponent> _gemMovementMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;

        public void Initialize(World world)
        {
            _world = world;
            var componentService = world.ComponentMapperService;

            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
            _boardFieldMapper = componentService.GetMapper<BoardFieldComponent>();
            _boardAppearanceMapper = componentService.GetMapper<BoardAppearanceComponent>();
            _gemMapper = componentService.GetMapper<GemComponent>();
            _gemMovementMapper = componentService.GetMapper<GemMovementComponent>();
            _spriteMapper = componentService.GetMapper<SpriteComponent>();
        }

        public GemTypes GetRandomGemType()
        {
            var gemTypes = (GemTypes[])Enum.GetValues(typeof(GemTypes));

            return gemTypes[_r.Next(0, gemTypes.Length)];
        }

        public Vector2 GetPositionOfGemField(GemField gemField, bool worldPosition = false)
        {
            var appearance = _boardAppearanceMapper.Get(gemField.GameBoardId);

            int gapCol = gemField.Col * appearance.FieldSpace;
            int gapRow = gemField.Row * appearance.FieldSpace;

            float x = gemField.Col * appearance.FieldWidth + gapCol;
            float y = gemField.Row * appearance.FieldWidth + gapRow;

            if(worldPosition)
            {
                var boardTransform = _transform2DMapper.Get(gemField.GameBoardId);
                var boardWorldPosition = boardTransform.WorldPosition;

                x += boardWorldPosition.X;
                y += boardWorldPosition.Y;
            }

            return new Vector2(x, y);
        }

        public void AttachGemToGameBoard(int gameBoardId, int gemId)
        {
            var boardTransform = _transform2DMapper.Get(gameBoardId);
            var boardAppearance = _boardAppearanceMapper.Get(gameBoardId);
            var gemTransform = _transform2DMapper.Get(gemId);
            var gemSprite = _spriteMapper.Get(gemId);

            gemTransform.Parent = boardTransform;
            gemSprite.Size = new Vector2(boardAppearance.FieldWidth, boardAppearance.FieldHeight);
        }

        public void DettachGemFromField(GemField gemField)
        {
            var gem = _gemMapper.Get(gemField.GemId);

            gem.DettachFromGemField();
            gemField.DetachGem();
        }

        public void AttachGemToField(GemField gemField, int gemId)
        {
            if (!gemField.IsEmpty)
                DettachGemFromField(gemField);

            var gem = _gemMapper.Get(gemId);

            gemField.AttachGem(gemId);
            gem.AttachToGemField(gemField);
        }

        public void SwitchGems(int activeGemId, int passiveGemId)
        {
            var activeGem = _gemMapper.Get(activeGemId);
            var activeGemMovement = _gemMovementMapper.Get(activeGemId);

            var passiveGem = _gemMapper.Get(passiveGemId);
            var passiveGemMovement = _gemMovementMapper.Get(passiveGemId);
           
            var activeGemPosition = GetPositionOfGemField(activeGem.GemField);
            var passiveGemPosition = GetPositionOfGemField(passiveGem.GemField);

            activeGemMovement.SwitchTo(passiveGemPosition, passiveGem.GemField);
            passiveGemMovement.SwitchTo(activeGemPosition, activeGem.GemField);
        }

        public void RemoveGem(int gemId)
        {
            var gem = _gemMapper.Get(gemId);

            DettachGemFromField(gem.GemField);

            _world.DestroyEntity(gemId);
        }

        public int[] GetGroupOfGem(int gemId)
        {
            if (gemId < 0)
            {
                throw new InvalidOperationException("Gem Id cannot be smaller than 0");
            }

            var gem = _gemMapper.Get(gemId);
            int gemCol = gem.GemField.Col;
            int gemRow = gem.GemField.Row;

            int gameBoardId = gem.GemField.GameBoardId;
            var boardField = _boardFieldMapper.Get(gameBoardId);

            var gemGroup = new List<int>();
            var horizontalGroup = new List<int>();
            var verticalGroup = new List<int>();

            // Horizontal group - left
            horizontalGroup.Add(gemId);

            if (gemCol > 0)
            {
                for (int col = gemCol - 1; col > -1; col--)
                {
                    var gemField = boardField.GetFieldAt(gemRow, col);

                    if (gemField.IsEmpty) break;

                    var prevGemId = gemField.GemId;
                    var prevGem = _gemMapper.Get(prevGemId);

                    if (gem.GemType.Equals(prevGem.GemType))
                    {
                        horizontalGroup.Add(prevGemId);
                    }
                    else break;
                }
            }

            // Horizontal group - right
            if (gemCol < boardField.Cols - 1)
            {
                for (int col = gemCol + 1; col < boardField.Cols; col++)
                {
                    var gemField = boardField.GetFieldAt(gemRow, col);

                    if (gemField.IsEmpty) break;

                    var nextGemId = gemField.GemId;
                    var nextGem = _gemMapper.Get(nextGemId);

                    if (gem.GemType.Equals(nextGem.GemType))
                    {
                        horizontalGroup.Add(nextGemId);
                    }
                    else break;
                }
            }

            // Vertical group - up
            verticalGroup.Add(gemId);

            if (gemRow > 0)
            {
                for (int row = gemRow - 1; row > -1; row--)
                {
                    var gemField = boardField.GetFieldAt(row, gemCol);

                    if (gemField.IsEmpty) break;

                    var prevGemId = gemField.GemId;
                    var prevGem = _gemMapper.Get(prevGemId);

                    if (gem.GemType.Equals(prevGem.GemType))
                    {
                        verticalGroup.Add(prevGemId);
                    }
                    else break;
                }
            }

            // Vertical group - down
            if (gemRow < boardField.Rows - 1)
            {
                for (int row = gemRow + 1; row < boardField.Rows; row++)
                {
                    var gemField = boardField.GetFieldAt(row, gemCol);

                    if (gemField.IsEmpty) break;

                    var nextGemId = gemField.GemId;
                    var nextGem = _gemMapper.Get(nextGemId);

                    if (gem.GemType.Equals(nextGem.GemType))
                    {
                        verticalGroup.Add(nextGemId);
                    }
                    else break;
                }
            }

            // Calculate group

            if (horizontalGroup.Count() >= boardField.MinCountForGemGroup)
            {
                gemGroup.AddRange(horizontalGroup);
            }

            if (verticalGroup.Count() >= boardField.MinCountForGemGroup)
            {
                gemGroup.AddRange(verticalGroup);
            }

            return gemGroup.Distinct().ToArray();
        }

        public int[] GetGroupsOfGems(IEnumerable<int> gems)
        {
            var gemGroups = new List<int>();

            foreach(var gemId in gems)
            {
                var gemGroup = GetGroupOfGem(gemId);

                gemGroups.AddRange(gemGroup);
            }

            return gemGroups.Distinct().ToArray();
        }

        public int[] GetGemsUpOfEmptyField(GemField emptyField)
        {
            var boardField = _boardFieldMapper.Get(emptyField.GameBoardId);

            var upGems = new List<int>();

            if (emptyField.Row > 0)
            {
                for (int row = emptyField.Row - 1; row > -1; row--)
                {
                    var gemField = boardField.GetFieldAt(row, emptyField.Col);

                    if (!gemField.IsEmpty) upGems.Add(gemField.GemId);
                }
            }

            return upGems.ToArray();
        }

        public int CreateRefillGem(int gameBoardId, int col, int index)
        {
            var boardField = _boardFieldMapper.Get(gameBoardId);
            var appearance = _boardAppearanceMapper.Get(gameBoardId);

            // Get Random GemType
            var gemType = GetRandomGemType();

            // Create Gem Entity
            var gemEntity = _entityFactory.CreateGem(gemType);
            var gemTransform = _transform2DMapper.Get(gemEntity.Id);
            var gemSprite = _spriteMapper.Get(gemEntity.Id);

            // Get Start Field
            var startField = boardField.GetFieldAt(0, col);
            var startFieldPosition = GetPositionOfGemField(startField);

            int gapRow = appearance.FieldSpace * index;

            // Set Gem Position Out of GameBoard
            gemTransform.Position = new Vector2(
                 startFieldPosition.X,
                 startFieldPosition.Y - appearance.FieldHeight * index - gapRow
             );

            // Attach Gem to GameBoard
            AttachGemToGameBoard(gameBoardId, gemEntity.Id);

            // Unvisible Gem
            gemSprite.IsVisible = false;

            return gemEntity.Id;
        }

        public int[] RefillGems(IEnumerable<GemField> emptyFields)
        {
            // Get lowest emptyFields
            var lowestEmptyFields = emptyFields
                .OrderByDescending(f => f.Row)
                .GroupBy(f => f.Col)
                .Select(g => g.First());

            var refilledGems = new List<int>();
  
            foreach (var lowestEmptyField in lowestEmptyFields)
            {
                var emptyFieldsInColumn = emptyFields.Where(f => f.Col == lowestEmptyField.Col);
                int index = 1;

                foreach (var emptyField in emptyFieldsInColumn)
                {
                    int gameBoardId = emptyField.GameBoardId;

                    var gemId = CreateRefillGem(gameBoardId, emptyField.Col, index);
                    refilledGems.Add(gemId);

                    index++;
                }
            }

            return refilledGems.ToArray();
        }

        public void SetFallingColumn(GemField lowestEmptyField, IEnumerable<int> fallingGems)
        {
            var boardField = _boardFieldMapper.Get(lowestEmptyField.GameBoardId);

            int rowCnt = 0;
            foreach (var gemId in fallingGems)
            {
                var targetField = boardField.GetFieldAt(lowestEmptyField.Row - rowCnt, lowestEmptyField.Col);

                if (targetField != null)
                {
                    var targetFieldPosition = GetPositionOfGemField(targetField);
                    var gemMovement = _gemMovementMapper.Get(gemId);

                    gemMovement.FallTo(targetFieldPosition, targetField);
                }

                rowCnt++;
            }
        }

        public int[] FallGems(IEnumerable<GemField> emptyFields, IEnumerable<int> refilledGems)
        {
            // Get lowest emptyFields
            var lowestEmptyFields = emptyFields
                .OrderByDescending(f => f.Row)
                .GroupBy(f => f.Col)
                .Select(g => g.First());

            var fallingGems = new List<int>();
            var refilledGemsList = refilledGems.ToList();

            foreach (var lowestEmptyField in lowestEmptyFields)
            {
                var position = GetPositionOfGemField(lowestEmptyField);

                // Get Gems Up of Empty Filed
                var gemsInColumn = GetGemsUpOfEmptyField(lowestEmptyField);

                // Get Refilled Gems in Column
                var refilledGemsInColumns = refilledGemsList
                    .FindAll(gemId => _transform2DMapper.Get(gemId).Position.X == position.X);

                refilledGemsList.RemoveAll(gemId => refilledGemsInColumns.Contains(gemId));

                // Set Gems in Column to Fall 
                var fallingColumn = gemsInColumn.Concat(refilledGemsInColumns);

                if (fallingColumn.Any())
                {
                    SetFallingColumn(lowestEmptyField, fallingColumn);
                }

                fallingGems.AddRange(fallingColumn);
            }

            return fallingGems.ToArray();
        }
    }
}
