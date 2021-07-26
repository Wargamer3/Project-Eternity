using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptStartBattleSequence : DeathmatchMapScript
        {
            private bool IsInit;

            private uint _RightUnitID;
            private uint _LeftUnitID;
            private Squad RightSquad;
            private Squad LeftSquad;

            private string _RightUnitWeaponName;
            private string _LeftUnitWeaponName;
            private bool _HorizontalMirror;

            private int _RightUnitAttackDamage;
            private int _LeftUnitAttackDamage;

            private List<GameScreen> ListNextAnimationScreen;

            public ScriptStartBattleSequence()
                : this(null)
            {
                _RightUnitWeaponName = "";
                DefendingUnitWeaponName = "";

                IsInit = false;
                IsEnded = false;
            }

            public ScriptStartBattleSequence(DeathmatchMap Map)
                : base(Map, 100, 50, "Start Battle Sequence", new string[] { "Start" }, new string[] { "Sequence Ended" })
            {
                _RightUnitWeaponName = "";
                DefendingUnitWeaponName = "";
                ListNextAnimationScreen = new List<GameScreen>();

                IsInit = false;
                IsEnded = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!IsInit)
                {
                    IsInit = true;
                    for (int P = 0; P < Map.ListPlayer.Count && (RightSquad == null || LeftSquad == null); P++)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && (RightSquad == null || LeftSquad == null); U++)
                        {
                            if (Map.ListPlayer[P].ListSquad[U].ID == _RightUnitID)
                            {
                                RightSquad = Map.ListPlayer[P].ListSquad[U];
                                Map.ActivePlayerIndex = P;
                                Map.ActiveSquadIndex = U;
                            }
                            else if (Map.ListPlayer[P].ListSquad[U].ID == _LeftUnitID)
                            {
                                LeftSquad = Map.ListPlayer[P].ListSquad[U];
                                Map.TargetPlayerIndex = P;
                                Map.TargetSquadIndex = U;
                            }
                            else
                                continue;
                        }
                    }

                    if (RightSquad == null || LeftSquad == null)
                    {
                        ExecuteEvent(this, 0);
                        IsEnded = true;
                        return;
                    }

                    BattleMap.SquadBattleResult RightUnitResult = new BattleMap.SquadBattleResult();
                    BattleMap.SquadBattleResult LeftUnitResult = new BattleMap.SquadBattleResult();
                    RightUnitResult.ArrayResult = new BattleResult[1] { new BattleResult() };
                    LeftUnitResult.ArrayResult = new BattleResult[1] { new BattleResult() };

                    RightUnitResult.ArrayResult[0].AttackDamage = _RightUnitAttackDamage;
                    LeftUnitResult.ArrayResult[0].AttackDamage = _LeftUnitAttackDamage;

                    Squad Attacker = RightSquad;
                    Squad Defender = LeftSquad;

                    //Play battle theme.
                    if (Attacker.CurrentLeader.BattleTheme == null || Attacker.CurrentLeader.BattleThemeName != GameScreen.FMODSystem.sndActiveBGMName)
                    {
                        if (Attacker.CurrentLeader.BattleTheme != null)
                        {
                            GameScreen.FMODSystem.sndActiveBGM.Stop();
                            Attacker.CurrentLeader.BattleTheme.SetLoop(true);
                            Attacker.CurrentLeader.BattleTheme.PlayAsBGM();
                            GameScreen.FMODSystem.sndActiveBGMName = Attacker.CurrentLeader.BattleThemeName;
                        }
                    }

                    RightSquad.CurrentLeader.AttackIndex = 0;

                    if (!string.IsNullOrWhiteSpace(AttackingUnitWeaponName))
                    {
                        for (int A = 0; A < RightSquad.CurrentLeader.ListAttack.Count; ++A)
                        {
                            if (RightSquad.CurrentLeader.ListAttack[A].ItemName == AttackingUnitWeaponName)
                            {
                                RightSquad.CurrentLeader.AttackIndex = A;
                            }
                        }
                    }

                    LeftSquad.CurrentLeader.AttackIndex = 0;

                    if (!string.IsNullOrWhiteSpace(AttackingUnitWeaponName))
                    {
                        for (int A = 0; A < LeftSquad.CurrentLeader.ListAttack.Count; ++A)
                        {
                            if (LeftSquad.CurrentLeader.ListAttack[A].ItemName == DefendingUnitWeaponName)
                            {
                                LeftSquad.CurrentLeader.AttackIndex = A;
                            }
                        }
                    }

                    SquadBattleResult AttackingResult = Map.CalculateFinalHP(RightSquad, null, 0,
                                                                            FormationChoices.Focused, LeftSquad, null, 1, true, true);
                    AttackingResult.ArrayResult[0].AttackMissed = false;
                    AttackingResult.ArrayResult[0].AttackDamage = AttackingUnitAttackDamage;

                    AnimationScreen.AnimationUnitStats UnitStats = new AnimationScreen.AnimationUnitStats(RightSquad, LeftSquad, true);

                    if (IsCounterattack)
                    {
                        ListNextAnimationScreen = Map.GenerateNextAnimationScreens(RightSquad, new SupportSquadHolder(), LeftSquad, new SupportSquadHolder(), UnitStats, AnimationScreen.BattleAnimationTypes.RightConteredByLeft, AttackingResult);
                    }
                    else
                    {
                        ListNextAnimationScreen = Map.GenerateNextAnimationScreens(RightSquad, new SupportSquadHolder(), LeftSquad, new SupportSquadHolder(), UnitStats, AnimationScreen.BattleAnimationTypes.RightAttackLeft, AttackingResult);
                    }
                }
                else
                {
                    if (!Map.ListGameScreen.Contains(ListNextAnimationScreen[0]))
                    {
                        ListNextAnimationScreen.Remove(ListNextAnimationScreen[0]);

                        if (ListNextAnimationScreen.Count > 0)
                        {
                            Map.PushScreen(ListNextAnimationScreen[0]);
                        }
                        else
                        {
                            ExecuteEvent(this, 0);
                            IsEnded = true;
                        }
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                AttackingUnitID = BR.ReadUInt32();
                AttackingUnitWeaponName = BR.ReadString();
                AttackingUnitAttackDamage = BR.ReadInt32();
                DefendingUnitID = BR.ReadUInt32();
                DefendingUnitWeaponName = BR.ReadString();
                DefendingUnitAttackDamage = BR.ReadInt32();
                IsCounterattack = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(AttackingUnitID);
                BW.Write(AttackingUnitWeaponName);
                BW.Write(AttackingUnitAttackDamage);
                BW.Write(DefendingUnitID);
                BW.Write(DefendingUnitWeaponName);
                BW.Write(DefendingUnitAttackDamage);
                BW.Write(IsCounterattack);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptStartBattleSequence(Map);
            }

            #region Properties

            [CategoryAttribute("Right Unit"),
            DescriptionAttribute("The ID of the right Unit.")]
            public UInt32 AttackingUnitID
            {
                get
                {
                    return _RightUnitID;
                }
                set
                {
                    _RightUnitID = value;
                }
            }

            [CategoryAttribute("Left Unit"),
            DescriptionAttribute("The ID of the left Unit.")]
            public UInt32 DefendingUnitID
            {
                get
                {
                    return _LeftUnitID;
                }
                set
                {
                    _LeftUnitID = value;
                }
            }

            [Editor(typeof(Selectors.WeaponSelector), typeof(UITypeEditor)),
            CategoryAttribute("Right Unit"),
            DescriptionAttribute("The name of the right Unit's Weapon.")]
            public string AttackingUnitWeaponName
            {
                get
                {
                    return _RightUnitWeaponName;
                }
                set
                {
                    _RightUnitWeaponName = value;
                }
            }

            [Editor(typeof(Selectors.WeaponSelector), typeof(UITypeEditor)),
            CategoryAttribute("Left Unit"),
            DescriptionAttribute("The name of the left Unit's Weapon.")]
            public string DefendingUnitWeaponName
            {
                get
                {
                    return _LeftUnitWeaponName;
                }
                set
                {
                    _LeftUnitWeaponName = value;
                }
            }

            [CategoryAttribute("Right Unit"),
            DescriptionAttribute("How much damage the right Unit's attack will do.")]
            public int AttackingUnitAttackDamage
            {
                get
                {
                    return _RightUnitAttackDamage;
                }
                set
                {
                    _RightUnitAttackDamage = value;
                }
            }

            [CategoryAttribute("Left Unit"),
            DescriptionAttribute("How much damage the left Unit's attack will do.")]
            public int DefendingUnitAttackDamage
            {
                get
                {
                    return _LeftUnitAttackDamage;
                }
                set
                {
                    _LeftUnitAttackDamage = value;
                }
            }

            [CategoryAttribute("Battle Attributes"),
            DescriptionAttribute("Which side will attack first.")]
            public bool IsCounterattack
            {
                get
                {
                    return _HorizontalMirror;
                }
                set
                {
                    _HorizontalMirror = value;
                }
            }

            #endregion
        }
    }
}
