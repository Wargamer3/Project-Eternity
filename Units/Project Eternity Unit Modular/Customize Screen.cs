using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;

namespace ProjectEternity.Core.Units.Modular
{
    public partial class UnitModular : Unit
    {
        public class PartsEquipScreen : GameScreen
        {
            public List<PartHead> ListPartHead = new List<PartHead>();
            public List<PartTorso> ListPartTorso = new List<PartTorso>();
            public List<PartArm> ListPartArm = new List<PartArm>();
            public List<PartLegs> ListPartLegs = new List<PartLegs>();
            public PartHead GetHeadByName(string Name)
            {
                return ListPartHead.Find(delegate (PartHead PartHead)
                { return PartHead.FullName == Name; });
            }

            public PartTorso GetTorsoByName(string Name)
            {
                return ListPartTorso.Find(x => x.FullName == Name);
            }

            public PartArm GetArmByName(string Name)
            {
                return ListPartArm.Find(delegate (PartArm PartArm)
                { return PartArm.FullName == Name; });
            }

            public PartLegs GetLegsByName(string Name)
            {
                return ListPartLegs.Find(delegate (PartLegs PartLegs)
                { return PartLegs.FullName == Name; });
            }
            public void LoadParts()
            {
                string[] Files = Directory.GetFiles("Content/Units/Modular/Head", "*.peup", SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");
                    string Name = CorrectedPath.Substring(0, CorrectedPath.Length - 5).Substring(27);
                    ListPartHead.Add(new PartHead(Name));
                }

                Files = Directory.GetFiles("Content/Units/Modular/Torso", "*.peup", SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");
                    string Name = CorrectedPath.Substring(0, CorrectedPath.Length - 5).Substring(28);
                    ListPartTorso.Add(new PartTorso(Name));
                }

                Files = Directory.GetFiles("Content/Units/Modular/Arm", "*.peup", SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");
                    string Name = CorrectedPath.Substring(0, CorrectedPath.Length - 5).Substring(26);
                    ListPartArm.Add(new PartArm(Name));
                }
                Files = Directory.GetFiles("Content/Units/Modular/Legs", "*.peup", SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");
                    string Name = CorrectedPath.Substring(0, CorrectedPath.Length - 5).Substring(27);
                    ListPartLegs.Add(new PartLegs(Name));
                }
            }

            private enum Selection { None = -1, Head = 0, Torso = 1, LeftArm = 2, RightArm = 3, Legs = 4 };

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

            private Texture2D sprRectangle;
            private SpriteFont fntArial8;
            private SpriteFont fntArial12;
            //Navigation related variables.
            private int CursorAlpha;

            private bool CursorAppearing;
            private int CursorIndex;
            private int CursorIndexMax;
            private int CursorIndexStart;//Starting position of the cursor from where to start drawing.

            private int CursorPartIndex;

            private PartMenu[] PartNames;
            private int Stage;
            private Selection CurrentSelection;
            private UnitModular CurrentUnit;

            private int HPChange;
            private int ENChange;
            private int ArmorChange;
            private int MobilityChange;
            private float MovementChange;

            private FilterItem CursorFilter;
            private FilterItem MainFilter;
            private int MainFilterIndex;

            public PartsEquipScreen(UnitModular CurrentUnit)
                : base()
            {
                this.CurrentUnit = CurrentUnit;

                CursorIndex = 0;
                CursorIndexMax = 0;
                CursorIndexStart = 0;
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
                LoadParts();

                CurrentUnit.Init();

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

                    Head.Add(new FilterItem(CurrentUnit.Parts.Head.FullName, ListItem, new List<FilterItem>()));
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

                    Torso.Add(new FilterItem(CurrentUnit.Parts.Torso.FullName, ListItem, new List<FilterItem>()));
                    Torso[0].IsOpen = true;
                    MainFilter.ListFilter.Add(new FilterItem("Torso", new List<ShopItem>(), Torso));
                }

                #endregion

                #region Left Arm

                if (CurrentUnit.Parts.LeftArm != null)
                {
                    List<FilterItem> LeftArm = new List<FilterItem>();
                    ListItem = new List<ShopItem>();

                    if (CurrentUnit.Parts.LeftArm.Shell != null)
                        ListItem.Add(CurrentUnit.Parts.LeftArm.Shell);
                    if (CurrentUnit.Parts.LeftArm.Strength != null)
                        ListItem.Add(CurrentUnit.Parts.LeftArm.Strength);

                    LeftArm.Add(new FilterItem(CurrentUnit.Parts.LeftArm.FullName, ListItem, new List<FilterItem>()));
                    LeftArm[0].IsOpen = true;
                    MainFilter.ListFilter.Add(new FilterItem("Left Arm", new List<ShopItem>(), LeftArm));
                }

                #endregion

                #region Left Arm

                if (CurrentUnit.Parts.RightArm != null)
                {
                    List<FilterItem> RightArm = new List<FilterItem>();
                    ListItem = new List<ShopItem>();

                    if (CurrentUnit.Parts.RightArm.Shell != null)
                        ListItem.Add(CurrentUnit.Parts.RightArm.Shell);
                    if (CurrentUnit.Parts.RightArm.Strength != null)
                        ListItem.Add(CurrentUnit.Parts.RightArm.Strength);

                    RightArm.Add(new FilterItem(CurrentUnit.Parts.RightArm.FullName, ListItem, new List<FilterItem>()));
                    RightArm[0].IsOpen = true;
                    MainFilter.ListFilter.Add(new FilterItem("Right Arm", new List<ShopItem>(), RightArm));
                }

                #endregion

                #region Legs

                if (CurrentUnit.Parts.Legs != null)
                {
                    List<FilterItem> Legs = new List<FilterItem>();
                    ListItem = new List<ShopItem>();

                    if (CurrentUnit.Parts.Legs.Shell != null)
                        ListItem.Add(CurrentUnit.Parts.Legs.Shell);
                    if (CurrentUnit.Parts.Legs.Strength != null)
                        ListItem.Add(CurrentUnit.Parts.Legs.Strength);

                    Legs.Add(new FilterItem(CurrentUnit.Parts.Legs.FullName, ListItem, new List<FilterItem>()));
                    Legs[0].IsOpen = true;
                    MainFilter.ListFilter.Add(new FilterItem("Legs", new List<ShopItem>(), Legs));
                }

                #endregion

                MainFilter.IsOpen = true;
                MainFilter.CursorIndex = 0;

                CursorFilter = MainFilter;
                SetCursorIndexMax(MainFilter);

                sprRectangle = Content.Load<Texture2D>("Pixel");
                fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
                fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
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
                    if (--CursorAlpha <= 150)
                        CursorAppearing = true;
                }

                #region Stage -1

                if (Stage == -1)
                {
                    if (InputHelper.InputCancelPressed())
                    {
                        RemoveScreen(this);
                    }
                    //Move cursor.
                    if (InputHelper.InputUpPressed())
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
                    else if (InputHelper.InputDownPressed())//Move cursor.
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
                    else if (InputHelper.InputConfirmPressed())
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
                    if (InputHelper.InputUpPressed())
                    {
                        CursorPartIndex -= (CursorPartIndex > 0) ? 1 : 0;
                        UpdateStatsChange();
                    }

                    #region Move down

                    else if (InputHelper.InputDownPressed())
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
                                        CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Head.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count - 1) ? 1 : 0;
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

                                            case PartTypes.Shell:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListPartShell.Count - 1) ? 1 : 0;
                                                break;
                                        }
                                    }
                                    else//Weapons
                                        CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Torso.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count - 1) ? 1 : 0;
                                    break;

                                #endregion

                                #region Left arm

                                case Selection.LeftArm:

                                    if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                                    {
                                        switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                        {
                                            case PartTypes.Shell:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListPartShell.Count - 1) ? 1 : 0;
                                                break;

                                            case PartTypes.Strength:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListPartStrength.Count - 1) ? 1 : 0;
                                                break;
                                        }
                                    }
                                    else//Weapons
                                        CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count - 1) ? 1 : 0;
                                    break;

                                #endregion

                                #region Right arm

                                case Selection.RightArm:

                                    if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                                    {
                                        switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                        {
                                            case PartTypes.Shell:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListPartShell.Count - 1) ? 1 : 0;
                                                break;

                                            case PartTypes.Strength:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListPartStrength.Count - 1) ? 1 : 0;
                                                break;
                                        }
                                    }
                                    else//Weapons
                                        CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.RightArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count - 1) ? 1 : 0;
                                    break;

                                #endregion

                                #region Legs

                                case Selection.Legs:

                                    if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                                    {
                                        switch (((Part)CursorFilter.ListItem[CursorFilter.CursorIndex]).PartType)
                                        {
                                            case PartTypes.Shell:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListPartShell.Count - 1) ? 1 : 0;
                                                break;

                                            case PartTypes.Strength:

                                                CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListPartStrength.Count - 1) ? 1 : 0;
                                                break;
                                        }
                                    }
                                    else//Weapons
                                        CursorPartIndex += (CursorPartIndex < CurrentUnit.Parts.Legs.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count - 1) ? 1 : 0;
                                    break;

                                #endregion
                            }

                            #endregion
                        }
                        UpdateStatsChange();
                    }

                    #endregion

                    else if (InputHelper.InputConfirmPressed())
                    {
                        if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                        {
                            switch (CurrentSelection)
                            {
                                case Selection.Head:
                                    CurrentUnit.Parts.Head = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]);
                                    CursorFilter.Name = CurrentUnit.Parts.Head.FullName;
                                    break;

                                case Selection.Torso:
                                    CurrentUnit.Parts.Torso = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]);
                                    CursorFilter.Name = CurrentUnit.Parts.Torso.FullName;
                                    break;

                                case Selection.LeftArm:
                                    CurrentUnit.Parts.LeftArm = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]);
                                    CursorFilter.Name = CurrentUnit.Parts.LeftArm.FullName;
                                    break;

                                case Selection.RightArm:
                                    CurrentUnit.Parts.RightArm = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]);
                                    CursorFilter.Name = CurrentUnit.Parts.RightArm.FullName;
                                    break;

                                case Selection.Legs:
                                    CurrentUnit.Parts.Legs = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]);
                                    CursorFilter.Name = CurrentUnit.Parts.Legs.FullName;
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

                                                break;

                                            case PartTypes.HeadEars:

                                                break;

                                            case PartTypes.HeadEyes:

                                                break;

                                            case PartTypes.HeadCPU:

                                                break;
                                        }
                                    }
                                    else//Weapons
                                    {
                                        CursorFilter.ListItem[CursorFilter.CursorIndex].FullName = CurrentUnit.Parts.Head.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize][CursorPartIndex];
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

                                                break;

                                            case PartTypes.TorsoRadiator:

                                                break;

                                            case PartTypes.Shell:

                                                break;
                                        }
                                    }
                                    else//Weapons
                                    {
                                        CursorFilter.ListItem[CursorFilter.CursorIndex].FullName = CurrentUnit.Parts.Torso.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize][CursorPartIndex];
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
                                            case PartTypes.Shell:

                                                break;

                                            case PartTypes.Strength:

                                                break;
                                        }
                                    }
                                    else//Weapons
                                    {
                                        CursorFilter.ListItem[CursorFilter.CursorIndex].FullName = CurrentUnit.Parts.LeftArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize][CursorPartIndex];
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
                                            case PartTypes.Shell:

                                                break;

                                            case PartTypes.Strength:

                                                break;
                                        }
                                    }
                                    else//Weapons
                                    {
                                        CursorFilter.ListItem[CursorFilter.CursorIndex].FullName = CurrentUnit.Parts.RightArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize][CursorPartIndex];
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
                                            case PartTypes.Shell:

                                                break;

                                            case PartTypes.Strength:

                                                break;
                                        }
                                    }
                                    else//Weapons
                                    {
                                        CursorFilter.ListItem[CursorFilter.CursorIndex].FullName = CurrentUnit.Parts.Legs.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize][CursorPartIndex];
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

                if (InputHelper.InputCancelPressed())
                {
                    Stage--;
                }

                if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                    RemoveScreen(this);
            }

            public override void Draw(CustomSpriteBatch g)
            {
                g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
                DrawBox(g, new Vector2(0, 0), Constants.Width - 260, Constants.Height - 200, Color.White);
                g.Draw(sprRectangle, new Rectangle(0, Constants.Height - 200, Constants.Height - 100, 97), Color.Black);
                DrawBox(g, new Vector2(0, Constants.Height - 103), Constants.Width - 260,  104, Color.White);

                #region Stage - 1

                if (Stage == -1)
                {
                    int Index = 0;
                    int Y = Constants.Height + 7;
                    //Bottom
                    g.DrawString(fntArial8, "Name: " + CurrentUnit.FullName, new Vector2(8, Y - fntArial8.LineSpacing * 8), Color.Black);
                    g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP, new Vector2(8, Y - fntArial8.LineSpacing * 7), Color.Black);
                    g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN, new Vector2(8, Y - fntArial8.LineSpacing * 6), Color.Black);
                    g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor, new Vector2(8, Y - fntArial8.LineSpacing * 5), Color.Black);
                    g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility, new Vector2(8, Y - fntArial8.LineSpacing * 4), Color.Black);
                    g.DrawString(fntArial8, "Movement: " + CurrentUnit.MaxMovement, new Vector2(8, Y - fntArial8.LineSpacing * 3), Color.Black);
                    g.DrawString(fntArial8, "MEL: " + CurrentUnit.PilotMEL, new Vector2(200, Y - fntArial8.LineSpacing * 7), Color.Black);
                    g.DrawString(fntArial8, "RNG: " + CurrentUnit.PilotRNG, new Vector2(200, Y - fntArial8.LineSpacing * 6), Color.Black);
                    g.DrawString(fntArial8, "DEF: " + CurrentUnit.PilotDEF, new Vector2(200, Y - fntArial8.LineSpacing * 5), Color.Black);
                    g.DrawString(fntArial8, "SKL: " + CurrentUnit.PilotSKL, new Vector2(200, Y - fntArial8.LineSpacing * 4), Color.Black);
                    g.DrawString(fntArial8, "EVA: " + CurrentUnit.PilotEVA, new Vector2(200, Y - fntArial8.LineSpacing * 3), Color.Black);
                    g.DrawString(fntArial8, "HIT: " + CurrentUnit.PilotHIT, new Vector2(200, Y - fntArial8.LineSpacing * 2), Color.Black);
                    //Abilities
                    DrawBox(g, new Vector2(Constants.Width - 260, 0), 260, 50, Color.White);
                    g.DrawString(fntArial12, "Abilities", new Vector2(400, 15), Color.Black);
                    DrawBox(g, new Vector2(Constants.Width - 260, 50), 260, 115, Color.White);
                    g.DrawString(fntArial8, "-", new Vector2(400, 35 + fntArial8.LineSpacing * 2), Color.Black);
                    g.DrawString(fntArial8, "-", new Vector2(400, 35 + fntArial8.LineSpacing * 4), Color.Black);
                    g.DrawString(fntArial8, "-", new Vector2(400, 35 + fntArial8.LineSpacing * 6), Color.Black);
                    g.DrawString(fntArial8, "-", new Vector2(400, 35 + fntArial8.LineSpacing * 8), Color.Black);
                    //Parts
                    DrawBox(g, new Vector2(Constants.Width - 260, 165), 260, 50, Color.White);
                    g.DrawString(fntArial12, "Parts", new Vector2(400, 180), Color.Black);
                    DrawBox(g, new Vector2(Constants.Width - 260, 215), 260, 265, Color.White);
                    DrawFilter(g, MainFilter, 385, 231, ref Index);
                }

                #endregion

                #region Part change

                else
                {
                    #region Base drawing

                    g.Draw(sprRectangle, new Rectangle(0, 0, Constants.Width - 238, Constants.Height), Color.White);
                    g.Draw(sprRectangle, new Rectangle(0, 280, Constants.Height - 100, 97), Color.Black);
                    g.Draw(sprRectangle, new Rectangle(380, 0, 1, Constants.Height), Color.Black);
                    //Bottom
                    g.DrawString(fntArial8, "Name: " + CurrentUnit.FullName, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 8), Color.Black);
                    if (HPChange > 0)
                        g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP + " + " + HPChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 7), Color.Green);
                    else if (HPChange < 0)
                        g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP + " - " + -HPChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 7), Color.Red);
                    else
                        g.DrawString(fntArial8, "HP: " + CurrentUnit.MaxHP, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 7), Color.Black);
                    if (ENChange > 0)
                        g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN + " + " + ENChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 6), Color.Green);
                    else if (ENChange < 0)
                        g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN + " - " + -ENChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 6), Color.Red);
                    else
                        g.DrawString(fntArial8, "EN: " + CurrentUnit.MaxEN, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 6), Color.Black);
                    if (ArmorChange > 0)
                        g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor + " + " + ArmorChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 5), Color.Green);
                    else if (ArmorChange < 0)
                        g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor + " - " + -ArmorChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 5), Color.Red);
                    else
                        g.DrawString(fntArial8, "Armor: " + CurrentUnit.Armor, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 5), Color.Black);
                    if (MobilityChange > 0)
                        g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility + " + " + MobilityChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 4), Color.Green);
                    else if (MobilityChange < 0)
                        g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility + " - " + -MobilityChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 4), Color.Red);
                    else
                        g.DrawString(fntArial8, "Mobility: " + CurrentUnit.Mobility, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 4), Color.Black);
                    if (MovementChange > 0)
                        g.DrawString(fntArial8, "Movement: " + CurrentUnit.MaxMovement + " + " + MovementChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 3), Color.Green);
                    else if (MovementChange < 0)
                        g.DrawString(fntArial8, "Movement: " + CurrentUnit.MaxMovement + " - " + -MovementChange, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 3), Color.Red);
                    else
                        g.DrawString(fntArial8, "Movement: " + CurrentUnit.MaxMovement, new Vector2(5, Constants.Height - fntArial8.LineSpacing * 3), Color.Black);
                    g.DrawString(fntArial8, "MEL: " + CurrentUnit.PilotMEL, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 7), Color.Black);
                    g.DrawString(fntArial8, "RNG: " + CurrentUnit.PilotRNG, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 6), Color.Black);
                    g.DrawString(fntArial8, "DEF: " + CurrentUnit.PilotDEF, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 5), Color.Black);
                    g.DrawString(fntArial8, "SKL: " + CurrentUnit.PilotSKL, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 4), Color.Black);
                    g.DrawString(fntArial8, "EVA: " + CurrentUnit.PilotEVA, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 3), Color.Black);
                    g.DrawString(fntArial8, "HIT: " + CurrentUnit.PilotHIT, new Vector2(200, Constants.Height - fntArial8.LineSpacing * 2), Color.Black);
                    g.Draw(sprRectangle, new Rectangle(Constants.Height - 100, 0, 238, 1), Color.Black);
                    g.DrawString(fntArial12, "Change parts", new Vector2(445, 15), Color.Black);
                    g.Draw(sprRectangle, new Rectangle(380, 50, 238, 1), Color.Black);

                    #endregion

                    #region Part Drawing

                    #region Part Unit

                    if (CursorFilter.CursorIndex >= CursorFilter.ListItem.Count)
                    {
                        switch (CurrentSelection)
                        {
                            case Selection.Head:

                                for (int H = 0; H < CurrentUnit.Parts.ListHead.Count; H++)
                                    g.DrawString(fntArial12, GetHeadByName(CurrentUnit.Parts.ListHead[H]).FullName, new Vector2(420, 60 + H * fntArial12.LineSpacing), Color.Black);
                                break;

                            case Selection.Torso:

                                for (int T = 0; T < CurrentUnit.Parts.ListTorso.Count; T++)
                                    g.DrawString(fntArial12, GetTorsoByName(CurrentUnit.Parts.ListTorso[T]).FullName, new Vector2(420, 60 + T * fntArial12.LineSpacing), Color.Black);
                                break;

                            case Selection.LeftArm:

                                for (int A = 0; A < CurrentUnit.Parts.ListLeftArm.Count; A++)
                                    g.DrawString(fntArial12, GetArmByName(CurrentUnit.Parts.ListLeftArm[A]).FullName, new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                                break;

                            case Selection.RightArm:

                                for (int A = 0; A < CurrentUnit.Parts.ListRightArm.Count; A++)
                                    g.DrawString(fntArial12, GetArmByName(CurrentUnit.Parts.ListRightArm[A]).FullName, new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                                break;

                            case Selection.Legs:

                                for (int L = 0; L < CurrentUnit.Parts.ListLegs.Count; L++)
                                    g.DrawString(fntArial12, GetLegsByName(CurrentUnit.Parts.ListLegs[L]).FullName, new Vector2(420, 60 + L * fntArial12.LineSpacing), Color.Black);
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
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Head.ListPartAntena[A], new Vector2(420, 60 + A * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.HeadEars:

                                            for (int E = 0; E < CurrentUnit.Parts.Head.ListPartEars.Count; E++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Head.ListPartEars[E], new Vector2(420, 60 + E * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.HeadEyes:

                                            for (int E = 0; E < CurrentUnit.Parts.Head.ListPartEyes.Count; E++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Head.ListPartEyes[E], new Vector2(420, 60 + E * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.HeadCPU:

                                            for (int C = 0; C < CurrentUnit.Parts.Head.ListPartCPU.Count; C++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Head.ListPartCPU[C], new Vector2(420, 60 + C * fntArial12.LineSpacing), Color.Black);
                                            break;
                                    }
                                }
                                else//Weapons
                                    for (int W = 0; W < CurrentUnit.Parts.Head.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count; W++)
                                        g.DrawString(fntArial12, CurrentUnit.Parts.Head.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize][W], new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
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
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Torso.ListPartCore[C], new Vector2(420, 60 + C * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.TorsoRadiator:

                                            for (int R = 0; R < CurrentUnit.Parts.Torso.ListPartRadiator.Count; R++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Torso.ListPartRadiator[R], new Vector2(420, 60 + R * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.Shell:

                                            for (int S = 0; S < CurrentUnit.Parts.Torso.ListPartShell.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Torso.ListPartShell[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;
                                    }
                                }
                                else//Weapons
                                    for (int W = 0; W < CurrentUnit.Parts.Torso.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count; W++)
                                        g.DrawString(fntArial12, CurrentUnit.Parts.Torso.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize][W], new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                                break;

                            #endregion

                            #region Left arm

                            case Selection.LeftArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                                {
                                    switch (SmallPart.PartType)
                                    {
                                        case PartTypes.Shell:

                                            for (int S = 0; S < CurrentUnit.Parts.LeftArm.ListPartShell.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.LeftArm.ListPartShell[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.Strength:

                                            for (int S = 0; S < CurrentUnit.Parts.LeftArm.ListPartStrength.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.LeftArm.ListPartStrength[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;
                                    }
                                }
                                else//Weapons
                                    for (int W = 0; W < CurrentUnit.Parts.LeftArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count; W++)
                                        g.DrawString(fntArial12, CurrentUnit.Parts.LeftArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize][W], new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                                break;

                            #endregion

                            #region Right arm

                            case Selection.RightArm:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                                {
                                    switch (SmallPart.PartType)
                                    {
                                        case PartTypes.Shell:

                                            for (int S = 0; S < CurrentUnit.Parts.RightArm.ListPartShell.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.RightArm.ListPartShell[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.Strength:

                                            for (int S = 0; S < CurrentUnit.Parts.RightArm.ListPartStrength.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.RightArm.ListPartStrength[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;
                                    }
                                }
                                else//Weapons
                                    for (int W = 0; W < CurrentUnit.Parts.RightArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count; W++)
                                        g.DrawString(fntArial12, CurrentUnit.Parts.RightArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize][W], new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
                                break;

                            #endregion

                            #region Legs

                            case Selection.Legs:

                                if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                                {
                                    switch (SmallPart.PartType)
                                    {
                                        case PartTypes.Shell:

                                            for (int S = 0; S < CurrentUnit.Parts.Legs.ListPartShell.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Legs.ListPartShell[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;

                                        case PartTypes.Strength:

                                            for (int S = 0; S < CurrentUnit.Parts.Legs.ListPartShell.Count; S++)
                                                g.DrawString(fntArial12, CurrentUnit.Parts.Legs.ListPartShell[S], new Vector2(420, 60 + S * fntArial12.LineSpacing), Color.Black);
                                            break;
                                    }
                                }
                                else//Weapons
                                    for (int W = 0; W < CurrentUnit.Parts.Legs.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count; W++)
                                        g.DrawString(fntArial12, CurrentUnit.Parts.Legs.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize][W], new Vector2(420, 60 + W * fntArial12.LineSpacing), Color.Black);
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

                            HPChange = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]).HP - CurrentUnit.Parts.Head.HP;
                            ENChange = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]).EN - CurrentUnit.Parts.Head.EN;
                            ArmorChange = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]).Armor - CurrentUnit.Parts.Head.Armor;
                            MobilityChange = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]).Mobility - CurrentUnit.Parts.Head.Mobility;
                            MovementChange = GetHeadByName(CurrentUnit.Parts.ListHead[CursorPartIndex]).Movement - CurrentUnit.Parts.Head.Movement;
                            break;

                        case Selection.Torso:

                            HPChange = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]).HP - CurrentUnit.Parts.Torso.HP;
                            ENChange = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]).EN - CurrentUnit.Parts.Torso.EN;
                            ArmorChange = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Armor - CurrentUnit.Parts.Torso.Armor;
                            MobilityChange = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Mobility - CurrentUnit.Parts.Torso.Mobility;
                            MovementChange = GetTorsoByName(CurrentUnit.Parts.ListTorso[CursorPartIndex]).Movement - CurrentUnit.Parts.Torso.Movement;
                            break;

                        case Selection.LeftArm:

                            HPChange = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).HP - CurrentUnit.Parts.LeftArm.HP;
                            ENChange = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).EN - CurrentUnit.Parts.LeftArm.EN;
                            ArmorChange = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Armor - CurrentUnit.Parts.LeftArm.Armor;
                            MobilityChange = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Mobility - CurrentUnit.Parts.LeftArm.Mobility;
                            MovementChange = GetArmByName(CurrentUnit.Parts.ListLeftArm[CursorPartIndex]).Movement - CurrentUnit.Parts.LeftArm.Movement;
                            break;

                        case Selection.RightArm:

                            HPChange = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).HP - CurrentUnit.Parts.RightArm.HP;
                            ENChange = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).EN - CurrentUnit.Parts.RightArm.EN;
                            ArmorChange = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Armor - CurrentUnit.Parts.RightArm.Armor;
                            MobilityChange = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Mobility - CurrentUnit.Parts.RightArm.Mobility;
                            MovementChange = GetArmByName(CurrentUnit.Parts.ListRightArm[CursorPartIndex]).Movement - CurrentUnit.Parts.RightArm.Movement;
                            break;

                        case Selection.Legs:

                            HPChange = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]).HP - CurrentUnit.Parts.Legs.HP;
                            ENChange = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]).EN - CurrentUnit.Parts.Legs.EN;
                            ArmorChange = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Armor - CurrentUnit.Parts.Legs.Armor;
                            MobilityChange = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Mobility - CurrentUnit.Parts.Legs.Mobility;
                            MovementChange = GetLegsByName(CurrentUnit.Parts.ListLegs[CursorPartIndex]).Movement - CurrentUnit.Parts.Legs.Movement;
                            break;
                    }
                }

                #endregion

                #region Parts

                else
                {
                    Part SmallPart = (Part)CursorFilter.ListItem[CursorFilter.CursorIndex];
                    Part ActivePart = null;

                    switch (CurrentSelection)
                    {
                        #region Head

                        case Selection.Head:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Head.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.HeadAntena:
                                        break;

                                    case PartTypes.HeadEars:
                                        break;

                                    case PartTypes.HeadEyes:
                                        break;

                                    case PartTypes.HeadCPU:
                                        break;
                                }
                            }
                            /*else//Weapons
                                CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Head.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Head.BaseSize].Count - 1) ? 1 : 0;*/
                            break;

                        #endregion

                        #region Torso

                        case Selection.Torso:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Torso.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.TorsoCore:
                                        break;

                                    case PartTypes.TorsoRadiator:
                                        break;

                                    case PartTypes.Shell:
                                        break;
                                }
                            }
                            /*else//Weapons
                                CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Torso.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Torso.BaseSize].Count - 1) ? 1 : 0;*/
                            break;

                        #endregion

                        #region Left arm

                        case Selection.LeftArm:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.LeftArm.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.Shell:
                                        break;

                                    case PartTypes.Strength:
                                        break;
                                }
                            }
                            /*else//Weapons
                                CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.LeftArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.LeftArm.BaseSize].Count - 1) ? 1 : 0;*/
                            break;

                        #endregion

                        #region Right arm

                        case Selection.RightArm:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.RightArm.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.Shell:
                                        break;

                                    case PartTypes.Strength:
                                        break;
                                }
                            }
                            /*else//Weapons
                                CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.RightArm.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.RightArm.BaseSize].Count - 1) ? 1 : 0;*/
                            break;

                        #endregion

                        #region Legs

                        case Selection.Legs:

                            if (CursorFilter.CursorIndex < CurrentUnit.Parts.Legs.BaseSize)
                            {
                                switch (SmallPart.PartType)
                                {
                                    case PartTypes.Shell:
                                        break;

                                    case PartTypes.Strength:
                                        break;
                                }
                            }
                            /*else//Weapons
                                CursorPartIndex -= (CursorPartIndex < CurrentUnit.Parts.Legs.ListWeaponSlot[CursorFilter.CursorIndex - CurrentUnit.Parts.Legs.BaseSize].Count - 1) ? 1 : 0;*/
                            break;

                        #endregion
                    }

                    if (ActivePart != null)
                    {
                        HPChange = ActivePart.BaseHP - SmallPart.BaseHP;
                        ENChange = ActivePart.BaseEN - SmallPart.BaseEN;
                        ArmorChange = ActivePart.BaseArmor - SmallPart.BaseArmor;
                        MobilityChange = ActivePart.BaseMobility - SmallPart.BaseMobility;
                        MovementChange = ActivePart.BaseMovement - SmallPart.BaseMovement;
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
            private int DrawFilter(CustomSpriteBatch g, FilterItem Filter, int X, int Y, ref int Index)
            {
                if (Filter.IsOpen)
                {//Loop through every ShopItem.
                    for (int i = 0; i < Filter.ListItem.Count; i++)
                    {
                        if (Index >= CursorIndexStart && Index < 20 + CursorIndexStart)
                        {
                            if (Filter.ListItem[i] is Attack)
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

                                    case PartTypes.Shell:
                                        g.DrawString(fntArial8, "Shell", new Vector2(X + 20, Y), Color.Black);
                                        break;

                                    case PartTypes.Strength:
                                        g.DrawString(fntArial8, "Strength", new Vector2(X + 20, Y), Color.Black);
                                        break;
                                }
                            }
                            //Draw the name,
                            g.DrawStringRightAligned(fntArial8, Filter.ListItem[i].FullName, new Vector2(X + 190, Y), Color.Black);
                            //If the current ShopItem is selected, highlight it.
                            if (i == Filter.CursorIndex)
                                g.Draw(sprRectangle, new Rectangle(X + 16, Y, 620 - X, fntArial8.LineSpacing), Color.FromNonPremultiplied(0, 0, 0, CursorAlpha));
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
                                    g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial12.MeasureString("+ " + Filter.ListFilter[F].Name).X + 10, fntArial12.LineSpacing), Color.FromNonPremultiplied(0, 0, 0, CursorAlpha));
                                Y += fntArial12.LineSpacing;
                            }
                            else
                            {
                                g.DrawString(fntArial8, Filter.ListFilter[F].Name, new Vector2(X + 20, Y), Color.Black);

                                //If the current FilterItem is selected, highlight it.
                                if (F == Filter.CursorIndex - Filter.ListItem.Count)
                                    g.Draw(sprRectangle, new Rectangle(X + 16, Y, (int)fntArial8.MeasureString("+ " + Filter.ListFilter[F].Name).X + 10, fntArial8.LineSpacing), Color.FromNonPremultiplied(0, 0, 0, CursorAlpha));
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
}
