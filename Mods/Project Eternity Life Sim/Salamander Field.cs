using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class SalamanderField
    {
        public const int Count = 3;
        public const int Level = 7;
        public const int HitPoints = 125;
        public const int ArmorClass = 26;
        public const int FireImmunity = 1000;

        // The glb stands about 0.68 units tall. Neighbours on this map are about 35 units tall.
        public const float BodyScale = 60f;
        public const string ModelFile = "Salamander.glb";

        static readonly float[] OffsetX = { 48f, 78f, 36f };
        static readonly float[] OffsetY = { 20f, 58f, 72f };

        public static void Place(NavMapGameManager Manager)
        {
            if (Manager == null || Manager.RootMapContainer == null || Already(Manager))
            {
                return;
            }

            float OriginX;
            float OriginY;
            MapInfo Home;
            Start(Manager, out OriginX, out OriginY, out Home);
            if (Home == null)
            {
                return;
            }

            for (int i = 0; i < Count; i++)
            {
                Drop(Manager, Home, i + 1, OriginX + OffsetX[i], OriginY + OffsetY[i], OriginX, OriginY);
            }
        }

        public static PlayerCharacter Make(int Index)
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Name = Index <= 1 ? "Salamander" : "Salamander " + Index;
            Who.Level = Level;
            Who.Ancestry = Ancestry();
            Who.Class = Class();
            Weapon Ranseur = Strike("+1 Ranseur", "2d10+3", "Piercing", "Ranseur", Weapon.HandTypes.TwoHands);
            Weapon Tail = Strike("Tail", "1d8+3", "Bludgeoning", "Tail", Weapon.HandTypes.OneHand);
            Who.Equip(Ranseur);
            Who.Hold(Tail);
            Finish(Who);
            return Who;
        }

        public static void Finish(PlayerCharacter Who)
        {
            if (Who == null)
            {
                return;
            }

            Who.Level = Level;
            Who.STR = 4;
            Who.DEX = 3;
            Who.CON = 4;
            Who.INT = 2;
            Who.WIS = 2;
            Who.CHA = 1;
            Who.PrepareForFight();
            Who.PrintedArmor = true;
            Who.AC = ArmorClass;
            Who.ResistanceFire = FireImmunity;
            Who.Side = SkirmishSide.Monster;
            Who.CreatureTrait = "Fire";
            Who.AIControlled = false;
            if (Who.ListAIObjective != null)
            {
                Who.ListAIObjective.Clear();
            }

            if (Who.ListCurrentAIObjectiveStep != null)
            {
                Who.ListCurrentAIObjectiveStep.Clear();
            }
        }

        static void Drop(NavMapGameManager Manager, MapInfo Home, int Index, float X, float Y, float FaceX, float FaceY)
        {
            PlayerCharacter Who = Make(Index);
            if (Who.SharedMapContex != null)
            {
                Who.SharedMapContex.RootMapContainer = Manager;
                Who.SharedMapContex.CurrentMapInfo = Home;
            }

            float Feet = Home.Heights != null ? HeightField.Stand(Home, X, Y) : 8f;
            Vector3 At = new Vector3(X, Y, Feet);
            LifeSimMap Map = Home.MapMapContainer == null ? null : Home.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null)
            {
                Map.SpawnCharacter(Who, 0, At);
            }
            else
            {
                Who.SetPosition(At);
                Who.Body.CanJump = true;
                Who.SeedBelief();
                Who.Init();
                Home.Adopt(Who);
            }

            // Init rebuilds ability scores from empty boosts, so the printed block is applied after that.
            Finish(Who);
            Wear(Who);
            Who.RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.UnitY, (float)Math.Atan2(FaceX - X, FaceY - Y));
        }

        static void Start(NavMapGameManager Manager, out float X, out float Y, out MapInfo Home)
        {
            X = 0f;
            Y = 0f;
            Home = Manager.RootMapContainer;
            if (Manager.ListLocalPlayer == null)
            {
                return;
            }

            for (int i = 0; i < Manager.ListLocalPlayer.Count; i++)
            {
                PlayerOverseer Player = Manager.ListLocalPlayer[i];
                PlayerCharacter Cursor = Player == null ? null : Player.InvisibleCharacterAsCursor;
                if (Cursor == null)
                {
                    continue;
                }

                X = Cursor.Position.X;
                Y = Cursor.Position.Z;
                if (Cursor.SharedMapContex != null && Cursor.SharedMapContex.CurrentMapInfo != null)
                {
                    Home = Cursor.SharedMapContex.CurrentMapInfo;
                }

                return;
            }
        }

        static bool Already(NavMapGameManager Manager)
        {
            if (Manager.RootMapContainer.People == null)
            {
                return false;
            }

            for (int i = 0; i < Manager.RootMapContainer.People.Count; i++)
            {
                PlayerCharacter Who = Manager.RootMapContainer.People[i];
                if (Who != null && Who.Name != null && Who.Name.StartsWith("Salamander", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        static CharacterAncestry Ancestry()
        {
            CharacterAncestry Body = new CharacterAncestry();
            Body.Name = "Salamander";
            Body.BaseHP = 41;
            Body.Speed = 20;
            Body.Size = CharacterAncestry.CharacterSizes.Medium;
            Trait Fire = new Trait();
            Fire.Name = "Fire";
            Body.ListTraits.Add(Fire);
            return Body;
        }

        static CharacterClass Class()
        {
            CharacterClass Body = new CharacterClass();
            Body.Name = "Salamander";
            Body.BaseHP = 8;
            Body.DicProficiencyLevelByName.Add("Ranseur", Rank("Ranseur", Proficiency.ProficiencyRanks.Master));
            Body.DicProficiencyLevelByName.Add("Tail", Rank("Tail", Proficiency.ProficiencyRanks.Master));
            Body.DicProficiencyLevelByName.Add("Perception", Rank("Perception", Proficiency.ProficiencyRanks.Master));
            Body.DicProficiencyLevelByName.Add("Fortitude", Rank("Fortitude", Proficiency.ProficiencyRanks.Expert));
            Body.DicProficiencyLevelByName.Add("Reflex", Rank("Reflex", Proficiency.ProficiencyRanks.Master));
            Body.DicProficiencyLevelByName.Add("Will", Rank("Will", Proficiency.ProficiencyRanks.Expert));
            Body.DicProficiencyLevelByName.Add("Athletics", Rank("Athletics", Proficiency.ProficiencyRanks.Master));
            return Body;
        }

        static ProficiencyLink Rank(string Name, Proficiency.ProficiencyRanks Level)
        {
            Proficiency Body = new Proficiency();
            Body.Name = Name;
            ProficiencyLink Link = new ProficiencyLink(Name);
            Link.LinkedProficiency = Body;
            Link.ProficiencyRank = Level;
            return Link;
        }

        static Weapon Strike(string Name, string Damage, string Type, string Group, Weapon.HandTypes Hands)
        {
            Weapon Body = new Weapon();
            Body.Name = Name;
            Body.Damage = Damage;
            Body.DamageType = Type;
            Body.WeaponType = Weapon.WeaponTypes.Melee;
            Body.HandType = Hands;
            Body.GroupName = Group;
            Body.FarRange = 10f * PracticeMath.FeetToMeters;
            Body.NearRange = Body.FarRange;
            Body.ListTraits = new List<Trait>();
            return Body;
        }

        static void Wear(PlayerCharacter Who)
        {
            if (Who == null || Who.BodyModel != null || GameScreen.GraphicsDevice == null)
            {
                return;
            }

            string Glb = Path.Combine("Content", "Life Sim", "Characters", "Models", "Monsters", ModelFile);
            if (!File.Exists(Glb))
            {
                return;
            }

            try
            {
                Who.WearBody(GameScreen.GraphicsDevice, Glb);
                Who.BodyScale = BodyScale;
                Who.Model3DPath = Glb;
            }
            catch (Exception)
            {
            }
        }
    }
}
