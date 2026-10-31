using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum SocialKind
    {
        Talk,
        Gossip,
        Flirt,
        HoldHand,
        Kiss,
        Job,
        Cuddle,
        More,
    }

    public class SocialStep : LifeSimStep
    {
        readonly PlayerCharacter Other;
        readonly SocialKind Kind;

        public SocialStep(PlayerCharacter Owner, PlayerCharacter Other, SocialKind Kind)
            : base(LabelOf(Kind), Owner, 0.4)
        {
            this.Other = Other;
            this.Kind = Kind;
        }

        protected override void OnFinished()
        {
            Apply(Owner, Other, Kind);
        }

        protected override AutomatedAction Copy()
        {
            return new SocialStep(Owner, Other, Kind);
        }

        public static void Apply(PlayerCharacter Who, PlayerCharacter Other, SocialKind Kind)
        {
            if (Who == null || Other == null)
            {
                return;
            }

            CharacterRelationship Bond = Who.RelationshipWith(Other);
            CharacterRelationship Back = Other.RelationshipWith(Who);
            if (Kind == SocialKind.Talk)
            {
                Social.Talk(Bond, 8f);
                Social.Talk(Back, 8f);
            }
            else if (Kind == SocialKind.Gossip)
            {
                Social.Talk(Bond, 6f);
                Social.Talk(Back, 6f);
            }
            else if (Kind == SocialKind.Flirt)
            {
                Social.Talk(Bond, 12f);
                Social.Talk(Back, 12f);
                Warm(Bond);
                Warm(Back);
            }
            else if (Kind == SocialKind.HoldHand)
            {
                Social.Talk(Bond, 10f);
                Social.Talk(Back, 10f);
            }
            else if (Kind == SocialKind.Kiss)
            {
                Social.Talk(Bond, 18f);
                Social.Talk(Back, 18f);
                Warm(Bond);
                Warm(Back);
            }
            else if (Kind == SocialKind.Job)
            {
                Social.Talk(Bond, 4f);
                Social.Talk(Back, 4f);
                if (Other.Life != null)
                {
                    Work.StartAway(Other.Life, LifeSkill.Foraging, Other.SimHour);
                }
            }
            else if (Kind == SocialKind.Cuddle)
            {
                Social.Talk(Bond, 20f);
                Social.Talk(Back, 20f);
                Warm(Bond);
                Warm(Back);
            }
            else if (Kind == SocialKind.More)
            {
                Social.Talk(Bond, 30f);
                Social.Talk(Back, 30f);
                Warm(Bond);
                Warm(Back);
                if (Bond.Status == RelationshipStatus.BestFriend)
                {
                    Bond.Status = RelationshipStatus.Lover;
                }

                if (Back.Status == RelationshipStatus.BestFriend)
                {
                    Back.Status = RelationshipStatus.Lover;
                }
            }
        }

        static void Warm(CharacterRelationship Bond)
        {
            if (Bond != null && Bond.Status != RelationshipStatus.Hated)
            {
                Bond.Mood = RelationshipMood.LoveyDovey;
            }
        }

        static string LabelOf(SocialKind Kind)
        {
            if (Kind == SocialKind.HoldHand)
            {
                return "Hold hand";
            }

            if (Kind == SocialKind.Job)
            {
                return "Tell a job";
            }

            return Kind.ToString();
        }
    }

    public class TalkInteraction : LifeInteraction
    {
        public override string Name => "Talk";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            OfferSocial(Context, Choices, "Talk", 10, SocialKind.Talk);
        }

        public static void OfferSocial(InteractionContext Context, List<InteractionChoice> Choices, string Label, int Order, SocialKind Kind)
        {
            if (Context == null || Context.Actor == null || Context.Other == null || Context.Actor == Context.Other)
            {
                return;
            }

            if (Kind == SocialKind.Cuddle || Kind == SocialKind.More)
            {
                if (!LifeOrder.Intimate(Context.Actor.RelationshipWith(Context.Other)) || !LifeOrder.SharingBed(Context.Actor, Context.Other, Context.Room))
                {
                    return;
                }
            }

            PlayerCharacter Who = Context.Actor;
            PlayerCharacter Other = Context.Other;
            LifeSimMap Map = Context.Map;
            Choices.Add(new InteractionChoice { Label = Label, Order = Order, Run = () => LifeOrder.Meet(Who, Other, Map, Label, Kind) });
        }
    }

    public class GossipInteraction : LifeInteraction
    {
        public override string Name => "Gossip";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Gossip", 20, SocialKind.Gossip);
        }
    }

    public class FlirtInteraction : LifeInteraction
    {
        public override string Name => "Flirt";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Flirt", 30, SocialKind.Flirt);
        }
    }

    public class HoldHandInteraction : LifeInteraction
    {
        public override string Name => "HoldHand";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Hold hand", 40, SocialKind.HoldHand);
        }
    }

    public class KissInteraction : LifeInteraction
    {
        public override string Name => "Kiss";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Kiss", 50, SocialKind.Kiss);
        }
    }

    public class TellJobInteraction : LifeInteraction
    {
        public override string Name => "TellJob";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Tell a job", 60, SocialKind.Job);
        }
    }

    public class CuddleInteraction : LifeInteraction
    {
        public override string Name => "Cuddle";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "Cuddle", 70, SocialKind.Cuddle);
        }
    }

    public class MoreInteraction : LifeInteraction
    {
        public override string Name => "More";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            TalkInteraction.OfferSocial(Context, Choices, "More", 80, SocialKind.More);
        }
    }
}
