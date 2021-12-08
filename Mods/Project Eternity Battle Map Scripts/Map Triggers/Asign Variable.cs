﻿using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class AsignVariableTrigger : BattleTrigger
    {
        private string _VariableName;
        private double _Value;
        private Operators.SignOperators _SignOperator;

        public AsignVariableTrigger()
            : this(null)
        {
        }

        public AsignVariableTrigger(BattleMap Map)
            : base(Map, 140, 70, "Asign Variable Event", new string[] { "Asign Variable" }, new string[] { "Variable asigned" })
        {
            _VariableName = "";
            _Value = 0;
            _SignOperator = Operators.SignOperators.Equal;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(VariableName);
            BW.Write(Value);
            BW.Write((byte)SignOperator);
        }

        public override void Load(BinaryReader BR)
        {
            VariableName = BR.ReadString();
            Value = BR.ReadDouble();
            SignOperator = (Operators.SignOperators)BR.ReadByte();
        }

        public override void Update(int Index)
        {
            string VariableNameLower = VariableName.ToLower();
            if (!Map.DicMapVariables.ContainsKey(VariableNameLower))
            {
                if (SignOperator == Operators.SignOperators.Equal)
                    Map.DicMapVariables.Add(VariableNameLower, Value);
                else
                    throw new Exception(SignOperator.ToString() + " can't be used on " + VariableNameLower + " as this variable is unassigned.");
            }
            else
            {
                switch (SignOperator)
                {
                    case Operators.SignOperators.Equal:
                        Map.DicMapVariables[VariableNameLower] = Value;
                        break;

                    case Operators.SignOperators.PlusEqual:
                        Map.DicMapVariables[VariableNameLower] += Value;
                        break;

                    case Operators.SignOperators.MinusEqual:
                        Map.DicMapVariables[VariableNameLower] -= Value;
                        break;

                    case Operators.SignOperators.MultiplicatedEqual:
                        Map.DicMapVariables[VariableNameLower] *= Value;
                        break;

                    case Operators.SignOperators.DividedEqual:
                        Map.DicMapVariables[VariableNameLower] /= Value;
                        break;

                    case Operators.SignOperators.ModuloEqual:
                        Map.DicMapVariables[VariableNameLower] %= Value;
                        break;
                }
            }
            Map.ExecuteFollowingScripts(this, 0);
        }

        public override MapScript CopyScript()
        {
            return new AsignVariableTrigger(Map);
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
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

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        [TypeConverter(typeof(SignOperatorConverter)),
        CategoryAttribute("Trigger values"),
        DescriptionAttribute(".")]
        public Operators.SignOperators SignOperator
        {
            get { return _SignOperator; }
            set { _SignOperator = value; }
        }

        #endregion
    }
}
