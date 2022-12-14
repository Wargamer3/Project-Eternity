using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelFallToDeath : ActionPanelDeathmatch
    {
        private const string PanelName = "FallToDeath";

        private const double AnimationLengthInSeconds = 1;
        private const double AnimationDamageLengthInSeconds = 3;

        private double TimeElapsed;
        public readonly int PlayerIndex;
        public int SquadIndex;
        private Squad SquadToKill;
        private string DestructionDamage;
        private bool IsDead = false;

        public ActionPanelFallToDeath(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelFallToDeath(DeathmatchMap Map, int PlayerIndex, int SquadIndex)
            : base(PanelName, Map, false)
        {
            this.PlayerIndex = PlayerIndex;
            this.SquadIndex = SquadIndex;

            SquadToKill = Map.ListPlayer[PlayerIndex].ListSquad[SquadIndex];
            DestructionDamage = SquadToKill.CurrentLeader.HP.ToString();
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (TimeElapsed < AnimationLengthInSeconds)
            {
                SquadToKill.Speed -= new Vector3(0, 0, (float)gameTime.ElapsedGameTime.TotalSeconds*5);
                Vector3 NextPostion = SquadToKill.Position + SquadToKill.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                SquadToKill.SetPosition(NextPostion);

                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (TimeElapsed < AnimationDamageLengthInSeconds)
            {
                if (!IsDead)
                {
                    Map.ListPlayer[PlayerIndex].ListSquad.RemoveAt(SquadIndex);
                    SquadToKill.CurrentLeader.KillUnit();
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
