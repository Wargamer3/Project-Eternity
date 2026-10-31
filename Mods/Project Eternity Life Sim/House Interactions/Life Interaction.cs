using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class InteractionChoice
    {
        public string Label;
        public int Order;
        public Action Run;
    }

    public class InteractionContext
    {
        public PlayerCharacter Actor;
        public PlayerCharacter Other;
        public FurnitureItem Box;
        public LifeSimMap Map;
        public HouseholdState Home;
        public PlaySession Session;
        public IList<FurnitureItem> Room;
        public IList<PlayerCharacter> People;
        public Action EnterDungeon;

        public InteractionContext Copy()
        {
            InteractionContext Next = new InteractionContext();
            Next.Actor = Actor;
            Next.Other = Other;
            Next.Box = Box;
            Next.Map = Map;
            Next.Home = Home;
            Next.Session = Session;
            Next.Room = Room;
            Next.People = People;
            Next.EnterDungeon = EnterDungeon;
            return Next;
        }
    }

    public class InteractTarget
    {
        public string Label;
        public FurnitureItem Box;
        public PlayerCharacter Person;
        public List<InteractionChoice> Choices = new List<InteractionChoice>();
    }

    /// <summary>
    /// One menu interaction. Reflection builds LifeOrder's dictionary from every concrete subclass,
    /// including ones loaded later from a DLL.
    /// </summary>
    public abstract class LifeInteraction
    {
        public abstract string Name { get; }

        public abstract void Offer(InteractionContext Context, List<InteractionChoice> Choices);
    }
}
