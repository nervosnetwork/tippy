using System;
using System.Collections.Generic;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class DynVecSerializer<TItem, TItemSerializer> : BaseSerializer<TItem[]> where TItemSerializer : BaseSerializer
    {
        private byte[] SerializedBody = Array.Empty<byte>();
        private readonly List<uint> Offsets = new List<uint>();

        public DynVecSerializer(TItem[] items) : base(items)
        {
            PreSerialize();
        }

        private uint HeaderSize()
        {
            return Convert.ToUInt32(1 + Value.Length) * Uint32Capacity;
        }

        private uint BytesSize()
        {
            return HeaderSize() + Convert.ToUInt32(Body.Length);
        }

        public override byte[] Header => new List<byte>(UInt32ToLEBytes(BytesSize())).Concat(Offsets.SelectMany(o => UInt32ToLEBytes(o)).ToList()).ToArray();

        public override byte[] Body => SerializedBody;

        private void PreSerialize()
        {
            if (Value.Length == 0)
            {
                return;
            }

            List<byte[]> serialized = Value.Select(item => ((TItemSerializer)Activator.CreateInstance(typeof(TItemSerializer), item)).Serialize()).ToList();
            Offsets.Add(HeaderSize());
            foreach (var serializedItem in serialized.Take(serialized.Count - 1))
            {
                Offsets.Add(Offsets.Last() + (uint)serializedItem.Length);
            }
            SerializedBody = serialized.SelectMany(x => x).ToArray();
        }
    }
}
