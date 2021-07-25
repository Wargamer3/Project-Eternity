﻿using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public enum StatusTypes : byte { MEL = 0, RNG = 1, DEF = 2, SKL = 3, EVA = 4, HIT = 5 }

    public enum UnitStats : byte { MaxHP, MaxEN, Armor, Mobility, MaxMV }

    public abstract class SkillEffect : BaseEffect
    {
        public const string LifetimeTypePermanent = "Permanent";
        public const string LifetimeTypeTurns = "Turns";
        public const string LifetimeTypeBattle = "Battle";
        public const string LifetimeTypeOnHit = "OnHit";
        public const string LifetimeTypeOnEnemyHit = "OnEnemyHit";
        public const string LifetimeTypeOnAttack = "OnAttack";
        public const string LifetimeTypeOnEnemyAttack = "OnEnemyAttack";
        public const string LifetimeTypeOnAction = "OnAction";

        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        protected readonly UnitEffectParams Params;

        public SkillEffect(string EffectTypeName, bool IsPassive)
           : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        public SkillEffect(string EffectTypeName, bool IsPassive, UnitEffectParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new UnitEffectParams(Params);
                this.Params.CopyGlobalIntoLocal();
                if (this.Params.LocalContext.EffectOwnerUnit != null && GameScreens.GameScreen.Debug != null)
                {
                    List<string> ListDebugText = new List<string>();
                    ListDebugText.Add("The context used was:");

                    if (this.Params.LocalContext.EffectOwnerSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectOwnerSquad.SquadName))
                        ListDebugText.Add("Owner Squad: " + this.Params.LocalContext.EffectOwnerSquad.SquadName);
                    if (this.Params.LocalContext.EffectOwnerUnit != null)
                        ListDebugText.Add("Owner Unit: " + this.Params.LocalContext.EffectOwnerUnit.RelativePath);
                    if (this.Params.LocalContext.EffectOwnerCharacter != null)
                        ListDebugText.Add("Owner Pilot: " + this.Params.LocalContext.EffectOwnerCharacter.FullName);

                    if (this.Params.LocalContext.EffectTargetSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectTargetSquad.SquadName))
                        ListDebugText.Add("Target Squad: " + this.Params.LocalContext.EffectTargetSquad.SquadName);
                    if (this.Params.LocalContext.EffectTargetUnit != null)
                        ListDebugText.Add("Target Unit: " + this.Params.LocalContext.EffectTargetUnit.RelativePath);
                    if (this.Params.LocalContext.EffectTargetCharacter != null)
                        ListDebugText.Add("Target Pilot: " + this.Params.LocalContext.EffectTargetCharacter.FullName);

                    GameScreens.GameScreen.Debug.AddDebugEffect(this, ListDebugText);
                }
            }
        }

        protected override void Load(BinaryReader BR)
        {
            if (LifetimeType == LifetimeTypePermanent)
                LifetimeTypeValue = -1;
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            BW.Write(Params.LocalContext.EffectOwnerSquad.ID);
            BW.Write(Params.LocalContext.EffectOwnerSquad.IndexOf(Params.LocalContext.EffectOwnerUnit));
            BW.Write(Array.IndexOf(Params.LocalContext.EffectOwnerUnit.ArrayCharacterActive, Params.LocalContext.EffectOwnerCharacter));

            BW.Write(Params.LocalContext.EffectTargetSquad.ID);
            BW.Write(Params.LocalContext.EffectOwnerSquad.IndexOf(Params.LocalContext.EffectTargetUnit));
            BW.Write(Array.IndexOf(Params.LocalContext.EffectTargetUnit.ArrayCharacterActive, Params.LocalContext.EffectTargetCharacter));
        }

        public override bool CanActivate()
        {
            return true;
        }
    }
}
