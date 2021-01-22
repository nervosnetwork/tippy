using System.Collections.Generic;
using IndexerTypes = Ckb.Types.IndexrTypes;
using Hex = Ckb.Types.Convert;

namespace Ckb.Rpc
{
    public class IndexerClient : BaseClient
    {
        public IndexerClient(string url) : base(url) { }

        public IndexerTypes.CellCapacity? GetCellsCapacity(IndexerTypes.SearchKey searchKey)
        {
            object[] methodParams = { searchKey };
            return Call<IndexerTypes.CellCapacity>("get_cells_capacity", methodParams);
        }

        public IndexerTypes.Result<IndexerTypes.Cell>? GetCells(IndexerTypes.SearchKey searchKey, string order = "asc", int limit = 100, string? afterCursor = null)
        {
            List<object> methodParams = new() { searchKey, order, Hex.Int32ToHex(limit) };
            if (afterCursor != null)
            {
                methodParams.Add(afterCursor);
            }
            return Call<IndexerTypes.Result<IndexerTypes.Cell>>("get_cells", methodParams.ToArray());
        }

        public IndexerTypes.Result<IndexerTypes.Transaction>? GetTransactions(IndexerTypes.SearchKey searchKey, string order = "asc", int limit = 100, string? afterCursor = null)
        {
            List<object> methodParams = new() { searchKey, order, Hex.Int32ToHex(limit) };
            if (afterCursor != null)
            {
                methodParams.Add(afterCursor);
            }
            return Call<IndexerTypes.Result<IndexerTypes.Transaction>>("get_transactions", methodParams.ToArray());
        }
    }
}
