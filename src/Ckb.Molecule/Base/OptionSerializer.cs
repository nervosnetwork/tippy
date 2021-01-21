using System;

namespace Ckb.Molecule.Base
{
    public class OptionSerializer<TItem, TItemSerializer> : BaseSerializer<TItem> where TItemSerializer : BaseSerializer<TItem>
    {
        private readonly TItemSerializer Serializer;

        public OptionSerializer(TItem value, TItemSerializer serializer) : base(value)
        {
            Serializer = serializer;
        }

        public OptionSerializer(TItem value) : base(value)
        {
            Serializer = null;
        }

        public override byte[] Header => Array.Empty<byte>();
        public override byte[] Body
        {
            get
            {
                if (Value != null && Serializer != null)
                {
                    return Serializer.Serialize();
                }
                return Array.Empty<byte>();
            }
        }
    }
}
