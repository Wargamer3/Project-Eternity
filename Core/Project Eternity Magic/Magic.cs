using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Roslyn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Magic
{
    public class EffectActivationExecuteOnly : AutomaticSkillTargetType
    {
        public static string Name = "Execute Only";

        public EffectActivationExecuteOnly()
            : base(Name)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return true;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            ActiveSkillEffect.ExecuteEffect();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationExecuteOnly();
        }
    }

    public class EffectActivationSelf : AutomaticSkillTargetType
    {
        public static string Name = "Self";

        private MagicUserContext GlobalContext;

        public EffectActivationSelf(MagicUserContext GlobalContext)
            : base(Name)
        {
            this.GlobalContext = GlobalContext;
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return true;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            GlobalContext.ActiveUser = GlobalContext.ActiveTarget;
            GlobalContext.ActiveUser.Effects.AddAndExecuteEffectWithoutCopy(ActiveSkillEffect, SkillName);
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSelf(GlobalContext);
        }
    }
    
    public class MagicCoreAttributes
    {
        public BaseEffect CurrentSpell;
        public BaseAutomaticSkill CurrentSkill;
        public int TotalManaCost;
    }

    public class MagicSpell
    {
        private List<BaseAutomaticSkill> ListMagicSpell;
        public List<MagicCore> ListMagicCore;//Used to design the magic spells
        public MagicUserContext GlobalContext;
        public readonly string Name;

        public MagicSpell(string Name, IMagicUser Owner, MagicUserContext GlobalContext, Dictionary<string, MagicElement> DicMagicElement)
        {
            this.Name = Name;

            ListMagicSpell = new List<BaseAutomaticSkill>();
            ListMagicCore = new List<MagicCore>();
            this.GlobalContext = GlobalContext;
            this.GlobalContext.ActiveUser = Owner;
            this.GlobalContext.ActiveTarget = Owner;

            FileStream FS = new FileStream("Content/Spells/" + Name + ".pes", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            int ListMagicCoreCount = BR.ReadInt32();
            for (int C = 0; C < ListMagicCoreCount; ++C)
            {
                string NewMagicCoreName = BR.ReadString();
                MagicCore NewMagicCore = (MagicCore)DicMagicElement[NewMagicCoreName].Copy();
                NewMagicCore.Load(BR, DicMagicElement);
                ListMagicCore.Add(NewMagicCore);
            }

            FS.Close();
            BR.Close();
        }

        public MagicSpell(MagicSpell Other, IMagicUser Owner)
        {
            Name = Other.Name;

            ListMagicSpell = new List<BaseAutomaticSkill>();
            ListMagicCore = new List<MagicCore>();
            GlobalContext = Other.GlobalContext;
            GlobalContext.ActiveUser = Owner;
            GlobalContext.ActiveTarget = Owner;

            for (int C = 0; C < Other.ListMagicCore.Count; ++C)
            {
                MagicCore NewMagicCore = (MagicCore)Other.ListMagicCore[C].Copy();
                NewMagicCore.CopyElements(Other.ListMagicCore[C]);
                ListMagicCore.Add(NewMagicCore);
            }
        }

        public MagicSpell(IMagicUser Owner)
        {
            ListMagicSpell = new List<BaseAutomaticSkill>();
            ListMagicCore = new List<MagicCore>();
            GlobalContext = new MagicUserContext();
            GlobalContext.ActiveUser = Owner;
            GlobalContext.ActiveTarget = Owner;
        }

        public MagicSpell(IMagicUser Owner, IMagicUser Target)
        {
            ListMagicSpell = new List<BaseAutomaticSkill>();
            ListMagicCore = new List<MagicCore>();
            GlobalContext = new MagicUserContext();
            GlobalContext.ActiveUser = Owner;
            GlobalContext.ActiveTarget = Target;
        }

        public List<BaseAutomaticSkill> ComputeSpell()
        {
            ListMagicSpell.Clear();
            MagicCoreAttributes ActiveCoreAttributes = new MagicCoreAttributes();

            foreach (MagicCore ActiveMagicCore in ListMagicCore)
            {
                ActiveMagicCore.Compute(ActiveCoreAttributes, GlobalContext);

                ListMagicSpell.Add(ActiveCoreAttributes.CurrentSkill);
            }

            return ListMagicSpell;
        }

        public void ExecuteSpell()
        {
            foreach (BaseAutomaticSkill ActiveMagicSpell in ListMagicSpell)
            {
                ActiveMagicSpell.AddSkillEffectsToTarget("Shoot");
            }
        }

        public void InitGraphics(ContentManager Content)
        {
            for (int C = 0; C < ListMagicCore.Count; ++C)
            {
                ListMagicCore[C].InitGraphics(Content);
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListMagicCore.Count);
            for (int C = 0; C < ListMagicCore.Count; ++C)
            {
                ListMagicCore[C].Save(BW);
            }
        }
    }

    public abstract class MagicElement
    {
        public Vector2 Position;
        public float Radius;
        public bool IsValid;
        List<float> ListMagicLinkAngle;
        readonly bool CanRotate;
        int ManaCost;
        public List<MagicElement> ListLinkedMagicElement;
        public string Name;
        protected MagicUserParams MagicParams;

        private MagicElementAttribute[] _ArrayAttributes;
        public MagicElementAttribute[] ArrayAttributes { get { return _ArrayAttributes; } }

        protected MagicElement(string Name, bool CanRotate, int Radius, MagicUserParams MagicParams)
        {
            _ArrayAttributes = new MagicElementAttribute[0];

            this.Name = Name;
            this.CanRotate = CanRotate;
            this.Radius = Radius;

            if (MagicParams != null)
            {
                this.MagicParams = new MagicUserParams(MagicParams);
            }

            ListLinkedMagicElement = new List<MagicElement>();
            ManaCost = 0;
            IsValid = true;
        }

        protected void SetAttributes(params MagicElementAttribute[] ArrayAttributes)
        {
            _ArrayAttributes = ArrayAttributes;
        }

        internal void CopyElements(MagicElement Other)
        {
            for (int E = 0; E < Other.ListLinkedMagicElement.Count; ++E)
            {
                MagicElement NewMagicElement = Other.ListLinkedMagicElement[E].Copy();
                NewMagicElement.CopyElements(Other.ListLinkedMagicElement[E]);
                ListLinkedMagicElement.Add(NewMagicElement);
            }
        }

        public virtual void Compute(MagicCoreAttributes Attributes, MagicUserContext GlobalContext)
        {
            Attributes.TotalManaCost += ManaCost;

            foreach (MagicElement ActiveListMagicElement in ListLinkedMagicElement)
            {
                ActiveListMagicElement.Compute(Attributes, GlobalContext);
            }
        }

        public abstract MagicElement Copy();

        public void InitGraphics(ContentManager Content)
        {
            foreach (MagicElementAttribute ActiveAttribute in ArrayAttributes)
            {
                ActiveAttribute.InitGraphics(Content);
            }
        }

        internal void Load(BinaryReader BR, Dictionary<string, MagicElement> DicMagicElement)
        {
            Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());

            DoLoad(BR);

            int ListLinkedMagicElementCount = BR.ReadInt32();
            for (int C = 0; C < ListLinkedMagicElementCount; ++C)
            {
                string NewMagicElementName = BR.ReadString();
                MagicElement NewMagicElement = DicMagicElement[NewMagicElementName].Copy();
                NewMagicElement.Load(BR, DicMagicElement);
                ListLinkedMagicElement.Add(NewMagicElement);
            }
        }

        protected abstract void DoLoad(BinaryReader BR);

        internal void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(Position.X);
            BW.Write(Position.Y);
            DoSave(BW);

            BW.Write(ListLinkedMagicElement.Count);
            for (int E = 0; E < ListLinkedMagicElement.Count; ++E)
            {
                ListLinkedMagicElement[E].Save(BW);
            }
        }

        protected abstract void DoSave(BinaryWriter BW);

        public static Dictionary<string, MagicElement> LoadAllMagicElements()
        {
            Dictionary<string, MagicElement> DicMagicElement = LoadFromAssemblyFiles(Directory.GetFiles("Magic", "*.dll", SearchOption.AllDirectories), typeof(MagicElement));

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Magic", "*.csx", SearchOption.AllDirectories);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, MagicElement> ActiveEffect in LoadFromAssembly(ActiveAssembly, typeof(MagicElement)))
                {
                    DicMagicElement.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicMagicElement;
        }

        public static Dictionary<string, MagicElement> LoadFromAssembly(Assembly ActiveAssembly, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, MagicElement> DicEffect = new Dictionary<string, MagicElement>();

            List<MagicElement> ListSkillEffect = ReflectionHelper.GetObjectsFromBaseTypes<MagicElement>(TypeOfRequirement, ActiveAssembly.GetTypes(), Args);

            foreach (MagicElement Instance in ListSkillEffect)
            {
                DicEffect.Add(Instance.Name, Instance);
            }

            return DicEffect;
        }

        public static Dictionary<string, MagicElement> LoadRegularCore(MagicUserParams Params)
        {
            Dictionary<string, MagicElement> DicMagicCoreByType = new Dictionary<string, MagicElement>();

            string[] Files = Directory.GetFiles("Magic", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MagicElement> ListMagicCore = ReflectionHelper.GetObjectsFromTypes<MagicElement>(typeof(MagicCore), ActiveAssembly.GetTypes(), Params);

                foreach (MagicElement Instance in ListMagicCore)
                {
                    DicMagicCoreByType.Add(Instance.Name, Instance);
                }
            }

            return DicMagicCoreByType;
        }

        public static Dictionary<string, MagicElement> LoadProjectileCores(MagicUserParams Params, ProjectileParams AttackParams)
        {
            Dictionary<string, MagicElement> DicMagicCoreByType = new Dictionary<string, MagicElement>();

            string[] Files = Directory.GetFiles("Magic", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MagicElement> ListMagicCore = ReflectionHelper.GetObjectsFromTypes<MagicElement>(typeof(ProjectileMagicCore), ActiveAssembly.GetTypes(), Params, AttackParams);

                foreach (MagicElement Instance in ListMagicCore)
                {
                    DicMagicCoreByType.Add(Instance.Name, Instance);
                }
            }

            return DicMagicCoreByType;
        }

        public static Dictionary<string, MagicElement> LoadElements(MagicUserParams Params)
        {
            Dictionary<string, MagicElement> DicMagicCoreByType = new Dictionary<string, MagicElement>();

            string[] Files = Directory.GetFiles("Magic", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MagicElement> ListMagicCore = ReflectionHelper.GetObjectsFromTypes<MagicElement>(typeof(MagicElement), ActiveAssembly.GetTypes(), Params);

                foreach (MagicElement Instance in ListMagicCore)
                {
                    DicMagicCoreByType.Add(Instance.Name, Instance);
                }
            }

            return DicMagicCoreByType;
        }

        public static Dictionary<string, MagicElement> LoadFromAssemblyFiles(string[] ArrayFilePath, Type TypeOfRequirement, params object[] Args)
        {
            Dictionary<string, MagicElement> DicEffect = new Dictionary<string, MagicElement>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, MagicElement> ActiveEffect in LoadFromAssembly(ActiveAssembly, TypeOfRequirement, Args))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            return DicEffect;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract class MagicCore : MagicElement
    {
        private int BasePower;
        private int RequiredMana;

        protected MagicCore(string Name, int BasePower, int RequiredMana, int Radius, MagicUserParams MagicParams)
            : base(Name, false, Radius, MagicParams)
        {
            this.BasePower = BasePower;
            this.RequiredMana = RequiredMana;
        }

        public override void Compute(MagicCoreAttributes Attributes, MagicUserContext GlobalContext)
        {
            IMagicUser OriginalUser = GlobalContext.ActiveUser;
            IMagicUser OriginalTarget = GlobalContext.ActiveTarget;
            Attributes.TotalManaCost += RequiredMana;
            MagicEffect SpellEffect = GetSpellEffect();
            BaseAutomaticSkill BaseSkill = CreateSpell(SpellEffect, GlobalContext);
            GlobalContext.ActiveTarget = SpellEffect;

            //Linked magic cores are considered as following skills
            if (Attributes.CurrentSpell != null)
            {
                GlobalContext.ActiveUser = OriginalTarget;
                Attributes.CurrentSpell.ListFollowingSkill.Add(BaseSkill);
                BaseSkill.CurrentSkillLevel.ListActivation[1].ListRequirement.Add(new ManaChanneledRequirement(Attributes.TotalManaCost, MagicParams));
            }
            else
            {
                BaseSkill.CurrentSkillLevel.ListActivation[1].ListRequirement.Add(new ManaChanneledRequirement(Attributes.TotalManaCost, MagicParams));
                //Add a final effect that will empty the channeled mana on its own after every other effects are executed.
                BaseSkill.CurrentSkillLevel.ListActivation[1].ListEffect.Add(new EmptyChanneledManaEffect(MagicParams));
            }

            Attributes.CurrentSkill = BaseSkill;
            Attributes.CurrentSpell = SpellEffect;

            foreach (MagicElement ActiveListMagicElement in ListLinkedMagicElement)
            {
                ActiveListMagicElement.Compute(Attributes, GlobalContext);

                Attributes.CurrentSkill = BaseSkill;
                Attributes.CurrentSpell = SpellEffect;
            }

            BaseSkill.CurrentSkillLevel.ListActivation[1].ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            GlobalContext.ActiveUser = OriginalUser;
            GlobalContext.ActiveTarget = OriginalTarget;
        }

        private BaseAutomaticSkill CreateSpell(BaseEffect CurrentSpell, MagicUserContext GlobalContext)
        {
            BaseAutomaticSkill NewSkill = BaseAutomaticSkill.CreateDummy(Name);

            //Insert a second activation use only for passive effects, should never have any requirement. Insert at 0 so they're processed before the spell requirements.
            NewSkill.CurrentSkillLevel.ListActivation.Insert(0, new BaseSkillActivation());

            NewSkill.CurrentSkillLevel.ListActivation[1].ListEffect.Add(CurrentSpell);
            NewSkill.CurrentSkillLevel.ListActivation[1].ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationSelf(GlobalContext) });

            return NewSkill;
        }

        public abstract MagicEffect GetSpellEffect();
    }

    public abstract class ProjectileMagicCore : MagicCore
    {
        protected ProjectileParams AttackParams;

        protected ProjectileMagicCore(string Name, int BasePower, int RequiredMana, int Radius,
            MagicUserParams Params, ProjectileParams AttackParams)
            : base(Name, BasePower, RequiredMana, Radius, Params)
        {
            this.AttackParams = new ProjectileParams(AttackParams);
        }
    }
}
