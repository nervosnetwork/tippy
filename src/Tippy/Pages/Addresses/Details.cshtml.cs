using System;
using Ckb.Address;
using Ckb.Rpc;
using Ckb.Types;
using Ckb.Types.IndexrTypes;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Util;

namespace Tippy.Pages.Addresses
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public AddressResult AddressResult { get; set; } = default!;

        public IActionResult OnGet(string address)
        {
            IndexerClient indexerClient = IndexerRpc();

            string prefix = AddressPrefix();
            Script lockScript;
            try
            {
                lockScript = Address.ParseAddress(address, prefix);
            }
            catch
            {
                return NotFound();
            }

            SearchKey searchKey = new(lockScript, "lock");
            CellCapacity? cellCapacity = indexerClient.GetCellsCapacity(searchKey);
            UInt64 balance = 0;
            if (cellCapacity != null)
            {
                balance = Hex.HexToUInt64(cellCapacity.Capacity);
            }

            int cellsCount = GetCellsCount(indexerClient, searchKey);

            AddressResult = new()
            {
                AddressHash = address,
                LockScript = lockScript,
                Balance = balance.ToString(),
                LiveCellsCount = cellsCount.ToString(),
            };

            return Page();
        }

        private static int GetCellsCount(IndexerClient indexerClient, SearchKey searchKey)
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
