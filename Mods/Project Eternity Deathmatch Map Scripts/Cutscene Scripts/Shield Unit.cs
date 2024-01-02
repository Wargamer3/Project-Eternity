using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptShieldUnit : DeathmatchMapScript
        {
            public enum BarrierTypes { Defend = 0, Dodge = 1 };

            private BarrierTypes _BarrierType;
            private string _ENCost;
            private Operators.NumberTypes _NumberType;
            private string _DamageReduction;
            private string _BreakingDamage;//Damage limit for the BarrierEffect to break.
            private List<string> ListEffectiveAttack;//List of Attacks the BarrierEffect is usefull against.
            private List<string> ListBreakingAttack;//List of Attacks the BarrierEffect is destroyed against.
            private List<string> ListBreakingSkill;//List of Skills the BarrierEffect is destroyed against.

            private uint _UnitToShieldID;

            public ScriptShieldUnit()
                : this(null)
            {
            }

            public ScriptShieldUnit(DeathmatchMap Map)
                : base(Map, 100, 50, "Shield Unit", new string[] { "Add To Unit" }, new string[] { "Effect Added" })
            {
                _UnitToShieldID = 0;

                _BarrierType = BarrierTypes.Defend;
                _NumberType = Operators.NumberTypes.Absolute;
                _ENCost = "0";
                _DamageReduction = "0";
                _BreakingDamage = "0";

                ListEffectiveAttack = new List<string>();
                ListBreakingAttack = new List<string>();
                ListBreakingSkill = new List<string>();
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToShieldID)
                            continue;

                        BarrierEffect ActiveEffect = new BarrierEffect(new UnitEffectParams(Map.Params.GlobalContext, UnitQuickLoadEffectContext.DefaultQuickLoadContext));

                        ActiveEffect.BarrierType = (BarrierEffect.BarrierTypes)BarrierType;
                        ActiveEffect.ENCost = ENCost;
                        ActiveEffect.NumberType = NumberType;
                        ActiveEffect.DamageReduction = DamageReduction;
                        ActiveEffect.BreakingDamage = BreakingDamage;
                        ActiveEffect.EffectiveAttacks = new List<string>(EffectiveAttacks);
                        ActiveEffect.BreakingAttacks = new List<string>(BreakingAttacks);
                        ActiveEffect.BreakingSkills = new List<string>(BreakingSkills);

                        Map.ListPlayer[P].ListSquad[U].CurrentLeader.Pilot.Effects.AddAndExecuteEffectWithoutCopy(ActiveEffect, "Barrier");
                        break;
                    }
                }

                IsEnded = true;
                ExecuteEvent(this, 0);
            }

            public override void Load(BinaryReader BR)
            {
                _UnitToShieldID = BR.ReadUInt32();

                _BarrierType = (BarrierTypes)BR.ReadByte();
                _ENCost = BR.ReadString();
                _NumberType = (Operators.NumberTypes)BR.ReadByte();
                _DamageReduction = BR.ReadString();
                _BreakingDamage = BR.ReadString();

                int BarrierEffectEffectiveAttacksCount = BR.ReadInt32();
                for (int A = BarrierEffectEffectiveAttacksCount - 1; A >= 0; --A)
                    ListEffectiveAttack.Add(BR.ReadString());

                int BarrierEffectBreakingAttacksCount = BR.ReadInt32();
                for (int A = BarrierEffectBreakingAttacksCount - 1; A >= 0; --A)
                    ListBreakingAttack.Add(BR.ReadString());

                int BarrierEffectBreakingSkillsCount = BR.ReadInt32();
                for (int A = BarrierEffectBreakingSkillsCount - 1; A >= 0; --A)
                    ListBreakingSkill.Add(BR.ReadString());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitToShieldID);

                BW.Write((byte)_BarrierType);
                BW.Write(_ENCost);
                BW.Write((byte)_NumberType);
                BW.Write(_DamageReduction);
                BW.Write(_BreakingDamage);

                BW.Write(ListEffectiveAttack.Count);
                for (int A = 0; A < ListEffectiveAttack.Count; A++)
                    BW.Write(ListEffectiveAttack[A]);

                BW.Write(ListBreakingAttack.Count);
                for (int A = 0; A < ListBreakingAttack.Count; A++)
                    BW.Write(ListBreakingAttack[A]);

                BW.Write(ListBreakingSkill.Count);
                for (int A = 0; A < ListBreakingSkill.Count; A++)
                    BW.Write(ListBreakingSkill[A]);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            protected override CutsceneScript DoCopyScript()
            {
                ScriptShieldUnit NewEffect = new ScriptShieldUnit(Map);

                NewEffect._BarrierType = _BarrierType;
                NewEffect._ENCost = _ENCost;
                NewEffect._NumberType = _NumberType;
                NewEffect._DamageReduction = _DamageReduction;
                NewEffect._BreakingDamage = _BreakingDamage;

                NewEffect.ListEffectiveAttack = new List<string>(ListEffectiveAttack.Count);
                for (int i = ListEffectiveAttack.Count - 1; i >= 0; --i)
                    NewEffect.ListEffectiveAttack.Add(ListEffectiveAttack[i]);

                NewEffect.ListBreakingAttack = new List<string>(ListBreakingAttack.Count);
                for (int i = ListBreakingAttack.Count - 1; i >= 0; --i)
                    NewEffect.ListBreakingAttack.Add(ListBreakingAttack[i]);

                NewEffect.ListBreakingSkill = new List<string>(ListBreakingSkill.Count);
                for (int i = ListBreakingSkill.Count - 1; i >= 0; --i)
                    NewEffect.ListBreakingSkill.Add(ListBreakingSkill[i]);

                return new ScriptShieldUnit(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToShieldID
            {
                get
                {
                    return _UnitToShieldID;
                }
                set
                {
                    _UnitToShieldID = value;
                }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute(".")]
            public BarrierTypes BarrierType
            {
                get { return _BarrierType; }
                set { _BarrierType = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute(".")]
            public string ENCost
            {
                get { return _ENCost; }
                set { _ENCost = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute(".")]
            public Operators.NumberTypes NumberType
            {
                get { return _NumberType; }
                set { _NumberType = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute(".")]
            public string DamageReduction
            {
                get { return _DamageReduction; }
                set { _DamageReduction = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute(".")]
            public string BreakingDamage
            {
                get { return _BreakingDamage; }
                set { _BreakingDamage = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute("."),
            Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor)),
            TypeConverter(typeof(CsvConverter))]
            public List<string> EffectiveAttacks
            {
                get { return ListEffectiveAttack; }
                set { ListEffectiveAttack = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute("."),
            Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor)),
            TypeConverter(typeof(CsvConverter))]
            public List<string> BreakingAttacks
            {
                get { return ListBreakingAttack; }
                set { ListBreakingAttack = value; }
            }

            [CategoryAttribute("Effect Attributes"),
            DescriptionAttribute("."),
            Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor)),
            TypeConverter(typeof(CsvConverter))]
            public List<string> BreakingSkills
            {
                get { return ListBreakingSkill; }
                set { ListBreakingSkill = value; }
            }

            #endregion
        }
    }
}
