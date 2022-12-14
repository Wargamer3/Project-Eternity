using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class ActionPanelSelectConstruction : ActionPanelWorldMap
    {
        private const string PanelName = "Select Construction";

        Construction ActiveConstruction;
        int Index;
        const int ConstructionPerLine = 3;

        public ActionPanelSelectConstruction(WorldMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelSelectConstruction(WorldMap Map, Construction ActiveConstruction)
            : base(PanelName, Map, false)
        {
            this.ActiveConstruction = ActiveConstruction;
            Index = 0;
        }

        public override void OnSelect()
        {
            ActiveConstruction.OpenInteractionMenu();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            #region Move Cursor

            if (InputHelper.InputRightPressed())
            {
                Index++;
                if (Index >= ActiveConstruction.ListInteractionVisible.Count)
                    Index = (ActiveConstruction.ListInteractionVisible.Count / ConstructionPerLine) * ConstructionPerLine;
                else if (Index % ConstructionPerLine == 0)
                    Index -= ConstructionPerLine;
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (Index % ConstructionPerLine == 0)
                {
                    Index += ConstructionPerLine - 1;
                    if (Index >= ActiveConstruction.ListInteractionVisible.Count)
                        Index = ActiveConstruction.ListInteractionVisible.Count - 1;
                }
                else
                    Index--;
            }
            else if (InputHelper.InputUpPressed())
            {
                if ((Index - ConstructionPerLine < 0))
                {
                    Index = (ActiveConstruction.ListInteractionVisible.Count / ConstructionPerLine) * ConstructionPerLine + Index % ConstructionPerLine;
                    if (Index >= ActiveConstruction.ListInteractionVisible.Count)
                        Index -= ConstructionPerLine;
                }
                else
                    Index -= ConstructionPerLine;
            }
            else if (InputHelper.InputDownPressed())
            {
                Index += ConstructionPerLine;
                if (Index >= ActiveConstruction.ListInteractionVisible.Count)
                    Index = Index % ConstructionPerLine;
            }

            #endregion

            else if (InputHelper.InputConfirmPressed())
            {
                AddToPanelListAndSelect(ActiveConstruction.GetSelectionPanel(Map, ActiveConstruction.ListInteractionVisible[Index].Index));

                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveAllSubActionPanels();

                Map.sndCancel.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            Index = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(Index);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSelectConstruction(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveConstruction.DrawExtraMenuInformation(g, Map);

            Map.DrawConstructionMenuInfo(g);
            g.DrawString(Map.fntArial12, ActiveConstruction.Name, new Vector2(Constants.Width - 97, 2), Color.White);
            //Draw Constructions
            for (int C = 0; C < ActiveConstruction.ListInteractionVisible.Count; C++)
            {
                g.Draw(ActiveConstruction.ListInteractionVisible[C].Sprite, new Vector2(Constants.Width - 98 + (C % ConstructionPerLine) * 35, Constants.Height - 200 + (C / ConstructionPerLine) * 35), Color.White);

                if (!ActiveConstruction.IsActionAvailable(Map.ListPlayer[Map.ActivePlayerIndex], ActiveConstruction.ListInteractionVisible[C].Index))
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 98 + (C % ConstructionPerLine) * 35, Constants.Height - 200 + (C / ConstructionPerLine) * 35,
                                                    ActiveConstruction.ListInteractionVisible[C].Sprite.Width, ActiveConstruction.ListInteractionVisible[C].Sprite.Height),
                                                    Color.FromNonPremultiplied(127, 127, 127, 200));
                }
            }

            //Draw Construction cursor
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 98 + (Index % ConstructionPerLine) * 35,
                Constants.Height - 200 + (Index / ConstructionPerLine) * 35, 32, 32), Color.FromNonPremultiplied(255, 255, 255, 190));
        }
    }
}
