using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class DiceRoll
    {
        private int DiceValue;

        public DiceRoll(string DiceText)
        {
            if (DiceText.StartsWith("d"))
            {
                DiceValue = int.Parse(DiceText.Substring(1));
            }
            else
            {
                DiceValue = int.Parse(DiceText);
            }
        }

        public int Roll()
        {
            return RandomHelper.Next(DiceValue);
        }
    }

    public class Proficiency
    {
        public enum ProficiencyRanks { None, Trained, Expert, Master, Legendary }

        public string Name;
        public string Description;
        public byte BaseValue;
        public string DiceText;
        public DiceRoll DiceRoll;

        public List<PlayerCharacter.CharacterStats> ListStatModifier;

        public List<CharacterAction> ListBaseAction;
        public List<string> ListBaseActionRelativePath;

        public List<CharacterAction> ListTrainedAction;
        public List<string> ListTrainedActionRelativePath;

        public List<CharacterAction> ListExpertAction;
        public List<string> ListExpertActionRelativePath;

        public List<CharacterAction> ListMasterAction;
        public List<string> ListMasterActionRelativePath;

        public List<CharacterAction> ListLegendaryAction;
        public List<string> ListLegendaryActionRelativePath;

        public Proficiency(string FilePath)
        {
            FileStream FS = new FileStream("Content/Life Sim/Proficiencies/" + FilePath + ".pep", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            BaseValue = BR.ReadByte();
            DiceText = BR.ReadString();
            DiceRoll = new DiceRoll(DiceText);

            byte ListStatModifierCount = BR.ReadByte();
            ListStatModifier = new List<PlayerCharacter.CharacterStats>(ListStatModifierCount);
            for (int S = 0; S < ListStatModifierCount; ++S)
            {
                ListStatModifier.Add((PlayerCharacter.CharacterStats)BR.ReadByte());
            }

            byte ListBaseActionCount = BR.ReadByte();
            ListBaseAction = new List<CharacterAction>(ListBaseActionCount);
            ListBaseActionRelativePath = new List<string>(ListBaseActionCount);
            for (int A = 0; A < ListBaseActionCount; ++A)
            {
                ListBaseActionRelativePath.Add(BR.ReadString());
            }

            byte ListTrainedActionCount = BR.ReadByte();
            ListTrainedAction = new List<CharacterAction>(ListTrainedActionCount);
            ListTrainedActionRelativePath = new List<string>(ListTrainedActionCount);
            for (int A = 0; A < ListTrainedActionCount; ++A)
            {
                ListTrainedActionRelativePath.Add(BR.ReadString());
            }

            byte ListExpertActionCount = BR.ReadByte();
            ListExpertAction = new List<CharacterAction>(ListExpertActionCount);
            ListExpertActionRelativePath = new List<string>(ListExpertActionCount);
            for (int A = 0; A < ListExpertActionCount; ++A)
            {
                ListExpertActionRelativePath.Add(BR.ReadString());
            }

            byte ListMasterActionCount = BR.ReadByte();
            ListMasterAction = new List<CharacterAction>(ListMasterActionCount);
            ListMasterActionRelativePath = new List<string>(ListMasterActionCount);
            for (int A = 0; A < ListMasterActionCount; ++A)
            {
                ListMasterActionRelativePath.Add(BR.ReadString());
            }

            byte ListLegendaryActionCount = BR.ReadByte();
            ListLegendaryAction = new List<CharacterAction>(ListLegendaryActionCount);
            ListLegendaryActionRelativePath = new List<string>(ListLegendaryActionCount);
            for (int A = 0; A < ListLegendaryActionCount; ++A)
            {
                ListLegendaryActionRelativePath.Add(BR.ReadString());
            }

            BR.Close();
            FS.Close();
        }
    }

    public class ProficiencyLink
    {
        public Proficiency LinkedProficiency;
        public string LinkedProficiencyRelativePath;
        public Proficiency.ProficiencyRanks ProficiencyRank;
        private int StatBonus;

        public ProficiencyLink(string LinkedProficiencyRelativePath)
        {
            this.LinkedProficiencyRelativePath = LinkedProficiencyRelativePath;
        }

        public ProficiencyLink(BinaryReader BR)
        {
            LinkedProficiencyRelativePath = BR.ReadString();
            ProficiencyRank = (Proficiency.ProficiencyRanks)BR.ReadByte();
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(LinkedProficiencyRelativePath);
            BW.Write((byte)ProficiencyRank);
        }

        public void ComputeStatBonuses(PlayerCharacter Owner)
        {
            StatBonus = 0;

            if (ProficiencyRank > Proficiency.ProficiencyRanks.None)
            {
                StatBonus += Owner.Level;

                switch (ProficiencyRank)
                {
                    case Proficiency.ProficiencyRanks.Trained:
                        StatBonus += 2;
                        break;

                    case Proficiency.ProficiencyRanks.Expert:
                        StatBonus += 4;
                        break;

                    case Proficiency.ProficiencyRanks.Master:
                        StatBonus += 6;
                        break;

                    case Proficiency.ProficiencyRanks.Legendary:
                        StatBonus += 8;
                        break;
                }

                foreach (PlayerCharacter.CharacterStats ActiveStat in LinkedProficiency.ListStatModifier)
                {
                    switch (ActiveStat)
                    {
                        case PlayerCharacter.CharacterStats.STR:
                            StatBonus += Owner.STR;
                            break;

                        case PlayerCharacter.CharacterStats.DEX:
                            StatBonus += Owner.DEX;
                            break;

                        case PlayerCharacter.CharacterStats.CON:
                            StatBonus += Owner.CON;
                            break;

                        case PlayerCharacter.CharacterStats.INT:
                            StatBonus += Owner.INT;
                            break;

                        case PlayerCharacter.CharacterStats.WIS:
                            StatBonus += Owner.WIS;
                            break;

                        case PlayerCharacter.CharacterStats.CHA:
                            StatBonus += Owner.CHA;
                            break;
                    }
                }
            }
        }

        public int Roll()
        {
            return LinkedProficiency.BaseValue + StatBonus + LinkedProficiency.DiceRoll.Roll();
        }

        public override string ToString()
        {
            return LinkedProficiencyRelativePath + " - " + ProficiencyRank;
        }
    }
}
