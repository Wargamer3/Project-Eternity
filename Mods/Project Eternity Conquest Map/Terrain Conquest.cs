using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

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

        public TerrainConquest(Terrain Other, Point Position, int LayerIndex)
            : base(Other, Position, LayerIndex)
        {
            TerrainTypeIndex = 0;
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainConquest(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = 0;
            CapturedPlayerIndex = -1;
        }

        public TerrainConquest(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
        }

        public TerrainConquest(BinaryReader BR, int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = BR.ReadByte();

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
}
