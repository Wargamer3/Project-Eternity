using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Scripts
{

    #region Visual Novel

    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptCheckVisualNovelVariable : CutsceneActionScript
        {
            private UInt32 _TargetID;
            private Operators.LogicOperators _LogicOperator;
            private string _VariableName;
            private int _VariableValue;

            public ScriptCheckVisualNovelVariable()
                : base(160, 50, "Check Visual Novel variable", new string[] { "Check variable" }, new string[] { "Condition is true", "Condition is false" })
            {
                _TargetID = 0;
                _LogicOperator = Operators.LogicOperators.Equal;
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
                            int Value;

                            if (scriptUnit.ActiveVisualNovel.DicMapVariables.TryGetValue(_VariableName, out Value))
                            {
                                bool ConditionIsTrue = Operators.CompareValue(LogicOperator, Value, _VariableValue);
                                if (ConditionIsTrue)
                                    ExecuteEvent(this, 0);
                                else
                                    ExecuteEvent(this, 1);
                            }

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
                LogicOperator = (Operators.LogicOperators)BR.ReadByte();
                VariableName = BR.ReadString();
                VariableValue = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TargetID);
                BW.Write((byte)LogicOperator);
                BW.Write(VariableName);
                BW.Write(VariableValue);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCheckVisualNovelVariable();
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

            [TypeConverter(typeof(LogicOperatorConverter)),
            CategoryAttribute("Visual Novel"),
            DescriptionAttribute(".")]
            public Operators.LogicOperators LogicOperator
            {
                get { return _LogicOperator; }
                set { _LogicOperator = value; }
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
