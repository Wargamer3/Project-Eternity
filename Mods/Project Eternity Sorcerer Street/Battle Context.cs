
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetBattleContext
    {
        public SorcererStreetMap Map;

        public CreatureCard Invader;
        public CreatureCard Defender;

        public SimpleAnimation InvaderCard;
        public SimpleAnimation DefenderCard;

        public Player InvaderPlayer;
        public Player DefenderPlayer;

        public int InvaderFinalHP;
        public int DefenderFinalHP;

        public int InvaderFinalST;
        public int DefenderFinalST;

        public Card InvaderItem;
        public Card DefenderItem;

        public CreatureCard UserCreature;
        public CreatureCard OpponentCreature;
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class SorcererStreetBattleParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        private readonly SorcererStreetBattleContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly SorcererStreetBattleContext LocalContext;

        public SorcererStreetBattleParams(SorcererStreetBattleContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
            LocalContext = new SorcererStreetBattleContext();
        }

        public SorcererStreetBattleParams(SorcererStreetBattleParams Clone)
            : this(Clone.GlobalContext)
        {
            CopyGlobalIntoLocal();
        }

        internal void CopyGlobalIntoLocal()
        {
            LocalContext.Map = GlobalContext.Map;
        }

        public void IncreaseSelfHP(int Value)
        {
            if (GlobalContext.UserCreature == GlobalContext.Invader)
            {
                GlobalContext.InvaderFinalHP += Value;
            }
            else if (GlobalContext.UserCreature == GlobalContext.Defender)
            {
                GlobalContext.DefenderFinalHP += Value;
            }
        }

        public void IncreaseOtherHP(int Value)
        {
            if (GlobalContext.UserCreature == GlobalContext.Invader)
            {
                GlobalContext.DefenderFinalHP += Value;
            }
            else if (GlobalContext.UserCreature == GlobalContext.Defender)
            {
                GlobalContext.InvaderFinalHP += Value;
            }
        }

        public void IncreaseSelfST(int Value)
        {
            if (GlobalContext.OpponentCreature == GlobalContext.Invader)
            {
                GlobalContext.InvaderFinalST += Value;
            }
            else if (GlobalContext.OpponentCreature == GlobalContext.Defender)
            {
                GlobalContext.DefenderFinalST += Value;
            }
        }

        public void IncreaseOtherST(int Value)
        {
            if (GlobalContext.OpponentCreature == GlobalContext.Invader)
            {
                GlobalContext.DefenderFinalST += Value;
            }
            else if (GlobalContext.OpponentCreature == GlobalContext.Defender)
            {
                GlobalContext.InvaderFinalST += Value;
            }
        }
    }
}
