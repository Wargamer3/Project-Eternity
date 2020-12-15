using ProjectEternity.Core.Item;
using System.Collections.Generic;

namespace ProjectEternity.Core.Magic
{
    public interface IMagicUser
    {
        float CurrentMana { get; set; }
        float ChanneledMana { get; set; }
        float ManaReserves { get; set; }//Extra mana on top of current Mana that will empty over time.
        EffectHolder Effects { get; }
        List<IMagicUser> ListLinkedUser { get; }//List of linked magic user to share mana with
    }

    public class MagicUser : IMagicUser
    {
        public float MaxMana;
        public float CurrentMana { get; set; }
        public float ChanneledMana { get; set; }
        public float ManaReserves { get; set; }
        public EffectHolder Effects { get; }
        public List<IMagicUser> ListLinkedUser { get; }

        public MagicUser()
        {
            MaxMana = 100;
            CurrentMana = MaxMana;
            ChanneledMana = 0;
            ManaReserves = 0;

            Effects = new EffectHolder();
            ListLinkedUser = new List<IMagicUser>();
        }
    }

    public class MagicUserParams
    {
        // This class is shared through every MagicUser used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly MagicUserContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly MagicUserContext LocalContext;

        public MagicUserParams(MagicUserContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;

            LocalContext = new MagicUserContext();
        }

        public MagicUserParams(MagicUserParams Clone)
        {
            GlobalContext = Clone.GlobalContext;
            LocalContext = new MagicUserContext();
            
            LocalContext.ActiveUser = Clone.LocalContext.ActiveUser;
            LocalContext.ActiveTarget = Clone.LocalContext.ActiveTarget;

            CopyGlobalIntoLocal();
        }

        private void CopyGlobalIntoLocal()
        {
            LocalContext.ActiveUser = GlobalContext.ActiveUser;
            LocalContext.ActiveTarget = GlobalContext.ActiveTarget;
        }

        public void SetMagicUser(IMagicUser ActiveUser)
        {
            GlobalContext.ActiveUser = ActiveUser;
        }

        public void SetMagicTarget(IMagicUser ActiveUser)
        {
            GlobalContext.ActiveTarget = ActiveUser;
        }

        public IMagicUser GetMagicTarget()
        {
            return GlobalContext.ActiveTarget;
        }
    }

    public class MagicUserContext
    {
        public IMagicUser ActiveUser;
        public IMagicUser ActiveTarget;
    }
}
