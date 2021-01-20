using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ckb.Molecule.Base
{
    public class FixVecSerializer<TItem, TItemSerializer> : BaseSerializer<TItem[]> where TItemSerializer : BaseSerializer<TItem>
    {
        private uint ItemsCount() => (uint)Value.Length;

        public FixVecSerializer(TItem[] value) : base(value) { }

        public override byte[] Header => UInt32ToLEBytes(ItemsCount());
        public override byte[] Body => Value.SelectMany(item => ((TItemSerializer)Activator.CreateInstance(typeof(TItemSerializer), item)).Serialize()).ToArray();
    }

    public class BytesSerializer : FixVecSerializer<byte, ByteSerializer>
    {
        public BytesSerializer(byte[] value) : base(value) { }
    }
}
