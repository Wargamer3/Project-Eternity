using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ProjectEternity.Core.Editor
{
    public delegate IEnumerable<ItemContainer> GetItems(string RootPath);

    public delegate ItemInfo GetItemInfo(string RootPath, string Key);

    public delegate string RenameItem();

    public partial class BaseEditor : Form
    {
        public string FilePath;
        protected static ItemSelector ItemSelectorMenu;
        public static GetItems GetItemsByRoot;
        public static GetItemInfo GetItemByKey;
        public const string GUIRootPathAnimations = "Animations";
        public const string GUIRootPathAnimationsSprites = "Animations/Sprites";
        public const string GUIRootPathAnimationsSpriteSheets = "Animations/Sprite Sheets";
        public const string GUIRootPathAnimationsBitmapAnimations = "Animations/Bitmap Animations";
        public const string GUIRootPathAnimationsBackgroundsAll = "Animations/Backgrounds";
        public const string GUIRootPathAnimationsBackgrounds3D = "Animations/Backgrounds 3D";
        public const string GUIRootPathAnimationsBackgrounds2D = "Animations/Backgrounds 2D";
        public const string GUIRootPathAnimationsBackground2DObject = "Animations/Background 2D Objects";
        public const string GUIRootPathAnimationsBackgroundSprites = "Animations/Backgrounds/Sprites";
        public const string GUIRootPathAnimationsBackground2DUsableItems = "Animations/Backgrounds 2D Usable Items";
        public const string GUIRootPathAnimationsBackground3DUsableItems = "Animations/Backgrounds 3D Usable Items";
        public const string GUIRootPathAnimationsBackground3DModels = "Animations/Models";
        public const string GUIRootPathSFX = "SFX";
        public const string GUIRootPathUnits = "Units";
        public const string GUIRootPathUnitsNormal = "Units\\Normal";
        public const string GUIRootPathUnitsNormalMapSprites = "Units/Normal/Map Sprites";
        public const string GUIRootPathUnitsNormalUnitSprites = "Units/Normal/Unit Sprites";
        public const string GUIRootPathUnitsNormalUnitModels = "Units/Normal/Models";
        public const string GUIRootPathUnitsBuilder = "Units\\Builder";
        public const string GUIRootPathUnitsConquest = "Units\\Conquest";
        public const string GUIRootPathUnitsMultiForm = "Units\\Multi-Form";
        public const string GUIRootPathUnitsCombining = "Units\\Combining";
        public const string GUIRootPathUnitsHub = "Units\\Hub";
        public const string GUIRootPathUnitsMagic = "Units\\Magic";
        public const string GUIRootPathUnitsTransforming = "Units\\Transforming";
        public const string GUIRootPathUnitsTripleThunder = "Units\\Triple Thunder";
        public const string GUIRootPathUnitsVehicleTripleThunder = "Units\\Triple Thunder\\Vehicle";
        public const string GUIRootPathUnitAbilities = "Units\\Abilities";
        public const string GUIRootPathUnitParts = "Units\\Parts";
        public const string GUIRootPathUnitStandardParts = "Units\\Standard Parts";
        public const string GUIRootPathUnitConsumableParts = "Units\\Consumable Parts";
        public const string GUIRootPathAttacks = "Attacks";
        public const string GUIRootPathAttackAttributes = "Attacks/Attributes";
        public const string GUIRootPathAttackModels = "Attacks/Models";
        public const string GUIRootPathAttackSkillChains = "Attacks/Skill Chains";
        public const string GUIRootPathAIs = "AIs";
        public const string GUIRootPathVehicles = "Vehicles";
        public const string GUIRootPathVehicleSprites = "Vehicles/Sprites";
        public const string GUIRootPathSpells = "Spells";
        public const string GUIRootPathTripleThunderCombos = "Triple Thunder/Combos";
        public const string GUIRootPathTripleThunderWeapons = "Triple Thunder/Weapons";
        public const string GUIRootPathTripleThunderProjectiles = "Triple Thunder/Projectiles";
        public const string GUIRootPathTripleThunderSkillChains = "Triple Thunder/Skill Chains";
        public const string GUIRootPathCharacters = "Characters";
        public const string GUIRootPathCharacterSkills = "Character/Skills";
        public const string GUIRootPathCharacterSpirits = "Character/Spirits";
        public const string GUIRootPathCharacterRelationships = "Character/Relationships";
        public const string GUIRootPathCharacterPersonalities = "Character/Personalities";
        public const string GUIRootPathVisualNovel = "Visual Novels";
        public const string GUIRootPathVisualNovelCharacters = "Visual Novels/Characters";
        public const string GUIRootPathVisualNovelBustPortraits = "Visual Novels/Bust Portraits";
        public const string GUIRootPathVisualNovelPortraits = "Visual Novels/Portraits";
        public const string GUIRootPathVisualNovelBackgrounds = "Visual Novels/Backgrounds";
        public const string GUIRootPathMaps = "Maps";
        public const string GUIRootPathDeathmatchMaps = "Deathmatch/Maps";
        public const string GUIRootPathWorldMaps = "Maps/World Maps";
        public const string GUIRootPathAdventureMaps = "Maps/Adventure Maps";
        public const string GUIRootPathAdventureRessources = "Maps/Adventure Ressources";
        public const string GUIRootPathConquestMaps = "Maps/Conquest Maps";
        public const string GUIRootPathRacingMaps = "Maps/Racing Maps";
        public const string GUIRootPathSorcererStreetMaps = "Maps/Sorcerer Street Maps";
        public const string GUIRootPathTripleThunderMaps = "Maps/Triple Thunder Maps";
        public const string GUIRootPathTripleThunderRessources = "Maps/Triple Thunder Ressources";
        public const string GUIRootPathMapTilesets = "Maps/Tilesets";
        public const string GUIRootPathMapTilesetImages = "Maps/Tilesets Images";
        public const string GUIRootPathMapTilesetPresets = "Maps/Tileset Presets";
        public const string GUIRootPathMapModels = "Maps/Models";
        public const string GUIRootPathMapBGM = "Maps/BGM";
        public const string GUIRootPathCutscenes = "Cutscenes";
        public const string GUIRootPathScenes = "Scenes";
        public const string GUIRootPathSorcererStreetCards = "SorcererStreet/Cards";
        public const string GUIRootPathSorcererStreetCharacters = "SorcererStreet/Characters";
        public const string GUIRootPathSorcererStreetCardsCreatures = "SorcererStreet/Cards/Creatures";
        public const string GUIRootPathSorcererStreetCardsItems = "SorcererStreet/Cards/Items";
        public const string GUIRootPathSorcererStreetCardsSpells = "SorcererStreet/Cards/Spells";
        public const string GUIRootPathSorcererStreetSkillChains = "SorcererStreet/Skill Chains";
        public const string GUIRootPathSorcererStreetShopSprites = "SorcererStreet/Shop Sprites";
        public const string GUIRootPathSorcererStreetMapSprites = "SorcererStreet/Map Sprites";
        public const string GUIRootPathSorcererStreetCharacterModels = "SorcererStreet/Models/Characters";
        public const string GUIRootPathSorcererStreetSpells = "SorcererStreet/Spells";

        public BaseEditor()
        {
            InitializeComponent();
        }

        public virtual void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false) { throw new NotImplementedException(); }

        public virtual void DeleteItem()
        {
            File.Delete(FilePath);
        }

        public virtual EditorInfo[] LoadEditors() { throw new NotImplementedException(); }

        public static Dictionary<string, T> GetAllExtensions<T>(params object[] Args)
        {
            Dictionary<string, T> DicExtension = LoadFromAssemblyFiles<T>(Directory.GetFiles("Editors Extensions", "*.dll", SearchOption.AllDirectories), typeof(T), Args);

            return DicExtension;
        }

        public static Dictionary<string, T> LoadFromAssembly<T>(Assembly ActiveAssembly, Type TypeOfExtension, params object[] Args)
        {
            Dictionary<string, T> DicExtension = new Dictionary<string, T>();
            List<T> ListExtension = ReflectionHelper.GetObjectsFromBaseInterface<T>(TypeOfExtension, ActiveAssembly.GetTypes(), Args);

            foreach (T Instance in ListExtension)
            {
                DicExtension.Add(Instance.ToString(), Instance);
            }

            return DicExtension;
        }

        public static Dictionary<string, T> LoadFromAssemblyFiles<T>(string[] ArrayFilePath, Type TypeOfExtension, params object[] Args)
        {
            Dictionary<string, T> DicExtension = new Dictionary<string, T>();

            for (int F = 0; F < ArrayFilePath.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(ArrayFilePath[F]));
                foreach (KeyValuePair<string, T> ActiveExtension in LoadFromAssembly<T>(ActiveAssembly, TypeOfExtension, Args))
                {
                    DicExtension.Add(ActiveExtension.Key, ActiveExtension.Value);
                }
            }

            return DicExtension;
        }

        #region Create Item selection menu

        public static List<string> ShowContextMenuWithItem(string RootPath, string MenuText = null, bool MutipleSelection = true)
        {
            ItemSelectorMenu = null;

            CreateListMenuWithItem(RootPath, MutipleSelection);
            if (ItemSelectorMenu == null)
                ItemSelectorMenu = new ItemSelector();

            if (MenuText != null)
                ItemSelectorMenu.Text = MenuText;
            ItemSelectorMenu.TopMost = true;
            ItemSelectorMenu.ShowDialog();
            //Item selected, call the ItemsSelected method.
            if (ItemSelectorMenu.DialogResult == DialogResult.OK)
                return ItemSelectorMenu.GetResult();
            return null;
        }

        public static string GetItemPathInRoot(string RootPath, string ItemPath)
        {
            MenuFilter OutMenu;
            GetMenuItemsFromRoot(RootPath, out OutMenu);
            if (OutMenu == null)
                return null;

            string Output = GetItemPathInRoot(OutMenu, ItemPath);
            if (Output != null)
            {
                return Output;
            }
            
            return null;
        }

        private static string GetItemPathInRoot(MenuFilter OutMenu, string ItemPath)
        {
            if (OutMenu.ListItem != null && OutMenu.ListItem.ContainsKey(ItemPath))
            {
                return OutMenu.ListItem[ItemPath];
            }

            foreach (MenuFilter Menu in OutMenu.ListMenu.Values)
            {
                string Output = GetItemPathInRoot(Menu, ItemPath);
                if (Output != null)
                {
                    return Output;
                }
            }

            return null;
        }

        private static void GetMenuItemsFromRoot(string RootPath, out MenuFilter OutMenu)
        {
            IEnumerable<ItemContainer> Items = GetItemsByRoot(RootPath);
            if (Items == null)
            {
                OutMenu = null;
                return;
            }
            MenuFilter RootMenu = new MenuFilter();
            MenuFilter ActiveMenuThing = RootMenu;
            OutMenu = new MenuFilter();
            OutMenu.ListItem = new Dictionary<string, string>();

            string[] SubString;

            //Create the List of paths to use.
            foreach (ItemContainer ActiveItem in Items)
            {
                ActiveMenuThing = RootMenu;
                SubString = ActiveItem.ContainerGUIPath.Split('\\');
                for (int s = 0; s < SubString.Count(); s++)
                {
                    if (!ActiveMenuThing.ListMenu.ContainsKey(SubString[s]))
                        ActiveMenuThing.ListMenu.Add(SubString[s], new MenuFilter());

                    ActiveMenuThing = ActiveMenuThing.ListMenu[SubString[s]];
                }
                ActiveMenuThing.ListItem = ActiveItem.ListItem;//Assign the ListItem to last part of the path list.
                ActiveMenuThing.MenuCount = 1;
            }
            //Count sub MenuFilters
            UpdatePath(ref RootMenu);

            CreateListMenuRoot(ref OutMenu, ref RootMenu, false, null);
        }

        private static void UpdatePath(ref MenuFilter Item)
        {
            MenuFilter ItemRef;
            for (int i = 0; i < Item.ListMenu.Count; i++)
            {
                ItemRef = Item.ListMenu.ElementAt(i).Value;
                UpdatePath(ref ItemRef);
                if (ItemRef.MenuCount > 0)//If the sub-Menuthing have items, increment the ItemCount.
                    Item.MenuCount++;
            }
        }

        #region List Menu

        private static void CreateListMenuWithItem(string RootPath, bool MutipleSelection = true)
        {
            MenuFilter OutMenu;
            GetMenuItemsFromRoot(RootPath, out OutMenu);
            if (OutMenu == null)
                return;
            
            ItemSelectorMenu = new ItemSelector(OutMenu, MutipleSelection);
        }

        private static void CreateListMenuRoot(ref MenuFilter cmsRoot, ref MenuFilter RootMenu, bool IsSubFilter, string MenuKey)
        {
            MenuFilter ActiveMenu;
            MenuFilter NewMenu;

            //Add the sub menus.
            for (int i = 0; i < RootMenu.ListMenu.Count; i++)
            {
                ActiveMenu = RootMenu.ListMenu.ElementAt(i).Value;
                //If there is multiple sub MenuFilter.
                if (RootMenu.MenuCount > 1)
                {
                    if (ActiveMenu.ListItem != null)
                    {
                        NewMenu = new MenuFilter();
                        cmsRoot.ListMenu.Add(RootMenu.ListMenu.ElementAt(i).Key + " ", NewMenu);//Filter name + Item Name.

                        //Asign the new MenuFilter content.
                        CopyMenuFilter(ref NewMenu, ActiveMenu, RootMenu.ListMenu.ElementAt(i).Key + " ");
                    }
                    else
                        CreateListMenuRoot(ref cmsRoot, ref ActiveMenu, true, RootMenu.ListMenu.ElementAt(i).Key);
                }
                else
                {
                    if (ActiveMenu.ListItem != null && IsSubFilter)
                    {
                        NewMenu = new MenuFilter();
                        cmsRoot.ListMenu.Add(MenuKey + " " + RootMenu.ListMenu.ElementAt(i).Key, NewMenu);//Filter name + Item Name.

                        //Asign the new MenuFilter content.
                        CopyMenuFilter(ref NewMenu, ActiveMenu, RootMenu.ListMenu.ElementAt(i).Key + " ");
                    }
                    else
                        CreateListMenuRoot(ref cmsRoot, ref ActiveMenu, false, null);
                }
            }
            //Add the items.
            if (RootMenu.ListItem != null)
            {
                foreach (KeyValuePair<string, string> Item in RootMenu.ListItem)
                {
                    cmsRoot.ListItem.Add(Item.Key, Item.Value);
                }
            }
        }

        /// <summary>
        /// Create a new MenuFilter based on an other.
        /// </summary>
        /// <param name="InputMenu">The MenuFilter to create content in.</param>
        /// <param name="CopyMenu">The MenuFilter used to copy content.</param>
        /// <param name="ParentKey">The parent key used to merge with a new MenuFilter.</param>
        private static void CopyMenuFilter(ref MenuFilter InputMenu, MenuFilter CopyMenu, string ParentKey = "")
        {
            ToolStripMenuItem tsiItems = new ToolStripMenuItem();
            MenuFilter ActiveMenu;
            MenuFilter NewMenu;

            for (int i = 0; i < CopyMenu.ListMenu.Count; i++)
            {
                ActiveMenu = CopyMenu.ListMenu.ElementAt(i).Value;
                NewMenu = new MenuFilter();
                if (ActiveMenu.ListItem != null)
                    InputMenu.ListMenu.Add(ParentKey + CopyMenu.ListMenu.ElementAt(i).Key, NewMenu);//Filter name + Item Name.
                //Asign the new MenuFilter content.
                CopyMenuFilter(ref NewMenu, ActiveMenu, ParentKey + " " + CopyMenu.ListMenu.ElementAt(i).Key);
            }
            if (CopyMenu.ListItem != null)
            {
                InputMenu.ListItem = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> Item in CopyMenu.ListItem)
                {
                    InputMenu.ListItem.Add(Item.Key, Item.Value);
                }
            }
        }

        #endregion

        #endregion
    }

    public class MenuFilter
    {
        public int MenuCount;
        public Dictionary<string, MenuFilter> ListMenu;
        //Item logic name, Item full path
        public Dictionary<string, string> ListItem;

        public MenuFilter()
        {
            this.ListMenu = new Dictionary<string, MenuFilter>();
            ListItem = null;
            MenuCount = 0;
        }
    }

    public struct ItemInfo
    {
        public string Path;
        public string Name;

        public ItemInfo(string Path, string Name)
        {
            this.Path = Path;
            this.Name = Name;
        }
    }

    public struct EditorInfo
    {
        public Type EditorType;
        public ItemContainer ItemContainer;
        public string[] ArrayLogicPath;//Used for grouping editors.
        public bool CanCreateNewItems;
        public object[] InitParams;

        public string[] ArrayFileExtention;

        public bool IsFolder;
        public string FolderPath;

        public EditorInfo(EditorInfo Clone)
        {
            this.EditorType = Clone.EditorType;
            this.ArrayLogicPath = Clone.ArrayLogicPath;
            this.ItemContainer = new ItemContainer(Clone.ItemContainer);
            this.InitParams = Clone.InitParams;
            this.CanCreateNewItems = Clone.CanCreateNewItems;
            this.ArrayFileExtention = Clone.ArrayFileExtention;
            this.IsFolder = false;
            FolderPath = "";
        }

        public EditorInfo(string[] ArrayLogicPath, string GUIContainerPath, string[] ArrayFileExtention, Type EditorType,
            bool CanCreateNewItems = true, object[] InitParams = null, bool AddNoneEntry = false)
        {
            this.EditorType = EditorType;
            this.ArrayLogicPath = ArrayLogicPath;
            this.InitParams = InitParams;
            this.CanCreateNewItems = CanCreateNewItems;
            this.ArrayFileExtention = ArrayFileExtention;
            this.IsFolder = false;
            FolderPath = "";

            string ContainerRootPath = "Content/" + GUIContainerPath;
            Dictionary<string, string> ListItem = new Dictionary<string, string>();

            if (AddNoneEntry)
            {
                ListItem.Add("None", null);
            }

            string[] Files;

            foreach (string FileExtention in ArrayFileExtention)
            {
                Files = Directory.GetFiles(ContainerRootPath, "*" + FileExtention, SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");

                    //Handle case where GetFiles doesn't return the proper files.
                    if (Item.EndsWith(FileExtention))
                    {
                        ListItem.Add(CorrectedPath.Substring(0, CorrectedPath.Length - FileExtention.Length).Replace(ContainerRootPath, ""), CorrectedPath);
                    }
                }
            }

            ItemContainer = new ItemContainer(GUIContainerPath, ContainerRootPath, ListItem);
        }
    }

    public struct ItemContainer
    {
        //Item logic name, Item full path
        public Dictionary<string, string> ListItem;

        public readonly string ContainerGUIPath;
        public readonly string ContainerRootPath;

        public ItemContainer(string ContainerGUIPath, string ContainerRootPath, Dictionary<string, string> ListItem)
        {
            this.ContainerGUIPath = ContainerGUIPath;
            this.ContainerRootPath = ContainerRootPath;
            this.ListItem = ListItem;
        }

        public ItemContainer(ItemContainer Clone)
        {
            ContainerGUIPath = Clone.ContainerGUIPath;
            ContainerRootPath = Clone.ContainerRootPath;
            ListItem = new Dictionary<string, string>(Clone.ListItem.Count);
            foreach (KeyValuePair<string, string> Item in Clone.ListItem)
            {
                ListItem.Add(Item.Key, Item.Value);
            }
        }
    }
}
