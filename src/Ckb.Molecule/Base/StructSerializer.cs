using System;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class StructSerializer : BaseSerializer
    {
        private readonly BaseSerializer[] FieldSerializers;

        public StructSerializer()
        {
            FieldSerializers = Array.Empty<BaseSerializer>();
        }

        public StructSerializer(BaseSerializer[] fieldSerializers)
        {
            FieldSerializers = fieldSerializers;
        }

        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body => FieldSerializers.SelectMany(s => s.Serialize()).ToArray();
    }
}
