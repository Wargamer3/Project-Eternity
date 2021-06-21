using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Units.Modular
{
    public partial class UnitModular : Unit
    {
        public struct UnitParts
        {
            public PartHead Head;
            public List<string> ListHead;
            public PartTorso Torso;
            public List<string> ListTorso;

            public PartArm LeftArm;
            public List<string> ListLeftArm;
            public PartArm RightArm;
            public List<string> ListRightArm;

            public PartLegs Legs;
            public List<string> ListLegs;

            public PartUnit this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0:
                            return Head;

                        case 1:
                            return Torso;

                        case 2:
                            return LeftArm;

                        case 3:
                            return RightArm;

                        case 4:
                            return Legs;
                    }
                    return null;
                }
                set
                {
                    switch (i)
                    {
                        case 0:
                            Head = (PartHead)value;
                            break;

                        case 1:
                            Torso = (PartTorso)value;
                            break;

                        case 2:
                            LeftArm = (PartArm)value;
                            break;

                        case 3:
                            RightArm = (PartArm)value;
                            break;

                        case 4:
                            Legs = (PartLegs)value;
                            break;
                    }
                }
            }

            public int HP
            {
                get
                {
                    int HP = 0;

                    if (Head != null)
                        HP += Head.HP;
                    if (Torso != null)
                        HP += Torso.HP;
                    if (LeftArm != null)
                        HP += LeftArm.HP;
                    if (RightArm != null)
                        HP += RightArm.HP;
                    if (Legs != null)
                        HP += Legs.HP;

                    return HP;
                }
            }

            public int EN
            {
                get
                {
                    int EN = 0;

                    if (Head != null)
                        EN += Head.EN;
                    if (Torso != null)
                        EN += Torso.EN;
                    if (LeftArm != null)
                        EN += LeftArm.EN;
                    if (RightArm != null)
                        EN += RightArm.EN;
                    if (Legs != null)
                        EN += Legs.EN;

                    return EN;
                }
            }

            public int Armor
            {
                get
                {
                    int Armor = 0;

                    if (Head != null)
                        Armor += Head.Armor;
                    if (Torso != null)
                        Armor += Torso.Armor;
                    if (LeftArm != null)
                        Armor += LeftArm.Armor;
                    if (RightArm != null)
                        Armor += RightArm.Armor;
                    if (Legs != null)
                        Armor += Legs.Armor;

                    return Armor;
                }
            }

            public int Mobility
            {
                get
                {
                    int Mobility = 0;

                    if (Head != null)
                        Mobility += Head.Mobility;
                    if (Torso != null)
                        Mobility += Torso.Mobility;
                    if (LeftArm != null)
                        Mobility += LeftArm.Mobility;
                    if (RightArm != null)
                        Mobility += RightArm.Mobility;
                    if (Legs != null)
                        Mobility += Legs.Mobility;

                    return Mobility;
                }
            }

            public float Movement
            {
                get
                {
                    float Movement = 0;

                    if (Head != null)
                        Movement += Head.Movement;
                    if (Torso != null)
                        Movement += Torso.Movement;
                    if (LeftArm != null)
                        Movement += LeftArm.Movement;
                    if (RightArm != null)
                        Movement += RightArm.Movement;
                    if (Legs != null)
                        Movement += Legs.Movement;

                    return Movement;
                }
            }

            public int MEL
            {
                get
                {
                    int MEL = 0;

                    if (Head != null)
                        MEL += Head.MEL;
                    if (Torso != null)
                        MEL += Torso.MEL;
                    if (LeftArm != null)
                        MEL += LeftArm.MEL;
                    if (RightArm != null)
                        MEL += RightArm.MEL;
                    if (Legs != null)
                        MEL += Legs.MEL;

                    return MEL;
                }
            }

            public int RNG
            {
                get
                {
                    int RNG = 0;

                    if (Head != null)
                        RNG += Head.RNG;
                    if (Torso != null)
                        RNG += Torso.RNG;
                    if (LeftArm != null)
                        RNG += LeftArm.RNG;
                    if (RightArm != null)
                        RNG += RightArm.RNG;
                    if (Legs != null)
                        RNG += Legs.RNG;

                    return RNG;
                }
            }

            public int DEF
            {
                get
                {
                    int DEF = 0;

                    if (Head != null)
                        DEF += Head.DEF;
                    if (Torso != null)
                        DEF += Torso.DEF;
                    if (LeftArm != null)
                        DEF += LeftArm.DEF;
                    if (RightArm != null)
                        DEF += RightArm.DEF;
                    if (Legs != null)
                        DEF += Legs.DEF;

                    return DEF;
                }
            }

            public int SKL
            {
                get
                {
                    int SKL = 0;

                    if (Head != null)
                        SKL += Head.SKL;
                    if (Torso != null)
                        SKL += Torso.SKL;
                    if (LeftArm != null)
                        SKL += LeftArm.SKL;
                    if (RightArm != null)
                        SKL += RightArm.SKL;
                    if (Legs != null)
                        SKL += Legs.SKL;

                    return SKL;
                }
            }

            public int EVA
            {
                get
                {
                    int EVA = 0;

                    if (Head != null)
                        EVA += Head.EVA;
                    if (Torso != null)
                        EVA += Torso.EVA;
                    if (LeftArm != null)
                        EVA += LeftArm.EVA;
                    if (RightArm != null)
                        EVA += RightArm.EVA;
                    if (Legs != null)
                        EVA += Legs.EVA;

                    return EVA;
                }
            }

            public int HIT
            {
                get
                {
                    int HIT = 0;

                    if (Head != null)
                        HIT += Head.HIT;
                    if (Torso != null)
                        HIT += Torso.HIT;
                    if (LeftArm != null)
                        HIT += LeftArm.HIT;
                    if (RightArm != null)
                        HIT += RightArm.HIT;
                    if (Legs != null)
                        HIT += Legs.HIT;

                    return HIT;
                }
            }
        }

        public override string UnitTypeName => "Modular";
        public UnitParts Parts;

        public UnitModular()
        { }

        public UnitModular(string Name, ContentManager Content)
            : base(Name)
        {
            FileStream FS = new FileStream("Content/Units/Modular/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            ArrayCharacterActive = new Character[0];
            this.Description = BR.ReadString();
            this.Price = BR.ReadInt32();

            _UnitStat = new UnitStats(new bool[1, 1] { { true } });
            //Read Pilots whitelist.
            Int32 ListPilotCount = BR.ReadInt32();
            for (int P = 0; P < ListPilotCount; P++)
            {
                string CharacterName = BR.ReadString();

                ListCharacterIDWhitelist.Add(CharacterName);
            }

            #region Load Parts Unit

            Parts.ListHead = new List<string>();
            Parts.ListTorso = new List<string>();
            Parts.ListLeftArm = new List<string>();
            Parts.ListRightArm = new List<string>();
            Parts.ListLegs = new List<string>();

            Int32 ListHeadCount = BR.ReadInt32();
            for (int Head = 0; Head < ListHeadCount; Head++)
            {
                string HeadName = BR.ReadString();
                //Add the Head.
                Parts.ListHead.Add(HeadName);
            }

            Int32 ListTorsoCount = BR.ReadInt32();
            for (int Torso = 0; Torso < ListTorsoCount; Torso++)
            {
                string TorsoName = BR.ReadString();
                //Add the Torso.
                Parts.ListTorso.Add(TorsoName);
            }

            Int32 ListLeftArmCount = BR.ReadInt32();
            for (int LeftArm = 0; LeftArm < ListLeftArmCount; LeftArm++)
            {
                string LeftArmName = BR.ReadString();
                //Add the LeftArm.
                Parts.ListLeftArm.Add(LeftArmName);
            }

            Int32 ListRightArmCount = BR.ReadInt32();
            for (int RightArm = 0; RightArm < ListRightArmCount; RightArm++)
            {
                string RightArmName = BR.ReadString();
                //Add the RightArm.
                Parts.ListRightArm.Add(RightArmName);
            }

            Int32 ListLegsCount = BR.ReadInt32();
            for (int Legs = 0; Legs < ListLegsCount; Legs++)
            {
                string LegsName = BR.ReadString();
                //Add the Legs.
                Parts.ListLegs.Add(LegsName);
            }

            #endregion

            if (Content != null)
            {
                string UnitDirectory = Path.GetDirectoryName("Content\\Units\\Normal\\" + Name);
                string XNADirectory = UnitDirectory.Substring(8);
                string FinalSpriteMapPath = "\\Map Sprite\\" + Name;
                if (string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "\\Map Sprite\\" + SpriteMapPath;

                string FinalSpriteUnitPath = "\\Unit Sprite\\" + Name;
                if (string.IsNullOrEmpty(SpriteUnitPath))
                    FinalSpriteUnitPath = "\\Unit Sprite\\" + SpriteUnitPath;

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
            }

            FS.Close();
            BR.Close();

            InitParts();
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            return new UnitModular(Name, Content);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
            throw new NotImplementedException();
        }
        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
        }

        private void InitParts()
        {
            if (Parts.ListHead.Count > 0)
            {
                Parts.Head = new PartHead(Parts.ListHead[0]);

                if (Parts.Head.ListPartAntena.Count > 0)
                {
                    Parts.Head.Antena = new Part(Parts.Head.ListPartAntena[0], PartTypes.HeadAntena);
                }
                if (Parts.Head.ListPartEars.Count > 0)
                {
                    Parts.Head.Ears = new Part(Parts.Head.ListPartEars[0], PartTypes.HeadEars);
                }
                if (Parts.Head.ListPartEyes.Count > 0)
                {
                    Parts.Head.Eyes = new Part(Parts.Head.ListPartEyes[0], PartTypes.HeadEyes);
                }
                if (Parts.Head.ListPartCPU.Count > 0)
                {
                    Parts.Head.CPU = new Part(Parts.Head.ListPartCPU[0], PartTypes.HeadCPU);
                }
            }

            if (Parts.ListTorso.Count > 0 && Parts.Torso == null)
            {
                Parts.Torso = new PartTorso(Parts.ListTorso[0]);
                if (Parts.Torso.ListPartCore.Count > 0)
                {
                    Parts.Torso.Core = new Part(Parts.Torso.ListPartCore[0], PartTypes.TorsoCore);
                }
                if (Parts.Torso.ListPartRadiator.Count > 0)
                {
                    Parts.Torso.Radiator = new Part(Parts.Torso.ListPartRadiator[0], PartTypes.TorsoRadiator);
                }
                if (Parts.Torso.ListPartShell.Count > 0)
                {
                    Parts.Torso.Shell = new Part(Parts.Torso.ListPartShell[0], PartTypes.Shell);
                }
            }

            if (Parts.ListLeftArm.Count > 0 && Parts.LeftArm == null)
            {
                Parts.LeftArm = new PartArm(Parts.ListLeftArm[0]);
                if (Parts.LeftArm.ListPartShell.Count > 0)
                {
                    Parts.LeftArm.Shell = new Part(Parts.LeftArm.ListPartShell[0], PartTypes.Shell);
                }
                if (Parts.LeftArm.ListPartStrength.Count > 0)
                {
                    Parts.LeftArm.Strength = new Part(Parts.LeftArm.ListPartStrength[0], PartTypes.Strength);
                }
            }

            if (Parts.ListRightArm.Count > 0 && Parts.RightArm == null)
            {
                Parts.RightArm = new PartArm(Parts.ListRightArm[0]);
                if (Parts.RightArm.ListPartShell.Count > 0)
                {
                    Parts.RightArm.Shell = new Part(Parts.RightArm.ListPartShell[0], PartTypes.Shell);
                }
                if (Parts.RightArm.ListPartStrength.Count > 0)
                {
                    Parts.RightArm.Strength = new Part(Parts.RightArm.ListPartStrength[0], PartTypes.Strength);
                }
            }

            if (Parts.ListLegs.Count > 0 && Parts.Legs == null)
            {
                Parts.Legs = new PartLegs(Parts.ListLegs[0]);
                if (Parts.Legs.ListPartShell.Count > 0)
                {
                    Parts.Legs.Shell = new Part(Parts.Legs.ListPartShell[0], PartTypes.Shell);
                }
                if (Parts.Legs.ListPartStrength.Count > 0)
                {
                    Parts.Legs.Strength = new Part(Parts.Legs.ListPartStrength[0], PartTypes.Strength);
                }
            }

            DoInit();
        }

        public override void DoInit()
        {
            //Initialise the Unit stats.
            this.MaxHP = Parts.HP;
            this.MaxEN = Parts.EN;
            this.Armor = Parts.Armor;
            this.Mobility = Parts.Mobility;
            this.MaxMovement = (int)Parts.Movement;
        }

        public override GameScreens.GameScreen GetCustomizeScreen()
        {
            return new PartsEquipScreen(this);
        }

        public List<string> CurrentPartList(int PartType)
        {
            switch (PartType)
            {
                case 0:
                    return Parts.ListHead;

                case 1:
                    return Parts.ListTorso;

                case 2:
                    return Parts.ListLeftArm;

                case 3:
                    return Parts.ListRightArm;

                case 4:
                    return Parts.ListLegs;
            }
            return null;
        }
    }
}
