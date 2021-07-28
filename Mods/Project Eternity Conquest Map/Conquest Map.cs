using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{/*en gros, le but du jeu ce joue principalement sur la capture de batiment, les ville serve a créé de l'argent, les port a créé des bateau, les caserne a créé des unité de terre etc...
  *et ultimement le but par défaut d'une partie c'est sois éliminé tout les unité ennemis ou sois de capturé le QG ennemis
 bob Le Nolife: c'est assez simpliste comme jeu, après il y a des subtilité comme le choix du générale qui donne des bonus, mais ça je m'en fou carrément, je prend toujours les généraux par défaut*/
 //Captured building give 1000g per turn
 //Any unit in a friendly city will repair 2 HP and deduct 20% of its cost
 //http://www.warsworldnews.com/index.php?page=aw/battlemechanics/index.php
 //http://www.warsworldnews.com/aw/damagechart/index.php
 //http://strategywiki.org/wiki/Advance_Wars:_Days_of_Ruin/Terrain
 //http://strategywiki.org/wiki/Advance_Wars:_Days_of_Ruin/Getting_Started
 //http://www.gamefaqs.com/ds/943675-advance-wars-days-of-ruin/faqs/51639
 //http://www.gamesradar.com/cheats/13550/
 //http://Conquest.wikia.com/wiki/Terrain
 //http://Conquest.wikia.com/wiki/Building
 //http://www.advance-wars-net.com/?g=awdor&a=articles/awdor/Terrain.html
 //http://ca.ign.com/wikis/advance-wars-2-black-hole-rising/Basics
 //http://ticc.uvt.nl/~pspronck/pubs/BNAIC2008Bergsma.pdf
 //http://www.warsworldnews.com/dor/aw4-color.pdf attacks

    public class TerrainConquest : Terrain
    {
        public int CapturePoints;
        public int CapturedPlayerIndex;//Index of the player which captured the Property.

        public TerrainConquest(Terrain Other)
            : base(Other)
        {
            TerrainTypeIndex = 0;
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainConquest(int XPos, int YPos)
            : base(XPos, YPos)
        {
            TerrainTypeIndex = 0;
            MVMoveCost = 1;
            CapturedPlayerIndex = -1;
        }

        public TerrainConquest(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
        }

        public TerrainConquest(BinaryReader BR, int XPos, int YPos)
            : base(XPos, YPos)
        {
            TerrainTypeIndex = BR.ReadInt32();

            if (TerrainTypeIndex >= 13)
                CapturePoints = 20;
            else
                CapturePoints = -1;
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
        }
    }

    public abstract class ConquestMapCutsceneScriptHolder : CutsceneScriptHolder
    {
        public abstract class ConquestMapScript : CutsceneActionScript
        {
            protected ConquestMap Map;

            protected ConquestMapScript(ConquestMap Map, int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
                : base(ScriptWidth, ScriptHeight, Name, NameTriggers, NameEvents)
            {
                this.Map = Map;
            }
        }

        public abstract class ConquestDataContainer : CutsceneDataContainer
        {
            protected ConquestMap Map;

            protected ConquestDataContainer(ConquestMap Map, int ScriptWidth, int ScriptHeight, string Name)
                : base(ScriptWidth, ScriptHeight, Name)
            {
                this.Map = Map;
            }
        }
    }

    public partial class ConquestMap : BattleMap
    {
        public List<Dictionary<string, int>> ListUnitMovementCost;//Terrain Type Index, Movement type, how much it cost to move.
        public Dictionary<string, List<string>> DicWeapon1EffectiveAgainst;//Unit Name, Target Name.
        public Dictionary<string, List<string>> DicWeapon2EffectiveAgainst;//Unit Name, Target Name.
        public Dictionary<string, Dictionary<string, int>> DicUnitDamageWeapon1;//Unit Name, <Target Name, Damage>.
        public Dictionary<string, Dictionary<string, int>> DicUnitDamageWeapon2;//Unit Name, <Target Name, Damage>.
        public List<MapLayer> ListLayer;
        public Dictionary<string, int> DicUnitCost;//Unit name, how much it cost to build.
        public int BuildingMenuCursor;
        public List<string> ListCurrentBuildingChoice;
        public Dictionary<string, List<string>> DicBuildingChoice;//Build type, list of units.
        public Vector3 LastPosition;
        public List<Player> ListPlayer;
        public MovementAlgorithm Pathfinder;

        public ConquestMap()
        {
        }

        public ConquestMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad)
            : base()
        {
            this.BattleMapPath = BattleMapPath;
            this.GameMode = GameMode;
            this.ListSpawnSquad = ListSpawnSquad;
            RequireDrawFocus = false;
            Pathfinder = new MovementAlgorithmConquest(this);
            ListPlayer = new List<Player>();
            ListLayer = new List<MapLayer>();
            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTilesetPreset = new List<Terrain.TilesetPreset>();

            CursorPosition = new Vector3(9, 13, 0);
            CursorPositionVisible = CursorPosition;
            ListTerrainType = new List<string>();
            ListCurrentBuildingChoice = new List<string>();

            ListTerrainType.Add("Plains");
            ListTerrainType.Add("Road");
            ListTerrainType.Add("Wood");
            ListTerrainType.Add("Mountains");
            ListTerrainType.Add("Wasteland");
            ListTerrainType.Add("Ruins");
            ListTerrainType.Add("Sea");
            ListTerrainType.Add("Bridge");
            ListTerrainType.Add("River");
            ListTerrainType.Add("Beach");
            ListTerrainType.Add("Rough Sea");
            ListTerrainType.Add("Mist");
            ListTerrainType.Add("Reef");
            ListTerrainType.Add("HQ");
            ListTerrainType.Add("City");
            ListTerrainType.Add("Factory");
            ListTerrainType.Add("Airport");
            ListTerrainType.Add("Port");
            ListTerrainType.Add("Com Tower");
            ListTerrainType.Add("Radar");
            ListTerrainType.Add("Temp Airport");
            ListTerrainType.Add("Temp Port");
            ListTerrainType.Add("Missile Silo");

            ListTileSet = new List<Texture2D>();
            this.CameraPosition = Vector3.Zero;

            this.BattleMapPath = BattleMapPath;
            this.ListPlayer = new List<Player>();
        }
        
        public override void Save(string FilePath)
        {
            //Create the Part file.
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveProperties(BW);

            SaveSpawns(BW);

            MapScript.SaveMapScripts(BW, ListMapScript);

            SaveTilesets(BW);
            
            BW.Write(ListLayer.Count);
            foreach (MapLayer ActiveLayer in ListLayer)
            {
                ActiveLayer.Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        public override void Load()
        {
            base.Load();
            LoadMap();
            LoadMapAssets();
            LoadConquestAIScripts();

            var ConquestScripts = CutsceneScriptHolder.LoadAllScripts(typeof(ConquestMapCutsceneScriptHolder), this);
            foreach (CutsceneScript ActiveListScript in ConquestScripts.Values)
            {
                DicCutsceneScript.Add(ActiveListScript.Name, ActiveListScript);
            }
            
            Player NewPlayer = new Player("Human", "Human", true, false, 0, Color.Red);
            ListPlayer.Add(NewPlayer);

            PopulateUnitMovementCost();
            PopulateUnitCost();
            PopulateBuildingChoice();
            PopulateUnitDamageWeapon1();
            PopulateUnitDamageWeapon2();
        }

        public void LoadMap(bool BackgroundOnly = false)
        {
            //Clear everything.
            ListTileSet = new List<Texture2D>();
            FileStream FS = new FileStream("Content/Maps/Conquest/" + BattleMapPath + ".pem", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            //Map parameters.
            MapName = Path.GetFileNameWithoutExtension(BattleMapPath);

            LoadProperties(BR);

            LoadSpawns(BR);

            ListMapScript = MapScript.LoadMapScripts(BR, DicMapEvent, DicMapCondition, DicMapTrigger, out ListMapEvent);

            LoadTilesets(BR);

            LoadMapGrid(BR);

            BR.Close();
            FS.Close();

            TogglePreview(BackgroundOnly);
        }

        protected void LoadMapGrid(BinaryReader BR)
        {
            int LayerCount = BR.ReadInt32();

            for (int i = 0; i < LayerCount; ++i)
            {
                if (i == 0)
                {
                    ListLayer.Add(new MapLayer(this, ListBackground, ListForeground, BR));
                }
                else
                {
                    ListLayer.Add(new MapLayer(this, null, null, BR));
                }
            }
        }

        public void LoadConquestAIScripts()
        {
            AIScriptHolder.DicAIScripts.Clear();

            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(CoreAI), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in ReflectionHelper.GetContentByName<AIScript>(typeof(ConquestAIScriptHolder), Directory.GetFiles("AI", "*.dll")))
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    AIScriptHolder.DicAIScripts.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }
        }

        public override void TogglePreview(bool UsePreview)
        {
            for (int i = 0; i < ListLayer.Count; ++i)
            {
                ListLayer[i].LayerGrid.TogglePreview(UsePreview);
            }
        }

        public void PopulateUnitMovementCost()
        {
            ListUnitMovementCost = new List<Dictionary<string, int>>(13);

            #region Terrains (0 to 12)

            #region Plains

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[0].Add("Infantry", 1);
            ListUnitMovementCost[0].Add("Bazooka", 1);
            ListUnitMovementCost[0].Add("TireA", 2);
            ListUnitMovementCost[0].Add("TireB", 1);
            ListUnitMovementCost[0].Add("Tank", 1);
            ListUnitMovementCost[0].Add("Air", 1);
            ListUnitMovementCost[0].Add("Ship", -1);
            ListUnitMovementCost[0].Add("Transport", -1);

            #endregion

            #region Road

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[1].Add("Infantry", 1);
            ListUnitMovementCost[1].Add("Bazooka", 1);
            ListUnitMovementCost[1].Add("TireA", 1);
            ListUnitMovementCost[1].Add("TireB", 1);
            ListUnitMovementCost[1].Add("Tank", 1);
            ListUnitMovementCost[1].Add("Air", 1);
            ListUnitMovementCost[1].Add("Ship", -1);
            ListUnitMovementCost[1].Add("Transport", -1);

            #endregion

            #region Wood

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[2].Add("Infantry", 1);
            ListUnitMovementCost[2].Add("Bazooka", 1);
            ListUnitMovementCost[2].Add("TireA", 3);
            ListUnitMovementCost[2].Add("TireB", 3);
            ListUnitMovementCost[2].Add("Tank", 2);
            ListUnitMovementCost[2].Add("Air", 1);
            ListUnitMovementCost[2].Add("Ship", -1);
            ListUnitMovementCost[2].Add("Transport", -1);

            #endregion

            #region Mountain

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[3].Add("Infantry", 2);
            ListUnitMovementCost[3].Add("Bazooka", 1);
            ListUnitMovementCost[3].Add("TireA", -1);
            ListUnitMovementCost[3].Add("TireB", -1);
            ListUnitMovementCost[3].Add("Tank", -1);
            ListUnitMovementCost[3].Add("Air", 1);
            ListUnitMovementCost[3].Add("Ship", -1);
            ListUnitMovementCost[3].Add("Transport", -1);

            #endregion

            #region Wasteland

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[4].Add("Infantry", 1);
            ListUnitMovementCost[4].Add("Bazooka", 1);
            ListUnitMovementCost[4].Add("TireA", 3);
            ListUnitMovementCost[4].Add("TireB", 3);
            ListUnitMovementCost[4].Add("Tank", 2);
            ListUnitMovementCost[4].Add("Air", 1);
            ListUnitMovementCost[4].Add("Ship", -1);
            ListUnitMovementCost[4].Add("Transport", -1);

            #endregion

            #region Ruins

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[5].Add("Infantry", 1);
            ListUnitMovementCost[5].Add("Bazooka", 1);
            ListUnitMovementCost[5].Add("TireA", 2);
            ListUnitMovementCost[5].Add("TireB", 1);
            ListUnitMovementCost[5].Add("Tank", 1);
            ListUnitMovementCost[5].Add("Air", 1);
            ListUnitMovementCost[5].Add("Ship", -1);
            ListUnitMovementCost[5].Add("Transport", -1);

            #endregion

            #region Sea

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[6].Add("Infantry", -1);
            ListUnitMovementCost[6].Add("Bazooka", -1);
            ListUnitMovementCost[6].Add("TireA", -1);
            ListUnitMovementCost[6].Add("TireB", -1);
            ListUnitMovementCost[6].Add("Tank", -1);
            ListUnitMovementCost[6].Add("Air", 1);
            ListUnitMovementCost[6].Add("Ship", 1);
            ListUnitMovementCost[6].Add("Transport", 1);

            #endregion

            #region Bridge

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[7].Add("Infantry", 1);
            ListUnitMovementCost[7].Add("Bazooka", 1);
            ListUnitMovementCost[7].Add("TireA", 1);
            ListUnitMovementCost[7].Add("TireB", 1);
            ListUnitMovementCost[7].Add("Tank", 1);
            ListUnitMovementCost[7].Add("Air", 1);
            ListUnitMovementCost[7].Add("Ship", 1);
            ListUnitMovementCost[7].Add("Transport", 1);

            #endregion

            #region River

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[8].Add("Infantry", 2);
            ListUnitMovementCost[8].Add("Bazooka", 1);
            ListUnitMovementCost[8].Add("TireA", -1);
            ListUnitMovementCost[8].Add("TireB", -1);
            ListUnitMovementCost[8].Add("Tank", -1);
            ListUnitMovementCost[8].Add("Air", 1);
            ListUnitMovementCost[8].Add("Ship", -1);
            ListUnitMovementCost[8].Add("Transport", -1);

            #endregion

            #region Beach

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[9].Add("Infantry", 1);
            ListUnitMovementCost[9].Add("Bazooka", 1);
            ListUnitMovementCost[9].Add("TireA", 2);
            ListUnitMovementCost[9].Add("TireB", 2);
            ListUnitMovementCost[9].Add("Tank", 1);
            ListUnitMovementCost[9].Add("Air", 1);
            ListUnitMovementCost[9].Add("Ship", -1);
            ListUnitMovementCost[9].Add("Transport", 1);

            #endregion

            #region Rough Sea

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[10].Add("Infantry", -1);
            ListUnitMovementCost[10].Add("Bazooka", -1);
            ListUnitMovementCost[10].Add("TireA", -1);
            ListUnitMovementCost[10].Add("TireB", -1);
            ListUnitMovementCost[10].Add("Tank", -1);
            ListUnitMovementCost[10].Add("Air", 1);
            ListUnitMovementCost[10].Add("Ship", 2);
            ListUnitMovementCost[10].Add("Transport", 2);

            #endregion

            #region Mist

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[11].Add("Infantry", -1);
            ListUnitMovementCost[11].Add("Bazooka", -1);
            ListUnitMovementCost[11].Add("TireA", -1);
            ListUnitMovementCost[11].Add("TireB", -1);
            ListUnitMovementCost[11].Add("Tank", -1);
            ListUnitMovementCost[11].Add("Air", 1);
            ListUnitMovementCost[11].Add("Ship", 1);
            ListUnitMovementCost[11].Add("Transport", 1);

            #endregion

            #region Reef

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[12].Add("Infantry", -1);
            ListUnitMovementCost[12].Add("Bazooka", -1);
            ListUnitMovementCost[12].Add("TireA", -1);
            ListUnitMovementCost[12].Add("TireB", -1);
            ListUnitMovementCost[12].Add("Tank", -1);
            ListUnitMovementCost[12].Add("Air", 1);
            ListUnitMovementCost[12].Add("Ship", 2);
            ListUnitMovementCost[12].Add("Transport", 2);

            #endregion

            #endregion

            #region Properties (13 to 22)

            #region HQ

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[13].Add("Infantry", 1);
            ListUnitMovementCost[13].Add("Bazooka", 1);
            ListUnitMovementCost[13].Add("TireA", 1);
            ListUnitMovementCost[13].Add("TireB", 1);
            ListUnitMovementCost[13].Add("Tank", 1);
            ListUnitMovementCost[13].Add("Air", 1);
            ListUnitMovementCost[13].Add("Ship", -1);
            ListUnitMovementCost[13].Add("Transport", -1);

            #endregion

            #region City

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[14].Add("Infantry", 1);
            ListUnitMovementCost[14].Add("Bazooka", 1);
            ListUnitMovementCost[14].Add("TireA", 1);
            ListUnitMovementCost[14].Add("TireB", 1);
            ListUnitMovementCost[14].Add("Tank", 1);
            ListUnitMovementCost[14].Add("Air", 1);
            ListUnitMovementCost[14].Add("Ship", -1);
            ListUnitMovementCost[14].Add("Transport", -1);

            #endregion

            #region Factory

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[15].Add("Infantry", 1);
            ListUnitMovementCost[15].Add("Bazooka", 1);
            ListUnitMovementCost[15].Add("TireA", 1);
            ListUnitMovementCost[15].Add("TireB", 1);
            ListUnitMovementCost[15].Add("Tank", 1);
            ListUnitMovementCost[15].Add("Air", 1);
            ListUnitMovementCost[15].Add("Ship", -1);
            ListUnitMovementCost[15].Add("Transport", -1);

            #endregion

            #region Airport

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[16].Add("Infantry", 1);
            ListUnitMovementCost[16].Add("Bazooka", 1);
            ListUnitMovementCost[16].Add("TireA", 1);
            ListUnitMovementCost[16].Add("TireB", 1);
            ListUnitMovementCost[16].Add("Tank", 1);
            ListUnitMovementCost[16].Add("Air", 1);
            ListUnitMovementCost[16].Add("Ship", -1);
            ListUnitMovementCost[16].Add("Transport", -1);

            #endregion

            #region Port

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[17].Add("Infantry", 1);
            ListUnitMovementCost[17].Add("Bazooka", 1);
            ListUnitMovementCost[17].Add("TireA", 1);
            ListUnitMovementCost[17].Add("TireB", 1);
            ListUnitMovementCost[17].Add("Tank", 1);
            ListUnitMovementCost[17].Add("Air", 1);
            ListUnitMovementCost[17].Add("Ship", 1);
            ListUnitMovementCost[17].Add("Transport", 1);

            #endregion

            #region Com Tower

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[18].Add("Infantry", 1);
            ListUnitMovementCost[18].Add("Bazooka", 1);
            ListUnitMovementCost[18].Add("TireA", 1);
            ListUnitMovementCost[18].Add("TireB", 1);
            ListUnitMovementCost[18].Add("Tank", 1);
            ListUnitMovementCost[18].Add("Air", 1);
            ListUnitMovementCost[18].Add("Ship", -1);
            ListUnitMovementCost[18].Add("Transport", -1);

            #endregion

            #region Radar

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[19].Add("Infantry", 1);
            ListUnitMovementCost[19].Add("Bazooka", 1);
            ListUnitMovementCost[19].Add("TireA", 1);
            ListUnitMovementCost[19].Add("TireB", 1);
            ListUnitMovementCost[19].Add("Tank", 1);
            ListUnitMovementCost[19].Add("Air", 1);
            ListUnitMovementCost[19].Add("Ship", -1);
            ListUnitMovementCost[19].Add("Transport", -1);

            #endregion

            #region Temp Airport

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[20].Add("Infantry", 1);
            ListUnitMovementCost[20].Add("Bazooka", 1);
            ListUnitMovementCost[20].Add("TireA", 1);
            ListUnitMovementCost[20].Add("TireB", 1);
            ListUnitMovementCost[20].Add("Tank", 1);
            ListUnitMovementCost[20].Add("Air", 1);
            ListUnitMovementCost[20].Add("Ship", -1);
            ListUnitMovementCost[20].Add("Transport", -1);

            #endregion

            #region Temp Port

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[21].Add("Infantry", 1);
            ListUnitMovementCost[21].Add("Bazooka", 1);
            ListUnitMovementCost[21].Add("TireA", 1);
            ListUnitMovementCost[21].Add("TireB", 1);
            ListUnitMovementCost[21].Add("Tank", 1);
            ListUnitMovementCost[21].Add("Air", 1);
            ListUnitMovementCost[21].Add("Ship", 1);
            ListUnitMovementCost[21].Add("Transport", 1);

            #endregion

            #region Missile Silo

            ListUnitMovementCost.Add(new Dictionary<string, int>(8));
            ListUnitMovementCost[22].Add("Infantry", 1);
            ListUnitMovementCost[22].Add("Bazooka", 1);
            ListUnitMovementCost[22].Add("TireA", 1);
            ListUnitMovementCost[22].Add("TireB", 1);
            ListUnitMovementCost[22].Add("Tank", 1);
            ListUnitMovementCost[22].Add("Air", 1);
            ListUnitMovementCost[22].Add("Ship", -1);
            ListUnitMovementCost[22].Add("Transport", -1);

            #endregion

            #endregion
        }

        public void PopulateUnitDamageWeapon1()
        {
            DicUnitDamageWeapon1 = new Dictionary<string, Dictionary<string, int>>();
            DicUnitDamageWeapon1.Add("Infantry", new Dictionary<string, int>());

            #region Mech

            DicUnitDamageWeapon1.Add("Mech", new Dictionary<string, int>());//Bazooka
            DicUnitDamageWeapon1["Mech"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Mech"].Add("Mech", -1);
            DicUnitDamageWeapon1["Mech"].Add("Bike", -1);
            DicUnitDamageWeapon1["Mech"].Add("Recon", 85);
            DicUnitDamageWeapon1["Mech"].Add("Flare", 80);
            DicUnitDamageWeapon1["Mech"].Add("Anti-Air", 55);
            DicUnitDamageWeapon1["Mech"].Add("Tank", 55);
            DicUnitDamageWeapon1["Mech"].Add("Md Tank", 25);
            DicUnitDamageWeapon1["Mech"].Add("War Tank", 15);
            DicUnitDamageWeapon1["Mech"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Mech"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Mech"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Mech"].Add("Missile", 85);
            DicUnitDamageWeapon1["Mech"].Add("Rig", 75);

            #endregion

            DicUnitDamageWeapon1.Add("Bike", new Dictionary<string, int>());
            DicUnitDamageWeapon1.Add("Recon", new Dictionary<string, int>());
            DicUnitDamageWeapon1.Add("Flare", new Dictionary<string, int>());

            #region Anti-Air

            DicUnitDamageWeapon1.Add("Anti-Air", new Dictionary<string, int>());//Vulcan Cannon
            DicUnitDamageWeapon1["Anti-Air"].Add("Infantry", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Mech", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Bike", 105);
            DicUnitDamageWeapon1["Anti-Air"].Add("Recon", 60);
            DicUnitDamageWeapon1["Anti-Air"].Add("Flare", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("Anti-Air", 45);
            DicUnitDamageWeapon1["Anti-Air"].Add("Tank", 15);
            DicUnitDamageWeapon1["Anti-Air"].Add("Md Tank", 10);
            DicUnitDamageWeapon1["Anti-Air"].Add("War Tank", 5);
            DicUnitDamageWeapon1["Anti-Air"].Add("Artillery", 50);
            DicUnitDamageWeapon1["Anti-Air"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon1["Anti-Air"].Add("Rocket", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Missile", 55);
            DicUnitDamageWeapon1["Anti-Air"].Add("Rig", 50);

            #endregion

            #region Tank

            DicUnitDamageWeapon1.Add("Tank", new Dictionary<string, int>());//Tank Gun
            DicUnitDamageWeapon1["Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["Tank"].Add("Recon", 85);
            DicUnitDamageWeapon1["Tank"].Add("Flare", 80);
            DicUnitDamageWeapon1["Tank"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Tank"].Add("Tank", 55);
            DicUnitDamageWeapon1["Tank"].Add("Md Tank", 35);
            DicUnitDamageWeapon1["Tank"].Add("War Tank", 20);
            DicUnitDamageWeapon1["Tank"].Add("Artillery", 70);
            DicUnitDamageWeapon1["Tank"].Add("Anti-Tank", 30);
            DicUnitDamageWeapon1["Tank"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Tank"].Add("Missile", 85);
            DicUnitDamageWeapon1["Tank"].Add("Rig", 75);

            #endregion

            #region Medium Tank

            DicUnitDamageWeapon1.Add("Medium Tank", new Dictionary<string, int>());//Heavy Tank Gun
            DicUnitDamageWeapon1["Medium Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["Medium Tank"].Add("Recon", 95);
            DicUnitDamageWeapon1["Medium Tank"].Add("Flare", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Anti-Air", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Tank", 70);
            DicUnitDamageWeapon1["Medium Tank"].Add("Md Tank", 55);
            DicUnitDamageWeapon1["Medium Tank"].Add("War Tank", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Artillery", 85);
            DicUnitDamageWeapon1["Medium Tank"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon1["Medium Tank"].Add("Rocket", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Missile", 90);
            DicUnitDamageWeapon1["Medium Tank"].Add("Rig", 90);

            #endregion

            #region War Tank

            DicUnitDamageWeapon1.Add("War Tank", new Dictionary<string, int>());//Mega Gun
            DicUnitDamageWeapon1["War Tank"].Add("Infantry", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Mech", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Bike", -1);
            DicUnitDamageWeapon1["War Tank"].Add("Recon", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Flare", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Anti-Air", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Tank", 85);
            DicUnitDamageWeapon1["War Tank"].Add("Md Tank", 75);
            DicUnitDamageWeapon1["War Tank"].Add("War Tank", 50);
            DicUnitDamageWeapon1["War Tank"].Add("Artillery", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Anti-Tank", 40);
            DicUnitDamageWeapon1["War Tank"].Add("Rocket", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Missile", 105);
            DicUnitDamageWeapon1["War Tank"].Add("Rig", 105);

            #endregion

            #region Artillery

            DicUnitDamageWeapon1.Add("Artillery", new Dictionary<string, int>());//Cannon
            DicUnitDamageWeapon1["Artillery"].Add("Infantry", 90);
            DicUnitDamageWeapon1["Artillery"].Add("Mech", 85);
            DicUnitDamageWeapon1["Artillery"].Add("Bike", 85);
            DicUnitDamageWeapon1["Artillery"].Add("Recon", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Flare", 75);
            DicUnitDamageWeapon1["Artillery"].Add("Anti-Air", 65);
            DicUnitDamageWeapon1["Artillery"].Add("Tank", 60);
            DicUnitDamageWeapon1["Artillery"].Add("Md Tank", 45);
            DicUnitDamageWeapon1["Artillery"].Add("War Tank", 35);
            DicUnitDamageWeapon1["Artillery"].Add("Artillery", 75);
            DicUnitDamageWeapon1["Artillery"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Artillery"].Add("Rocket", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Missile", 80);
            DicUnitDamageWeapon1["Artillery"].Add("Rig", 70);

            #endregion

            #region Anti-Tank

            DicUnitDamageWeapon1.Add("Anti-Tank", new Dictionary<string, int>());//Cannon
            DicUnitDamageWeapon1["Anti-Tank"].Add("Infantry", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Mech", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Bike", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Recon", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Flare", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Tank", 75);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Md Tank", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("War Tank", 55);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Artillery", 65);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Anti-Tank", 55);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Rocket", 70);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Missile", 70);
            DicUnitDamageWeapon1["Anti-Tank"].Add("Rig", 65);

            #endregion

            #region Rockets

            DicUnitDamageWeapon1.Add("Rockets", new Dictionary<string, int>());//Rockets
            DicUnitDamageWeapon1["Rockets"].Add("Infantry", 95);
            DicUnitDamageWeapon1["Rockets"].Add("Mech", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Bike", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Recon", 90);
            DicUnitDamageWeapon1["Rockets"].Add("Flare", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Anti-Air", 75);
            DicUnitDamageWeapon1["Rockets"].Add("Tank", 70);
            DicUnitDamageWeapon1["Rockets"].Add("Md Tank", 55);
            DicUnitDamageWeapon1["Rockets"].Add("War Tank", 45);
            DicUnitDamageWeapon1["Rockets"].Add("Artillery", 80);
            DicUnitDamageWeapon1["Rockets"].Add("Anti-Tank", 65);
            DicUnitDamageWeapon1["Rockets"].Add("Rocket", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Missile", 85);
            DicUnitDamageWeapon1["Rockets"].Add("Rig", 80);

            #endregion

            #region Missiles

            DicUnitDamageWeapon1.Add("Missiles", new Dictionary<string, int>());//Anti-Air Missiles
            DicUnitDamageWeapon1["Missiles"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Mech", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Bike", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Recon", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Flare", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Anti-Air", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Md Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("War Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Artillery", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Anti-Tank", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Rocket", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Missile", -1);
            DicUnitDamageWeapon1["Missiles"].Add("Rig", -1);

            #endregion

            #region Rig

            DicUnitDamageWeapon1.Add("Rig", new Dictionary<string, int>());
            DicUnitDamageWeapon1["Rig"].Add("Infantry", -1);
            DicUnitDamageWeapon1["Rig"].Add("Mech", -1);
            DicUnitDamageWeapon1["Rig"].Add("Bike", -1);
            DicUnitDamageWeapon1["Rig"].Add("Recon", -1);
            DicUnitDamageWeapon1["Rig"].Add("Flare", -1);
            DicUnitDamageWeapon1["Rig"].Add("Anti-Air", -1);
            DicUnitDamageWeapon1["Rig"].Add("Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Md Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("War Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Artillery", -1);
            DicUnitDamageWeapon1["Rig"].Add("Anti-Tank", -1);
            DicUnitDamageWeapon1["Rig"].Add("Rocket", -1);
            DicUnitDamageWeapon1["Rig"].Add("Missile", -1);
            DicUnitDamageWeapon1["Rig"].Add("Rig", -1);

            #endregion
        }

        public void PopulateUnitDamageWeapon2()
        {
            DicUnitDamageWeapon2 = new Dictionary<string, Dictionary<string, int>>();

            #region Infantry

            DicUnitDamageWeapon2.Add("Infantry", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Infantry"].Add("Infantry", 55);
            DicUnitDamageWeapon2["Infantry"].Add("Mech", 45);
            DicUnitDamageWeapon2["Infantry"].Add("Bike", 45);
            DicUnitDamageWeapon2["Infantry"].Add("Recon", 12);
            DicUnitDamageWeapon2["Infantry"].Add("Flare", 10);
            DicUnitDamageWeapon2["Infantry"].Add("Anti-Air", 3);
            DicUnitDamageWeapon2["Infantry"].Add("Tank", 5);
            DicUnitDamageWeapon2["Infantry"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Infantry"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Infantry"].Add("Artillery", 10);
            DicUnitDamageWeapon2["Infantry"].Add("Anti-Tank", 30);
            DicUnitDamageWeapon2["Infantry"].Add("Rocket", 20);
            DicUnitDamageWeapon2["Infantry"].Add("Missile", 20);
            DicUnitDamageWeapon2["Infantry"].Add("Rig", 14);

            #endregion

            #region Mech

            DicUnitDamageWeapon2.Add("Mech", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Mech"].Add("Infantry", 65);
            DicUnitDamageWeapon2["Mech"].Add("Mech", 55);
            DicUnitDamageWeapon2["Mech"].Add("Bike", 55);
            DicUnitDamageWeapon2["Mech"].Add("Recon", 18);
            DicUnitDamageWeapon2["Mech"].Add("Flare", 15);
            DicUnitDamageWeapon2["Mech"].Add("Anti-Air", 5);
            DicUnitDamageWeapon2["Mech"].Add("Tank", 8);
            DicUnitDamageWeapon2["Mech"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Mech"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Mech"].Add("Artillery", 15);
            DicUnitDamageWeapon2["Mech"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon2["Mech"].Add("Rocket", 35);
            DicUnitDamageWeapon2["Mech"].Add("Missile", 35);
            DicUnitDamageWeapon2["Mech"].Add("Rig", 20);

            #endregion

            #region Bike

            DicUnitDamageWeapon2.Add("Bike", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Bike"].Add("Infantry", 65);
            DicUnitDamageWeapon2["Bike"].Add("Mech", 55);
            DicUnitDamageWeapon2["Bike"].Add("Bike", 55);
            DicUnitDamageWeapon2["Bike"].Add("Recon", 18);
            DicUnitDamageWeapon2["Bike"].Add("Flare", 15);
            DicUnitDamageWeapon2["Bike"].Add("Anti-Air", 5);
            DicUnitDamageWeapon2["Bike"].Add("Tank", 8);
            DicUnitDamageWeapon2["Bike"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Bike"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Bike"].Add("Artillery", 15);
            DicUnitDamageWeapon2["Bike"].Add("Anti-Tank", 35);
            DicUnitDamageWeapon2["Bike"].Add("Rocket", 35);
            DicUnitDamageWeapon2["Bike"].Add("Missile", 35);
            DicUnitDamageWeapon2["Bike"].Add("Rig", 20);

            #endregion

            #region Recon

            DicUnitDamageWeapon2.Add("Recon", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Recon"].Add("Infantry", 75);
            DicUnitDamageWeapon2["Recon"].Add("Mech", 65);
            DicUnitDamageWeapon2["Recon"].Add("Bike", 65);
            DicUnitDamageWeapon2["Recon"].Add("Recon", 35);
            DicUnitDamageWeapon2["Recon"].Add("Flare", 30);
            DicUnitDamageWeapon2["Recon"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Recon"].Add("Tank", 8);
            DicUnitDamageWeapon2["Recon"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Recon"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Recon"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Recon"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon2["Recon"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Recon"].Add("Missile", 55);
            DicUnitDamageWeapon2["Recon"].Add("Rig", 45);

            #endregion

            #region Flare

            DicUnitDamageWeapon2.Add("Flare", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Flare"].Add("Infantry", 80);
            DicUnitDamageWeapon2["Flare"].Add("Mech", 70);
            DicUnitDamageWeapon2["Flare"].Add("Bike", 70);
            DicUnitDamageWeapon2["Flare"].Add("Recon", 60);
            DicUnitDamageWeapon2["Flare"].Add("Flare", 50);
            DicUnitDamageWeapon2["Flare"].Add("Anti-Air", 45);
            DicUnitDamageWeapon2["Flare"].Add("Tank", 10);
            DicUnitDamageWeapon2["Flare"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Flare"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Flare"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Flare"].Add("Anti-Tank", 25);
            DicUnitDamageWeapon2["Flare"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Flare"].Add("Missile", 55);
            DicUnitDamageWeapon2["Flare"].Add("Rig", 45);

            #endregion

            DicUnitDamageWeapon2.Add("Anti-Air", new Dictionary<string, int>());

            #region Tank

            DicUnitDamageWeapon2.Add("Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Tank"].Add("Infantry", 75);
            DicUnitDamageWeapon2["Tank"].Add("Mech", 70);
            DicUnitDamageWeapon2["Tank"].Add("Bike", 70);
            DicUnitDamageWeapon2["Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["Tank"].Add("Rocket", 55);
            DicUnitDamageWeapon2["Tank"].Add("Missile", 55);
            DicUnitDamageWeapon2["Tank"].Add("Rig", 45);

            #endregion

            #region Medium Tank

            DicUnitDamageWeapon2.Add("Medium Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["Medium Tank"].Add("Infantry", 90);
            DicUnitDamageWeapon2["Medium Tank"].Add("Mech", 80);
            DicUnitDamageWeapon2["Medium Tank"].Add("Bike", 80);
            DicUnitDamageWeapon2["Medium Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["Medium Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["Medium Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["Medium Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["Medium Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["Medium Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["Medium Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["Medium Tank"].Add("Rocket", 60);
            DicUnitDamageWeapon2["Medium Tank"].Add("Missile", 60);
            DicUnitDamageWeapon2["Medium Tank"].Add("Rig", 45);

            #endregion

            #region War Tank

            DicUnitDamageWeapon2.Add("War Tank", new Dictionary<string, int>());//Machine gun
            DicUnitDamageWeapon2["War Tank"].Add("Infantry", 90);
            DicUnitDamageWeapon2["War Tank"].Add("Mech", 80);
            DicUnitDamageWeapon2["War Tank"].Add("Bike", 80);
            DicUnitDamageWeapon2["War Tank"].Add("Recon", 40);
            DicUnitDamageWeapon2["War Tank"].Add("Flare", 35);
            DicUnitDamageWeapon2["War Tank"].Add("Anti-Air", 8);
            DicUnitDamageWeapon2["War Tank"].Add("Tank", 8);
            DicUnitDamageWeapon2["War Tank"].Add("Md Tank", 5);
            DicUnitDamageWeapon2["War Tank"].Add("War Tank", 1);
            DicUnitDamageWeapon2["War Tank"].Add("Artillery", 45);
            DicUnitDamageWeapon2["War Tank"].Add("Anti-Tank", 1);
            DicUnitDamageWeapon2["War Tank"].Add("Rocket", 60);
            DicUnitDamageWeapon2["War Tank"].Add("Missile", 60);
            DicUnitDamageWeapon2["War Tank"].Add("Rig", 45);

            #endregion

            DicUnitDamageWeapon2.Add("Artillery", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Anti-Tank", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Rockets", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Missiles", new Dictionary<string, int>());
            DicUnitDamageWeapon2.Add("Rig", new Dictionary<string, int>());
        }

        public void PopulateUnitCost()
        {
            DicUnitCost = new Dictionary<string, int>();

            DicUnitCost.Add("Infantry", 1000);
            DicUnitCost.Add("Mech", 2500);
            DicUnitCost.Add("Bike", 2500);
            DicUnitCost.Add("Recon", 4000);
            DicUnitCost.Add("Flare", 5000);
            DicUnitCost.Add("Anti-Air", 7000);
            DicUnitCost.Add("Tank", 7000);
            DicUnitCost.Add("Medium Tank", 12000);
            DicUnitCost.Add("War Tank", 16000);
            DicUnitCost.Add("Anti-Tank", 11000);
            DicUnitCost.Add("Artillery", 6000);
            DicUnitCost.Add("Rockets", 15000);
            DicUnitCost.Add("Missiles", 12000);
            DicUnitCost.Add("Rig", 5000);
        }

        public void PopulateBuildingChoice()
        {
            DicBuildingChoice = new Dictionary<string, List<string>>();

            DicBuildingChoice.Add("Infantry", new List<string>());
            DicBuildingChoice["Infantry"].Add("Infantry");
            DicBuildingChoice["Infantry"].Add("Mech");
            DicBuildingChoice["Infantry"].Add("Bike");
            DicBuildingChoice.Add("Vehicle", new List<string>());
            DicBuildingChoice["Vehicle"].Add("Recon");
            DicBuildingChoice["Vehicle"].Add("Flare");
            DicBuildingChoice["Vehicle"].Add("Anti-Air");
            DicBuildingChoice["Vehicle"].Add("Tank");
            DicBuildingChoice["Vehicle"].Add("Medium Tank");
            DicBuildingChoice["Vehicle"].Add("War Tank");
            DicBuildingChoice["Vehicle"].Add("Artillery");
            DicBuildingChoice["Vehicle"].Add("Anti-Tank");
            DicBuildingChoice["Vehicle"].Add("Rockets");
            DicBuildingChoice["Vehicle"].Add("Missiles");
            DicBuildingChoice["Vehicle"].Add("Rig");
        }

        public bool CheckForObstacleAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            return CheckForUnitAtPosition(PlayerIndex, Position, Displacement) >= 0;
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            bool ObstacleFound = false;

            for (int P = 0; P < ListPlayer.Count && !ObstacleFound; P++)
                ObstacleFound = CheckForObstacleAtPosition(P, Position, Displacement);

            return ObstacleFound;
        }

        public int CheckForUnitAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListUnit.Count == 0)
                return -1;

            float CurrentZ = Position.Z;
            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X > MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y > MapSize.Y)
                return -1;

            float ZChange = GetTerrain((int)FinalPosition.X, (int)FinalPosition.Y, ActiveLayerIndex).Position.Z - Position.Z;
            FinalPosition.Z += ZChange;

            int S = 0;
            bool SquadFound = false;
            //Check if there's a Construction.
            while (S < ListPlayer[PlayerIndex].ListUnit.Count && !SquadFound)
            {
                if (ListPlayer[PlayerIndex].ListUnit[S] == null)
                {
                    ++S;
                    continue;
                }
                if (ListPlayer[PlayerIndex].ListUnit[S].IsUnitAtPosition(FinalPosition))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }

        public List<Vector3> GetMVChoice(UnitConquest CurrentUnit)
        {
            Vector3 Position = CurrentUnit.Position;

            int MaxMVCost = CurrentUnit.MaxMovement;//Maximum distance you can reach.

            MaxMVCost += CurrentUnit.Boosts.MovementModifier;

            for (int X = -MaxMVCost; X <= MaxMVCost; X++)
            {
                for (int Y = -MaxMVCost; Y <= MaxMVCost; Y++)
                {
                    if (Position.X + X < 0 || Position.X + X >= MapSize.X || Position.Y + Y < 0 || Position.Y + Y >= MapSize.Y)
                        continue;

                    TerrainConquest ActiveTerrain = GetTerrain((int)Position.X + X, (int)Position.Y + Y, CurrentUnit.Components.LayerIndex);
                    ActiveTerrain.Parent = null;

                    int MovementCost;

                    if (ListUnitMovementCost[ActiveTerrain.TerrainTypeIndex].TryGetValue(CurrentUnit.ListTerrainChoices[0], out MovementCost))
                        ActiveTerrain.MVMoveCost = MovementCost;
                    else
                    {
                        ActiveTerrain.MovementCost = -1;
                        continue;
                    }

                    bool UnitFound = false;

                    for (int P = 0; P < ListPlayer.Count && !UnitFound; P++)
                    {//Only check for enemies, can move through allies, can't move through ennemies.
                        if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team)
                            continue;

                        //Check if there's a Unit.
                        //If a Unit was found.
                        if (CheckForObstacleAtPosition(P, Position, new Vector3(X, Y, 0)))
                            UnitFound = true;
                    }
                    //If there is an enemy Unit.
                    if (UnitFound)
                        ActiveTerrain.MovementCost = -1;//Make it impossible to go there.
                    else
                        ActiveTerrain.MovementCost = 0;
                }
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetTerrain((int)Position.X, (int)Position.Y, CurrentUnit.Components.LayerIndex), CurrentUnit.Components, CurrentUnit.UnitStat, MaxMVCost);

            List<Vector3> ListMVChoice = new List<Vector3>();
            for (int i = 0; i < ListAllNode.Count; i++)
            {
                bool UnitFound = false;
                for (int P = 0; P < ListPlayer.Count && !UnitFound; P++)
                {
                    //Don't check for yourself.
                    if (ListAllNode[i].Position.X == Position.X && ListAllNode[i].Position.Y == Position.Y)
                        continue;
                    //Check if there's a Unit.
                    if (CheckForObstacleAtPosition(P, ListAllNode[i].Position, Vector3.Zero))
                        UnitFound = true;
                }
                //If there is no Unit.
                if (!UnitFound)
                    ListMVChoice.Add(ListAllNode[i].Position);
            }

            return ListMVChoice;
        }

        public int GetSquadMaxMovement(UnitConquest ActiveUnit)
        {
            if (ActiveUnit.CurrentMovement == "Air")
            {
                int StartingMV = Math.Min(ActiveUnit.MaxMovement, ActiveUnit.EN);//Maximum distance you can reach.

                return StartingMV;
            }
            else
            {
                int StartingMV = ActiveUnit.MaxMovement;//Maximum distance you can reach.

                return StartingMV;
            }
        }

        public void FinalizeMovement(UnitConquest ActiveUnit)
        {
            TerrainConquest ActiveTerrain = GetTerrain(ActiveUnit.Components);

            if (ActiveUnit.Position != LastPosition && ActiveTerrain.CapturePoints > 0)
                ActiveTerrain.CapturePoints = 20;

            if (!ActiveUnit.IsFlying)
                ActiveUnit.CurrentMovement = ListTerrainType[ActiveTerrain.TerrainTypeIndex];

            //Make it so it can't move anymore.
            ActiveUnit.EndTurn();

            UpdateMapEvent(EventTypeUnitMoved, 0);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < ListLayer.Count; ++i)
            {
                ListLayer[i].Update(gameTime);
            }

            if (!IsInit)
            {
                Init();
            }
            else if (MovementAnimation.Count > 0)
            {
                MoveSquad();
            }
            else if (GameMode == 1)
            {
                if (!ListPlayer[ActivePlayerIndex].IsOnline)
                    ListPlayer[ActivePlayerIndex].PlayerStep(gameTime);
            }
            else
            {
                if (!ListActionMenuChoice.HasMainPanel)
                {
                    if (ListPlayer[ActivePlayerIndex].IsHuman)
                    {
                        ListActionMenuChoice.Add(new ActionPanelPlayerDefault(this));
                    }
                    else
                    {
                        ListActionMenuChoice.Add(new ActionPanelAIDefault(this));
                    }
                }

                ListActionMenuChoice.Last().Update(gameTime);
            }

            UpdateCursorVisiblePosition(gameTime);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.BeginUnscaled(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);

            if (ShowAllLayers)
            {
                for (int i = 0; i < ListLayer.Count; ++i)
                {
                    ListLayer[i].BeginDraw(g);
                }
            }
            else
            {
                ListLayer[ActiveLayerIndex].BeginDraw(g);
            }

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (ShowAllLayers)
            {
                for (int i = 0; i < ListLayer.Count; ++i)
                {
                    ListLayer[i].Draw(g);
                }
            }
            else
            {
                ListLayer[ActiveLayerIndex].Draw(g);
            }

            if (IsOnTop)
            {
                if (ListActionMenuChoice.HasMainPanel)
                {
                    ListActionMenuChoice.Last().Draw(g);
                }
            }
        }

        public TerrainConquest GetTerrain(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public TerrainConquest GetTerrain(UnitMapComponent ActiveUnit)
        {
            return GetTerrain((int)ActiveUnit.X, (int)ActiveUnit.Y, ActiveUnit.LayerIndex);
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override void SaveTemporaryMap()
        {
            throw new NotImplementedException();
        }

        public override GameScreen GetMultiplayerScreen()
        {
            throw new NotImplementedException();
        }

        public override BattleMap GetNewMap(string BattleMapPath, int GameMode, List<Squad> ListSpawnSquad)
        {
            return new ConquestMap(BattleMapPath, GameMode, ListSpawnSquad);
        }

        public override string GetMapType()
        {
            return "Conquest";
        }

        public void SpawnUnit(int PlayerIndex, UnitConquest NewUnit, Vector3 Position)
        {
            NewUnit.Components.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), NewUnit.SpriteMap, 1);
            NewUnit.InitStat();

            while (ListPlayer.Count <= PlayerIndex)
            {
                Player NewPlayer = new Player("Enemy", "CPU", false, false, PlayerIndex, Color.Red);
                ListPlayer.Add(NewPlayer);
            }
            NewUnit.SetPosition(Position);

            ListPlayer[PlayerIndex].IsAlive = true;
            ListPlayer[PlayerIndex].ListUnit.Add(NewUnit);
        }

        public uint GetNextUnusedUnitID()
        {
            uint NewUnitID = 0;
            bool SameIDFound = false;
            do
            {
                SameIDFound = false;
                for (int P = 0; P < ListPlayer.Count; ++P)
                {
                    for (int S = 0; S < ListPlayer[P].ListUnit.Count; S++)
                    {
                        if (ListPlayer[P].ListUnit[S].ID == NewUnitID)
                        {
                            NewUnitID++;
                            SameIDFound = true;
                        }
                    }
                }
            }
            while (SameIDFound);

            return NewUnitID;
        }
    }
}
