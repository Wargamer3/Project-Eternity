using System;
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

        private static byte[] Bytecode;

        private readonly Effect Compiled;
        private readonly EffectParameter WorldViewProjParameter;
        private readonly EffectParameter WorldParameter;
        private readonly EffectParameter WorldInverseTransposeParameter;
        private readonly EffectParameter EyePositionParameter;
        private readonly EffectParameter BonesParameter;
        private readonly EffectParameter UvOffsetParameter;

        public SkinnedBoneEffect(GraphicsDevice Device, Texture2D Texture, Vector3 Diffuse, float Alpha)
        {
            if (Device == null)
            {
                throw new ArgumentNullException(nameof(Device));
            }

            Compiled = new Effect(Device, EffectBytecode());
            WorldViewProjParameter = Parameter("WorldViewProj");
            WorldParameter = Parameter("World");
            WorldInverseTransposeParameter = Parameter("WorldInverseTranspose");
            EyePositionParameter = Parameter("EyePosition");
            BonesParameter = Parameter("Bones");
            UvOffsetParameter = Parameter("UvOffset");
            UvOffsetParameter.SetValue(Vector2.Zero);
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
        }

        public void SetUvOffset(Vector2 Offset)
        {
            UvOffsetParameter.SetValue(Offset);
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

        private static byte[] EffectBytecode()
        {
            if (Bytecode != null)
            {
                return Bytecode;
            }

            string Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Shaders", "SkinnedBones.xnb");
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("SkinnedBones.xnb was not found in Content/Shaders.", Path);
            }

            byte[] FileBytes = File.ReadAllBytes(Path);
            Bytecode = ReadEffectBytecode(FileBytes);
            return Bytecode;
        }

        private static byte[] ReadEffectBytecode(byte[] FileBytes)
        {
            if (FileBytes.Length < 10 || FileBytes[0] != (byte)'X' || FileBytes[1] != (byte)'N' || FileBytes[2] != (byte)'B')
            {
                throw new InvalidDataException("SkinnedBones.xnb is not an effect.");
            }

            if ((FileBytes[5] & 0x80) != 0)
            {
                throw new InvalidDataException("SkinnedBones.xnb is compressed.");
            }

            int Cursor = 10;
            int ReaderCount = Read7Bit(FileBytes, ref Cursor);
            for (int Reader = 0; Reader < ReaderCount; Reader++)
            {
                int NameLength = Read7Bit(FileBytes, ref Cursor);
                Cursor += NameLength + 4;
            }

            Read7Bit(FileBytes, ref Cursor);
            Read7Bit(FileBytes, ref Cursor);
            int Length = BitConverter.ToInt32(FileBytes, Cursor);
            Cursor += 4;
            if (Length <= 0 || Cursor + Length > FileBytes.Length)
            {
                throw new InvalidDataException("SkinnedBones.xnb has no effect bytecode.");
            }

            byte[] EffectCode = new byte[Length];
            Buffer.BlockCopy(FileBytes, Cursor, EffectCode, 0, Length);
            return EffectCode;
        }

        private static int Read7Bit(byte[] Source, ref int Cursor)
        {
            int Value = 0;
            int Shift = 0;
            while (Shift <= 28)
            {
                if (Cursor >= Source.Length)
                {
                    throw new InvalidDataException("SkinnedBones.xnb ended early.");
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

            throw new InvalidDataException("SkinnedBones.xnb has a bad length.");
        }
    }
}
