using System.Linq;
using Ckb.Molecule.Base;
using Ckb.Types;

namespace Ckb.Molecule.Type
{
    public class RawTransactionSerializer : TableSerializer<Transaction>
    {
        public RawTransactionSerializer(Transaction tx)
            : base(tx, new BaseSerializer[] {
                new UInt32Serializer(tx.Version),
                new FixVecSerializer<CellDep, CellDepSerializer>(tx.CellDeps),
                new FixVecSerializer<byte[], Byte32Serializer>(tx.HeaderDeps.Select(hd => HexStringToBytes(hd)).ToArray()),
                new FixVecSerializer<Input, CellInputSerializer>(tx.Inputs),
                new DynVecSerializer<Output, CellOutputSerializer>(tx.Outputs),
                new DynVecSerializer<byte[], BytesSerializer>(tx.OutputsData.Select(data => HexStringToBytes(data)).ToArray()),
            })
        { }
    }

    public class TransactionSerializer : TableSerializer<Transaction>
    {
        public TransactionSerializer(Transaction tx)
            : base(tx, new BaseSerializer[]
            {
                new RawTransactionSerializer(tx),
                new DynVecSerializer<byte[], BytesSerializer>(tx.Witnesses.Select(wit => HexStringToBytes(wit)).ToArray()),
            })
        { }
    }
}
