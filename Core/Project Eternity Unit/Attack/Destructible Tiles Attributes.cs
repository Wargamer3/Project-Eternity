using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Attacks
{
    public class DestructibleTilesAttackAttributes
    {
        public enum DestructibleTypes { Regular, Forest, Road, Bridge, Rubble, River, Ocean, Shoal, Waterfall, Pipes, Slave /*Only used during loading to fill the tilesets created by multi tileset presets*/ }

        public struct GenericAttribute
        {
            public DestructibleTypes TypeIndex;
            public int Damage;

            public GenericAttribute(DestructibleTypes TypeIndex)
            {
                this.TypeIndex = TypeIndex;
                Damage = 10;
            }

            public GenericAttribute(BinaryReader BR)
            {
                TypeIndex = (DestructibleTypes)BR.ReadByte();
                Damage = BR.ReadInt32();
            }

            public override string ToString()
            {
                return TypeIndex.ToString();
            }
        }

        public struct UniqueAttribute
        {
            public string TypeName;
            public int Damage;

            public UniqueAttribute(string TypeName)
            {
                this.TypeName = TypeName;
                Damage = 10;
            }

            public UniqueAttribute(BinaryReader BR)
            {
                TypeName = BR.ReadString();
                Damage = BR.ReadInt32();
            }

            public override string ToString()
            {
                return TypeName;
            }
        }

        public List<GenericAttribute> ListGenericAttribute;
        public List<UniqueAttribute> ListUniqueAttribute;

        public DestructibleTilesAttackAttributes()
        {
            ListGenericAttribute = new List<GenericAttribute>();
            ListUniqueAttribute = new List<UniqueAttribute>();
        }

        public DestructibleTilesAttackAttributes(BinaryReader BR)
        {
            int ListGenericAttributeCount = BR.ReadByte();
            int ListUniqueAttributeCount = BR.ReadByte();

            ListGenericAttribute = new List<GenericAttribute>(ListGenericAttributeCount);
            for (int G = 0; G < ListGenericAttributeCount; ++G)
            {
                ListGenericAttribute.Add(new GenericAttribute(BR));
            }

            ListUniqueAttribute = new List<UniqueAttribute>(ListUniqueAttributeCount);
            for (int U = 0; U < ListGenericAttributeCount; ++U)
            {
                ListUniqueAttribute.Add(new UniqueAttribute(BR));
            }
        }
    }
}
