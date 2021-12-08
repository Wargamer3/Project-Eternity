using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptSpawnHealthCrate : BattleMapScript
        {
            public ScriptSpawnHealthCrate()
                : this(null)
            {
            }

            public ScriptSpawnHealthCrate(BattleMap Map)
                : base(Map, 100, 50, "Spawn Health Crate", new string[] { "Spawn" }, new string[0])
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                int PosX = RandomHelper.Random.Next(Map.MapSize.X);
                int PosY = RandomHelper.Random.Next(Map.MapSize.Y);
                HPCrate NewCrate = new HPCrate(new Microsoft.Xna.Framework.Vector3(5, 10, 0), Map);
                NewCrate.Load(Map.Content);
                Map.ListProp.Add(NewCrate);
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
                return new ScriptSpawnHealthCrate(Map);
            }
        }
    }
}
