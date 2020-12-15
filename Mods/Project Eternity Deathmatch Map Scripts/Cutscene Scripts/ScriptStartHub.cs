using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptStartHub : DeathmatchMapScript
        {
            private uint _UnitToMoveID;

            public ScriptStartHub()
                : this(null)
            {
                _UnitToMoveID = 0;
            }

            public ScriptStartHub(DeathmatchMap Map)
                : base(Map, 100, 50, "Start Hub", new string[] { "Start" }, new string[0])
            {
                _UnitToMoveID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsEnded = true;

                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID == _UnitToMoveID)
                        {
                            Map.ListActionMenuChoice.Add(new ActionPanelHubStep(Map, Map.ListPlayer[P].ListSquad[U]));
                            return;
                        }
                    }
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                UnitToMoveID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToMoveID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptStartHub(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToMoveID
            {
                get
                {
                    return _UnitToMoveID;
                }
                set
                {
                    _UnitToMoveID = value;
                }
            }

            #endregion
        }
    }
}
