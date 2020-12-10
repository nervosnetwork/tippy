using System;
using Ckb.Address;
using Ckb.Rpc;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Types = Ckb.Types;
using IndexerTypes = Ckb.Types.IndexrTypes;
using Tippy.Util;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AddressesController : ApiControllerBase
    {
        [HttpGet("{address}")]
        public ActionResult Index(string address)
        {
            IndexerClient? indexerClient = NewIndexerClient();
            if (indexerClient == null)
            {
                return NoContent();
            }

            string prefix = AddressPrefix();
            Types.Script lockScript = Address.ParseAddress(address, prefix);

            IndexerTypes.SearchKey searchKey = new(lockScript, "lock");
            IndexerTypes.CellCapacity? cellCapacity = indexerClient.GetCellsCapacity(searchKey);
            UInt64 balance = 0;
            if (cellCapacity != null)
            {
                balance = Hex.HexToUInt64(cellCapacity.Capacity);
            }

            int cellsCount = GetCellsCount(indexerClient, searchKey);

            AddressResult addressResult = new()
            {
                AddressHash = address,
                LockScript = lockScript,
                Balance = balance.ToString(),
                LiveCellsCount = cellsCount.ToString(),
            };

            Result<AddressResult> result = new("address", addressResult);

            return Ok(result);
        }

        private static int GetCellsCount(IndexerClient indexerClient, IndexerTypes.SearchKey searchKey)
        {
            string? afterCursor = null;
            int totalCount = 0;
            int limit = 100;

            while (true)
            {
                var result = indexerClient.GetCells(searchKey, limit: limit, afterCursor: afterCursor);
                if (result == null || result.LastCursor == null)
                {
                    break;
                }
                afterCursor = result.LastCursor;
                int count = result.Objects.Length;
                totalCount += count;
                if (count < limit)
                {
                    break;
                }
            }

            return totalCount;
        }
    }
}
