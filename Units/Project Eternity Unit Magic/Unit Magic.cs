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

        public readonly ProjectileContext GlobalProjectileContext;
        public readonly ProjectileParams MagicProjectileParams;

        public readonly MagicUserContext GlobalMagicContext;
        public readonly MagicUserParams MagicParams;

        private BattleMap Map;

        public UnitMagic()
            : base(null)
        {
            ArrayCharacterActive = new Core.Characters.Character[0];
            ListMagicSpell = new List<MagicSpell>();
            GlobalProjectileContext = new ProjectileContext();
            MagicProjectileParams = new ProjectileParams(GlobalProjectileContext);

            GlobalMagicContext = new MagicUserContext();
            MagicParams = new MagicUserParams(GlobalMagicContext);
        }

        public UnitMagic(List<MagicSpell> ListMagicSpell)
            : this()
        {
            this.ListMagicSpell = ListMagicSpell;
        }

        public UnitMagic(BattleMap Map)
            : this()
        {
            this.Map = Map;
        }

        public UnitMagic(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect)
        {
        }

        public UnitMagic(string Name, ContentManager Content, BattleMap Map, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base(Name)
        {
            this.Map = Map;
            ArrayCharacterActive = new Core.Characters.Character[0];
            ListMagicSpell = new List<MagicSpell>();
            GlobalProjectileContext = new ProjectileContext();
            MagicProjectileParams = new ProjectileParams(GlobalProjectileContext);

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
                OriginalUnit = Unit.FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect);
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
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + "\\Map Sprite\\" + this.FullName);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + "\\Unit Sprite\\" + Name + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + "\\Unit Sprite\\" + this.FullName);
            }

            FS.Close();
            BR.Close();
        }


        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitMagic Other = (UnitMagic)InitializedUnitBase;
            Map = Other.Map;

            if (OriginalUnit == null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect);
                _UnitStat = OriginalUnit.UnitStat;
            }
        }

        public override void OnTurnEnd(Squad ActiveSquad)
        {
        }

        public override List<ActionPanel> OnMenuMovement(Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return null;
        }

        public override List<ActionPanel> OnMenuSelect(Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return new List<ActionPanel>() { new ActionPanelSpellSelection(Map, this), new ActionPanelChannelExternalMana(ListActionMenuChoice, this) };
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            if (Map == null)
            {
                return new UnitMagic(Name, Content, null, DicRequirement, DicEffect);
            }
            else
            {
                return new UnitMagic(Name, Content, Map, Map.DicUnitType, DicRequirement, DicEffect);
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
                Attack SpellAttack = new Attack();
                SpellAttack.ItemName = ActiveMagicSpell.Name;
                SpellAttack.FullName = ActiveMagicSpell.Name;
                SpellAttack.PowerFormula = "30";
                SpellAttack.RangeMinimum = 1;
                SpellAttack.RangeMaximum = 10;
                SpellAttack.Ammo = 10;
                SpellAttack.MaxAmmo = 10;
                SpellAttack.ArrayAttackAttributes = new BaseAutomaticSkill[0];
                SpellAttack.Animations[0] = new MagicAttackAnimationStartInfo("Default Animations/Idle", ActiveMagicSpell, GlobalProjectileContext, MagicProjectileParams.SharedParams);
                for (int A = 1; A < SpellAttack.Animations.Count; ++A)
                {
                    SpellAttack.Animations[A] = new MagicAttackAnimationInfo("Default Animations/Attack", ActiveMagicSpell, GlobalProjectileContext, MagicProjectileParams.SharedParams);
                }
                ListAttack.Add(SpellAttack);
            }
        }

        public override GameScreen GetCustomizeScreen()
        {
            return new MagicEditor(ListMagicSpell[0], GlobalProjectileContext, MagicProjectileParams.SharedParams);
        }
    }
}
