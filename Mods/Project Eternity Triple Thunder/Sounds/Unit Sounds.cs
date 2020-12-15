using System;
using System.IO;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public struct UnitSounds
    {
        public enum CrouchStartSounds { None, Default }
        public enum CrouchMoveSounds { None, Default }
        public enum CrouchEndSounds { None, Default }

        public enum JetpackStartSounds { None, Default }
        public enum JetpackUseSounds { None, Default }
        public enum JetpackEndSounds { None, Default }

        public enum ProneStartSounds { None, Default }
        public enum ProneMoveSounds { None, Default }
        public enum ProneEndSounds { None, Default }

        public enum JumpStartSounds { None, Default }
        public enum JumpEndSounds { None, Default }
        public enum JumpStrainSounds { None, Male, Female }

        public enum StepNormalSounds { None, Default }
        public enum StepGrassSounds { None, Default }
        public enum StepWaterSounds { None, Default }

        public enum DeathSounds { None, Male, Female }
        public enum RollSounds { None, Default }
        public enum DashSounds { None, Default }

        public readonly CrouchStartSounds CrouchStartSound;
        public readonly CrouchMoveSounds CrouchMoveSound;
        public readonly CrouchEndSounds CrouchEndSound;

        public readonly JetpackStartSounds JetpackStartSound;
        public readonly JetpackUseSounds JetpackUseSound;
        public readonly JetpackEndSounds JetpackEndSound;

        public readonly ProneStartSounds ProneStartSound;
        public readonly ProneMoveSounds ProneMoveSound;
        public readonly ProneEndSounds ProneEndSound;

        public readonly JumpStartSounds JumpStartSound;
        public readonly JumpEndSounds JumpEndSound;
        public readonly JumpStrainSounds JumpStrainSound;

        public readonly StepNormalSounds StepNormalSound;
        public readonly StepGrassSounds StepGrassSound;
        public readonly StepWaterSounds StepWaterSound;

        public readonly DeathSounds DeathSound;
        public readonly RollSounds RollSound;
        public readonly DashSounds DashSound;

        public UnitSounds(BinaryReader BR)
        {
            Enum.TryParse(BR.ReadString(), out CrouchStartSound);
            Enum.TryParse(BR.ReadString(), out CrouchMoveSound);
            Enum.TryParse(BR.ReadString(), out CrouchEndSound);

            Enum.TryParse(BR.ReadString(), out JetpackStartSound);
            Enum.TryParse(BR.ReadString(), out JetpackUseSound);
            Enum.TryParse(BR.ReadString(), out JetpackEndSound);

            Enum.TryParse(BR.ReadString(), out ProneStartSound);
            Enum.TryParse(BR.ReadString(), out ProneMoveSound);
            Enum.TryParse(BR.ReadString(), out ProneEndSound);

            Enum.TryParse(BR.ReadString(), out JumpStartSound);
            Enum.TryParse(BR.ReadString(), out JumpEndSound);
            Enum.TryParse(BR.ReadString(), out JumpStrainSound);

            Enum.TryParse(BR.ReadString(), out StepNormalSound);
            Enum.TryParse(BR.ReadString(), out StepGrassSound);
            Enum.TryParse(BR.ReadString(), out StepWaterSound);

            Enum.TryParse(BR.ReadString(), out DeathSound);
            Enum.TryParse(BR.ReadString(), out RollSound);
            Enum.TryParse(BR.ReadString(), out DashSound);
        }
    }
}
