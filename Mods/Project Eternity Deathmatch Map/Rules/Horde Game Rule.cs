using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class HordeGameInfo : GameModeInfo
    {
        public const string ModeName = "Horde";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public HordeGameInfo(bool IsUnlocked, Texture2D sprPreview)
            : base(ModeName, "Wave survival mode, respawn at the start of each wave.", CategoryPVE, IsUnlocked, sprPreview)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new HordeGameRule((DeathmatchMap)Map, this);
        }

        public override GameModeInfo Copy()
        {
            return new HordeGameInfo(IsUnlocked, sprPreview);
        }

        [DisplayNameAttribute("Maximum Resapwn"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("How many points are allowed to respawn."),
        DefaultValueAttribute(3)]
        public int MaximumResapwn
        {
            get
            {
                return _MaximumResapwn;
            }
            set
            {
                _MaximumResapwn = value;
            }
        }
    }

    class HordeGameRule : LobbyMPGameRule
    {
        private readonly DeathmatchMap Owner;
        private readonly HordeGameInfo GameInfo;

        public override string Name => GameInfo.Name;

        public HordeGameRule(DeathmatchMap Owner, HordeGameInfo GameInfo)
            : base(Owner)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
        }

        public override void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
            bool HordePlayerAILost = true;

            for (int i = 0; HordePlayerAILost && i < Owner.ListSubMap.Count; i++)
            {
                DeathmatchMap ActiveMap = (DeathmatchMap)Owner.ListSubMap[i];

                foreach (Squad ActiveSquad in ActiveMap.ListPlayer[10].ListSquad)
                {
                    if (!ActiveSquad.IsDead)
                    {
                        HordePlayerAILost = false;
                        break;
                    }
                }
            }

            if (HordePlayerAILost)
            {
                ActionPanelPlayerAIStep.FinishAIPlayerTurn(Owner);
            }
        }
    }
}
