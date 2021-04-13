using DiamondRush.Data;
using DiamondRush.Data.Classes;
using DiamondRush.Data.Enums;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoECS.Ecs;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiamondRush.Services
{
    public class GameBuilder
    {
        private readonly GameApp _gameApp;
        private readonly EntityFactory _entityFactory;
        private readonly GameBoardService _gameBoardService;
        private readonly Random _r;

        public GameBuilder(GameApp gameApp)
        {
            _gameApp = gameApp;
            _entityFactory = _gameApp.Services.GetService<EntityFactory>();
            _gameBoardService = _gameApp.Services.GetService<GameBoardService>();
            _r = new Random();
        }

        private GemTypes[,] CreateRandomGemTypeField(BoardFieldComponent boardField)
        {
            GemTypes[,] gemTypesField = new GemTypes[boardField.Rows, boardField.Cols];

            for (int row = 0; row < boardField.Rows; row++)
            {
                for (int col = 0; col < boardField.Cols; col++)
                {
                    gemTypesField[row, col] = CreateRandomGemTypeNotInGroup(
                        gemTypesField, row, col, boardField.MinCountForGemGroup);
                }
            }

            return gemTypesField;
        }

        private GemTypes CreateRandomGemTypeNotInGroup(GemTypes[,] field, int row, int col, int minCountOfGroup)
        {
            List<GemTypes> generableGemTypes = new List<GemTypes>();
            generableGemTypes.AddRange((GemTypes[])Enum.GetValues(typeof(GemTypes)));


            //Cehck group at col
            if (col > minCountOfGroup - 2)
            {
                GemTypes? potentionalGroupType = null;

                for (int i = 0; i < minCountOfGroup - 2; i++)
                {
                    GemTypes currentType = field[row, col - (i + 1)];
                    GemTypes previousType = field[row, col - (i + 2)];

                    if (currentType == previousType)
                    {
                        potentionalGroupType = currentType;
                    }
                    else
                    {
                        potentionalGroupType = null;
                        break;
                    }
                }

                if (potentionalGroupType != null)
                {
                    //Groupt at col exists
                    generableGemTypes.Remove(potentionalGroupType.Value);
                }
            }

            //Check group at row
            if (row > minCountOfGroup - 2)
            {
                GemTypes? potentionalGroupType = null;

                for (int i = 0; i < minCountOfGroup - 2; i++)
                {
                    GemTypes currentType = field[row - (i + 1), col];
                    GemTypes previousType = field[row - (i + 2), col];

                    if (currentType == previousType)
                    {
                        potentionalGroupType = currentType;
                    }
                    else
                    {
                        potentionalGroupType = null;
                        break;
                    }
                }

                if (potentionalGroupType != null)
                {
                    //Groupt at row exists
                    generableGemTypes.Remove(potentionalGroupType.Value);
                }
            }

            // Generate random gem type
            int index = _r.Next(generableGemTypes.Count());

            return generableGemTypes[index];
        }

        private void FillBoardWithRandomGems(Entity gameBoardEntity)
        {
            var boardField = gameBoardEntity.GetComponent<BoardFieldComponent>();

            GemTypes[,] gemTypesField = CreateRandomGemTypeField(boardField);

            for (int row = 0; row < boardField.Rows; row++)
            {
                for (int col = 0; col < boardField.Cols; col++)
                {
                    // Get GemField
                    var gemField = boardField.GetFieldAt(row, col);

                    // Get Random GemType
                    var gemType = gemTypesField[row, col];

                    // Create Gem Entity
                    var gemEntity = _entityFactory.CreateGem(gemType);
                    var gemTransform = gemEntity.GetComponent<Transform2DComponent>();

                    // Set Bounds of Gem
                    gemTransform.Position = _gameBoardService.GetPositionOfGemField(gemField);

                    // Attach Gem to GameBoard
                    _gameBoardService.AttachGemToGameBoard(gameBoardEntity.Id, gemEntity.Id);

                    // Attach Gem To GemField
                    _gameBoardService.AttachGemToField(gemField, gemEntity.Id);
                }
            }

            Debug.WriteLine("GameBoard filled with random gems");
        }

        private void CreateBackground()
        {
            var background = _entityFactory.CreateBackground();

            var renderer = background.GetComponent<RendererComponent>();

            renderer.MainTexture = _gameApp.Content.Load<Texture2D>(Res.Textures.BACKGROUND);
        }

        private Entity CreateGameBoard(int rows, int cols)
        {
            var gameBoardEntity = _entityFactory.CreateGameBoard();

            var transform2D = gameBoardEntity.GetComponent<Transform2DComponent>();
            var appearance = gameBoardEntity.GetComponent<BoardAppearanceComponent>();
            var boardField = gameBoardEntity.GetComponent<BoardFieldComponent>();
            var renderer = gameBoardEntity.GetComponent<RendererComponent>();

            var field = new GemField[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    field[row, col] = new GemField(gameBoardEntity.Id, row, col);
                }
            }

            boardField.SetField(field);

            int width = cols * appearance.FieldWidth + (cols - 1) * appearance.FieldSpace;
            int height = rows * appearance.FieldHeight + (rows - 1) * appearance.FieldSpace;
            var size = new Vector2(width, height);

            var position = new Vector2(Options.GameApp.Default_Width / 2, Options.GameApp.Default_Height / 2);
            position -= size / 2;

            transform2D.Position = position;
            renderer.Size = size;

            return gameBoardEntity;
        }

        public void Build()
        {
            // Create Background
            CreateBackground();

            // Create GameBoard
            var gameBoardEntity = CreateGameBoard(8, 8);

            // Fill GameBoard by Random Gems
            FillBoardWithRandomGems(gameBoardEntity);
        }

       
    }
}
