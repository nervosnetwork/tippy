using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class TableSerializer : BaseSerializer
    {
        private readonly BaseSerializer[] FieldSerializers;

        private byte[] SerializedBody = Array.Empty<byte>();
        private readonly List<uint> Offsets = new List<uint>();

        public TableSerializer(BaseSerializer[] fieldSerializers)
        {
            FieldSerializers = fieldSerializers;
            PreSerialize();
        }

        public TableSerializer()
        {
            FieldSerializers = Array.Empty<BaseSerializer>();
            PreSerialize();
        }

        private uint HeaderSize() => Convert.ToUInt32(1 + FieldSerializers.Length) * Uint32Capacity;

        private uint BytesSize() => HeaderSize() + (uint)Body.Length;

        public override byte[] Header => new List<byte>(UInt32ToLEBytes(BytesSize())).Concat(Offsets.SelectMany(o => UInt32ToLEBytes(o)).ToList()).ToArray();
        public override byte[] Body => SerializedBody;

        private void PreSerialize()
        {
            if (FieldSerializers.Length == 0)
            {
                return;
            }

            List<byte[]> serialized = FieldSerializers.Select(s => s.Serialize()).ToList();
            Offsets.Add(HeaderSize());
            foreach (var serializedItem in serialized.Take(serialized.Count - 1).ToList())
            {
                Offsets.Add(Offsets.Last() + (uint)serializedItem.Length);
            }
            SerializedBody = serialized.SelectMany(x => x).ToArray();
        }
    }
}
