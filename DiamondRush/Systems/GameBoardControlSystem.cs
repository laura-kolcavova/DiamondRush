using DiamondRush.Data;
using DiamondRush.Data.Enums;
using DiamondRush.Components;
using DiamondRush.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoECS.Ecs;
using MonoECS.Ecs.Systems;
using MonoECS.Engine.Graphics.Sprites;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DiamondRush.Systems
{
    public class GameBoardControlSystem : EntitySystem, IUpdateSystem
    {
        private readonly GameApp _gameApp;
        private readonly GameBoardService _gameBoardService;

        public GameBoardControlSystem(GameApp gameApp) : base (Aspect
            .All(typeof(BoardPlayComponent)))
        {
            _gameApp = gameApp;
            _gameBoardService = _gameApp.Services.GetService<GameBoardService>();
        }

        private ComponentMapper<Transform2DComponent> _transform2DMapper;
        private ComponentMapper<BoardPlayComponent> _boardPlayMapper;
        private ComponentMapper<GemComponent> _gemMapper;
        private ComponentMapper<GemMovementComponent> _gemMovementMapper;
        private ComponentMapper<AnimationComponent> _animationMapper;
        private ComponentMapper<SpriteComponent> _spriteMapper;

        private SoundEffect _sfxPing;
        private SoundEffect _sfxCollect;

        public override void Initialize(IComponentMapperService componentService)
        {
            _transform2DMapper = componentService.GetMapper<Transform2DComponent>();
            _boardPlayMapper = componentService.GetMapper<BoardPlayComponent>();
            _gemMapper = componentService.GetMapper<GemComponent>();
            _gemMovementMapper = componentService.GetMapper<GemMovementComponent>();
            _animationMapper = componentService.GetMapper<AnimationComponent>();
            _spriteMapper = componentService.GetMapper<SpriteComponent>();

            _sfxPing = _gameApp.Content.Load<SoundEffect>(Res.Sfx.GEM_PING);
            _sfxCollect = _gameApp.Content.Load<SoundEffect>(Res.Sfx.COLLECT_GEMS);
        }

        public void Update(GameTime gameTime)
        {
            foreach(var entityId in ActiveEntities)
            {
                var boardPlay = _boardPlayMapper.Get(entityId);

                if (boardPlay.State == BoardStates.TRY_SWITCH_GEMS)
                {
                    Proccess_TrySwitchGems(boardPlay);
                }
                else if (boardPlay.State == BoardStates.GEMS_SWITCHING)
                {
                    Proccess_GemsSwitching(boardPlay);
                }
                else if (boardPlay.State == BoardStates.GEMS_SWITCHING_BACK)
                {
                    Proccess_GemsSwitchingBack(boardPlay);
                }
                else if (boardPlay.State == BoardStates.GEMS_COLLECTING)
                {
                    Proccess_GemsCollecting(boardPlay);
                }
                else if (boardPlay.State == BoardStates.GEMS_FALLING)
                {
                    Proccess_GemsFalling(boardPlay, entityId);
                }             
            }
        }

        private void Proccess_TrySwitchGems(BoardPlayComponent boardPlay)
        {
            
            Debug.WriteLine($"TRY SWITCH GEMS #{boardPlay.ActiveGemId} AND #{boardPlay.PassiveGemId}");

            // Play Sound
            _sfxPing.Play();

            // Switch Gems
            _gameBoardService.SwitchGems(boardPlay.ActiveGemId, boardPlay.PassiveGemId);

            // Set State
            boardPlay.State = BoardStates.GEMS_SWITCHING;
        }

        private void Proccess_GemsSwitching(BoardPlayComponent boardPlay)
        {
            // Check if Gems Moved to their Target Fields
            var activeGemMovement = _gemMovementMapper.Get(boardPlay.ActiveGemId);
            var passiveGemMovement = _gemMovementMapper.Get(boardPlay.PassiveGemId);

            if (!activeGemMovement.IsSwitching && !passiveGemMovement.IsSwitching)
            {
                Debug.WriteLine("BOTH GEMS SWITCHED");
                Debug.WriteLine("CHECK FOR GEM GROUPS");

                var activeGemGroup = _gameBoardService.GetGroupOfGem(boardPlay.ActiveGemId);
                var passiveGemGroup = _gameBoardService.GetGroupOfGem(boardPlay.PassiveGemId);

                if (activeGemGroup.Length == 0 && passiveGemGroup.Length == 0)
                {
                    // Siwtch Gems Back
                    _gameBoardService.SwitchGems(boardPlay.ActiveGemId, boardPlay.PassiveGemId);
                    boardPlay.State = BoardStates.GEMS_SWITCHING_BACK;
                }
                else
                {
                    // Get Gems to Collect
                    var collectGems = activeGemGroup
                        .Concat(passiveGemGroup)
                        .ToArray();

                    // Collect Gems
                    CollectGems(boardPlay, collectGems);

                    // Reset Active And Passive Gems
                    boardPlay.ActiveGemId = -1;
                    boardPlay.PassiveGemId = -1;

                    // Set State
                    boardPlay.State = BoardStates.GEMS_COLLECTING;
                }
            }
        }

        private void Proccess_GemsSwitchingBack(BoardPlayComponent boardPlay)
        {
            // Check if Gems Moved to their Target Fields
            var activeGemMovement = _gemMovementMapper.Get(boardPlay.ActiveGemId);
            var passiveGemMovement = _gemMovementMapper.Get(boardPlay.PassiveGemId);

            if (!activeGemMovement.IsSwitching && !passiveGemMovement.IsSwitching)
            {
                Debug.WriteLine("BOTH GEMS SWITCHED BACK");
                
                // Reset Active And Passive Gems
                boardPlay.ActiveGemId = -1;
                boardPlay.PassiveGemId = -1;

                // Set State
                boardPlay.State = BoardStates.PLAY;
            }
        }

        private void Proccess_GemsCollecting(BoardPlayComponent boardPlay)
        {
            // ALL Gems Collected
            if (boardPlay.CollectingGems.Length == boardPlay.CollectedGemsCnt)
            {
                Debug.WriteLine("GEMS COLLECTED");

                // Get EmptyFields
                var emptyFields = boardPlay.CollectingGems
                    .Select(gemId => _gemMapper.Get(gemId).GemField)
                    .ToArray();

                // Remove Gems
                foreach (var gemId in boardPlay.CollectingGems)
                {
                    _gameBoardService.RemoveGem(gemId);
                }

                // Reset Board Play Values
                boardPlay.CollectingGems = Array.Empty<int>();
                boardPlay.CollectedGemsCnt = 0;

                // Refill Gems
                boardPlay.RefillingGems = _gameBoardService.RefillGems(emptyFields);

                // Set Falling Gems
                boardPlay.FallingGems = _gameBoardService.FallGems(emptyFields, boardPlay.RefillingGems);

                // Set State
                boardPlay.State = BoardStates.GEMS_FALLING;
            }
        }

        private void Proccess_GemsFalling(BoardPlayComponent boardPlay, int gameBoardId )
        {
            int reffiledGemsCnt = 0;
            int fallenGemsCnt = 0;

            // Check if RefillingGems aproached GameBoard -> Set Visible = true
            foreach(var gemId in boardPlay.RefillingGems)
            {
                var gemTransform = _transform2DMapper.Get(gemId);
                var gemSprite = _spriteMapper.Get(gemId);

                if (!gemSprite.IsVisible && gemTransform.Position.Y >= 0)
                {
                    gemSprite.IsVisible = true;
                    reffiledGemsCnt++;
                }
            }

            //Check if All Gems were Refilled -> Reset BoardPlay Values
            if(boardPlay.RefillingGems.Length == reffiledGemsCnt)
            {
                boardPlay.RefillingGems = Array.Empty<int>();
            }

            // Check If All Gems have Fallen to Target Field
            foreach (var gemId in boardPlay.FallingGems)
            {
                var gemMovement = _gemMovementMapper.Get(gemId);

                if (!gemMovement.IsFalling)
                {
                    fallenGemsCnt++;
                }
            }

            if (boardPlay.FallingGems.Length == fallenGemsCnt)
            {
                Debug.WriteLine("ALL GEMS FALLEN");

                // Get Gem Groups of Fallen Gems
                var gemGroups = _gameBoardService.GetGroupsOfGems(boardPlay.FallingGems);

                // Reset Board Play Values
                boardPlay.FallingGems = Array.Empty<int>();

                if (gemGroups.Length > 0)
                {
                    // Collect Gems
                    CollectGems(boardPlay, gemGroups);

                    // Set State
                    boardPlay.State = BoardStates.GEMS_COLLECTING;
                }
                else
                {
                    // Set State
                    boardPlay.State = BoardStates.PLAY;
                }
            }
        }

        // METHODS
        // *************************************** //

        private void CollectGems(BoardPlayComponent boardPlay, int[] collectGems)
        {
            // Sound
            _sfxCollect.Play();

            // Collect Gemes
            boardPlay.CollectingGems = collectGems;

            // Play Hide Animation
            foreach(var gemId in collectGems)
            {
                var animation = _animationMapper.Get(gemId);

                animation.Play("Hide", () =>
                {
                    boardPlay.CollectedGemsCnt++;
                });
            }; 
        }

        
    }
}
