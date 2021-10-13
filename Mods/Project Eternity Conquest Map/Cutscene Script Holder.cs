using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{/*en gros, le but du jeu ce joue principalement sur la capture de batiment, les ville serve a créé de l'argent, les port a créé des bateau, les caserne a créé des unité de terre etc...
  *et ultimement le but par défaut d'une partie c'est sois éliminé tout les unité ennemis ou sois de capturé le QG ennemis
 bob Le Nolife: c'est assez simpliste comme jeu, après il y a des subtilité comme le choix du générale qui donne des bonus, mais ça je m'en fou carrément, je prend toujours les généraux par défaut*/
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
}
