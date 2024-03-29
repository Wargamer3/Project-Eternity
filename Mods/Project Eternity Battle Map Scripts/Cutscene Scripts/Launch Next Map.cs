﻿using System;
using System.IO;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptLaunchNextMap : BattleMapScript
        {
            public ScriptLaunchNextMap()
                : this(null)
            {
            }

            public ScriptLaunchNextMap(BattleMap Map)
                : base(Map, 100, 50, "Launch Next Map", new string[] { "Launch map" }, new string[0])
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                BattleMap NewMap = BattleMap.DicBattmeMapType[BattleMap.NextMapType].GetNewMap(Map.GetAvailableGameModes()[Map.GameRule.Name], string.Empty);

                NewMap.BattleMapPath = BattleMap.NextMapPath;
                NewMap.ListGameScreen = Owner.ListGameScreen;
                Owner.RemoveAllScreens();
                Owner.PushScreen(NewMap);
                NewMap.TogglePreview(true);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
            }

            public override void Save(BinaryWriter BW)
            {
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptLaunchNextMap(Map);
            }

            #region Properties

            #endregion
        }
    }
}
