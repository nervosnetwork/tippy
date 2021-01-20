using System;
using System.Linq;

namespace Ckb.Molecule.Base
{
    public class ArraySerializer<TItem, TItemSerializer> : BaseSerializer<TItem[]> where TItemSerializer : BaseSerializer<TItem>
    {
        public override byte[] Header => new byte[0];

        public override byte[] Body => Value.SelectMany(item => ((TItemSerializer)Activator.CreateInstance(typeof(TItemSerializer), item)).Serialize()).ToArray();

        public ArraySerializer(TItem[] value) : base(value) { }
    }
}
