using System.Collections.Generic;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class StructSerializer<T> : BaseSerializer<T>
    {
        private readonly List<BaseSerializer> FieldSerializers;

        public StructSerializer(T value) : base(value)
        {
            FieldSerializers = new List<BaseSerializer>();
        }

        public StructSerializer(T value, List<BaseSerializer> fieldSerializers) : base(value)
        {
            FieldSerializers = fieldSerializers;
        }

        public override byte[] Header => new byte[0];
        public override byte[] Body => FieldSerializers.SelectMany(s => s.Serialize()).ToArray();
    }
}
