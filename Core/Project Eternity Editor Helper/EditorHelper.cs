using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectEternity.Core.Editor
{
    public delegate IEnumerable<ItemContainer> GetItems(string RootPath);

    public delegate ItemInfo GetItemInfo(string RootPath, string Key);

    public delegate string RenameItem();

    public class EditorHelper
    {
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
        public const string GUIRootPathBuildings = "Buildings";
        public const string GUIRootPathBuildingsConquest = "Conquest/Buildings";
        public const string GUIRootPathBuildingsWorldMap = "World Map/Buildings";
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
        public const string GUIRootPathConquestMaps = "Maps/Conquest";
        public const string GUIRootPathRacingMaps = "Maps/Racing Maps";
        public const string GUIRootPathSorcererStreetMaps = "Maps/Sorcerer Street Maps";
        public const string GUIRootPathTripleThunderMaps = "Maps/Triple Thunder Maps";
        public const string GUIRootPathTripleThunderRessources = "Maps/Triple Thunder Ressources";
        public const string GUIRootPathMapTilesets = "Maps/Tilesets";
        public const string GUIRootPathMapTilesetImages = "Maps/Tilesets Images";
        public const string GUIRootPathMapTilesetPresets = "Maps/Tileset Presets";
        public const string GUIRootPathMapTilesetPresetsConquest = "Maps/Tileset Presets/Conquest";
        public const string GUIRootPathMapTilesetPresetsDeathmatch = "Maps/Tileset Presets/Deathmatch";
        public const string GUIRootPathMapTilesetPresetsSorcererStreet = "Maps/Tileset Presets/Sorcerer Street";
        public const string GUIRootPathMapAutotilesImages = "Maps/Autotiles Images";
        public const string GUIRootPathMapAutotilesPresets = "Maps/Autotiles Presets";
        public const string GUIRootPathMapAutotilesPresetsConquest = "Maps/Autotiles Presets/Conquest";
        public const string GUIRootPathMapAutotilesPresetsDeathmatch = "Maps/Autotiles Presets/Deathmatch";
        public const string GUIRootPathMapAutotilesPresetsSorcererStreet = "Maps/Autotiles Presets/Sorcerer Street";
        public const string GUIRootPathMapDestroyableTilesImages = "Maps/Destroyable Tiles Images";
        public const string GUIRootPathMapDestroyableTilesPresets = "Maps/Destroyable Tiles Presets";
        public const string GUIRootPathMapDestroyableTilesPresetsConquest = "Maps/Destroyable Tiles Presets/Conquest";
        public const string GUIRootPathMapDestroyableTilesPresetsDeathmatch = "Maps/Destroyable Tiles Presets/Deathmatch";
        public const string GUIRootPathMapDestroyableTilesPresetsSorcererStreet = "Maps/Destroyable Tiles Presets/Sorcerer Street";
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

        protected static ItemSelector ItemSelectorMenu;
        public static GetItems GetItemsByRoot;
        public static GetItemInfo GetItemByKey;

        #region Create Item selection menu

        public static List<string> ShowContextMenuWithItem(string RootPath, string MenuText = null, bool MutipleSelection = true, params string[] ArrayExtraItems)
        {
            ItemSelectorMenu = null;

            CreateListMenuWithItem(RootPath, MutipleSelection);
            if (ItemSelectorMenu == null)
                ItemSelectorMenu = new ItemSelector();

            if (MenuText != null)
                ItemSelectorMenu.Text = MenuText;

            ItemSelectorMenu.AddExtraItems(ArrayExtraItems);
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
}
