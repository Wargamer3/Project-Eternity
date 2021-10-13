using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen.Online
{
    public sealed class DeathmatchMapLobyScriptHolder : OnlineScriptHolder
    {
        public override KeyValuePair<string, List<OnlineScript>> GetNameAndContent(params object[] args)
        {
            List<OnlineScript> ListOnlineScript = ReflectionHelper.GetNestedTypes<OnlineScript>(typeof(DeathmatchMapLobyScriptHolder), args);
            return new KeyValuePair<string, List<OnlineScript>>("Deathmatch Loby", ListOnlineScript);
        }

        public abstract class BattleMapScript : OnlineScript
        {
            protected readonly DeathmatchMap Map;

            protected BattleMapScript(string Name, DeathmatchMap Map)
                : base(Name)
            {
                this.Map = Map;
            }
        }

        public class SetAttackingSquadScript : BattleMapScript
        {
            int ActiveSquadIndex;
            int ActivePlayerIndex;

            public SetAttackingSquadScript()
                : this(null)
            {
            }

            public SetAttackingSquadScript(DeathmatchMap Map)
                : this(Map, -1, -1)
            {
            }

            public SetAttackingSquadScript(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
                : base("Set Attacking Squad", Map)
            {
                this.ActivePlayerIndex = ActivePlayerIndex;
                this.ActiveSquadIndex = ActiveSquadIndex;
            }

            public override OnlineScript Copy()
            {
                return new SetAttackingSquadScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.ActiveSquadIndex = ActiveSquadIndex;
            }

            protected override void Read(OnlineReader Sender)
            {
                ActivePlayerIndex = Sender.ReadInt32();

                if (ActivePlayerIndex != Map.ActivePlayerIndex)
                    throw new Exception("Error on Attacking Squad selection.");

                ActiveSquadIndex = Sender.ReadInt32();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(ActivePlayerIndex);
                WriteBuffer.AppendInt32(ActiveSquadIndex);
            }
        }

        public class SetDefendingSquadScript : BattleMapScript
        {
            int ActivePlayerIndex;
            int TargetSquadIndex;

            public SetDefendingSquadScript()
                : this(null)
            {
            }

            public SetDefendingSquadScript(DeathmatchMap Map)
                : this(Map, -1, -1)
            {
            }

            public SetDefendingSquadScript(DeathmatchMap Map, int ActivePlayerIndex, int TargetSquadIndex)
                : base("Set Defending Squad", Map)
            {
                this.ActivePlayerIndex = ActivePlayerIndex;
                this.TargetSquadIndex = TargetSquadIndex;
            }

            public override OnlineScript Copy()
            {
                return new SetDefendingSquadScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.TargetSquadIndex = TargetSquadIndex;
            }

            protected override void Read(OnlineReader Sender)
            {
                ActivePlayerIndex = Sender.ReadInt32();

                if (ActivePlayerIndex == Map.ActivePlayerIndex)
                    throw new Exception("Error on Defending Squad selection.");

                TargetSquadIndex = Sender.ReadInt32();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(ActivePlayerIndex);
                WriteBuffer.AppendInt32(TargetSquadIndex);
            }
        }

        public class NewPhaseScript : BattleMapScript
        {
            public NewPhaseScript()
                : this(null)
            {
            }

            public NewPhaseScript(DeathmatchMap Map)
                : base("New Phase", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new NewPhaseScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                ActionPanelPhaseChange PhaseEnd = new ActionPanelPhaseChange(Map);
                Map.ListActionMenuChoice.Add(PhaseEnd);
                PhaseEnd.OnSelect();
            }

            protected override void Read(OnlineReader Sender)
            {
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
            }
        }

        public class MoveCursorScript : BattleMapScript
        {
            int CursorPositionX;
            int CursorPositionY;
            int CameraPositionX;
            int CameraPositionY;
            Squad ActiveSquad;

            public MoveCursorScript(Squad ActiveSquad)
                : this(null, ActiveSquad)
            {
            }

            public MoveCursorScript(DeathmatchMap Map, Squad ActiveSquad)
                : base("Move Cursor", Map)
            {
                this.ActiveSquad = ActiveSquad;
            }

            public override OnlineScript Copy()
            {
                return new MoveCursorScript(Map, ActiveSquad);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.CursorControl();
                Map.CursorPosition.X = CursorPositionX;
                Map.CursorPosition.Y = CursorPositionY;
                Map.CameraPosition.X = CameraPositionX;
                Map.CameraPosition.Y = CameraPositionY;
            }

            protected override void Read(OnlineReader Sender)
            {
                Map.CursorControl();
                CursorPositionX = Sender.ReadInt32();
                CursorPositionY = Sender.ReadInt32();
                CameraPositionX = Sender.ReadInt32();
                CameraPositionY = Sender.ReadInt32();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(CursorPositionX);
                WriteBuffer.AppendInt32(CursorPositionY);
                WriteBuffer.AppendInt32(CameraPositionX);
                WriteBuffer.AppendInt32(CameraPositionY);
            }
        }

        public class GetReadyToMoveScript : BattleMapScript
        {
            public GetReadyToMoveScript()
                : this(null)
            {
            }

            public GetReadyToMoveScript(DeathmatchMap Map)
                : base("Get Ready To Move", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new GetReadyToMoveScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.GetMVChoice(Map.ActiveSquad);
            }

            protected override void Read(OnlineReader Sender)
            {
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
            }
        }

        public class MoveUnitScript : BattleMapScript
        {
            int UpdatedPlayerIndex;
            int UpdatedSquadIndex;
            float CursorPositionX;
            float CursorPositionY;
            float CursorPositionZ;

            public MoveUnitScript()
                : this(null)
            {
            }

            public MoveUnitScript(DeathmatchMap Map)
                : base("Move Unit", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new MoveUnitScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Squad UpdatedSquad = Map.ListPlayer[UpdatedPlayerIndex].ListSquad[UpdatedSquadIndex];
                //Movement initialisation.
                Map.MovementAnimation.Add(UpdatedSquad.X, UpdatedSquad.Y, UpdatedSquad);
                //Move the Unit to the cursor position
                Map.CursorPosition.X = CursorPositionX;
                Map.CursorPosition.Y = CursorPositionY;
                Map.CursorPosition.Z = CursorPositionZ;
                UpdatedSquad.SetPosition(Map.CursorPosition);
            }

            protected override void Read(OnlineReader Sender)
            {
                UpdatedPlayerIndex = Sender.ReadInt32();
                UpdatedSquadIndex = Sender.ReadInt32();
                
                CursorPositionX = Sender.ReadFloat();
                CursorPositionY = Sender.ReadFloat();
                CursorPositionZ = Sender.ReadFloat();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(UpdatedPlayerIndex);
                WriteBuffer.AppendInt32(UpdatedSquadIndex);

                WriteBuffer.AppendFloat(CursorPositionX);
                WriteBuffer.AppendFloat(CursorPositionY);
                WriteBuffer.AppendFloat(CursorPositionZ);
            }
        }

        public class UpdateUnitStatusScript : BattleMapScript
        {
            int UpdatedPlayerIndex;
            int UpdatedSquadIndex;
            bool IsFlying;

            public UpdateUnitStatusScript()
                : this(null)
            {
            }

            public UpdateUnitStatusScript(DeathmatchMap Map)
                : this(null, -1, -1)
            {
            }

            public UpdateUnitStatusScript(DeathmatchMap Map, int UpdatedPlayerIndex, int UpdatedSquadIndex)
                : base("Update Unit status", Map)
            {
                IsFlying = Map.ListPlayer[UpdatedPlayerIndex].ListSquad[UpdatedSquadIndex].IsFlying;
            }

            public override OnlineScript Copy()
            {
                return new UpdateUnitStatusScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Squad UpdatedSquad = null;
                if (UpdatedSquadIndex == -1)
                {
                    if (UpdatedPlayerIndex == Map.ActivePlayerIndex)
                        UpdatedSquad = Map.ActiveSquad;
                    else if (UpdatedPlayerIndex == Map.TargetPlayerIndex)
                        UpdatedSquad = Map.TargetSquad;
                }
                else
                {
                    UpdatedSquad = Map.ListPlayer[UpdatedPlayerIndex].ListSquad[UpdatedSquadIndex];
                }

                UpdatedSquad.IsFlying = IsFlying;
            }

            protected override void Read(OnlineReader Sender)
            {
                UpdatedPlayerIndex = Sender.ReadInt32();
                UpdatedSquadIndex = Sender.ReadInt32();
                
                IsFlying = Sender.ReadBoolean();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(UpdatedPlayerIndex);
                WriteBuffer.AppendInt32(UpdatedSquadIndex);

                WriteBuffer.AppendBoolean(IsFlying);
            }
        }

        public class OpenWeaponMenuScript : BattleMapScript
        {
            int WeaponCount;
            bool[] ArrayCanAttack;

            public OpenWeaponMenuScript()
                : this(null)
            {
            }

            public OpenWeaponMenuScript(DeathmatchMap Map)
                : base("Open Weapon menu", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new OpenWeaponMenuScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.ActiveSquad.CurrentLeader.AttackIndex = 0;

                if (Map.ActiveSquad.CurrentLeader.ListAttack.Count != WeaponCount)
                    throw new Exception("Error while opening Weapon panel.");

            }

            protected override void Read(OnlineReader Sender)
            {
                WeaponCount = Sender.ReadByte();
                ArrayCanAttack = new bool[WeaponCount];

                for (int W = 0; W < WeaponCount; W++)
                {
                    ArrayCanAttack[W] = Sender.ReadBoolean();
                }
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(WeaponCount);

                for (int W = 0; W < WeaponCount; W++)
                {
                    WriteBuffer.AppendBoolean(ArrayCanAttack[W]);
                }
            }
        }

        public class UpdateWeaponMenuScript : BattleMapScript
        {
            int AttackIndex;

            public UpdateWeaponMenuScript()
                : this(null)
            {
            }

            public UpdateWeaponMenuScript(DeathmatchMap Map)
                : base("Update Weapon menu", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new UpdateWeaponMenuScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.ActiveSquad.CurrentLeader.AttackIndex = AttackIndex;
            }

            protected override void Read(OnlineReader Sender)
            {
                AttackIndex = Sender.ReadInt32();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(AttackIndex);
            }
        }

        public class PrepareToAttackScript : BattleMapScript
        {
            int MVChoiceCount;
            Point[] ArrayAttackChoice;

            public PrepareToAttackScript()
                : this(null)
            {
            }

            public PrepareToAttackScript(DeathmatchMap Map)
                : base("Prepare to Attack", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new PrepareToAttackScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
            }

            protected override void Read(OnlineReader Sender)
            {
                MVChoiceCount = Sender.ReadInt32();
                ArrayAttackChoice = new Point[MVChoiceCount];
                for (int M = 0; M < MVChoiceCount; M++)
                {
                    ArrayAttackChoice[M] = new Point(Sender.ReadInt32(), Sender.ReadInt32());
                }
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(MVChoiceCount);

                for (int M = 0; M < MVChoiceCount; M++)
                {
                    WriteBuffer.AppendInt32(ArrayAttackChoice[M].X);
                    WriteBuffer.AppendInt32(ArrayAttackChoice[M].Y);
                }
            }
        }

        public class ComputeDefensePatternScript : BattleMapScript
        {
            int IncomingDefendingPlayerIndex;

            public ComputeDefensePatternScript()
                : this(null)
            {
            }

            public ComputeDefensePatternScript(DeathmatchMap Map)
                : base("Compute Defence Pattern", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new ComputeDefensePatternScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                if (Map.TargetPlayerIndex != IncomingDefendingPlayerIndex)
                    throw new Exception("Computing online defence pattern has failed");

                DeathmatchMap.PrepareDefenseSquadForBattle(Map, Map.ActiveSquad, Map.TargetSquad);
                DeathmatchMap.PrepareAttackSquadForBattle(Map, Map.ActiveSquad, Map.TargetSquad);
            }

            protected override void Read(OnlineReader Sender)
            {
                IncomingDefendingPlayerIndex = Sender.ReadInt32();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(IncomingDefendingPlayerIndex);
            }
        }

        public class UpdateBattleMenuScript : BattleMapScript
        {
            DeathmatchMap.BattleMenuChoices BattleMenuCursorIndex;
            int BattleMenuCursorIndexSecond;
            int ActiveSquadAttackIndex;
            int TargetSquadAttackIndex;
            DeathmatchMap.BattleMenuStages BattleMenuStage;
            bool ShowAnimation;

            public UpdateBattleMenuScript()
                : this(null)
            {
            }

            public UpdateBattleMenuScript(DeathmatchMap Map)
                : base("Update Battle menu", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new UpdateBattleMenuScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.BattleMenuCursorIndex = BattleMenuCursorIndex;
                Map.BattleMenuCursorIndexSecond = BattleMenuCursorIndexSecond;
                Map.ActiveSquad.CurrentLeader.AttackIndex = ActiveSquadAttackIndex;
                Map.TargetSquad.CurrentLeader.AttackIndex = TargetSquadAttackIndex;
                Map.BattleMenuStage = BattleMenuStage;
                Constants.ShowAnimation = ShowAnimation;
            }

            protected override void Read(OnlineReader Sender)
            {
                BattleMenuCursorIndex = (DeathmatchMap.BattleMenuChoices)Sender.ReadInt32();
                BattleMenuCursorIndexSecond = Sender.ReadInt32();
                ActiveSquadAttackIndex = Sender.ReadInt32();
                TargetSquadAttackIndex = Sender.ReadInt32();
                BattleMenuStage = (DeathmatchMap.BattleMenuStages)Sender.ReadInt32();
                ShowAnimation = Sender.ReadBoolean();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32((int)BattleMenuCursorIndex);
                WriteBuffer.AppendInt32(BattleMenuCursorIndexSecond);
                WriteBuffer.AppendInt32(ActiveSquadAttackIndex);
                WriteBuffer.AppendInt32(TargetSquadAttackIndex);
                WriteBuffer.AppendInt32((int)BattleMenuStage);
                WriteBuffer.AppendBoolean(ShowAnimation);
            }
        }

        public class StartBattleScript : BattleMapScript
        {
            bool Defender;

            public StartBattleScript()
                : this(null)
            {
            }

            public StartBattleScript(DeathmatchMap Map)
                : base("Start Battle", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new StartBattleScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
            }

            protected override void Read(OnlineReader Sender)
            {
                Defender = Sender.ReadBoolean();
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendBoolean(Defender);
            }
        }

        public class OpenStatusMenuScript : BattleMapScript
        {
            int ActiveStatusScreenPlayerIndex;
            int ActiveStatusScreenSquadIndex;

            public OpenStatusMenuScript()
                : this(null)
            {
            }

            public OpenStatusMenuScript(DeathmatchMap Map)
                : base("Open Status Menu", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new OpenStatusMenuScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                Map.StatusMenu.OpenStatusMenuScreen(Map.ListPlayer[ActiveStatusScreenPlayerIndex].ListSquad[ActiveStatusScreenSquadIndex]);
            }

            protected override void Read(OnlineReader Sender)
            {
                ActiveStatusScreenPlayerIndex = Sender.ReadInt32();
                ActiveStatusScreenSquadIndex = Sender.ReadInt32();

            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
                WriteBuffer.AppendInt32(ActiveStatusScreenPlayerIndex);
                WriteBuffer.AppendInt32(ActiveStatusScreenSquadIndex);
            }
        }

        public class UpdateStatusMenuScript : BattleMapScript
        {
            public UpdateStatusMenuScript()
                : this(null)
            {
            }

            public UpdateStatusMenuScript(DeathmatchMap Map)
                : base("Update Status Menu", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new UpdateStatusMenuScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
            }

            protected override void Read(OnlineReader Sender)
            {

            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
            }
        }

        public class SpawnSquadScript : BattleMapScript
        {
            public SpawnSquadScript()
                : this(null)
            {
            }

            public SpawnSquadScript(DeathmatchMap Map)
                : base("Spawn Squad", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new StartBattleScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
            }

            protected override void Read(OnlineReader Sender)
            {
                int NewSquadPlayerIndex;
                Squad NewSquad = GetOnlineSquad(Sender, out NewSquadPlayerIndex);
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
            }

            public Squad GetOnlineSquad(OnlineReader ActivePlayer, out int SquadPlayerIndex)
            {
                SquadPlayerIndex = ActivePlayer.ReadInt32();
                float SquadPositionX = ActivePlayer.ReadFloat();
                float SquadPositionY = ActivePlayer.ReadFloat();
                float SquadPositionZ = ActivePlayer.ReadFloat();
                string ActiveSquadSquadName = ActivePlayer.ReadString();

                byte UnitsInSquad = ActivePlayer.ReadByte();

                Unit NewLeader = null, NewWingmanA = null, NewWingmanB = null;

                if (UnitsInSquad >= 1)
                {
                    string UnitTypeName = ActivePlayer.ReadString();
                    string UnitName = ActivePlayer.ReadString();
                    int CharacterCount = ActivePlayer.ReadInt32();

                    NewLeader = Unit.FromType(UnitTypeName, UnitName, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                    NewLeader.ArrayCharacterActive = new Character[CharacterCount];

                    for (int C = 0; C < CharacterCount; C++)
                        NewLeader.ArrayCharacterActive[C] = new Character(ActivePlayer.ReadString(), Map.Content, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget, Map.DicManualSkillTarget);

                    //Initialise the Unit stats.
                    NewLeader.Init();
                }
                if (UnitsInSquad >= 2)
                {
                    string UnitTypeName = ActivePlayer.ReadString();
                    string UnitName = ActivePlayer.ReadString();
                    int CharacterCount = ActivePlayer.ReadInt32();

                    NewWingmanA = Unit.FromType(UnitTypeName, UnitName, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                    NewWingmanA.ArrayCharacterActive = new Character[CharacterCount];

                    for (int C = 0; C < CharacterCount; C++)
                        NewWingmanA.ArrayCharacterActive[C] = new Character(ActivePlayer.ReadString(), Map.Content, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget, Map.DicManualSkillTarget);

                    //Initialise the Unit stats.
                    NewWingmanA.Init();
                }
                if (UnitsInSquad >= 3)
                {
                    string UnitTypeName = ActivePlayer.ReadString();
                    string UnitName = ActivePlayer.ReadString();
                    int CharacterCount = ActivePlayer.ReadInt32();

                    NewWingmanB = Unit.FromType(UnitTypeName, UnitName, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget);
                    NewWingmanB.ArrayCharacterActive = new Character[CharacterCount];

                    for (int C = 0; C < CharacterCount; C++)
                        NewWingmanB.ArrayCharacterActive[C] = new Character(ActivePlayer.ReadString(), Map.Content, Map.DicRequirement, Map.DicEffect, Map.DicAutomaticSkillTarget, Map.DicManualSkillTarget);

                    //Initialise the Unit stats.
                    NewWingmanB.Init();
                }

                Squad NewSquad = new Squad(ActiveSquadSquadName, NewLeader, NewWingmanA, NewWingmanB);
                Map.SpawnSquad(SquadPlayerIndex, NewSquad, 0, new Vector3(SquadPositionX, SquadPositionY, SquadPositionZ));

                return NewSquad;
            }
        }
    }
}
