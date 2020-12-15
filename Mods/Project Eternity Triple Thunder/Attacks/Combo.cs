using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Combo
    {
        public string ComboName;
        public string AnimationName;
        public AnimationTypes AnimationType;
        public RobotAnimation Owner;
        public List<InputChoice> ListInputChoice;//Input required to use that combo.
        public int CurrentInputIndex;
        public ComboRotationTypes ComboRotationType;
        public bool InstantActivation;

        public List<Combo> ListNextCombo;
        public List<int> ListStart;
        public List<int> ListEnd;

        public Combo()
        {
            ListNextCombo = new List<Combo>();
            ListInputChoice = new List<InputChoice>();
            ComboName = "";
            AnimationName = "";
            AnimationType = AnimationTypes.FullAnimation;
            CurrentInputIndex = 0;
            ComboRotationType = ComboRotationTypes.None;
        }

        public Combo(string Path)
        {
            FileStream FS = new FileStream("Content/Triple Thunder/Combos/" + Path + ".ttc", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            ComboName = Path;
            AnimationName = BR.ReadString();
            AnimationType = (AnimationTypes)BR.ReadInt32();
            ComboRotationType = (ComboRotationTypes)BR.ReadByte();
            InstantActivation = BR.ReadBoolean();

            int ListNextComboCount = BR.ReadInt32();
            ListNextCombo = new List<Combo>(ListNextComboCount);
            ListStart = new List<int>(ListNextComboCount);
            ListEnd = new List<int>(ListNextComboCount);

            for (int C = 0; C < ListNextComboCount; C++)
            {
                string NextComboName = BR.ReadString();
                int Start = BR.ReadInt32();
                int End = BR.ReadInt32();
                Combo NextCombo;

                if (NextComboName != ComboName)
                {
                    NextCombo = new Combo(NextComboName);
                }
                else
                {
                    NextCombo = this;
                }

                ListStart.Add(Start);
                ListEnd.Add(End);
                NextCombo.Owner = Owner;

                int ListInputChoiceCount = BR.ReadInt32();
                NextCombo.ListInputChoice = new List<InputChoice>(ListInputChoiceCount);
                for (int I = 0; I < ListInputChoiceCount; I++)
                {
                    InputChoice NewInputChoice = new InputChoice();
                    NewInputChoice.AttackInput = (AttackInputs)BR.ReadByte();
                    NewInputChoice.MovementInput = (MovementInputs)BR.ReadByte();
                    NewInputChoice.NextInputDelay = BR.ReadInt32();
                    NextCombo.ListInputChoice.Add(NewInputChoice);
                }

                ListNextCombo.Add(NextCombo);
            }

            BR.Close();
            FS.Close();
        }
        
        public override string ToString()
        {
            return ComboName;
        }

        public void Reset()
        {
            CurrentInputIndex = 0;
        }
    }
}
