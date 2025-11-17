using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetPlayerDirectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Player Direction";

        public enum PlayerDirections { None, Left, Right, Up, Down}

        private PlayerDirections _PlayerDirection;

        public SetPlayerDirectionEffect()
            : base(Name, false)
        {
        }

        public SetPlayerDirectionEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetPlayerDirectionEffect(SorcererStreetBattleParams Params, PlayerDirections PlayerDirection)
            : base(Name, false, Params)
        {
            _PlayerDirection = PlayerDirection;
        }

        protected override void Load(BinaryReader BR)
        {
            _PlayerDirection = (PlayerDirections)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_PlayerDirection);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            switch (_PlayerDirection)
            {
                case PlayerDirections.None:
                    Params.GlobalPlayerContext.ActivePlayer.GamePiece.Direction = UnitMapComponent.DirectionNone;
                    break;
            }

            return "Set Player Direction";
        }

        protected override BaseEffect DoCopy()
        {
            SetPlayerDirectionEffect NewEffect = new SetPlayerDirectionEffect(Params, _PlayerDirection);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties


        #endregion
    }
}
