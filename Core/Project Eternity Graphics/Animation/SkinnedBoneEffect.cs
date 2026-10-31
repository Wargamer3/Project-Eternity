using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Graphics
{
    /// <summary>
    /// vs_4_0 skinned effect. The palette holds 256 bones because blend indices are bytes.
    /// </summary>
    sealed class SkinnedBoneEffect : IDisposable
    {
        public const int MaxBones = 256;

        private static readonly Dictionary<string, byte[]> BytecodeByFile = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        private readonly Effect Compiled;
        private readonly EffectParameter WorldViewProjParameter;
        private readonly EffectParameter WorldParameter;
        private readonly EffectParameter WorldInverseTransposeParameter;
        private readonly EffectParameter EyePositionParameter;
        private readonly EffectParameter BonesParameter;
        private readonly EffectParameter UvOffsetParameter;
        private readonly EffectParameter LightingModeParameter;
        private readonly EffectParameter AlphaTestParameter;
        private readonly EffectParameter CharacterAmbientParameter;
        private readonly EffectParameter CharacterLightDirectionParameter;
        private readonly EffectParameter CharacterLightColorParameter;
        private readonly EffectParameter TwoSidedLightParameter;
        private readonly EffectParameter SelfIlluminationParameter;
        private readonly EffectParameter WetLineParameter;
        private readonly EffectParameter WetAmountParameter;
        private readonly EffectParameter OcclusionMapParameter;
        private readonly EffectParameter OcclusionViewProjParameter;
        private readonly EffectParameter OccludeParameter;

        public SkinnedBoneEffect(GraphicsDevice Device, Texture2D Texture, Vector3 Diffuse, float Alpha)
            : this(Device, Texture, Diffuse, Alpha, "SkinnedBones.xnb")
        {
        }

        internal SkinnedBoneEffect(GraphicsDevice Device, Texture2D Texture, Vector3 Diffuse, float Alpha, string EffectFile)
        {
            if (Device == null)
            {
                throw new ArgumentNullException(nameof(Device));
            }

            if (string.IsNullOrWhiteSpace(EffectFile))
            {
                throw new ArgumentNullException(nameof(EffectFile));
            }

            Compiled = new Effect(Device, EffectBytecode(EffectFile));
            WorldViewProjParameter = Parameter("WorldViewProj");
            WorldParameter = Parameter("World");
            WorldInverseTransposeParameter = Parameter("WorldInverseTranspose");
            EyePositionParameter = Parameter("EyePosition");
            BonesParameter = Parameter("Bones");
            UvOffsetParameter = Parameter("UvOffset");
            LightingModeParameter = Parameter("LightingMode");
            AlphaTestParameter = Parameter("AlphaTest");
            CharacterAmbientParameter = Parameter("CharacterAmbient");
            CharacterLightDirectionParameter = Parameter("CharacterLightDirection");
            CharacterLightColorParameter = Parameter("CharacterLightColor");
            TwoSidedLightParameter = Parameter("TwoSidedLight");
            SelfIlluminationParameter = Parameter("SelfIllumination");
            WetLineParameter = Optional("WetLine");
            WetAmountParameter = Optional("WetAmount");
            OcclusionMapParameter = Optional("OcclusionMap");
            OcclusionViewProjParameter = Optional("OcclusionViewProj");
            OccludeParameter = Optional("Occlude");
            UvOffsetParameter.SetValue(Vector2.Zero);
            SetDefaultLight();
            Parameter("DiffuseColor").SetValue(new Vector4(Diffuse, Alpha));
            Parameter("DiffuseTexture").SetValue(Texture);
        }

        public EffectPassCollection Passes
        {
            get
            {
                return Compiled.CurrentTechnique.Passes;
            }
        }

        public void Prepare(Matrix WorldMatrix, Matrix ViewMatrix, Matrix ProjectionMatrix, Matrix[] BoneMatrices)
        {
            if (BoneMatrices == null)
            {
                throw new ArgumentNullException(nameof(BoneMatrices));
            }

            if (BoneMatrices.Length > MaxBones)
            {
                throw new ArgumentException("A skinned mesh uses " + BoneMatrices.Length.ToString() + " bones. The effect holds " + MaxBones.ToString() + ".");
            }

            WorldViewProjParameter.SetValue(WorldMatrix * ViewMatrix * ProjectionMatrix);
            WorldParameter.SetValue(WorldMatrix);
            WorldInverseTransposeParameter.SetValue(Matrix.Transpose(Matrix.Invert(WorldMatrix)));
            EyePositionParameter.SetValue(Matrix.Invert(ViewMatrix).Translation);
            BonesParameter.SetValue(BoneMatrices);
            UvOffsetParameter.SetValue(Vector2.Zero);
            if (OccludeParameter != null && OcclusionViewProjParameter != null)
            {
                bool Live = SceneOcclusion.On && SceneOcclusion.Map != null;
                OccludeParameter.SetValue(Live ? 1f : 0f);
                OcclusionViewProjParameter.SetValue(ViewMatrix * ProjectionMatrix);
                if (Live && OcclusionMapParameter != null)
                {
                    OcclusionMapParameter.SetValue(SceneOcclusion.Map);
                }
            }
        }

        public void SetDefaultLight()
        {
            LightingModeParameter.SetValue(0f);
            AlphaTestParameter.SetValue(0f);
            TwoSidedLightParameter.SetValue(0f);
            SelfIlluminationParameter.SetValue(0f);
        }

        // AlphaTest 1 keeps texels at 255. AlphaTest 2 keeps the softer texels.
        // SelfIllumination blends the lamp back toward the unlit texel, then the result is clamped to 1.
        public void SetCharacterLight(Vector3 Ambient, Vector4 Direction, Vector3 Color, bool TwoSided, float AlphaTest, float SelfIllumination)
        {
            LightingModeParameter.SetValue(1f);
            CharacterAmbientParameter.SetValue(Ambient);
            CharacterLightDirectionParameter.SetValue(Direction);
            CharacterLightColorParameter.SetValue(Color);
            TwoSidedLightParameter.SetValue(TwoSided ? 1f : 0f);
            AlphaTestParameter.SetValue(AlphaTest);
            SelfIlluminationParameter.SetValue(SelfIllumination);
        }

        public void SetWet(float Line, float Amount)
        {
            if (WetLineParameter != null)
            {
                WetLineParameter.SetValue(Line);
            }

            if (WetAmountParameter != null)
            {
                WetAmountParameter.SetValue(Amount);
            }
        }

        public void SetUvOffset(Vector2 Offset)
        {
            UvOffsetParameter.SetValue(Offset);
        }

        internal void BindShade(Action<Effect> Shade)
        {
            if (Shade != null)
            {
                Shade(Compiled);
            }
        }

        public void Dispose()
        {
            Compiled.Dispose();
        }

        private EffectParameter Parameter(string Name)
        {
            EffectParameter Found = Compiled.Parameters[Name];
            if (Found == null)
            {
                throw new InvalidDataException("SkinnedBones.fx has no parameter \"" + Name + "\".");
            }

            return Found;
        }

        private EffectParameter Optional(string Name)
        {
            return Compiled.Parameters[Name];
        }

        private static byte[] EffectBytecode(string FileName)
        {
            byte[] Cached;
            if (BytecodeByFile.TryGetValue(FileName, out Cached))
            {
                return Cached;
            }

            string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Shaders", FileName);
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException(FileName + " was not found in Content/Shaders.", Path);
            }

            byte[] FileBytes = File.ReadAllBytes(Path);
            byte[] Bytecode = ReadEffectBytecode(FileBytes, FileName);
            BytecodeByFile[FileName] = Bytecode;
            return Bytecode;
        }

        private static byte[] ReadEffectBytecode(byte[] FileBytes, string FileName)
        {
            if (FileBytes.Length < 10 || FileBytes[0] != (byte)'X' || FileBytes[1] != (byte)'N' || FileBytes[2] != (byte)'B')
            {
                throw new InvalidDataException(FileName + " is not an effect.");
            }

            if ((FileBytes[5] & 0x80) != 0)
            {
                throw new InvalidDataException(FileName + " is compressed.");
            }

            int Cursor = 10;
            int ReaderCount = Read7Bit(FileBytes, ref Cursor, FileName);
            for (int Reader = 0; Reader < ReaderCount; Reader++)
            {
                int NameLength = Read7Bit(FileBytes, ref Cursor, FileName);
                Cursor += NameLength + 4;
            }

            Read7Bit(FileBytes, ref Cursor, FileName);
            Read7Bit(FileBytes, ref Cursor, FileName);
            int Length = BitConverter.ToInt32(FileBytes, Cursor);
            Cursor += 4;
            if (Length <= 0 || Cursor + Length > FileBytes.Length)
            {
                throw new InvalidDataException(FileName + " has no effect bytecode.");
            }

            byte[] EffectCode = new byte[Length];
            Buffer.BlockCopy(FileBytes, Cursor, EffectCode, 0, Length);
            return EffectCode;
        }

        private static int Read7Bit(byte[] Source, ref int Cursor, string FileName)
        {
            int Value = 0;
            int Shift = 0;
            while (Shift <= 28)
            {
                if (Cursor >= Source.Length)
                {
                    throw new InvalidDataException(FileName + " ended early.");
                }

                byte Next = Source[Cursor];
                Cursor++;
                Value |= (Next & 0x7f) << Shift;
                if ((Next & 0x80) == 0)
                {
                    return Value;
                }

                Shift += 7;
            }

            throw new InvalidDataException(FileName + " has a bad length.");
        }
    }
}
