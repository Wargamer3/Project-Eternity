using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WeaponHolder
    {
        private readonly List<Weapon> ListActivePrimaryWeapon;
        private readonly List<Weapon> ListActiveSecondaryWeapon;
        private readonly Dictionary<string, Weapon> DicWeaponByName;
        private readonly List<Weapon> ListHolstereddPrimaryWeapon;

        private int PrimaryWeaponCharge;
        private int SecondaryWeaponCharge;
        private List<string> ListWeaponByIndex;

        public bool HasActiveWeapons { get { return ListActivePrimaryWeapon.Count > 0; } }
        public bool HasWeapons { get { return DicWeaponByName.Count > 0; } }
        public List<Weapon> ActivePrimaryWeapons { get { return ListActivePrimaryWeapon; } }
        public List<Weapon> ActiveSecondaryWeapons { get { return ListActiveSecondaryWeapon; } }
        public int PrimaryCharge { get { return PrimaryWeaponCharge; } }
        public int SecondaryCharge { get { return SecondaryWeaponCharge; } }
        public bool HasHolsteredWeapons { get { return ListHolstereddPrimaryWeapon.Count > 0; } }

        public WeaponHolder(int ListWeaponCount)
        {
            DicWeaponByName = new Dictionary<string, Weapon>(ListWeaponCount);
            ListActivePrimaryWeapon = new List<Weapon>();
            ListActiveSecondaryWeapon = new List<Weapon>();
            ListWeaponByIndex = new List<string>(ListWeaponCount);
            ListHolstereddPrimaryWeapon = new List<Weapon>(ListWeaponCount);
        }

        public void Load(ContentManager Content)
        {
            foreach (Weapon ActiveWeapon in DicWeaponByName.Values)
            {
                ActiveWeapon.Load(Content);
            }

            foreach (Weapon ActiveWeapon in ListActiveSecondaryWeapon)
            {
                ActiveWeapon.Load(Content);
            }
        }

        public void ChangeMap(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            List<string> ListWeaponName = new List<string>(DicWeaponByName.Keys);
            for (int W = 0; W < ListWeaponName.Count; ++W)
            {
                DicWeaponByName[ListWeaponName[W]] = new Weapon(ListWeaponName[W], DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            for (int W = 0; W < ListActiveSecondaryWeapon.Count; ++W)
            {
                ListActiveSecondaryWeapon[W] = new Weapon(ListActiveSecondaryWeapon[W].Name, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            for (int W = 0; W < ListActivePrimaryWeapon.Count; ++W)
            {
                ListActivePrimaryWeapon[W] = DicWeaponByName[ListActivePrimaryWeapon[W].Name];
            }
            for (int W = 0; W < ListHolstereddPrimaryWeapon.Count; ++W)
            {
                ListHolstereddPrimaryWeapon[W] = DicWeaponByName[ListHolstereddPrimaryWeapon[W].Name];
            }
        }

        public void RemoveAllActiveWeapons()
        {
            ListActivePrimaryWeapon.Clear();
            //Don't remove secondary weapons as you can't equip them
        }

        public void HolsterAllActiveWeapons()
        {
            ListHolstereddPrimaryWeapon.AddRange(ListActivePrimaryWeapon);
            ListActivePrimaryWeapon.Clear();
            //Don't remove secondary weapons as you can't equip them
        }

        public void UseWeapon(Weapon WeaponToUse)
        {
            ListActivePrimaryWeapon.Add(WeaponToUse);
        }

        public void UseWeapon(string WeaponName)
        {
            ListActivePrimaryWeapon.Add(DicWeaponByName[WeaponName]);
        }

        public List<Weapon> UseHolsteredWeapons()
        {
            ListActivePrimaryWeapon.AddRange(ListHolstereddPrimaryWeapon);
            List<Weapon> ListUnHolsteredWeapon = new List<Weapon>(ListHolstereddPrimaryWeapon);
            ListHolstereddPrimaryWeapon.Clear();

            return ListUnHolsteredWeapon;
        }

        public Weapon GetWeapon(string WeaponName)
        {
            return DicWeaponByName[WeaponName];
        }

        public string GetWeaponName(int WeaponIndex)
        {
            return ListWeaponByIndex[WeaponIndex];
        }

        public bool HasActiveWeapon(Weapon ActiveWeapon)
        {
            return ListActivePrimaryWeapon.Contains(ActiveWeapon);
        }

        public void ActivateSecondaryWeapon()
        {
            if (DicWeaponByName.Count > 2)
            {
                ListActiveSecondaryWeapon.Add(DicWeaponByName[ListWeaponByIndex[2]]);
                DicWeaponByName.Remove(ListWeaponByIndex[2]);
            }
        }

        public void AddWeaponToStash(Weapon NewWeapon)
        {
            DicWeaponByName.Add(NewWeapon.Name, NewWeapon);
            ListWeaponByIndex.Add(NewWeapon.Name);
        }

        public void ChargePrimaryWeapon(int ChargeAmount, int MaxCharge)
        {
            PrimaryWeaponCharge = Math.Min(PrimaryWeaponCharge + ChargeAmount, MaxCharge);
        }

        public void ChargeSecondaryWeapon(int ChargeAmount, int MaxCharge)
        {
            SecondaryWeaponCharge = Math.Min(SecondaryWeaponCharge + ChargeAmount, MaxCharge);
        }

        public void ResetPrimaryWeaponCharge()
        {
            PrimaryWeaponCharge = 0;
        }

        public void ResetSecondaryWeaponCharge()
        {
            SecondaryWeaponCharge = 0;
        }
    }
}
