using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelFallToDeath : ActionPanelConquest
    {
        private const string PanelName = "FallToDeath";

        private const double AnimationLengthInSeconds = 1;
        private const double AnimationDamageLengthInSeconds = 3;

        private double TimeElapsed;
        public readonly int PlayerIndex;
        public int SquadIndex;
        private UnitConquest SquadToKill;
        private string DestructionDamage;
        private bool IsDead = false;

        public ActionPanelFallToDeath(ConquestMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelFallToDeath(ConquestMap Map, int PlayerIndex, int SquadIndex)
            : base(PanelName, Map, false)
        {
            this.PlayerIndex = PlayerIndex;
            this.SquadIndex = SquadIndex;

            SquadToKill = Map.ListPlayer[PlayerIndex].ListUnit[SquadIndex];
            DestructionDamage = SquadToKill.HP.ToString();
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (TimeElapsed < AnimationLengthInSeconds)
            {
                SquadToKill.Components.Speed -= new Vector3(0, 0, (float)gameTime.ElapsedGameTime.TotalSeconds*5);
                Vector3 NextPostion = SquadToKill.Position + SquadToKill.Components.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                SquadToKill.SetPosition(NextPostion);

                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (TimeElapsed < AnimationDamageLengthInSeconds)
            {
                if (!IsDead)
                {
                    Map.ListPlayer[PlayerIndex].ListUnit.RemoveAt(SquadIndex);
                    SquadToKill.KillUnit();
                    IsDead = true;
                }

                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
                Map.LayerManager.AddDamageNumber(DestructionDamage, SquadToKill.Position);
            }
            else
            {
                RemoveFromPanelList(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelFallToDeath(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
