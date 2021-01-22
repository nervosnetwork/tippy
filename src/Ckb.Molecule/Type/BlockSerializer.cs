using System.Linq;
using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class BlockSerializer : TableSerializer
    {
        public BlockSerializer(Block block)
            : base(new BaseSerializer[]
            {
                new HeaderSerializer(block.Header),
                new DynVecSerializer<Uncle, UncleBlockSerializer>(block.Uncles),
                new DynVecSerializer<Transaction, TransactionSerializer>(block.Transactions),
                new FixVecSerializer<byte[], ProposalShortIdSerializer>(block.Proposals.Select(p => HexStringToBytes(p)).ToArray()),
            })
        { }
    }
}
