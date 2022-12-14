using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptViewTilesPopup : DeathmatchMapScript
        {
            private TileInformationPopupManagerDeathmatch PopupManager;
            private List<Vector2> ListTerrainChangeLocation;
            private bool AnimationFinished;

            public ScriptViewTilesPopup()
                : this(null)
            {
            }

            public ScriptViewTilesPopup(DeathmatchMap Map)
                : base(Map, 140, 70, "View Tiles Popup", new string[] { "Show popup" }, new string[] { "Animation Finished", "Popup closed" })
            {
                ListTerrainChangeLocation = new List<Vector2>();

                if (Map != null)
                {
                    PopupManager = new TileInformationPopupManagerDeathmatch(Map, Map.LayerManager);
                }
            }

            public override void ExecuteTrigger(int Index)
            {
                PopupManager.SetPopups(ListTerrainChangeLocation, 0);
                IsActive = true;
                IsDrawn = true;
            }

            public override void Update(GameTime gameTime)
            {
                PopupManager.Update(gameTime);
                if (PopupManager.HasFinishedAnimating && !AnimationFinished)
                {
                    AnimationFinished = true;
                    ExecuteEvent(this, 0);
                }
                if (PopupManager.IsClosed)
                {
                    ExecuteEvent(this, 1);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                PopupManager.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                int ListTerrainChangeLocationCount = BR.ReadInt32();
                for (int T = 0; T < ListTerrainChangeLocationCount; ++T)
                {
                    ListTerrainChangeLocation.Add(new Vector2(BR.ReadSingle(), BR.ReadSingle()));
                }

                if (PopupManager != null)
                {
                    PopupManager.Load(Map.Content);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(ListTerrainChangeLocation.Count);
                for (int T = 0; T < ListTerrainChangeLocation.Count; ++T)
                {
                    BW.Write(ListTerrainChangeLocation[T].X);
                    BW.Write(ListTerrainChangeLocation[T].Y);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptViewTilesPopup(Map);
            }

            #region Properties

            [Editor(typeof(MapDestinationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Terrain change locations"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public List<Vector2> MapDestination
            {
                get
                {
                    return ListTerrainChangeLocation;
                }
                set
                {
                    ListTerrainChangeLocation = value;
                }
            }
            #endregion
        }
    }
}
