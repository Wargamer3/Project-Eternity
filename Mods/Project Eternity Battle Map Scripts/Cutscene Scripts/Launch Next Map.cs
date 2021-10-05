using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;

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
                BattleMap NewMap = BattleMap.DicBattmeMapType[BattleMap.NextMapType].GetNewMap(BattleMap.NextMapPath, 0, new Dictionary<string, List<Squad>>());
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
