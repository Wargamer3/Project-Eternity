using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum CameraStyle
    {
        Free,
        Eyes,
        Orbit,
        Life,
        TopDown,
    }

    public class PlaySession
    {
        public readonly List<PlayerCharacter> Assigned = new List<PlayerCharacter>();
        public bool AdminOpen;
        public bool BuildOpen;
        public CameraStyle View = CameraStyle.Free;
        public CameraStyle ViewBeforeFight = CameraStyle.Eyes;
        public bool Fighting;
        public PlayerCharacter Controlled;
        public PlayerCharacter LifeSelection;
        public PracticeEncounter Practice;
        public string PracticeNote = "";
        public int TakeCount;
        int purse = 2000;
        public HouseholdState Home;
        public string HouseholdId = HouseholdState.Id;
        public bool InDungeon;
        public int DungeonAimX;
        public int DungeonAimY;
        public int Seat;
        public string PartyId = "";
        public PlayerOverseer User;

        public int Funds
        {
            get
            {
                return Home == null ? purse : Home.Funds;
            }

            set
            {
                if (Home == null)
                {
                    purse = value;
                }
                else
                {
                    Home.Funds = value;
                }
            }
        }

        public void ApplyKey(Keys Key)
        {
            if (Key == Keys.F1)
            {
                AdminOpen = !AdminOpen;
            }
            else if (Key == Keys.Escape)
            {
                BuildOpen = !BuildOpen;
            }
            else if (Key == Keys.Q)
            {
                SwitchAssigned(-1);
            }
            else if (Key == Keys.E)
            {
                SwitchAssigned(1);
            }
            else if (Key >= Keys.D1 && Key <= Keys.D9)
            {
                SelectSlot(Key - Keys.D1 + 1);
            }
            else if (Key >= Keys.NumPad1 && Key <= Keys.NumPad9)
            {
                SelectSlot(Key - Keys.NumPad1 + 1);
            }
            else if (Key == Keys.Space)
            {
                Jump();
            }
        }

        public void CloseAdmin()
        {
            AdminOpen = false;
        }

        public void CloseBuild()
        {
            BuildOpen = false;
        }

        public void TakeCharacter(PlayerCharacter Who)
        {
            if (Who == null || Who == Controlled)
            {
                return;
            }

            if (Controlled != null)
            {
                Controlled.RemoveControl();
            }

            Who.TakeControl(User);
            Controlled = Who;
            TakeCount++;
            if (!Fighting)
            {
                View = CameraStyle.Eyes;
            }
        }

        public void ReleaseCharacter()
        {
            if (Controlled != null)
            {
                Controlled.RemoveControl();
            }

            Controlled = null;
            if (!Fighting)
            {
                View = CameraStyle.Life;
            }
        }

        public void ToggleView()
        {
            if (Fighting || View == CameraStyle.TopDown)
            {
                return;
            }

            if (View == CameraStyle.Eyes)
            {
                View = CameraStyle.Orbit;
                return;
            }

            if (View == CameraStyle.Orbit)
            {
                PlayerCharacter Who = Controlled;
                if (Who != null)
                {
                    Who.RemoveControl();
                    LifeSelection = Who;
                }

                Controlled = null;
                View = CameraStyle.Life;
                return;
            }

            PlayerCharacter Next = LifeSelection ?? Controlled;
            if (Next == null)
            {
                return;
            }

            View = CameraStyle.Eyes;
            TakeCharacter(Next);
        }

        public void SwitchAssigned(int Direction)
        {
            if (Assigned.Count == 0)
            {
                return;
            }

            int Index = Assigned.IndexOf(Controlled);
            if (Index < 0)
            {
                Index = 0;
            }
            else
            {
                Index += Direction;
                if (Index < 0)
                {
                    Index += Assigned.Count;
                }

                Index %= Assigned.Count;
            }

            TakeCharacter(Assigned[Index]);
        }

        public void SelectSlot(int OneBased)
        {
            if (OneBased < 1 || OneBased > Assigned.Count)
            {
                return;
            }

            TakeCharacter(Assigned[OneBased - 1]);
        }

        public bool ToggleAssigned(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return false;
            }

            if (Assigned.Contains(Who))
            {
                Assigned.Remove(Who);
                return false;
            }

            Assigned.Add(Who);
            return true;
        }

        public bool Hop;

        public bool Jump()
        {
            if (InDungeon)
            {
                Hop = true;
                return true;
            }

            if (Controlled == null)
            {
                return false;
            }

            return Controlled.TryJump();
        }

        public void BeginPractice(PlayerCharacter Who, FurnitureItem Dummy)
        {
            if (Who == null || Dummy == null)
            {
                return;
            }

            if (Practice != null && Practice.Fighter != null && Practice.Fighter != Who)
            {
                Practice.Fighter.LeavePractice();
            }

            Practice = new PracticeEncounter(Who, Dummy);
            PracticeNote = "";
            Who.EnterPractice();
            WatchFight(Who);
        }

        public void EndPractice()
        {
            PlayerCharacter Who = Practice == null ? null : Practice.Fighter;
            string Note = Practice == null ? "" : Practice.LastResult;
            if (Who != null)
            {
                Who.LeavePractice();
            }

            Practice = null;
            if (!string.IsNullOrEmpty(Note))
            {
                PracticeNote = Note;
            }

            WatchFight(Who);
            if (User != null)
            {
                User.ClosePracticeMenus();
            }
        }

        public void EndPracticeTurn()
        {
            if (Practice == null)
            {
                return;
            }

            if (PracticeMath.Busy(Practice.Fighter))
            {
                Practice.LastResult = "Still moving.";
                return;
            }

            Practice.EndTurn();
        }

        public void StrikePractice()
        {
            StrikePractice(null);
        }

        public void StrikePractice(string RequestedType)
        {
            if (Practice == null)
            {
                return;
            }

            int Natural = RandomHelper.Next(20) + 1;
            bool Down = Practice.Strike(Natural, RequestedType);
            if (Down)
            {
                EndPractice();
            }
        }

        public void WatchFight(PlayerCharacter Who)
        {
            if (Practice != null)
            {
                Who = Practice.Fighter;
            }

            bool Now = HasFight(Who);
            if (Now && !Fighting)
            {
                ViewBeforeFight = View == CameraStyle.Free ? CameraStyle.Eyes : View;
                View = CameraStyle.TopDown;
                if (Who != null)
                {
                    Who.TurnBased = true;
                }

                Fighting = true;
            }
            else if (!Now && Fighting)
            {
                View = ViewBeforeFight;
                if (Who != null)
                {
                    Who.TurnBased = false;
                }

                Fighting = false;
                ClearHidden(Who);
            }
        }

        public static bool HasFight(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return false;
            }

            for (int i = 0; i < Who.ListAIObjective.Count; i++)
            {
                if (Who.ListAIObjective[i].IsFight)
                {
                    return true;
                }
            }

            return Who.IsInCombat;
        }

        public static void ClearHidden(PlayerCharacter Who)
        {
            if (Who == null || Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo == null)
            {
                if (Who != null)
                {
                    Who.HiddenFromBattle = false;
                }

                return;
            }

            MapInfo Stood = Who.SharedMapContex.CurrentMapInfo;
            if (Stood.People == null)
            {
                Who.HiddenFromBattle = false;
                return;
            }

            for (int i = 0; i < Stood.People.Count; i++)
            {
                if (Stood.People[i] != null)
                {
                    Stood.People[i].HiddenFromBattle = false;
                }
            }
        }
    }

    public static class CharacterPage
    {
        public static List<string> Lines(PlayerCharacter Who)
        {
            List<string> Text = new List<string>();
            if (Who == null)
            {
                Text.Add("Nobody taken");
                return Text;
            }

            Text.Add("STR: " + Who.STR);
            Text.Add("Hunger: " + Who.Hunger);
            Text.Add("Sleep: " + Who.Sleep);
            if (Who.ListPersonality != null)
            {
                for (int i = 0; i < Who.ListPersonality.Count; i++)
                {
                    Text.Add(Who.ListPersonality[i].Name);
                }
            }

            if (Who.DicRelationshipByCharacter != null)
            {
                foreach (KeyValuePair<PlayerCharacter, CharacterRelationship> Pair in Who.DicRelationshipByCharacter)
                {
                    CharacterRelationship Bond = Pair.Value;
                    string Other = Bond.Other != null && !string.IsNullOrEmpty(Bond.Other.Name) ? Bond.Other.Name : "Someone";
                    Text.Add(Other + " " + Bond.Status + " " + Bond.Mood + " " + (int)Bond.Progress);
                }
            }

            return Text;
        }
    }

    public static class PlayInput
    {
        static KeyboardState Last;

        public static bool Pressed(Keys Key)
        {
            KeyboardState Now = Keyboard.GetState();
            return Now.IsKeyDown(Key) && Last.IsKeyUp(Key);
        }

        public static void EndFrame()
        {
            Last = Keyboard.GetState();
        }
    }
}
