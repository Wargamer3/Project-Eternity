using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.Units.Magic
{
    public class UnitMagic : Unit, IMagicUser
    {
        public override string UnitTypeName => "Magic";

        public float CurrentMana { get; set; }
        public float ChanneledMana { get; set; }
        public float ManaReserves { get; set; }

        public EffectHolder Effects => Pilot.Effects;

        public List<IMagicUser> ListLinkedUser => throw new NotImplementedException();

        private Unit OriginalUnit;
        public readonly string OriginalUnitName;
        public List<MagicSpell> ListMagicSpell;

        public readonly Projectile2DContext GlobalProjectileContext;
        public readonly Projectile2DParams MagicProjectileParams;

        public readonly MagicUserContext GlobalMagicContext;
        public readonly MagicUserParams MagicParams;

        private BattleParams Params;

        public UnitMagic()
            : base()
        {
            ArrayCharacterActive = new Core.Characters.Character[0];
            ListMagicSpell = new List<MagicSpell>();
            GlobalProjectileContext = new Projectile2DContext();
            MagicProjectileParams = new Projectile2DParams(GlobalProjectileContext);

            GlobalMagicContext = new MagicUserContext();
            MagicParams = new MagicUserParams(GlobalMagicContext);
        }

        public UnitMagic(string RelativePath, List<MagicSpell> ListMagicSpell)
            : base(RelativePath)
        {
            this.ListMagicSpell = ListMagicSpell;
        }

        public UnitMagic(BattleParams Params)
            : this()
        {
            this.Params = Params;
        }

        public UnitMagic(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitMagic(string Name, ContentManager Content, BattleParams Params, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name)
        {
            this.Params = Params;
            this.ItemName = Name;
            ArrayCharacterActive = new Core.Characters.Character[0];
            ListMagicSpell = new List<MagicSpell>();
            GlobalProjectileContext = new Projectile2DContext();
            MagicProjectileParams = new Projectile2DParams(GlobalProjectileContext);

            GlobalMagicContext = new MagicUserContext();
            MagicParams = new MagicUserParams(GlobalMagicContext);

            Dictionary<string, MagicElement> DicMagicElement = MagicElement.LoadRegularCore(MagicParams);
            foreach (KeyValuePair<string, MagicElement> ActiveMagicElement in MagicElement.LoadProjectileCores(MagicParams, MagicProjectileParams))
            {
                DicMagicElement.Add(ActiveMagicElement.Key, ActiveMagicElement.Value);
            }
            foreach (KeyValuePair<string, MagicElement> ActiveMagicElement in MagicElement.LoadElements(MagicParams))
            {
                DicMagicElement.Add(ActiveMagicElement.Key, ActiveMagicElement.Value);
            }

            FileStream FS = new FileStream("Content/Units/Magic/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            OriginalUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(OriginalUnitName) && DicUnitType != null)
            {
                OriginalUnit = Unit.FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                _UnitStat = OriginalUnit.UnitStat;
            }

            int ListMagicSpellCount = BR.ReadInt32();
            ListMagicSpell = new List<MagicSpell>(ListMagicSpellCount);
            for (int S = 0; S < ListMagicSpellCount; ++S)
            {
                ListMagicSpell.Add(new MagicSpell(BR.ReadString(), this, GlobalMagicContext, DicMagicElement));
            }

            if (Content != null)
            {
                string UnitDirectory = Path.GetDirectoryName("Content\\Units\\Normal\\" + Name);
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + "\\Map Sprite\\" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + "\\Map Sprite\\" + this.RelativePath);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + "\\Unit Sprite\\" + Name + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + "\\Unit Sprite\\" + this.RelativePath);
            }

            FS.Close();
            BR.Close();
        }


        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitMagic Other = (UnitMagic)InitializedUnitBase;
            Params = Other.Params;

            if (OriginalUnit == null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Params.Map.Content, Params.DicUnitType, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                _UnitStat = OriginalUnit.UnitStat;
            }
        }

        public override void OnTurnEnd(int ActivePlayerIndex, Squad ActiveSquad)
        {
        }

        public override List<ActionPanel> OnMenuMovement(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return null;
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return new List<ActionPanel>() { new ActionPanelSpellSelection(Params.Map, this), new ActionPanelChannelExternalMana(ListActionMenuChoice, this) };
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            if (Params == null || Params.Map == null)
            {
                return new UnitMagic(Name, Content, null, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            else
            {
                return new UnitMagic(Name, Content, Params, Params.DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
        }

        public override void DoInit()
        {
            CreateAttackFromMagic();
        }

        private void CreateAttackFromMagic()
        {
            foreach (MagicSpell ActiveMagicSpell in ListMagicSpell)
            {
                Attack SpellAttack = new Attack(ActiveMagicSpell.Name);
                SpellAttack.ItemName = ActiveMagicSpell.Name;
                SpellAttack.PowerFormula = "30";
                SpellAttack.RangeMinimum = 1;
                SpellAttack.RangeMaximum = 10;
                SpellAttack.SetMaxAmmo(10);
                SpellAttack.RefillAmmo();
                SpellAttack.ArrayAttackAttributes = new BaseAutomaticSkill[0];
                SpellAttack.Animations[0].Animations[0] = new MagicAttackAnimationStartInfo("Default Animations/Idle", ActiveMagicSpell, GlobalProjectileContext, MagicProjectileParams.SharedParams);
                for (int A = 1; A < SpellAttack.Animations.Count; ++A)
                {
                    SpellAttack.Animations[0].Animations[A] = new MagicAttackAnimationInfo("Default Animations/Attack", ActiveMagicSpell, GlobalProjectileContext, MagicProjectileParams.SharedParams);
                }
                ListAttack.Add(SpellAttack);
            }
        }

        public override GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            return new MagicEditor(ListMagicSpell[0], GlobalProjectileContext, MagicProjectileParams.SharedParams);
        }
    }
}
