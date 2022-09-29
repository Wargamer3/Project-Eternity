using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Units.Transforming
{
    public class ActionPanelTranform2WingmanWill : ActionPanelDeathmatch
    {
        private const string PanelName = "Transform2Will";

        private int ActivePlayerIndex;
        private int TransformingUnitIndex;
        private int TransformationChoice;
        private UnitTransforming TransformingUnit;
        private Squad ActiveSquad;
        private bool ShowSquadMembers;

        public ActionPanelTranform2WingmanWill(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelTranform2WingmanWill(DeathmatchMap Map, int ActivePlayerIndex, int TransformingUnitIndex, int TransformationChoice, Squad ActiveSquad, bool ShowSquadMembers)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.TransformingUnitIndex = TransformingUnitIndex;
            this.TransformationChoice = TransformationChoice;
            this.ActiveSquad = ActiveSquad;
            this.ShowSquadMembers = ShowSquadMembers;

            TransformingUnit = (UnitTransforming)ActiveSquad[TransformingUnitIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                TransformingUnit.ChangeUnit(TransformationChoice);

                Map.ActiveSquadIndex = -1;
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            TransformingUnitIndex = BR.ReadInt32();
            TransformationChoice = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[BR.ReadInt32()];
            ShowSquadMembers = BR.ReadBoolean();
            TransformingUnit = (UnitTransforming)ActiveSquad[TransformingUnitIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(TransformingUnitIndex);
            BW.AppendInt32(TransformationChoice);
            BW.AppendInt32(Map.ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad));
            BW.AppendBoolean(ShowSquadMembers);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelTranform2WingmanWill(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X, Y;

            DrawNextChoice(g);

            X = (Map.CursorPosition.X + 1 - Map.CameraPosition.X) * Map.TileSize.X;
            Y = (Map.CursorPosition.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth - MinActionMenuWidth;

            if (ShowSquadMembers)
            {
                for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
                {
                    GameScreen.DrawBox(g, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), MinActionMenuWidth, PannelHeight, Color.White);
                    if (!(ActiveSquad[U] is UnitTransforming) ||
                        TransformingUnit.PermanentTransformation)
                        TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
                    else
                        TextHelper.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                }
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + ActiveSquad.IndexOf(TransformingUnit) * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));
            }

            for (int U = 0; U < TransformingUnit.ArrayTransformingUnit.Length; ++U)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth + MinActionMenuWidth, (int)Y + U * PannelHeight, 50, 20), Color.Navy);
                if (TransformingUnit.CanTransform(U, ActiveSquad.CurrentWingmanA, ActiveSquad.CurrentWingmanB))
                    TextHelper.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.White);
                else
                    TextHelper.DrawText(g, TransformingUnit.ArrayTransformingUnit[U].TransformingUnitName, new Vector2(X + MinActionMenuWidth + MinActionMenuWidth, Y + U * PannelHeight), Color.Gray);
            }

            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + MinActionMenuWidth, (int)Y + TransformationChoice * PannelHeight, 50, 20), Color.FromNonPremultiplied(255, 255, 255, 128));

            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 - 100, Constants.Height / 2 - 25, 200, 50), Color.Navy);
            TextHelper.DrawText(g, ActiveSquad.CurrentLeader.RelativePath + " will transform into " + TransformingUnit.ArrayTransformingUnit[TransformationChoice].TransformingUnitName + ".\n\rThis will be permanent. Continue?", new Vector2(Constants.Width / 2 - 100, Constants.Height / 2 - 25), Color.White);
        }
    }
}
