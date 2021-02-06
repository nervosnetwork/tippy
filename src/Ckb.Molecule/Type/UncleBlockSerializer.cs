using System.Linq;
using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class UncleBlockSerializer : TableSerializer
    {
        public UncleBlockSerializer(Uncle uncle)
            : base(new BaseSerializer[]
            {
                new HeaderSerializer(uncle.Header),
                new FixVecSerializer<byte[], ProposalShortIdSerializer>(uncle.Proposals.Select(p => HexStringToBytes(p)).ToArray()),
            })
        { }
    }
}
