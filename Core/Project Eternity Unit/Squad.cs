using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Units
{
    public class Squad : UnitMapComponent
    {
        private Unit[] ArrayUnit;
        public Unit CurrentLeader { get { return ListUnitIndex.Count > 0 && ListUnitIndex[0] >= 0 ? ArrayUnit[ListUnitIndex[0]] : null; } }

        private Unit CurrentWingmanA { get { return ListUnitIndex.Count > 1 && ListUnitIndex[1] >= 0 ? ArrayUnit[ListUnitIndex[1]] : null; } }

        private Unit CurrentWingmanB { get { return ListUnitIndex.Count > 2 && ListUnitIndex[2] >= 0 ? ArrayUnit[ListUnitIndex[2]] : null; } }

        private Unit CurrentWingmanC { get { return ListUnitIndex.Count > 3 && ListUnitIndex[3] >= 0 ? ArrayUnit[ListUnitIndex[3]] : null; } }

        private List<int> ListUnitIndex;

        public string SquadName;
        public AIContainer SquadAI;
        public string SquadDefenseBattleBehavior;

        public bool IsEventSquad;
        public bool IsNameLocked;
        public bool IsLeaderLocked;
        public bool IsWingmanALocked;
        public bool IsWingmanBLocked;
        public bool IsDead;

        private Matrix WorldPosition;

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

                    case 3:
                        return CurrentWingmanC;

                    default:
                        if (i < 0 || i >= ListUnitIndex.Count)
                            return null;
                        return ArrayUnit[ListUnitIndex[i]];
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

        public Squad(string SquadName, Unit Leader = null, Unit WingmanA = null, Unit WingmanB = null, Unit WingmanC = null)
        {
            IsDead = false;
            IsOnGround = true;
            ListUnitIndex = new List<int>();
            StartTurn();
            this.SquadName = SquadName;

            ListUnitIndex.Add(0);

            SquadDefenseBattleBehavior = "";
            ListAttackedTeam = new List<int>();
            ListParthDrop = new List<string>();
            TeamTags = new TagSystem();

            if (WingmanC != null)
            {
                ArrayUnit = new Unit[4];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;
                ArrayUnit[2] = WingmanB;
                ArrayUnit[3] = WingmanC;

                ListUnitIndex.Add(1);
                ListUnitIndex.Add(2);
                ListUnitIndex.Add(3);
            }
            else if (WingmanB != null)
            {
                ArrayUnit = new Unit[3];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;
                ArrayUnit[2] = WingmanB;

                ListUnitIndex.Add(1);
                ListUnitIndex.Add(2);
            }
            else if (WingmanA != null)
            {
                ArrayUnit = new Unit[2];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;

                ListUnitIndex.Add(1);
            }
            else
            {
                ArrayUnit = new Unit[1];
                ArrayUnit[0] = Leader;
            }
        }

        public void Init(UnitEffectContext Context)
        {
            ListUnitIndex.Clear();

            for (int U = 0; U < ArrayUnit.Length; ++U)
            {
                Context.SetContext(this, ArrayUnit[U], null, null, null, null, null);
                ArrayUnit[U].Init();
                ListUnitIndex.Add(U);

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

        public override void SetPosition(Vector3 Position)
        {
            base.SetPosition(Position);

            if (ItemHeld != null)
            {
                ItemHeld.Item3D.SetPosition(
                    Position.X,
                    (Position.Z + 1f),
                    (Position.Y - 0.5f));
            }

            if (CurrentLeader.Unit3DModel == null)
            {
                CurrentLeader.Unit3DSprite.SetPosition(Position.X, Position.Y, Position.Z);
            }
            else
            {
                Matrix RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(Direction));

                Vector3 FinalPosition = new Vector3(Position.X, Position.Y, Position.Z);

                if (CurrentLeader.UnitStat.ArrayMapSize.GetLength(0) > 1)
                {
                    FinalPosition.X += 0.5f * CurrentLeader.UnitStat.ArrayMapSize.GetLength(0) - 0.5f;
                    FinalPosition.Y += 0.5f * CurrentLeader.UnitStat.ArrayMapSize.GetLength(1) - 0.5f;
                }

                WorldPosition = RotationMatrix * Matrix.CreateTranslation(FinalPosition.X, FinalPosition.Z, FinalPosition.Y);
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(CurrentLeader.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), null, UnitColor, 0f, new Vector2(CurrentLeader.SpriteMap.Width / 2, CurrentLeader.SpriteMap.Height / 2), SpriteEffects.None, 0.2f);
        }

        public override void Draw3DOnMap(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection)
        {
            if (ItemHeld != null)
            {
                ItemHeld.Item3D.SetViewMatrix(View);

                ItemHeld.Item3D.Draw(GraphicsDevice);
            }

            if (CurrentLeader.Unit3DModel == null)
            {
                CurrentLeader.Unit3DSprite.SetViewMatrix(View);

                CurrentLeader.Unit3DSprite.Draw(GraphicsDevice);
            }
            else
            {
                CurrentLeader.Unit3DModel.Draw(View, Projection, WorldPosition);
            }
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            if (CurrentWingmanA != null)
                g.Draw(CurrentWingmanA.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, CurrentWingmanA.SpriteMap.Width / 2, CurrentWingmanA.SpriteMap.Height / 2), Color.White);
            if (CurrentWingmanB != null)
                g.Draw(CurrentWingmanB.SpriteMap, new Rectangle((int)Position.X + CurrentLeader.SpriteMap.Width / 2, (int)Position.Y, CurrentWingmanB.SpriteMap.Width / 2, CurrentWingmanB.SpriteMap.Height / 2), Color.White);
            if (CurrentWingmanC != null)
                g.Draw(CurrentWingmanC.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y + CurrentLeader.SpriteMap.Width / 2, CurrentWingmanB.SpriteMap.Height / 2, CurrentWingmanB.SpriteMap.Height / 2), Color.White);
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

        public bool IsUnitAtPosition(Vector3 PositionToCheck, Point TerrainSize)
        {
            return CurrentLeader.UnitStat.IsUnitAtPosition(Position, PositionToCheck, TerrainSize);
        }

        public void UpdateSquad()
        {
            IsDead = false;

            ListUnitIndex.Clear();

            for (int U = 0; U < ArrayUnit.Length; U++)
            {
                if (ArrayUnit[U].HP > 0)
                {
                    ListUnitIndex.Add(U);
                }
            }

            if (ListUnitIndex.Count == 0)
            {
                IsDead = true;
                Speed = Vector3.Zero;
            }
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
            for (int U = ArrayUnit.Length - 1; U >= 0; --U)
            {
                if (ArrayUnit[U] == ActiveUnit)
                    return U;
            }
            return -1;
        }

        public void SetLeader(int LeaderIndex)
        {
        }

        public void SetWingmanA(int WingmanAIndex)
        {
            if (WingmanAIndex >= 0 || WingmanAIndex < ArrayUnit.Length)
                ListUnitIndex[1] = WingmanAIndex;
        }

        public void SetWingmanB(int WingmanBIndex)
        {
            if (WingmanBIndex >= 0 || WingmanBIndex < ArrayUnit.Length)
                ListUnitIndex[2] = WingmanBIndex;
        }

        public void SetNewLeader(Unit NewLeader)
        {
            ArrayUnit[0] = NewLeader;
        }

        public void SetNewWingmanA(Unit NewLeader)
        {
            ArrayUnit[1] = NewLeader;
        }

        public void SetNewWingmanB(Unit NewLeader)
        {
            ArrayUnit[2] = NewLeader;
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

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(ID);
            BW.Write(CanMove);
            BW.Write(ActionsRemaining);
            BW.Write(X);
            BW.Write(Y);
            BW.Write(Z);
            BW.Write(SquadName);
            BW.Write(CurrentTerrainIndex);
            BW.Write(IsUnderTerrain);
            BW.Write(IsPlayerControlled);
            if (SquadAI == null || SquadAI.Path == null)
            {
                BW.Write(string.Empty);
            }
            else
            {
                BW.Write(SquadAI.Path);
            }

            BW.Write(ArrayUnit.Length);
            for (int U = 0; U < ArrayUnit.Length; ++U)
            {
                BW.Write(ListUnitIndex[U]);
                ArrayUnit[U].QuickSave(BW);
            }

            //List of Attacked Teams.
            BW.Write(ListAttackedTeam.Count);
            for (int U = 0; U < ListAttackedTeam.Count; ++U)
                BW.Write(ListAttackedTeam[U]);
        }

        public override string ToString()
        {
            return SquadName;
        }
    }
}
