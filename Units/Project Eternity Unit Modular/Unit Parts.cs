using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Units.Modular
{
    public enum PartUnitTypes { Head = 0, Torso = 1, Arm = 2, Legs = 3 }

    public abstract class PartUnit : ShopItem
    {
        public class WeaponSlot
        {
            public List<string> ListWeaponSlot;
            public int Count { get { return ListWeaponSlot.Count; } }

            public WeaponSlot()
            {
                ListWeaponSlot = new List<string>();
            }
            public WeaponSlot(int ListWeaponSlotCapacity)
            {
                ListWeaponSlot = new List<string>(ListWeaponSlotCapacity);
            }
            public string this[int i]
            {
                get
                {
                    return ListWeaponSlot[i];
                }
            }
        }
        public int BaseSize;//Number of components.
        public List<WeaponSlot> ListWeaponSlot;//List of possible weapons choice.
        public List<int> ActiveWeapons;//List of active weapons.
        //Unit boosts.
        protected int BaseHP, BaseEN, BaseArmor, BaseMobility;
        protected float BaseMovement;

        protected PartUnit(string RelativePath)
            : base(RelativePath)
        {
        }

        protected void LoadBaseData(BinaryReader BR)
        {
            this.Description = BR.ReadString();
            this.Price = BR.ReadInt32();

            this.BaseHP = BR.ReadInt32();
            this.BaseEN = BR.ReadInt32();
            this.BaseArmor = BR.ReadInt32();
            this.BaseMobility = BR.ReadInt32();
            this.BaseMovement = BR.ReadSingle();

            #region Weapons

            int ListWeaponSlotCount = BR.ReadInt32();
            this.ListWeaponSlot = new List<WeaponSlot>(ListWeaponSlotCount);

            for (int S = 0; S < ListWeaponSlotCount; S++)
            {
                int ListWeaponCount = BR.ReadInt32();
                WeaponSlot ListWeapon = new WeaponSlot(ListWeaponCount);
                for (int W = 0; W < ListWeaponCount; W++)
                {
                    string WeaponName = BR.ReadString();
                    ListWeapon.ListWeaponSlot.Add(WeaponName);
                }

                this.ListWeaponSlot.Add(ListWeapon);
            }

            #endregion
        }

        public virtual List<string> this[int i]
        {
            get
            {
                return null;
            }
        }
        public virtual Part this[uint index]
        {
            get
            {
                return null;
            }
            set
            {

            }
        }

        public int Size
        {
            get
            {
                if (ListWeaponSlot != null)
                    return BaseSize + ListWeaponSlot.Count;
                return BaseSize;
            }
        }
        public int HP
        {
            get
            {
                int ReturnValue = BaseHP;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].BaseHP;
                return ReturnValue;
            }
        }
        public int EN
        {
            get
            {
                int ReturnValue = BaseEN;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].BaseEN;
                return ReturnValue;
            }
        }
        public int Armor
        {
            get
            {
                int ReturnValue = BaseArmor;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].BaseArmor;
                return ReturnValue;
            }
        }
        public int Mobility
        {
            get
            {
                int ReturnValue = BaseMobility;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].BaseMobility;
                return ReturnValue;
            }
        }
        public float Movement
        {
            get
            {
                float ReturnValue = BaseMovement;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].BaseMovement;
                return ReturnValue;
            }
        }
        public int MEL
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].MEL;
                return ReturnValue;
            }
        }
        public int RNG
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].RNG;
                return ReturnValue;
            }
        }
        public int DEF
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].DEF;
                return ReturnValue;
            }
        }
        public int SKL
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].SKL;
                return ReturnValue;
            }
        }
        public int EVA
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].EVA;
                return ReturnValue;
            }
        }
        public int HIT
        {
            get
            {
                int ReturnValue = 0;
                for (uint i = 0; i < Size; i++)
                    if (this[i] != null)
                        ReturnValue += this[i].HIT;
                return ReturnValue;
            }
        }
    }

    public class PartHead : PartUnit
    {
        public Part Antena;
        public Part Ears;
        public Part Eyes;
        public Part CPU;
        public List<string> ListPartAntena;
        public List<string> ListPartEars;
        public List<string> ListPartEyes;
        public List<string> ListPartCPU;

        public PartHead(string RelativePath)
            : base(RelativePath)
        {
            FileStream FS = new FileStream("Content/Deathmatch/Units/Modular/Head/" + RelativePath + ".peup", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            LoadBaseData(BR);

            this.BaseSize = 0;
            this.ActiveWeapons = new List<int>();

            #region Specific part components
            this.ListPartAntena = new List<string>();
            this.ListPartEars = new List<string>();
            this.ListPartEyes = new List<string>();
            this.ListPartCPU = new List<string>();

            int ListHeadAntenaCount = BR.ReadInt32();
            for (int Antena = 0; Antena < ListHeadAntenaCount; Antena++)
            {
                string AntenaName = BR.ReadString();
                //Add the Antena.
                this.ListPartAntena.Add(AntenaName);
            }
            if (ListHeadAntenaCount > 0)
                this.BaseSize++;

            int ListHeadEarsCount = BR.ReadInt32();
            for (int Ears = 0; Ears < ListHeadEarsCount; Ears++)
            {
                string EarsName = BR.ReadString();
                //Add the Ears.
                this.ListPartEars.Add(EarsName);
            }
            if (ListHeadEarsCount > 0)
                this.BaseSize++;

            int ListHeadEyesCount = BR.ReadInt32();
            for (int Eyes = 0; Eyes < ListHeadEyesCount; Eyes++)
            {
                string EyesName = BR.ReadString();
                //Add the Eyes.
                this.ListPartEyes.Add(EyesName);
            }
            if (ListHeadEyesCount > 0)
                this.BaseSize++;

            int ListHeadCPUCount = BR.ReadInt32();
            for (int CPU = 0; CPU < ListHeadCPUCount; CPU++)
            {
                string CPUName = BR.ReadString();
                //Add the CPU.
                this.ListPartCPU.Add(CPUName);
            }
            if (ListHeadCPUCount > 0)
                this.BaseSize++;

            #endregion

            FS.Close();
            BR.Close();
        }

        public override List<string> this[int i]
        {
            get
            {
                if (i < BaseSize)
                {
                    switch (i)
                    {
                        case 0:
                            return ListPartAntena;
                        case 1:
                            return ListPartEars;
                        case 2:
                            return ListPartEyes;
                        case 3:
                            return ListPartCPU;
                    }
                }
                return null;
            }
        }
        public override Part this[uint index]
        {
            get
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            return Antena;
                        case 1:
                            return Ears;
                        case 2:
                            return Eyes;
                        case 3:
                            return CPU;
                    }
                }
                return null;
            }
            set
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            Antena = new Part(value);
                            break;
                        case 1:
                            Ears = new Part(value);
                            break;
                        case 2:
                            Eyes = new Part(value);
                            break;
                        case 3:
                            CPU = new Part(value);
                            break;
                    }
                }
            }
        }
    }

    public class PartTorso : PartUnit
    {
        public Part Core;
        public Part Radiator;
        public Part Shell;
        public List<string> ListPartCore;
        public List<string> ListPartRadiator;
        public List<string> ListPartShell;

        public PartTorso(string RelativePath)
            : base(RelativePath)
        {
            FileStream FS = new FileStream("Content/Deathmatch/Units/Modular/Torso/" + RelativePath + ".peup", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            LoadBaseData(BR);

            this.BaseSize = 0;
            this.ActiveWeapons = new List<int>();

            #region Specific part components
            this.ListPartCore = new List<string>();
            this.ListPartRadiator = new List<string>();
            this.ListPartShell = new List<string>();

            int ListTorsoCoreCount = BR.ReadInt32();
            for (int Core = 0; Core < ListTorsoCoreCount; Core++)
            {
                string CoreName = BR.ReadString();
                //Add the Core.
                this.ListPartCore.Add(CoreName);
            }
            if (ListTorsoCoreCount > 0)
                this.BaseSize++;

            int ListTorsoRadiatorCount = BR.ReadInt32();
            for (int Radiator = 0; Radiator < ListTorsoRadiatorCount; Radiator++)
            {
                string RadiatorName = BR.ReadString();
                //Add the Radiator.
                this.ListPartRadiator.Add(RadiatorName);
            }
            if (ListTorsoRadiatorCount > 0)
                this.BaseSize++;

            int ListTorsoShellCount = BR.ReadInt32();
            for (int W = 0; W < ListTorsoShellCount; W++)
            {
                string ShellName = BR.ReadString();
                //Add the Shell.
                this.ListPartShell.Add(ShellName);
            }
            if (ListTorsoShellCount > 0)
                this.BaseSize++;

            #endregion

            FS.Close();
            BR.Close();
        }

        public override List<string> this[int i]
        {
            get
            {
                if (i < BaseSize)
                {
                    switch (i)
                    {
                        case 0:
                            return ListPartCore;
                        case 1:
                            return ListPartRadiator;
                        case 2:
                            return ListPartShell;
                    }
                }
                return null;
            }
        }
        public override Part this[uint index]
        {
            get
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            return Core;
                        case 1:
                            return Radiator;
                        case 2:
                            return Shell;
                    }
                }
                return null;
            }
            set
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            Core = new Part(value);
                            break;
                        case 1:
                            Radiator = new Part(value);
                            break;
                        case 2:
                            Shell = new Part(value);
                            break;
                    }
                }
            }
        }
    }

    public class PartArm : PartUnit
    {
        public Part Shell;
        public Part Strength;
        public List<string> ListPartShell;
        public List<string> ListPartStrength;

        public PartArm(string RelativePath)
            : base(RelativePath)
        {
            FileStream FS = new FileStream("Content/Deathmatch/Units/Modular/Arm/" + RelativePath + ".peup", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            LoadBaseData(BR);

            this.BaseSize = 0;
            this.ActiveWeapons = new List<int>();

            #region Specific part components

            this.ListPartShell = new List<string>();
            this.ListPartStrength = new List<string>();

            int ListArmShellCount = BR.ReadInt32();
            for (int W = 0; W < ListArmShellCount; W++)
            {
                string ShellName = BR.ReadString();
                //Add the Shell.
                this.ListPartShell.Add(ShellName);
            }
            if (ListArmShellCount > 0)
                this.BaseSize++;

            int ListArmStrengthCount = BR.ReadInt32();
            for (int Strength = 0; Strength < ListArmStrengthCount; Strength++)
            {
                string StrengthName = BR.ReadString();
                //Add the Strength.
                this.ListPartStrength.Add(StrengthName);
            }
            if (ListArmStrengthCount > 0)
                this.BaseSize++;

            #endregion

            FS.Close();
            BR.Close();
        }

        public override List<string> this[int i]
        {
            get
            {
                if (i < BaseSize)
                {
                    switch (i)
                    {
                        case 0:
                            return ListPartShell;
                        case 1:
                            return ListPartStrength;
                    }
                }
                return null;
            }
        }
        public override Part this[uint index]
        {
            get
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            return Shell;
                        case 1:
                            return Strength;
                    }
                }
                return null;
            }
            set
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            Shell = new Part(value);
                            break;
                        case 1:
                            Strength = new Part(value);
                            break;
                    }
                }
            }
        }
    }

    public class PartLegs : PartUnit
    {
        public Part Shell;
        public Part Strength;
        public List<string> ListPartShell;
        public List<string> ListPartStrength;

        public PartLegs(string RelativePath)
            : base(RelativePath)
        {
            FileStream FS = new FileStream("Content/Deathmatch/Units/Modular/Legs/" + RelativePath + ".peup", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            LoadBaseData(BR);

            this.BaseSize = 0;
            this.ActiveWeapons = new List<int>();

            #region Specific part components

            this.ListPartShell = new List<string>();
            this.ListPartStrength = new List<string>();

            int ListLegsShellCount = BR.ReadInt32();
            for (int W = 0; W < ListLegsShellCount; W++)
            {
                string ShellName = BR.ReadString();
                //Add the Shell.
                this.ListPartShell.Add(ShellName);
            }
            if (ListLegsShellCount > 0)
                this.BaseSize++;

            int ListLegsStrengthCount = BR.ReadInt32();
            for (int Strength = 0; Strength < ListLegsStrengthCount; Strength++)
            {
                string StrengthName = BR.ReadString();
                //Add the Strength.
                this.ListPartStrength.Add(StrengthName);
            }
            if (ListLegsStrengthCount > 0)
                this.BaseSize++;

            #endregion

            FS.Close();
            BR.Close();
        }

        public override List<string> this[int i]
        {
            get
            {
                if (i < BaseSize)
                {
                    switch (i)
                    {
                        case 0:
                            return ListPartShell;
                        case 1:
                            return ListPartStrength;
                    }
                }
                return null;
            }
        }
        public override Part this[uint index]
        {
            get
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            return Shell;
                        case 1:
                            return Strength;
                    }
                }
                return null;
            }
            set
            {
                if (index < BaseSize)
                {
                    switch (index)
                    {
                        case 0:
                            Shell = new Part(value);
                            break;
                        case 1:
                            Strength = new Part(value);
                            break;
                    }
                }
            }
        }
    }
}