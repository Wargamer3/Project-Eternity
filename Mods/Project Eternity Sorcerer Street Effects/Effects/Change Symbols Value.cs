using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;
using System.Collections.Generic;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;
using System.Globalization;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeSymbolsValueEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Symbols Value";

        public enum Targets { Terrain, All, Fire, Water, Earth, Air, Neutral }

        private Targets _Target;
        private SignOperators _SignOperator;
        private string _SymbolsValue;

        public ChangeSymbolsValueEffect()
            : base(Name, false)
        {
            _Target = Targets.Terrain;
            _SignOperator = SignOperators.PlusEqual;
            _SymbolsValue = string.Empty;
        }

        public ChangeSymbolsValueEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Terrain;
            _SignOperator = SignOperators.PlusEqual;
            _SymbolsValue = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _SignOperator = (SignOperators)BR.ReadByte();
            _SymbolsValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_SignOperator);
            BW.Write(_SymbolsValue);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_SymbolsValue);
            Dictionary<ElementalAffinity, int> DicSymbolValue = new Dictionary<ElementalAffinity, int>();
            Targets ActiveTarget = _Target;

            if (ActiveTarget == Targets.Terrain)
            {
                if (Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.AirElement)
                {
                    ActiveTarget = Targets.Air;
                }
                else if (Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.EarthElement)
                {
                    ActiveTarget = Targets.Earth;
                }
                else if(Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.FireElement)
                {
                    ActiveTarget = Targets.Fire;
                }
                else if(Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.WaterElement)
                {
                    ActiveTarget = Targets.Water;
                }
                else if(Params.GlobalContext.TerrainHolder.ListTerrainType[Params.GlobalContext.ActiveTerrain.TerrainTypeIndex] == TerrainSorcererStreet.NeutralElement)
                {
                    ActiveTarget = Targets.Neutral;
                }
            }

            if (ActiveTarget == Targets.All)
            {
                foreach (KeyValuePair<ElementalAffinity, int> ActiveElement in Params.Map.DicSymbolValue)
                {
                    DicSymbolValue.Add(ActiveElement.Key, ActiveElement.Value);
                }
            }
            else if (ActiveTarget == Targets.Air)
            {
                DicSymbolValue.Add(ElementalAffinity.Air, Params.Map.DicSymbolValue[ElementalAffinity.Air]);
            }
            else if (ActiveTarget == Targets.Earth)
            {
                DicSymbolValue.Add(ElementalAffinity.Earth, Params.Map.DicSymbolValue[ElementalAffinity.Earth]);
            }
            else if (ActiveTarget == Targets.Fire)
            {
                DicSymbolValue.Add(ElementalAffinity.Fire, Params.Map.DicSymbolValue[ElementalAffinity.Fire]);
            }
            else if (ActiveTarget == Targets.Water)
            {
                DicSymbolValue.Add(ElementalAffinity.Water, Params.Map.DicSymbolValue[ElementalAffinity.Water]);
            }
            else if (ActiveTarget == Targets.Neutral)
            {
                DicSymbolValue.Add(ElementalAffinity.Neutral, Params.Map.DicSymbolValue[ElementalAffinity.Neutral]);
            }

            foreach (KeyValuePair<ElementalAffinity, int> ActiveElement in DicSymbolValue)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        Params.Map.DicSymbolValue[ActiveElement.Key] = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.PlusEqual:
                        Params.Map.DicSymbolValue[ActiveElement.Key] += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        Params.Map.DicSymbolValue[ActiveElement.Key] = (int)(ActiveElement.Value / float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
                        break;

                    case SignOperators.MinusEqual:
                        Params.Map.DicSymbolValue[ActiveElement.Key] -= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.MultiplicatedEqual:
                        Params.Map.DicSymbolValue[ActiveElement.Key] = (int)(ActiveElement.Value * float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
                        break;

                    case SignOperators.ModuloEqual:
                        Params.Map.DicSymbolValue[ActiveElement.Key] = (int)(ActiveElement.Value % float.Parse(EvaluationResult, CultureInfo.InvariantCulture));
                        break;
                }
            }

            return "Symbol Value+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeSymbolsValueEffect NewEffect = new ChangeSymbolsValueEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._SymbolsValue = _SymbolsValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeSymbolsValueEffect NewEffect = (ChangeSymbolsValueEffect)Copy;

            _Target = NewEffect._Target;
            _SignOperator = NewEffect._SignOperator;
            _SymbolsValue = NewEffect._SymbolsValue;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public SignOperators SignOperator
        {
            get
            {
                return _SignOperator;
            }
            set
            {
                _SignOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Value
        {
            get
            {
                return _SymbolsValue;
            }
            set
            {
                _SymbolsValue = value;
            }
        }

        #endregion
    }
}
