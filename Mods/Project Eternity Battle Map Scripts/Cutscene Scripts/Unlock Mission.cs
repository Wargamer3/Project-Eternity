using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptUnlockMission : BattleMapScript
        {
            string _MissionName;
            public ScriptUnlockMission()
                : this(null)
            {
            }

            public ScriptUnlockMission(BattleMap Map)
                : base(Map, 100, 50, "Unlock Mission", new string[] { "Unlock" }, new string[] { })
            {
                _MissionName = string.Empty;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                foreach (BattleMapPlayer ActivePlayer in Map.Room.GetLocalPlayers())
                {
                    if (!ActivePlayer.Inventory.DicOwnedMission.ContainsKey(_MissionName))
                    {
                        ActivePlayer.Inventory.DicOwnedMission.Add(_MissionName, new MissionInfo(_MissionName, 0));
                    }
                }

                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _MissionName = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_MissionName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptUnlockMission(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.MapSelector), typeof(UITypeEditor)),
            CategoryAttribute("BattleMap attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string MissionName
            {
                get
                {
                    return _MissionName;
                }
                set
                {
                    _MissionName = value;
                }
            }

            #endregion
        }
    }
}
