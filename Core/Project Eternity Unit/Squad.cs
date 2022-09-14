using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Units
{
    public class Squad : UnitMapComponent
    {
        private Unit[] ArrayUnit;
        public Unit CurrentLeader { get { return CurrentLeaderIndex >= 0 ? ArrayUnit[CurrentLeaderIndex] : null; } }

        public Unit CurrentWingmanA { get { return CurrentWingmanAIndex >= 0 ? ArrayUnit[CurrentWingmanAIndex] : null; } }

        public Unit CurrentWingmanB { get { return CurrentWingmanBIndex >= 0 ? ArrayUnit[CurrentWingmanBIndex] : null; } }

        public int CurrentLeaderIndex;
        public int CurrentWingmanAIndex;
        public int CurrentWingmanBIndex;

        public string SquadName;
        public AIContainer SquadAI;
        public string SquadDefenseBattleBehavior;
        
        public bool IsEventSquad;
        public bool IsNameLocked;
        public bool IsLeaderLocked;
        public bool IsWingmanALocked;
        public bool IsWingmanBLocked;
        public bool IsDead;

        public override int Width { get { return CurrentLeader.SpriteMap.Width; } }
        public override int Height { get { return CurrentLeader.SpriteMap.Height; } }
        public override bool[,] ArrayMapSize { get { return CurrentLeader.UnitStat.ArrayMapSize; } }
        public override bool IsActive { get { return (CurrentLeader != null || IsEventSquad) && !IsDead; } }

        /// <summary>
        /// Returns CurrentLeader(0), CurrentWingmanA(1) or CurrentWingmanB(2) depending of the index.
        /// Since the SelectedUnit change which Unit is returned, ArrayUnit would not return the right unit if acceded directly.
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public Unit this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return CurrentLeader;

                    case 1:
                        return CurrentWingmanA;

                    case 2:
                        return CurrentWingmanB;

                    default:
                        return null;
                }
            }
        }

        public int UnitsInSquad
        {
            get
            {
                return ArrayUnit.Length;
            }
        }

        public int UnitsAliveInSquad
        {
            get
            {
                int ActiveUnit = 0;
                if (ArrayUnit[0].HP > 0)
                    ++ActiveUnit;
                if (ArrayUnit.Length >= 2 && ArrayUnit[1].HP > 0)
                    ++ActiveUnit;
                if (ArrayUnit.Length >= 3 && ArrayUnit[2].HP > 0)
                    ++ActiveUnit;

                return ActiveUnit;
            }
        }

        public Squad(string SquadName, Unit Leader = null, Unit WingmanA = null, Unit WingmanB = null)
        {
            IsDead = false;
            StartTurn();
            this.SquadName = SquadName;

            CurrentLeaderIndex = 0;
            CurrentWingmanAIndex = -1;
            CurrentWingmanBIndex = -1;

            SquadDefenseBattleBehavior = "";
            ListAttackedTeam = new List<int>();
            ListParthDrop = new List<string>();
            TeamTags = new TagSystem();

            if (WingmanB != null)
            {
                ArrayUnit = new Unit[3];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;
                ArrayUnit[2] = WingmanB;

                CurrentWingmanAIndex = 1;
                CurrentWingmanBIndex = 2;
            }
            else if (WingmanA != null)
            {
                ArrayUnit = new Unit[2];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;

                CurrentWingmanAIndex = 1;
            }
            else
            {
                ArrayUnit = new Unit[1];
                ArrayUnit[0] = Leader;
            }
        }

        public void Init(UnitEffectContext Context)
        {
            Context.SetContext(this, ArrayUnit[0], null, null, null, null, null);
            
            ArrayUnit[0].Init();
            CurrentLeaderIndex = 0;
            if (ArrayUnit.Length >= 2)
            {
                Context.SetContext(this, ArrayUnit[1], null, null, null, null, null);
                ArrayUnit[1].Init();
                CurrentWingmanAIndex = 1;
            }
            if (ArrayUnit.Length >= 3)
            {
                Context.SetContext(this, ArrayUnit[2], null, null, null, null, null);
                ArrayUnit[2].Init();
                CurrentWingmanBIndex = 2;
            }

            for (int U = 0; U < ArrayUnit.Length; U++)
            {
                for (int C = 0; C < this[U].ArrayCharacterActive.Length; C++)
                {
                    ArrayUnit[U].ArrayCharacterActive[C].SP = ArrayUnit[U].ArrayCharacterActive[C].MaxSP;
                }
                
                //Load the Battle Themes.
                for (int C = ArrayUnit[U].ArrayCharacterActive.Length - 1; C >= 0; --C)
                    if (!string.IsNullOrEmpty(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName))
                        if (!Character.DicBattleTheme.ContainsKey(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName))
                            Character.DicBattleTheme.Add(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName, new FMOD.FMODSound(GameScreens.GameScreen.FMODSystem, "Content/Maps/BGM/" + ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName + ".mp3"));
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(CurrentLeader.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            if (CurrentWingmanA != null)
                g.Draw(CurrentWingmanA.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, CurrentWingmanA.SpriteMap.Width / 2, CurrentWingmanA.SpriteMap.Height / 2), Color.White);
            if (CurrentWingmanB != null)
                g.Draw(CurrentWingmanB.SpriteMap, new Rectangle((int)Position.X + CurrentLeader.SpriteMap.Width / 2, (int)Position.Y, CurrentWingmanB.SpriteMap.Width / 2, CurrentWingmanB.SpriteMap.Height / 2), Color.White);
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
            if (Constants.ShowHealthBar)
            {
                DrawHealthBar(g, Position, CurrentLeader.HP, CurrentLeader.MaxHP, CurrentLeader.SpriteMap.Width);
            }
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
            if (TimeOfDay > 20)
            {
                g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)Position.X, (int)Position.Y, CurrentLeader.SpriteMap.Width, CurrentLeader.SpriteMap.Height), Color.White);
            }
        }

        public override void OnMenuSelect(ActionPanel PanelOwner, int ActivePlayerIndex, ActionPanelHolder ListActionMenuChoice)
        {
            List<ActionPanel> DicActionPanel = new List<ActionPanel>();

            for (int U = UnitsAliveInSquad - 1; U >= 0; --U)
            {
                foreach (ActionPanel OptionalPanel in ArrayUnit[U].OnMenuSelect(ActivePlayerIndex, this, ListActionMenuChoice))
                {
                    if (OptionalPanel != null && !DicActionPanel.Contains(OptionalPanel))
                    {
                        DicActionPanel.Add(OptionalPanel);
                        PanelOwner.AddChoiceToCurrentPanel(OptionalPanel);
                    }
                }
            }
        }

        public bool IsUnitAtPosition(Vector3 PositionToCheck)
        {
            return CurrentLeader.UnitStat.IsUnitAtPosition(Position, PositionToCheck);
        }

        public void UpdateSquad()
        {
            IsDead = false;

            CurrentLeaderIndex = -1;
            CurrentWingmanAIndex = -1;
            CurrentWingmanBIndex = -1;

            for (int U = 0; U < ArrayUnit.Length; U++)
            {
                if (ArrayUnit[U].HP > 0)
                {
                    if (CurrentLeader == null)
                    {
                        CurrentLeaderIndex = U;
                    }
                    else if (CurrentWingmanA == null && ArrayUnit[U] != CurrentLeader)
                    {
                        CurrentWingmanAIndex = U;
                    }
                    else if (CurrentWingmanB == null && ArrayUnit[U] != CurrentLeader && ArrayUnit[U] != CurrentWingmanA)
                    {
                        CurrentWingmanBIndex = U;
                    }
                }
            }

            if (CurrentLeader == null)
                IsDead = true;
        }

        public void ReloadSkills(Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
             Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            for (int U = 0; U < ArrayUnit.Length; ++U)
            {
                ArrayUnit[U].ReloadSkills(DicUnitType[ArrayUnit[U].UnitTypeName], DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            }
        }

        public Unit At(int Index)
        {
            if (Index < ArrayUnit.Length)
                return ArrayUnit[Index];
            else
                return null;
        }

        public int IndexOf(Unit ActiveUnit)
        {
            for (int U = ArrayUnit.Length - 1; U>=0; --U)
            {
                if (ArrayUnit[U] == ActiveUnit)
                    return U;
            }
            return -1;
        }

        public void SetLeader(int LeaderIndex)
        {
            CurrentLeaderIndex = LeaderIndex;

            if (LeaderIndex < 0 || LeaderIndex >= ArrayUnit.Length)
                CurrentLeaderIndex = -1;
        }

        public void SetWingmanA(int WingmanAIndex)
        {
            CurrentWingmanAIndex = WingmanAIndex;

            if (WingmanAIndex < 0 || WingmanAIndex >= ArrayUnit.Length)
                CurrentWingmanAIndex = -1;
        }

        public void SetWingmanB(int WingmanBIndex)
        {
            CurrentWingmanBIndex = WingmanBIndex;

            if (WingmanBIndex < 0 || WingmanBIndex >= ArrayUnit.Length)
                CurrentWingmanBIndex = -1;
        }

        public void SetNewLeader(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public void SetNewWingmanA(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public void SetNewWingmanB(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public static Squad LoadSquadWithProgression(BinaryReader BR, List<Character> ListTeamCharacter, ContentManager Content, Dictionary<string, Unit> DicUnitType,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string SquadName = BR.ReadString();
            bool IsNameLocked = BR.ReadBoolean();
            bool IsLeaderLocked = BR.ReadBoolean();
            bool IsWingmanALocked = BR.ReadBoolean();
            bool IsWingmanBLocked = BR.ReadBoolean();

            int UnitsInSquad = BR.ReadInt32();
            Unit NewLeader = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            Unit NewWingmanA = null;
            Unit NewWingmanB = null;

            if (UnitsInSquad >= 2)
                NewWingmanA = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            if (UnitsInSquad >= 3)
                NewWingmanB = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            Squad NewSquad = new Squad(SquadName, NewLeader, NewWingmanA, NewWingmanB);
            NewSquad.IsNameLocked = IsNameLocked;
            NewSquad.IsLeaderLocked = IsLeaderLocked;
            NewSquad.IsWingmanALocked = IsWingmanALocked;
            NewSquad.IsWingmanBLocked = IsWingmanBLocked;

            return NewSquad;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(SquadName);
            BW.Write(IsNameLocked);
            BW.Write(IsLeaderLocked);
            BW.Write(IsWingmanALocked);
            BW.Write(IsWingmanBLocked);

            BW.Write(UnitsInSquad);
            for (int U = 0; U < ArrayUnit.Length; U++)
                ArrayUnit[U].Save(BW);
        }

        public override string ToString()
        {
            return SquadName;
        }
    }
}
