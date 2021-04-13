using DiamondRush.Data;
using DiamondRush.Data.Enums;
using DiamondRush.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoECS.Ecs;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Graphics.TextureAtlases;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Services
{
    public class EntityFactory
    {
        private readonly GameApp _gameApp;
        private readonly ContentManager _content;

        public EntityFactory(GameApp gameApp)
        {
            _gameApp = gameApp;
            _content = _gameApp.Content;
        }

        private World _world;
        private Dictionary<GemTypes, TextureRegion2D> _gemTextureRegions;

        public void Initialize(World world)
        {
            _world = world;

            var gemSheet = _content.Load<Texture2D>(Res.Sheets.GEM_SHEET);

            _gemTextureRegions = new Dictionary<GemTypes, TextureRegion2D>()
            {
                { GemTypes.Blue, new TextureRegion2D("blue", gemSheet, new Rectangle(0, 524, 502, 502)) },
                { GemTypes.Green, new TextureRegion2D("green", gemSheet, new Rectangle(1043, 524, 502, 502)) },
                { GemTypes.Orange, new TextureRegion2D("orange", gemSheet, new Rectangle(0, 0, 502, 502)) },
                { GemTypes.Purple, new TextureRegion2D("purple", gemSheet, new Rectangle(523, 524, 502, 502))},
                { GemTypes.Red, new TextureRegion2D("red", gemSheet, new Rectangle(1043, 0, 502, 502)) }
            };
        }

        public Entity CreateBackground()
        {
            var entity = _world.CreateEntity();

            entity.AttachComponent(new Transform2DComponent());
            entity.AttachComponent(new ModelComponent() { Name = "Background" });
            entity.AttachComponent(new RendererComponent() { Size = new Vector2(Options.GameApp.Default_Width, Options.GameApp.Default_Height) } );

            return entity;
        }

        public Entity CreateGameBoard()
        {
            var entity = _world.CreateEntity();

            entity.AttachComponent(new Transform2DComponent());
            entity.AttachComponent(new ModelComponent() { Name = "GameBoard" });
            entity.AttachComponent(new RendererComponent());
            entity.AttachComponent(new BoardFieldComponent() { MinCountForGemGroup = Options.GameBoard.MinCountForGemGroup });
            entity.AttachComponent(new BoardAppearanceComponent()
            {
                FieldWidth = Options.GameBoard.Field_Width,
                FieldHeight = Options.GameBoard.Field_Height,
                FieldSpace = Options.GameBoard.Field_Space,
                Theme = BoardThemes.DEFAULT
            });
            entity.AttachComponent(new BoardPlayComponent() { State = BoardStates.PLAY } );

            return entity;
        }

        public Entity CreateGem(GemTypes gemType)
        {
            var entity = _world.CreateEntity();

            entity.AttachComponent(new Transform2DComponent());
            entity.AttachComponent(new ModelComponent() { Name = "Gem" });
            entity.AttachComponent(new SpriteComponent(_gemTextureRegions[gemType]));
            entity.AttachComponent(new GemComponent() { GemType = gemType });
            entity.AttachComponent(new GemMovementComponent()
            {
                SwitchingSpeed = Options.Gem.Switch_Speed,
                FallingSpeed = Options.Gem.Fall_Speed
            });
            entity.AttachComponent(new AnimationComponent());

            entity.Group = "Gems";

            return entity;
        }
    }
}
