using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{

    #region Visual Novel

    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptSetVisualNovelVariable : CutsceneActionScript
        {
            private UInt32 _TargetID;
            private string _VariableName;
            private int _VariableValue;

            public ScriptSetVisualNovelVariable()
                : base(160, 50, "Set Visual Novel variable", new string[] { "Set" }, new string[0])
            {
                _TargetID = 0;
                _VariableName = "";
                _VariableValue = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        ScriptVisualNovel scriptUnit = (ScriptVisualNovel)GetDataContainerByID(_TargetID, ScriptVisualNovel.ScriptName);
                        if (scriptUnit != null)
                        {
                            if (scriptUnit.ActiveVisualNovel.DicMapVariables.ContainsKey(_VariableName))
                                scriptUnit.ActiveVisualNovel.DicMapVariables[_VariableName] = _VariableValue;
                            else
                                scriptUnit.ActiveVisualNovel.DicMapVariables.Add(_VariableName, _VariableValue);

                            ExecuteEvent(this, 0);
                            IsEnded = true;
                            break;
                        }
                        break;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TargetID = BR.ReadUInt32();
                VariableName = BR.ReadString();
                VariableValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TargetID);
                BW.Write(VariableName);
                BW.Write(VariableValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSetVisualNovelVariable();
            }

            #region Properties

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("The targeted Visual Novel."),
            DefaultValueAttribute("")]
            public UInt32 TargetID
            {
                get
                {
                    return _TargetID;
                }
                set
                {
                    _TargetID = value;
                }
            }

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("."),
            DefaultValueAttribute("")]
            public string VariableName
            {
                get
                {
                    return _VariableName;
                }
                set
                {
                    _VariableName = value;
                }
            }

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("."),
            DefaultValueAttribute(0)]
            public int VariableValue
            {
                get
                {
                    return _VariableValue;
                }
                set
                {
                    _VariableValue = value;
                }
            }

            #endregion
        }
    }

    #endregion
}
