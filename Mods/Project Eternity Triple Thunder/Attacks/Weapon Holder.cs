﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WeaponHolder
    {
        private readonly List<WeaponBase> ListActiveWeapon;
        private readonly Dictionary<string, WeaponBase> DicWeaponByName;
        private readonly List<WeaponBase> ListHolsteredWeapon;

        private int WeaponCharge;
        private List<string> ListWeaponByIndex;

        public bool HasActiveWeapons { get { return ListActiveWeapon.Count > 0; } }
        public bool HasWeapons { get { return DicWeaponByName.Count > 0; } }
        public List<WeaponBase> ActiveWeapons { get { return ListActiveWeapon; } }
        public int Charge { get { return WeaponCharge; } }
        public bool HasHolsteredWeapons { get { return ListHolsteredWeapon.Count > 0; } }
        public int HolsteredWeaponsCount { get { return ListHolsteredWeapon.Count; } }

        public WeaponHolder(int ListWeaponCount)
        {
            DicWeaponByName = new Dictionary<string, WeaponBase>(ListWeaponCount);
            ListActiveWeapon = new List<WeaponBase>();
            ListWeaponByIndex = new List<string>(ListWeaponCount);
            ListHolsteredWeapon = new List<WeaponBase>(ListWeaponCount);
        }

        public void Load(ContentManager Content)
        {
            foreach (WeaponBase ActiveWeapon in DicWeaponByName.Values)
            {
                ActiveWeapon.Load(Content);
            }
        }

        public void ChangeMap(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            List<string> ListWeaponName = new List<string>(DicWeaponByName.Keys);
            for (int W = 0; W < ListWeaponName.Count; ++W)
            {
                DicWeaponByName[ListWeaponName[W]] = WeaponBase.CreateFromFile(DicWeaponByName[ListWeaponName[W]].OwnerName, ListWeaponName[W], true, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            for (int W = 0; W < ListActiveWeapon.Count; ++W)
            {
                ListActiveWeapon[W] = DicWeaponByName[ListActiveWeapon[W].WeaponPath];
            }
            for (int W = 0; W < ListHolsteredWeapon.Count; ++W)
            {
                ListHolsteredWeapon[W] = DicWeaponByName[ListHolsteredWeapon[W].WeaponPath];
            }
        }

        public void RemoveAllActiveWeapons()
        {
            ListActiveWeapon.Clear();
            //Don't remove secondary weapons as you can't equip them
        }

        public void HolsterAllActiveWeapons()
        {
            ListHolsteredWeapon.AddRange(ListActiveWeapon);
            ListActiveWeapon.Clear();
            //Don't remove secondary weapons as you can't equip them
        }

        public void UseWeapon(WeaponBase WeaponToUse)
        {
            ListActiveWeapon.Add(WeaponToUse);
        }

        public void UseWeapon(string WeaponName)
        {
            ListActiveWeapon.Add(DicWeaponByName[WeaponName]);
        }

        public List<WeaponBase> UseHolsteredWeapons()
        {
            ListActiveWeapon.AddRange(ListHolsteredWeapon);
            List<WeaponBase> ListUnHolsteredWeapon = new List<WeaponBase>(ListHolsteredWeapon);
            ListHolsteredWeapon.Clear();

            return ListUnHolsteredWeapon;
        }

        public WeaponBase GetWeapon(string WeaponName)
        {
            return DicWeaponByName[WeaponName];
        }

        public string GetWeaponName(int WeaponIndex)
        {
            return ListWeaponByIndex[WeaponIndex];
        }

        public bool HasActiveWeapon(string ActiveWeaponName)
        {
            return DicWeaponByName.ContainsKey(ActiveWeaponName);
        }

        public void AddWeaponToStash(WeaponBase NewWeapon)
        {
            DicWeaponByName.Add(NewWeapon.WeaponPath, NewWeapon);
            ListWeaponByIndex.Add(NewWeapon.WeaponPath);
        }

        public void DropActiveWeapon()
        {
            foreach (WeaponBase WeaponToDrop in ListActiveWeapon)
            {
                DicWeaponByName.Remove(WeaponToDrop.WeaponPath);
                ListWeaponByIndex.Remove(WeaponToDrop.WeaponPath);
            }

            RemoveAllActiveWeapons();
        }

        public List<WeaponDrop> DropActiveWeapon(Vector2 Position, Layer CurrentLayer)
        {
            List<WeaponDrop> ListDroppedWeapon = new List<WeaponDrop>();

            foreach (WeaponBase WeaponToDrop in ListActiveWeapon)
            {
                DicWeaponByName.Remove(WeaponToDrop.WeaponPath);
                ListWeaponByIndex.Remove(WeaponToDrop.WeaponPath);

                ListDroppedWeapon.Add(new WeaponDrop(WeaponToDrop.sprMapIcon, WeaponToDrop.WeaponName, CurrentLayer, Position, WeaponToDrop.WeaponAngle));
            }

            RemoveAllActiveWeapons();

            return ListDroppedWeapon;
        }

        public void ChargePrimaryWeapon(int ChargeAmount, int MaxCharge)
        {
            WeaponCharge = Math.Min(WeaponCharge + ChargeAmount, MaxCharge);
        }

        public void ResetPrimaryWeaponCharge()
        {
            WeaponCharge = 0;
        }
    }
}
