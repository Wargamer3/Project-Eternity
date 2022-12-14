using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Transforming;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptTransformUnit : DeathmatchMapScript
        {
            private uint _UnitToTransformID;
            private int _TransformationIndex;

            public ScriptTransformUnit()
                : this(null)
            {
            }

            public ScriptTransformUnit(DeathmatchMap Map)
                : base(Map, 100, 50, "Transform Unit", new string[] { "Transform" }, new string[] { "Unit Transformed" })
            {
                _UnitToTransformID = 0;
                _TransformationIndex = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    Squad MovingSquad = null;

                    for (int P = 0; P < Map.ListPlayer.Count && MovingSquad == null; P++)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && MovingSquad == null; U++)
                        {
                            if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToTransformID || Map.ListPlayer[P].ListSquad[U].CurrentLeader.HP <= 0)
                                continue;

                            MovingSquad = Map.ListPlayer[P].ListSquad[U];
                        }
                    }

                    if (MovingSquad != null)
                    {
                        UnitTransforming SpawnedUnit = MovingSquad.CurrentLeader as UnitTransforming;
                        SpawnedUnit.ChangeUnit(0);
                    }

                    IsActive = true;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                UnitToTransformID = BR.ReadUInt32();
                _TransformationIndex = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToTransformID);
                BW.Write(_TransformationIndex);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptTransformUnit(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToTransformID
            {
                get
                {
                    return _UnitToTransformID;
                }
                set
                {
                    _UnitToTransformID = value;
                }
            }

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute(".")]
            public int TransformationIndex
            {
                get
                {
                    return _TransformationIndex;
                }
                set
                {
                    _TransformationIndex = value;
                }
            }

            #endregion
        }
    }
}
