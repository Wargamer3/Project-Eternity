using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.GameScreen;
using ProjectEternity.Core.Units.ArmoredCore;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Weapons;

namespace Project_Eternity
{
    public class PartsEquipScreen : GameScreen
    {
        enum Selection { None = -1, Head = 0, Torso = 1, LeftArm = 2, RightArm = 3, Legs = 4 };
        private struct PartMenu
        {
            public string Name;
            public string[] Categories;
            public bool Open;
            public PartMenu(string Name, string[] Categories)
            {
                this.Name = Name;
                this.Categories = Categories;
                this.Open = false;
            }
        };
        Texture2D sprRectangle;
        SpriteFont fntArial8;
        SpriteFont fntArial12;
        //Navigation related variables.
        int CursorAlpha;
        bool CursorAppearing;
        int CursorIndex;
        int CursorIndexMax;
        int CursorIndexStart;//Starting position of the cursor from where to start drawing.

        int CursorPartIndex;

        PartMenu[] PartNames;
        int Stage;
        Selection CurrentSelection;
        UnitArmoredCore CurrentUnit;

        int HPChange;
        int ENChange;
        int ArmorChange;
        int MobilityChange;
        float MovementChange;

        FilterItem CursorFilter;
        FilterItem MainFilter;
        int MainFilterIndex;

        public PartsEquipScreen(IServiceProvider serviceProvider, UnitArmoredCore CurrentUnit)
            : base(serviceProvider)
        {
            CursorIndex = 0;
            CursorIndexMax = 0;
            CursorIndexStart = 0;
            CurrentUnit.Init();
            this.CurrentUnit = CurrentUnit;
            CursorPartIndex = -1;
            CursorAlpha = 0;
            Stage = -1;
            CurrentSelection = Selection.None;
            PartNames = new PartMenu[] { new PartMenu("Head", new string[] {"Antena", "Ears", "Eyes", "Central procesing unit"}),
                                        new PartMenu("Torso", new string[] {"Core", "Radiator", "Shell"}),
                                        new PartMenu("Left Arm", new string[] {"Shell", "Strength"}),
                                        new PartMenu("Right Arm", new string[] {"Shell", "Strength"}),
                                        new PartMenu("Legs", new string[] {"Shell", "Strength"}) };
        }
        public override void Load()
        {
            MainFilter = new FilterItem("", new List<ShopItem>(), new List<FilterItem>());
            List<ShopItem> ListItem;

            #region Head

            if (CurrentUnit.Parts.Head != null)
            {
                List<FilterItem> Head = new List<FilterItem>();
                ListItem = new List<ShopItem>();
                if (CurrentUnit.Parts.Head.Antena != null)
                    ListItem.Add(CurrentUnit.Parts.Head.Antena);

                if (CurrentUnit.Parts.Head.Ears != null)
                    ListItem.Add(CurrentUnit.Parts.Head.Ears);

                if (CurrentUnit.Parts.Head.Eyes != null)
                    ListItem.Add(CurrentUnit.Parts.Head.Eyes);

                if (CurrentUnit.Parts.Head.CPU != null)
                    ListItem.Add(CurrentUnit.Parts.Head.CPU);

                for (int W = 0; W < CurrentUnit.Parts.Head.ListWeapon.Count; W++)
                    for (int j = 0; j < CurrentUnit.Parts.Head.ListWeapon[W].Count; j++)
                        if (CurrentUnit.Parts.Head.ListWeapon[W].Count > 0)
                            ListItem.Add((Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Head.ListWeapon[W][CurrentUnit.Parts.Head.ActiveWeapons[j]]].LoadItem());

                Head.Add(new FilterItem(CurrentUnit.Parts.Head.Name, ListItem, new List<FilterItem>()));
                Head[0].IsOpen = true;
                MainFilter.ListFilter.Add(new FilterItem("Head", new List<ShopItem>(), Head));
            }

            #endregion

            #region Torso

            if (CurrentUnit.Parts.Torso != null)
            {
                List<FilterItem> Torso = new List<FilterItem>();
                ListItem = new List<ShopItem>();

                if (CurrentUnit.Parts.Torso.Core != null)
                    ListItem.Add(CurrentUnit.Parts.Torso.Core);

                if (CurrentUnit.Parts.Torso.Radiator != null)
                    ListItem.Add(CurrentUnit.Parts.Torso.Radiator);

                if (CurrentUnit.Parts.Torso.Shell != null)
                    ListItem.Add(CurrentUnit.Parts.Torso.Shell);

                for (int W = 0; W < CurrentUnit.Parts.Torso.ListWeapon.Count; W++)
                    for (int j = 0; j < CurrentUnit.Parts.Torso.ListWeapon[W].Count; j++)
                        if (CurrentUnit.Parts.Torso.ListWeapon[W].Count > 0)
                            ListItem.Add((Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Torso.ListWeapon[W][CurrentUnit.Parts.Torso.ActiveWeapons[j]]].LoadItem());

                Torso.Add(new FilterItem(CurrentUnit.Parts.Torso.Name, ListItem, new List<FilterItem>()));
                Torso[0].IsOpen = true;
                MainFilter.ListFilter.Add(new FilterItem("Torso", new List<ShopItem>(), Torso));
            }

            #endregion

            if (CurrentUnit.Parts.LeftArm != null)
            {
                List<FilterItem> LeftArm = new List<FilterItem>();
                ListItem = new List<ShopItem>();

                if (CurrentUnit.Parts.LeftArm.Shell != null)
                    ListItem.Add(CurrentUnit.Parts.LeftArm.Shell);

                if (CurrentUnit.Parts.LeftArm.Strength != null)
                    ListItem.Add(CurrentUnit.Parts.LeftArm.Strength);

                for (int W = 0; W < CurrentUnit.Parts.LeftArm.ListWeapon.Count; W++)
                    for (int j = 0; j < CurrentUnit.Parts.LeftArm.ListWeapon[W].Count; j++)
                        if (CurrentUnit.Parts.LeftArm.ListWeapon[W].Count > 0)
                            ListItem.Add((Weapon)Weapon.ListWeapon[CurrentUnit.Parts.LeftArm.ListWeapon[W][CurrentUnit.Parts.LeftArm.ActiveWeapons[j]]].LoadItem());

                LeftArm.Add(new FilterItem(CurrentUnit.Parts.LeftArm.Name, ListItem, new List<FilterItem>()));
                LeftArm[0].IsOpen = true;
                MainFilter.ListFilter.Add(new FilterItem("Left arm", new List<ShopItem>(), LeftArm));
            }

            if (CurrentUnit.Parts.ListRightArm.Count > 0)
            {
                List<FilterItem> RightArm = new List<FilterItem>();
                ListItem = new List<ShopItem>();

                if (CurrentUnit.Parts.RightArm.Shell != null)
                    ListItem.Add(CurrentUnit.Parts.RightArm.Shell);


                if (CurrentUnit.Parts.RightArm.Strength != null)
                    ListItem.Add(CurrentUnit.Parts.RightArm.Strength);

                for (int W = 0; W < CurrentUnit.Parts.RightArm.ListWeapon.Count; W++)
                    for (int j = 0; j < CurrentUnit.Parts.RightArm.ListWeapon[W].Count; j++)
                        if (CurrentUnit.Parts.RightArm.ListWeapon[W].Count > 0)
                            ListItem.Add((Weapon)Weapon.ListWeapon[CurrentUnit.Parts.RightArm.ListWeapon[W][CurrentUnit.Parts.RightArm.ActiveWeapons[j]]].LoadItem());

                RightArm.Add(new FilterItem(CurrentUnit.Parts.RightArm.Name, ListItem, new List<FilterItem>()));
                RightArm[0].IsOpen = true;
                MainFilter.ListFilter.Add(new FilterItem("Right arm", new List<ShopItem>(), RightArm));
            }

            if (CurrentUnit.Parts.ListLegs.Count > 0)
            {
                List<FilterItem> Legs = new List<FilterItem>();
                ListItem = new List<ShopItem>();

                if (CurrentUnit.Parts.Legs.Shell != null)
                    ListItem.Add(CurrentUnit.Parts.Legs.Shell);


                if (CurrentUnit.Parts.Legs.Strength != null)
                    ListItem.Add(CurrentUnit.Parts.Legs.Strength);

                for (int W = 0; W < CurrentUnit.Parts.Legs.ListWeapon.Count; W++)
                    for (int j = 0; j < CurrentUnit.Parts.Legs.ListWeapon[W].Count; j++)
                        if (CurrentUnit.Parts.Legs.ListWeapon[W].Count > 0)
                            ListItem.Add((Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Legs.ListWeapon[W][CurrentUnit.Parts.Legs.ActiveWeapons[j]]].LoadItem());

                Legs.Add(new FilterItem(CurrentUnit.Parts.Legs.Name, ListItem, new List<FilterItem>()));
                Legs[0].IsOpen = true;
                MainFilter.ListFilter.Add(new FilterItem("Legs", new List<ShopItem>(), Legs));
            }

            MainFilter.IsOpen = true;
            MainFilter.CursorIndex = 0;

            CursorFilter = MainFilter;
            SetCursorIndexMax(MainFilter);

            sprRectangle = Content.Load<Texture2D>("Rectangle");
            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            CurrentUnit.Init();
        }
        public override void Update(GameTime gameTime)
        {
            //Change the alpha of the cursor.
            if (CursorAppearing)
            {
                if (++CursorAlpha >= 200)
                    CursorAppearing = false;
            }
            else
            {
                if (--CursorAlpha <= 20)
                    CursorAppearing = true;
            }

            #region Stage -1

            if (Stage == -1)
            {
                if (KeyboardHelper.KeyPressed(KeyboardHelper.AlternateChoice[0]) || KeyboardHelper.KeyPressed(KeyboardHelper.AlternateChoice[1]))
                {
                    GameScreen.RemoveScreen(this);
                }
                //Move cursor.
                if (KeyboardHelper.InputUpPressed())
                {
                    CursorIndex--;
                    if (CursorIndex < 0)
                    {
                        CursorIndex = CursorIndexMax - 1;
                        if (CursorIndex > 20)
                            CursorIndexStart = CursorIndexMax - 19;
                    }
                    if (CursorIndex < CursorIndexStart)
                        CursorIndexStart--;
                    //Reset the CursorFilter.
                    int CursorPos = CursorIndex;
                    ResetCursorFilter(MainFilter);
                    GetCursorFilter(MainFilter, ref CursorPos);
                }
                else if (KeyboardHelper.InputDownPressed())//Move cursor.
                {
                    CursorIndex++;
                    if (CursorIndex > CursorIndexMax - 1)
                    {
                        CursorIndex = 0;
                        CursorIndexStart = 0;
                    }
                    if (CursorIndex >= 19 + CursorIndexStart)
                        CursorIndexStart++;
                    //Reset the CursorFilter.
                    int CursorPos = CursorIndex;
                    ResetCursorFilter(MainFilter);
                    GetCursorFilter(MainFilter, ref CursorPos);
                }
                else if (KeyboardHelper.InputConfirmPressed())
                {
                    //If the cursor points on a FilterItem.
                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count && CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].ListItem.Count == 0)
                    {//Open/Close the selected FilterItem.
                        CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen = !CursorFilter.ListFilter[CursorFilter.CursorIndex - CursorFilter.ListItem.Count].IsOpen;
                        //Reset the CursorIndexMax.
                        CursorIndexMax = 0;
                        SetCursorIndexMax(MainFilter);
                    }
                    else
                    {
                        CursorPartIndex = 0;
                        switch (MainFilterIndex)
                        {
                            case 0:
                                CurrentSelection = Selection.Head;
                                break;
                            case 1:
                                CurrentSelection = Selection.Torso;
                                break;
                            case 2:
                                CurrentSelection = Selection.LeftArm;
                                break;
                            case 3:
                                CurrentSelection = Selection.RightArm;
                                break;
                            case 4:
                                CurrentSelection = Selection.Legs;
                                break;
                        }
                        
                        Stage++;
                    }
                }
            }

            #endregion

            #region Part selection

            else//If in stage 1.
            {
                if (KeyboardHelper.InputUpPressed())
                {
                    CursorPartIndex -= (CursorPartIndex > 0) ? 1 : 0;
                    UpdateStatsChange();
                }

                #region Move down

                else if (KeyboardHelper.InputDownPressed())
                {
                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                    {
                        switch (CurrentSelection)
                        {
                            case Selection.Head:
                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.BaseSize - 1) ? 1 : 0;
                                break;
                            case Selection.Torso:
                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.BaseSize - 1) ? 1 : 0;
                                break;
                            case Selection.LeftArm:
                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.BaseSize - 1) ? 1 : 0;
                                break;
                            case Selection.RightArm:
                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.BaseSize - 1) ? 1 : 0;
                                break;
                            case Selection.Legs:
                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.BaseSize - 1) ? 1 : 0;
                                break;
                        }
                    }
                    else
                    {
                        #region Parts

                        switch (CurrentSelection)
                        {

                            #region Head

                            case Selection.Head:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Head.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {
                                        case PartTypes.HeadAntena:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListPartAntena.Count - 1) ? 1 : 0;
                                            break;
                                        case PartTypes.HeadEars:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListPartEars.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.HeadEyes:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListPartEyes.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.HeadCPU:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListPartCPU.Count - 1) ? 1 : 0;
                                            break;
                                    }
                                }
                                else//Weapons
                                    CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count - 1) ? 1 : 0;
                                break;

                            #endregion

                            #region Torso

                            case Selection.Torso:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Torso.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {
                                        case PartTypes.TorsoCore:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListPartCore.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.TorsoRadiator:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListPartRadiator.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.TorsoShell:
                                        case PartTypes.GenericShell:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListPartShell.Count - 1) ? 1 : 0;
                                            break;
                                    }
                                }
                                else//Weapons
                                    CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count - 1) ? 1 : 0;
                                break;

                            #endregion

                            #region Left arm

                            case Selection.LeftArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.ArmShell:
                                        case PartTypes.GenericShell:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListPartShell.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.ArmStrength:
                                        case PartTypes.GenericStrength:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListPartStrength.Count - 1) ? 1 : 0;
                                            break;
                                    }
                                }
                                else//Weapons
                                    CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count - 1) ? 1 : 0;
                                break;

                            #endregion

                            #region Right arm

                            case Selection.RightArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.ArmShell:
                                        case PartTypes.GenericShell:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListPartShell.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.ArmStrength:
                                        case PartTypes.GenericStrength:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListPartStrength.Count - 1) ? 1 : 0;
                                            break;
                                    }
                                }
                                else//Weapons
                                    CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count - 1) ? 1 : 0;
                                break;

                            #endregion

                            #region Legs

                            case Selection.Legs:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.LegsShell:
                                        case PartTypes.GenericShell:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListPartShell.Count - 1) ? 1 : 0;
                                            break;

                                        case PartTypes.LegsStrength:
                                        case PartTypes.GenericStrength:

                                            CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListPartStrength.Count - 1) ? 1 : 0;
                                            break;
                                    }
                                }
                                else//Weapons
                                    CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count - 1) ? 1 : 0;
                                break;

                            #endregion
                        }

                        #endregion
                    }
                    UpdateStatsChange();
                }

                #endregion

                else if (KeyboardHelper.InputConfirmPressed())
                {
                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                    {
                        switch (CurrentSelection)
                        {
                            case Selection.Head:
                                CurrentUnit.Parts.Head = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]);
                                CursorFilter.Name = CurrentUnit.Parts.Head.Name;
                                break;
                            case Selection.Torso:
                                CurrentUnit.Parts.Torso = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]);
                                CursorFilter.Name = CurrentUnit.Parts.Torso.Name;
                                break;
                            case Selection.LeftArm:
                                CurrentUnit.Parts.LeftArm = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]);
                                CursorFilter.Name = CurrentUnit.Parts.LeftArm.Name;
                                break;
                            case Selection.RightArm:
                                CurrentUnit.Parts.RightArm = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]);
                                CursorFilter.Name = CurrentUnit.Parts.RightArm.Name;
                                break;
                            case Selection.Legs:
                                CurrentUnit.Parts.Legs = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]);
                                CursorFilter.Name = CurrentUnit.Parts.Legs.Name;
                                break;
                        }
                    }
                    else
                    {
                        #region Parts

                        switch (CurrentSelection)
                        {

                            #region Head

                            case Selection.Head:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Head.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {
                                        case PartTypes.HeadAntena:

                                            CurrentUnit.Parts.Head.Antena = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Head.Antena;
                                            break;
                                        case PartTypes.HeadEars:

                                            CurrentUnit.Parts.Head.Ears = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Head.Ears;
                                            break;

                                        case PartTypes.HeadEyes:

                                            CurrentUnit.Parts.Head.Eyes = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Head.Eyes;
                                            break;

                                        case PartTypes.HeadCPU:

                                            CurrentUnit.Parts.Head.CPU = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Head.CPU;
                                            break;
                                    }
                                }
                                else//Weapons
                                {
                                    CursorFilter.ListItem[CursorFilter.CursorIndex] = (Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Head.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize][CursorPartIndex]].LoadItem();
                                    CurrentUnit.Parts.Head.ActiveWeapons[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize] = CursorPartIndex;
                                }
                                break;

                            #endregion

                            #region Torso

                            case Selection.Torso:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Torso.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {
                                        case PartTypes.TorsoCore:

                                            CurrentUnit.Parts.Torso.Core = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Torso.Core;
                                            break;

                                        case PartTypes.TorsoRadiator:

                                            CurrentUnit.Parts.Torso.Radiator = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Torso.Radiator;
                                            break;

                                        case PartTypes.TorsoShell:
                                        case PartTypes.GenericShell:

                                            CurrentUnit.Parts.Torso.Shell = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Torso.Shell;
                                            break;
                                    }
                                }
                                else//Weapons
                                {
                                    CursorFilter.ListItem[CursorFilter.CursorIndex] = (Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Torso.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize][CursorPartIndex]].LoadItem();
                                    CurrentUnit.Parts.Torso.ActiveWeapons[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize] = CursorPartIndex;
                                }
                                break;

                            #endregion

                            #region Left arm

                            case Selection.LeftArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.ArmShell:
                                        case PartTypes.GenericShell:

                                            CurrentUnit.Parts.LeftArm.Shell = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.LeftArm.Shell;
                                            break;

                                        case PartTypes.ArmStrength:
                                        case PartTypes.GenericStrength:

                                            CurrentUnit.Parts.LeftArm.Strength = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.LeftArm.Strength;
                                            break;
                                    }
                                }
                                else//Weapons
                                {
                                    CursorFilter.ListItem[CursorFilter.CursorIndex] = (Weapon)Weapon.ListWeapon[CurrentUnit.Parts.LeftArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize][CursorPartIndex]].LoadItem();
                                    CurrentUnit.Parts.LeftArm.ActiveWeapons[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize] = CursorPartIndex;
                                }
                                break;

                            #endregion

                            #region Right arm

                            case Selection.RightArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.ArmShell:
                                        case PartTypes.GenericShell:

                                            CurrentUnit.Parts.RightArm.Shell = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.RightArm.Shell;
                                            break;

                                        case PartTypes.ArmStrength:
                                        case PartTypes.GenericStrength:

                                            CurrentUnit.Parts.RightArm.Strength = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.RightArm.Strength;
                                            break;
                                    }
                                }
                                else//Weapons
                                {
                                    CursorFilter.ListItem[CursorFilter.CursorIndex] = (Weapon)Weapon.ListWeapon[CurrentUnit.Parts.RightArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize][CursorPartIndex]].LoadItem();
                                    CurrentUnit.Parts.RightArm.ActiveWeapons[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize] = CursorPartIndex;
                                }
                                break;

                            #endregion

                            #region Legs

                            case Selection.Legs:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                                {
                                    switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                    {

                                        case PartTypes.LegsShell:
                                        case PartTypes.GenericShell:

                                            CurrentUnit.Parts.Legs.Shell = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Legs.Shell;
                                            break;

                                        case PartTypes.ArmStrength:
                                        case PartTypes.GenericStrength:

                                            CurrentUnit.Parts.Legs.Strength = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]);
                                            CursorFilter.ListItem[CursorFilter.CursorIndex] = CurrentUnit.Parts.Legs.Strength;
                                            break;
                                    }
                                }
                                else//Weapons
                                {
                                    CursorFilter.ListItem[CursorFilter.CursorIndex] = (Weapon)Weapon.ListWeapon[CurrentUnit.Parts.Legs.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize][CursorPartIndex]].LoadItem();
                                    CurrentUnit.Parts.Legs.ActiveWeapons[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize] = CursorPartIndex;
                                }
                                break;

                            #endregion
                        }

                        #endregion
                    }
                    CursorPartIndex = 0;
                    CurrentUnit.Init();
                    HPChange = 0;
                    ENChange = 0;
                    ArmorChange = 0;
                    MobilityChange = 0;
                    MovementChange = 0;
                    Stage--;
                }
            }

            #endregion

            if (KeyboardHelper.KeyPressed(KeyboardHelper.AlternateChoice[0]) || KeyboardHelper.KeyPressed(KeyboardHelper.AlternateChoice[1]))
            {
                Stage--;
            }

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                GameScreen.RemoveScreen(this);
        }
        public override void Draw(SpriteBatch g)
        {
            g.Draw(sprRectangle, new Rectangle(0, 0, Game.Width - 238, Game.Height), Color.White);
            g.Draw(sprRectangle, new Rectangle(0, Game.Height - 200, Game.Height - 100, 97), Color.Black);
            g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 0, 1, Game.Height), Color.Black);
            //Top border
            g.Draw(sprRectangle, new Rectangle(0, 0, Game.Height - 100, 31), Color.Black);
            g.DrawString(fntArial12, "Change Unit", new Vector2(240, 5), Color.White, 0, new Vector2(fntArial12.MeasureString("Change Unit").X, 0), 1, SpriteEffects.None, 0);

            #region Stage - 1

            if (Stage == -1)
            {
                int Index = 0;
                int Y = DrawFilter(g, MainFilter, 405, 231, ref Index);
                //Bottom
                g.DrawString(fntArial8, "Name: " + CurrentUnit.Name, new Vector2(5, Game.Height - fntArial8.LineSpacing * 8), Color.Black);
                g.DrawString(fntArial8, "Land: " + CurrentUnit.TerrainGrade.TerrainGradeLand, new Vector2(5, Game.Height - fntArial8.LineSpacing), Color.Black);
                g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP, new Vector2(5, Game.Height - fntArial8.LineSpacing * 7), Color.Black);
                g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN, new Vector2(5, Game.Height - fntArial8.LineSpacing * 6), Color.Black);
                g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor, new Vector2(5, Game.Height - fntArial8.LineSpacing * 5), Color.Black);
                g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility, new Vector2(5, Game.Height - fntArial8.LineSpacing * 4), Color.Black);
                g.DrawString(fntArial8, "Movement: " + CurrentUnit.Movement, new Vector2(5, Game.Height - fntArial8.LineSpacing * 3), Color.Black);
                g.DrawString(fntArial8, "MEL: " + CurrentUnit.PilotMEL, new Vector2(200, Game.Height - fntArial8.LineSpacing * 7), Color.Black);
                g.DrawString(fntArial8, "RNG: " + CurrentUnit.PilotRNG, new Vector2(200, Game.Height - fntArial8.LineSpacing * 6), Color.Black);
                g.DrawString(fntArial8, "DEF: " + CurrentUnit.PilotDEF, new Vector2(200, Game.Height - fntArial8.LineSpacing * 5), Color.Black);
                g.DrawString(fntArial8, "SKL: " + CurrentUnit.PilotSKL, new Vector2(200, Game.Height - fntArial8.LineSpacing * 4), Color.Black);
                g.DrawString(fntArial8, "EVA: " + CurrentUnit.PilotEVA, new Vector2(200, Game.Height - fntArial8.LineSpacing * 3), Color.Black);
                g.DrawString(fntArial8, "HIT: " + CurrentUnit.PilotHIT, new Vector2(200, Game.Height - fntArial8.LineSpacing * 2), Color.Black);
                //Abilities
                g.DrawString(fntArial12, "Abilities", new Vector2(Game.Height - 80, 15), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 50, 238, 1), Color.Black);
                g.DrawString(fntArial8, "-", new Vector2(Game.Height - 80, 35 + fntArial8.LineSpacing * 2), Color.Black);
                g.DrawString(fntArial8, "-", new Vector2(Game.Height - 80, 35 + fntArial8.LineSpacing * 4), Color.Black);
                g.DrawString(fntArial8, "-", new Vector2(Game.Height - 80, 35 + fntArial8.LineSpacing * 6), Color.Black);
                g.DrawString(fntArial8, "-", new Vector2(Game.Height - 80, 35 + fntArial8.LineSpacing * 8), Color.Black);
                //Parts
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 165, 238, 1), Color.Black);
                g.DrawString(fntArial12, "Parts", new Vector2(Game.Height - 80, 180), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 215, 238, 1), Color.Black);
            }

            #endregion

            #region Part change

            else
            {
                #region Base drawing

                g.Draw(sprRectangle, new Rectangle(0, 0, Game.Width - 238, Game.Height), Color.White);
                g.Draw(sprRectangle, new Rectangle(0, Game.Height - 200, Game.Height - 100, 97), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 0, 1, Game.Height), Color.Black);
                //Bottom
                g.DrawString(fntArial8, "Name: " + CurrentUnit.Name, new Vector2(5, Game.Height - fntArial8.LineSpacing * 8), Color.Black);
                g.DrawString(fntArial8, "Land: " + CurrentUnit.TerrainGrade.TerrainGradeLand, new Vector2(5, Game.Height - fntArial8.LineSpacing), Color.Black);
                if (HPChange > 0)
                    g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP + " + " + HPChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 7), Color.Green);
                else if (HPChange < 0)
                    g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP + " - " + -HPChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 7), Color.Red);
                else
                    g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP, new Vector2(5, Game.Height - fntArial8.LineSpacing * 7), Color.Black);
                if (ENChange > 0)
                    g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN + " + " + ENChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 6), Color.Green);
                else if (ENChange < 0)
                    g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN + " - " + -ENChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 6), Color.Red);
                else
                    g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN, new Vector2(5, Game.Height - fntArial8.LineSpacing * 6), Color.Black);
                if (ArmorChange > 0)
                    g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor + " + " + ArmorChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 5), Color.Green);
                else if (ArmorChange < 0)
                    g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor + " - " + -ArmorChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 5), Color.Red);
                else
                    g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor, new Vector2(5, Game.Height - fntArial8.LineSpacing * 5), Color.Black);
                if (MobilityChange > 0)
                    g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility + " + " + MobilityChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 4), Color.Green);
                else if (MobilityChange < 0)
                    g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility + " - " + -MobilityChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 4), Color.Red);
                else
                    g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility, new Vector2(5, Game.Height - fntArial8.LineSpacing * 4), Color.Black);
                if (MovementChange > 0)
                    g.DrawString(fntArial8, "Movement: " + CurrentUnit.Movement + " + " + MovementChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 3), Color.Green);
                else if (MovementChange < 0)
                    g.DrawString(fntArial8, "Movement: " + CurrentUnit.Movement + " - " + -MovementChange, new Vector2(5, Game.Height - fntArial8.LineSpacing * 3), Color.Red);
                else
                    g.DrawString(fntArial8, "Movement: " + CurrentUnit.Movement, new Vector2(5, Game.Height - fntArial8.LineSpacing * 3), Color.Black);
                g.DrawString(fntArial8, "MEL: " + CurrentUnit.PilotMEL, new Vector2(200, Game.Height - fntArial8.LineSpacing * 7), Color.Black);
                g.DrawString(fntArial8, "RNG: " + CurrentUnit.PilotRNG, new Vector2(200, Game.Height - fntArial8.LineSpacing * 6), Color.Black);
                g.DrawString(fntArial8, "DEF: " + CurrentUnit.PilotDEF, new Vector2(200, Game.Height - fntArial8.LineSpacing * 5), Color.Black);
                g.DrawString(fntArial8, "SKL: " + CurrentUnit.PilotSKL, new Vector2(200, Game.Height - fntArial8.LineSpacing * 4), Color.Black);
                g.DrawString(fntArial8, "EVA: " + CurrentUnit.PilotEVA, new Vector2(200, Game.Height - fntArial8.LineSpacing * 3), Color.Black);
                g.DrawString(fntArial8, "HIT: " + CurrentUnit.PilotHIT, new Vector2(200, Game.Height - fntArial8.LineSpacing * 2), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 0, 238, 1), Color.Black);
                g.DrawString(fntArial12, "Change parts", new Vector2(Game.Height - 35, 15), Color.Black);
                g.Draw(sprRectangle, new Rectangle(Game.Height - 100, 50, 238, 1), Color.Black);

                #endregion

                #region Part Drawing

                #region Part Unit

                if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                {
                    switch (CurrentSelection)
                    {
                        case Selection.Head:

                            for (int H = 0; H < CurrentUnit.Parts.ListHead.Count; H++)
                                g.DrawString(fntArial12, PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[H]).Name, new Vector2(420, 60 + H * fntArial12.LineSpacing), Color.Black);
                            break;

                        case Selection.Torso:

                            for (int T = 0; T < CurrentUnit.Parts.ListTorso.Count; T++)
                                g.DrawString(fntArial12, PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[T]).Name, new Vector2(420, 60 + T * fntArial12.LineSpacing), Color.Black);
                            break;

                        case Selection.LeftArm:

                            for (int A = 0; A < CurrentUnit.Parts.ListLeftArm.Count; A++)
                                g.DrawString(fntArial12, PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[A]).Name, new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                            break;

                        case Selection.RightArm:

                            for (int A = 0; A < CurrentUnit.Parts.ListRightArm.Count; A++)
                                g.DrawString(fntArial12, PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[A]).Name, new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                            break;

                        case Selection.Legs:

                            for (int L = 0; L < CurrentUnit.Parts.ListLegs.Count; L++)
                                g.DrawString(fntArial12, PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[L]).Name, new Vector2(420, 60 + L * fntArial12.LineSpacing), Color.Black);
                            break;
                    }
                }

                #endregion

                #region Parts

                else
                {
                    Part SmallPart = (Part)CursorFilter.ListItem[CursorFilter.CursorIndex];

                    switch (CurrentSelection)
                    {

                        #region Head

                        case Selection.Head:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Head.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.HeadAntena:

                                        for (int A = 0; A < CurrentUnit.Parts.Head.ListPartAntena.Count; A++)
                                            g.DrawString(fntArial12, PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[A]).Name, new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                                        break;
                                    case PartTypes.HeadEars:

                                        for (int E = 0; E < CurrentUnit.Parts.Head.ListPartEars.Count; E++)
                                            g.DrawString(fntArial12, PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[E]).Name, new Vector2(420, 60 + E * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.HeadEyes:

                                        for (int E = 0; E < CurrentUnit.Parts.Head.ListPartEyes.Count; E++)
                                            g.DrawString(fntArial12, PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[E]).Name, new Vector2(420, 60 + E * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.HeadCPU:

                                        for (int C = 0; C < CurrentUnit.Parts.Head.ListPartCPU.Count; C++)
                                            g.DrawString(fntArial12, PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[C]).Name, new Vector2(420, 60 + C * fntArial12.LineSpacing), Color.Black);
                                        break;
                                }
                            }
                            else//Weapons
                                for (int W = 0; W < CurrentUnit.Parts.Head.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count; W++)
                                    g.DrawString(fntArial12, ((Weapon)Weapon.ListWeapon[(CurrentUnit.Parts.Head.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize][W])].LoadItem()).Name, new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                            break;

                        #endregion

                        #region Torso

                        case Selection.Torso:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Torso.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.TorsoCore:

                                        for (int C = 0; C < CurrentUnit.Parts.Torso.ListPartCore.Count; C++)
                                            g.DrawString(fntArial12, PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[C]).Name, new Vector2(420, 60 + C * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.TorsoRadiator:

                                        for (int R = 0; R < CurrentUnit.Parts.Torso.ListPartRadiator.Count; R++)
                                            g.DrawString(fntArial12, PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[R]).Name, new Vector2(420, 60 + R * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.TorsoShell:
                                    case PartTypes.GenericShell:

                                        for (int S = 0; S < CurrentUnit.Parts.Torso.ListPartShell.Count; S++)
                                            g.DrawString(fntArial12, PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;
                                }
                            }
                            else//Weapons
                                for (int W = 0; W < CurrentUnit.Parts.Torso.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count; W++)
                                    g.DrawString(fntArial12, ((Weapon)Weapon.ListWeapon[(CurrentUnit.Parts.Torso.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize][W])].LoadItem()).Name, new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                            break;

                        #endregion

                        #region Left arm

                        case Selection.LeftArm:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {

                                    case PartTypes.ArmShell:
                                    case PartTypes.GenericShell:

                                        for (int S = 0; S < CurrentUnit.Parts.LeftArm.ListPartShell.Count; S++)
                                            g.DrawString(fntArial12, PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.ArmStrength:
                                    case PartTypes.GenericStrength:

                                        for (int S = 0; S < CurrentUnit.Parts.LeftArm.ListPartStrength.Count; S++)
                                            g.DrawString(fntArial12, PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartStrength[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;
                                }
                            }
                            else//Weapons
                                for (int W = 0; W < CurrentUnit.Parts.LeftArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count; W++)
                                    g.DrawString(fntArial12, ((Weapon)Weapon.ListWeapon[(CurrentUnit.Parts.LeftArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize][W])].LoadItem()).Name, new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                            break;

                        #endregion

                        #region Right arm

                        case Selection.RightArm:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {

                                    case PartTypes.ArmShell:
                                    case PartTypes.GenericShell:

                                        for (int S = 0; S < CurrentUnit.Parts.RightArm.ListPartShell.Count; S++)
                                            g.DrawString(fntArial12, PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.ArmStrength:
                                    case PartTypes.GenericStrength:

                                        for (int S = 0; S < CurrentUnit.Parts.RightArm.ListPartStrength.Count; S++)
                                            g.DrawString(fntArial12, PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;
                                }
                            }
                            else//Weapons
                                for (int W = 0; W < CurrentUnit.Parts.RightArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count; W++)
                                    g.DrawString(fntArial12, ((Weapon)Weapon.ListWeapon[(CurrentUnit.Parts.RightArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize][W])].LoadItem()).Name, new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                            break;

                        #endregion

                        #region Legs

                        case Selection.Legs:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {

                                    case PartTypes.LegsShell:
                                    case PartTypes.GenericShell:

                                        for (int S = 0; S < CurrentUnit.Parts.Legs.ListPartShell.Count; S++)
                                            g.DrawString(fntArial12, PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;

                                    case PartTypes.LegsStrength:
                                    case PartTypes.GenericStrength:

                                        for (int S = 0; S < CurrentUnit.Parts.Legs.ListPartShell.Count; S++)
                                            g.DrawString(fntArial12, PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartShell[S]).Name, new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                        break;
                                }
                            }
                            else//Weapons
                                for (int W = 0; W < CurrentUnit.Parts.Legs.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count; W++)
                                    g.DrawString(fntArial12, ((Weapon)Weapon.ListWeapon[(CurrentUnit.Parts.Legs.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize][W])].LoadItem()).Name, new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                            break;

                        #endregion
                    }
                }

                #endregion

                g.Draw(sprRectangle, new Rectangle(409, 59 + CursorPartIndex * fntArial12.LineSpacing, 237, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));

                #endregion
            }

            #endregion

        }

        private void UpdateStatsChange()
        {

            #region Part Unit

            if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
            {
                switch (CurrentSelection)
                {
                    case Selection.Head:

                        HPChange = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]).HP - CurrentUnit.Parts.Head.HP;
                        ENChange = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]).EN - CurrentUnit.Parts.Head.EN;
                        ArmorChange = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]).Armor - CurrentUnit.Parts.Head.Armor;
                        MobilityChange = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]).Mobility - CurrentUnit.Parts.Head.Mobility;
                        MovementChange = PartHead.GetHeadByID(CurrentUnit.Parts.ListHead[CursorPartIndex]).Movement - CurrentUnit.Parts.Head.Movement;
                        break;

                    case Selection.Torso:

                        HPChange = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]).HP - CurrentUnit.Parts.Torso.HP;
                        ENChange = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]).EN - CurrentUnit.Parts.Torso.EN;
                        ArmorChange = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Armor - CurrentUnit.Parts.Torso.Armor;
                        MobilityChange = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Mobility - CurrentUnit.Parts.Torso.Mobility;
                        MovementChange = PartTorso.GetTorsoByID(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Movement - CurrentUnit.Parts.Torso.Movement;
                        break;

                    case Selection.LeftArm:

                        HPChange = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).HP - CurrentUnit.Parts.LeftArm.HP;
                        ENChange = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).EN - CurrentUnit.Parts.LeftArm.EN;
                        ArmorChange = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Armor - CurrentUnit.Parts.LeftArm.Armor;
                        MobilityChange = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Mobility - CurrentUnit.Parts.LeftArm.Mobility;
                        MovementChange = PartArm.GetArmByID(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Movement - CurrentUnit.Parts.LeftArm.Movement;
                        break;

                    case Selection.RightArm:

                        HPChange = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).HP - CurrentUnit.Parts.RightArm.HP;
                        ENChange = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).EN - CurrentUnit.Parts.RightArm.EN;
                        ArmorChange = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Armor - CurrentUnit.Parts.RightArm.Armor;
                        MobilityChange = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Mobility - CurrentUnit.Parts.RightArm.Mobility;
                        MovementChange = PartArm.GetArmByID(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Movement - CurrentUnit.Parts.RightArm.Movement;
                        break;

                    case Selection.Legs:

                        HPChange = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]).HP - CurrentUnit.Parts.Legs.HP;
                        ENChange = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]).EN - CurrentUnit.Parts.Legs.EN;
                        ArmorChange = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Armor - CurrentUnit.Parts.Legs.Armor;
                        MobilityChange = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Mobility - CurrentUnit.Parts.Legs.Mobility;
                        MovementChange = PartLegs.GetLegsByID(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Movement - CurrentUnit.Parts.Legs.Movement;
                        break;
                }
            }

            #endregion

            #region Parts

            else
            {
                Part SmallPart = (Part)CursorFilter.ListItem[CursorFilter.CursorIndex];

                switch (CurrentSelection)
                {

                    #region Head

                    case Selection.Head:

                        if (CursorFilter.CursorIndex < CurrentUnit.Parts.Head.BaseSize)
                        {
                            switch (SmallPart.PartType)
                            {
                                case PartTypes.HeadAntena:

                                    HPChange = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartAntena.GetAntenaByID(CurrentUnit.Parts.Head.ListPartAntena[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                                case PartTypes.HeadEars:

                                    HPChange = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartEars.GetEarsByID(CurrentUnit.Parts.Head.ListPartEars[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.HeadEyes:

                                    HPChange = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartEyes.GetEyesByID(CurrentUnit.Parts.Head.ListPartEyes[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.HeadCPU:

                                    HPChange = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartCPU.GetCPUByID(CurrentUnit.Parts.Head.ListPartCPU[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                            }
                        }
                        /*else//Weapons
                            CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Head.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count - 1) ? 1 : 0;*/
                        break;

                    #endregion

                    #region Torso

                    case Selection.Torso:

                        if (CursorFilter.CursorIndex < CurrentUnit.Parts.Torso.BaseSize)
                        {
                            switch (SmallPart.PartType)
                            {
                                case PartTypes.TorsoCore:

                                    HPChange = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartCore.GetCoreByID(CurrentUnit.Parts.Torso.ListPartCore[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                                case PartTypes.TorsoRadiator:

                                    HPChange = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartRadiator.GetRadiatorByID(CurrentUnit.Parts.Torso.ListPartRadiator[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.TorsoShell:
                                case PartTypes.GenericShell:

                                    HPChange = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartShell.GetShellByID(CurrentUnit.Parts.Torso.ListPartShell[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                            }
                        }
                        /*else//Weapons
                            CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Torso.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count - 1) ? 1 : 0;*/
                        break;

                    #endregion

                    #region Left arm

                    case Selection.LeftArm:

                        if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                        {
                            switch (SmallPart.PartType)
                            {

                                case PartTypes.ArmShell:
                                case PartTypes.GenericShell:

                                    HPChange = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartShell.GetShellByID(CurrentUnit.Parts.LeftArm.ListPartShell[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.ArmStrength:
                                case PartTypes.GenericStrength:

                                    HPChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.LeftArm.ListPartStrength[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                            }
                        }
                        /*else//Weapons
                            CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count - 1) ? 1 : 0;*/
                        break;

                    #endregion

                    #region Right arm

                    case Selection.RightArm:

                        if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                        {
                            switch (SmallPart.PartType)
                            {

                                case PartTypes.ArmShell:
                                case PartTypes.GenericShell:

                                    HPChange = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartShell.GetShellByID(CurrentUnit.Parts.RightArm.ListPartShell[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.ArmStrength:
                                case PartTypes.GenericStrength:

                                    HPChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.RightArm.ListPartStrength[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                            }
                        }
                        /*else//Weapons
                            CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.RightArm.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count - 1) ? 1 : 0;*/
                        break;

                    #endregion

                    #region Legs

                    case Selection.Legs:

                        if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                        {
                            switch (SmallPart.PartType)
                            {

                                case PartTypes.LegsShell:
                                case PartTypes.GenericShell:

                                    HPChange = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartShell.GetShellByID(CurrentUnit.Parts.Legs.ListPartShell[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;

                                case PartTypes.LegsStrength:
                                case PartTypes.GenericStrength:

                                    HPChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]).BaseHP - SmallPart.BaseHP;
                                    ENChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]).BaseEN - SmallPart.BaseEN;
                                    ArmorChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]).BaseArmor - SmallPart.BaseArmor;
                                    MobilityChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]).BaseMobility - SmallPart.BaseMobility;
                                    MovementChange = PartStrength.GetStrengthByID(CurrentUnit.Parts.Legs.ListPartStrength[CursorPartIndex]).BaseMovement - SmallPart.BaseMovement;
                                    break;
                            }
                        }
                        /*else//Weapons
                            CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Legs.ListWeapon[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count - 1) ? 1 : 0;*/
                        break;

                    #endregion
                }
            }

            #endregion
        }

        /// <summary>
        /// Set the CursorIndex of a FilterItem and its child to -1.
        /// </summary>
        /// <param name="Filter">FilterItem to reset.</param>
        private void ResetCursorFilter(FilterItem Filter)
        {
            Filter.CursorIndex = -1;
            //Reset its child.
            for (int i = 0; i < Filter.ListFilter.Count; i++)
                ResetCursorFilter(Filter.ListFilter[i]);
        }
        /// <summary>
        /// Give CursorFilter a value based on a CursorPos and a FilterItem.
        /// </summary>
        /// <param name="Filter">FilterItem to calculate from.</param>
        /// <param name="CursorPos">Index of the cursor to use.</param>
        /// <returns></returns>
        private bool GetCursorFilter(FilterItem Filter, ref int CursorPos)
        {
            if (Filter.IsOpen)
            {
                //If the CursorPos is lower then the count of ShopItem.
                if (CursorPos < Filter.ListItem.Count)
                {//Set the CursorFilter and its CursorIndex.
                    CursorFilter = Filter;
                    CursorFilter.CursorIndex = CursorPos;
                    return true;
                }
                //Decrement CursorPos of the number of ShopItem in the ListItem.
                CursorPos -= Filter.ListItem.Count;
                //Loop through the ListFilter to get any child FilterItem.
                for (int F = 0; F < Filter.ListFilter.Count; F++)
                {//If the CursorPos didn't reached 0.
                    if (CursorPos > 0)
                    {
                        CursorPos--;
                        //Move in the child FilterItem, if it suceed then the CursorFitler is set and there's no need to keep on.
                        if (GetCursorFilter(Filter.ListFilter[F], ref CursorPos))
                        {
                            MainFilterIndex = F;
                            return true;
                        }
                    }
                    //Else if the CursorPos reached 0
                    else if (CursorPos == 0)
                    {//Set the CursorFilter and its CursorIndex.
                        CursorFilter = Filter;
                        CursorFilter.CursorIndex = F + Filter.ListItem.Count;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Set the CursorIndexMax by counting the content of a FilterItem.
        /// </summary>
        /// <param name="Filter">FilterItem to calculate.</param>
        private void SetCursorIndexMax(FilterItem Filter)
        {
            //Add the size of the ListItem and the ListFilter.
            CursorIndexMax += Filter.ListItem.Count + Filter.ListFilter.Count;
            //Loop through every FilterItem in the ListFilter to add them to the CursorIndexMax.
            for (int F = 0; F < Filter.ListFilter.Count; F++)
                if (Filter.ListFilter[F].IsOpen)
                    SetCursorIndexMax(Filter.ListFilter[F]);
        }
        /// <summary>
        /// Start drawing A FilterItem and its content at a given position.
        /// </summary>
        /// <param name="g">SpriteBatch to draw on.</param>
        /// <param name="Filter">Filter to draw.</param>
        /// <param name="X">X Position to start drawing.</param>
        /// <param name="Y">Y Position to start drawing.</param>
        /// <returns></returns>
        private int DrawFilter(SpriteBatch g, FilterItem Filter, int X, int Y, ref int Index)
        {
            if (Filter.IsOpen)
            {//Loop through every ShopItem.
                for (int i = 0; i < Filter.ListItem.Count; i++)
                {
                    if (Index >= CursorIndexStart && Index < 20 + CursorIndexStart)
                    {
                        if (Filter.ListItem[i] is Weapon)
                            g.DrawString(fntArial8, "Weapon", new Vector2(X + 20, Y), Color.Black);
                        else
                        {
                            Part SmallPart = (Part)Filter.ListItem[i];
                            switch (SmallPart.PartType)
                            {
                                case PartTypes.HeadAntena:
                                    g.DrawString(fntArial8, "Antena", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.HeadEars:
                                    g.DrawString(fntArial8, "Ears", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.HeadEyes:
                                    g.DrawString(fntArial8, "Eyes", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.HeadCPU:
                                    g.DrawString(fntArial8, "CPU", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.TorsoCore:
                                    g.DrawString(fntArial8, "Core", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.TorsoRadiator:
                                    g.DrawString(fntArial8, "Radiator", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.TorsoShell:
                                case PartTypes.ArmShell:
                                case PartTypes.LegsShell:
                                case PartTypes.GenericShell:
                                    g.DrawString(fntArial8, "Shell", new Vector2(X + 20, Y), Color.Black);
                                    break;

                                case PartTypes.ArmStrength:
                                case PartTypes.LegsStrength:
                                case PartTypes.GenericStrength:
                                    g.DrawString(fntArial8, "Strength", new Vector2(X + 20, Y), Color.Black);
                                    break;
                            }
                        }
                        //Draw the name,
                        g.DrawString(fntArial8, Filter.ListItem[i].Name, new Vector2(X + 190, Y), Color.Black, 0, new Vector2(fntArial8.MeasureString(Filter.ListItem[i].Name).X, 0), 1, SpriteEffects.None, 1);
                        //If the current ShopItem is selected, highlight it.
                        if (i == Filter.CursorIndex)
                            g.Draw(sprRectangle, new Rectangle(X + 16, Y, 620 - X, fntArial8.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                        Y += fntArial8.LineSpacing;
                    }
                    Index++;
                }
                //Loop through every FilterItem.
                for (int F = 0; F < Filter.ListFilter.Count; F++)
                {
                    if (Index >= CursorIndexStart && Index < 32 + CursorIndexStart)
                    {
                        if (Filter.ListFilter[F].ListItem.Count == 0)
                        {
                            //If it's open
                            if (Filter.ListFilter[F].IsOpen)
                                g.DrawString(fntArial12, "- " + Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.Black);
                            else
                                g.DrawString(fntArial12, "+ " + Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.Black);
                            //If the current FilterItem is selected, highlight it.
                            if (F == Filter.CursorIndex - Filter.ListItem.Count)
                                g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial12.MeasureString("+ " + Filter.ListFilter[F].Name).X + 10, fntArial12.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                            Y += fntArial12.LineSpacing;
                        }
                        else
                        {
                            g.DrawString(fntArial8, Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.Black);

                            //If the current FilterItem is selected, highlight it.
                            if (F == Filter.CursorIndex - Filter.ListItem.Count)
                                g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial8.MeasureString("+ " + Filter.ListFilter[F].Name).X + 10, fntArial8.LineSpacing), Color.FromNonPremultiplied(255, 255, 255, CursorAlpha));
                            Y += fntArial8.LineSpacing;
                        }
                    }
                    Index++;
                    //Loop in the filter so it draws itself if open.
                    Y = DrawFilter(g, Filter.ListFilter[F], X + 20, Y, ref Index);

                }
            }
            return Y;
        }
    }
}