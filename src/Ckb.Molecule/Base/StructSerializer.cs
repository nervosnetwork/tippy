using System;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class StructSerializer<T> : BaseSerializer<T>
    {
        private readonly BaseSerializer[] FieldSerializers;

        public StructSerializer(T value) : base(value)
        {
            FieldSerializers = Array.Empty<BaseSerializer>();
        }

        public StructSerializer(T value, BaseSerializer[] fieldSerializers) : base(value)
        {
            FieldSerializers = fieldSerializers;
        }

        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body => FieldSerializers.SelectMany(s => s.Serialize()).ToArray();
    }
}
