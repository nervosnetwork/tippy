<p align="center">
  <img src="logo.png" width="256px">
</p>

![tippy](https://github.com/nervosnetwork/tippy/workflows/tippy/badge.svg)

![Tippy](tippy.png)

## Getting Started

You can download Tippy binary for your platform of choice from the [releases](https://github.com/nervosnetwork/tippy/releases) page.

Tippy is pre-built as self-contained .Net Core application. You don't have to install .Net Core to run it. If you're running it on Linux or macOS and want to use the Debugger feature, please install debugger dependencies following [this](#install-dependencies) section.

### On Windows:

* Download `tippy-win-x64.zip`
* Extract the zip file (default to `tippy-win-x64`)
* Enter `tippy-win-x64` folder and click `Tippy.exe` to start

### On Linux:

* Download `tippy-linux-x64.tar.gz`
* Extract the tar.gz file (default to `tippy-linux-x64`)
* Make `Tippy` executable: `chmod +x ./tippy-linux-x64/Tippy`
* `cd tippy-linux-x64` then run `./Tippy` to start

We also provide a Homebrew formula to install with one-liner:

```
brew install nervosnetwork/tap/tippy-linux
```

Then simply run `tippy` to start

### On macOS:

* Download `Tippy.dmg`
* Open the dmg file and drag `Tippy.app` to `/Applications` folder
* From `/Applications` click `Tippy.app` to start

We also provide a Homebrew cask to install with one-liner:

```
brew install nervosnetwork/tap/tippy
```

### Tippy Console and UI

While Tippy runs as a console application, it also provides web UI. By default the dashboard UI will be opened automatically, if not you can access it by visiting [http://localhost:5000/Home](http://localhost:5000/Home) from a browser.

### Debugger

Tippy ships with [CKB Debugger](https://github.com/nervosnetwork/ckb-standalone-debugger) to help off-chain contract development.

*Note: debugger is only supported on Linux and macOS. It's unavailable on Windows.*

![CKB Debugger](debugger.png)

## Install Dependencies

Debugger requires `ttyd` and `gdb`. We recommend that you install them with homebrew.

For Linux

```bash
brew install ttyd gdb
```

For macOS, build gdb from source (with `--build-from-source` option)

```bash
brew install gdb --HEAD --build-from-source
brew install ttyd
```

## API

Tippy exposes a set of RPCs in JSON-RPC 2.0 protocols for controlling a devchain.

It also proxies API calls to the active running devchain for transparent CKB interactions.

The URL of Tippy RPC is <code>http://localhost:5000/api</code>.

### CKB RPCs

For CKB RPCs, simply call any API method with Tippy API URL. For example:

```
echo '{
  "id": 2,
  "jsonrpc": "2.0",
  "method": "get_tip_block_number",
  "params": []
}' \
| tr -d '\n' \
| curl -H 'content-type: application/json' -d @- \
http://localhost:5000/api
```

See [CKB JSON-RPC doc](https://docs.nervos.org/docs/reference/rpc) for more information.

### Tippy RPCs

#### Method `create_chain`
* `create_chain({assembler_lock_arg, genesis_issued_cells})`
  * `assembler_lock_arg`(optional): Lock arg for block assembler (miner address).
  * `genesis_issued_cells`(optional): An array of genesis issued cells. See example for the structure.
* result: `{ id, name }`

Create a devchain and set it as current active chain.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "create_chain",
      "params": [
        {
          "assembler_lock_arg": "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d8",
          "genesis_issued_cells": [
              {
                "capacity": "0x5af3107a4000",
                "lock": {
                  "code_hash": "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8",
                  "args": "0xf2cb132b2f6849ef8abe57e98cddf713bb8d71cb",
                  "hash_type": "type"
                }
            }
          ]
        }
      ]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": {
        "id": 4,
        "name": "CKB devchain"
      }
    }

#### Method `delete_chain`
* `delete_chain(chain_id)`
  * `chain_id`: ID of the chain to delete.
* result: `"ok"`

Delete a chain.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "delete_chain",
      "params": [4]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "ok"
    }

#### Method `start_chain`

* `start_chain()`
* result: `"ok"`

Start the current active chain.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "start_chain",
      "params": []
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "ok"
    }

#### Method `stop_chain`

* `stop_chain()`
* result: `"ok"`

Stop the current active chain if it's running.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "stop_chain",
      "params": []
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "ok"
    }

#### Method `start_miner`

* `start_miner()`
* result: `"ok"`

Start the default miner.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "start_miner",
      "params": []
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "ok"
    }

#### Method `stop_miner`

* `stop_miner()`
* result: `"ok"`

Stop the current running default miner.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "stop_miner",
      "params": []
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "ok"
    }

#### Method `mine_blocks`

* `mine_blocks(number_of_blocks)`
* result: `"Wait for blocks to be mined."`

Mine `number_of_blocks` blocks at the interval of 1 second.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "mine_blocks",
      "params": [3]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "Wait for blocks to be mined."
    }

#### Method `revert_blocks`

* `revert_blocks(number_of_blocks)`
* result: `"Reverted blocks."`

Mine `number_of_blocks` blocks at the interval of 1 second.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "revert_blocks",
      "params": [3]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "Reverted blocks."
    }

#### Method `ban_transaction`

* `ban_transaction(tx_hash, type)`
  * `tx_hash`: Tx hash of the transaction.
  * `type`: Deny type, `propose` or `commit`.
* result: `"Added to denylist."`

Add a tx to denylist.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "ban_transaction",
      "params": ["0x9a0580274e9921e04e139214b58ffc60df1625055ab7806ee635b56d329d7732", "propose"]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "Added to denylist."
    }


#### Method `unban_transaction`

* `unban_transaction(tx_hash, type)`
  * `tx_hash`: Tx hash of the transaction.
  * `type`: Deny type, `propose` or `commit`.
* result: `"Removed from denylist."`

Remove a tx from denylist.

**Example**

Request

    {
      "id": "1",
      "jsonrpc": "2.0",
      "method": "unban_transaction",
      "params": ["0x9a0580274e9921e04e139214b58ffc60df1625055ab7806ee635b56d329d7732", "propose"]
    }

Response

    {
      "jsonrpc": "2.0",
      "id": "1",
      "result": "Removed from denylist."
    }

## Contributing

1. Fetch the codebase: `git clone https://github.com/nervosnetwork/tippy.git`
2. Install [.NET Core SDK](https://www.microsoft.com/net/download) 5.0
3. Install CKB related binary dependencies:
  ```shell
  ./tools/download-binaries.ps1
  ```
  or
  ```shell
  ./tools/download-binaries.sh
  ```
4. Open `Tippy.sln` with Visual Studio 2019 (v16.8 or later), Visual Studio 2019 for Mac (v8.8 or later), or Visual Studio Code
5. Select `Tippy` as startup project for the solution, then start debugging it
6. Browse `http://localhost:5000/home` in your browser

### Add Database Migration

`EF` models are located in `Tippy.Core` project. When making any changes to them and migration is needed, run this

```shell
dotnet ef migrations add [MigrationName] --project src/Tippy.Core --startup-project src/Tippy
```

Or open `Package Manager Console` in Visual Studio, select `Tippy.Core` as `Default project`, then run

```shell
Add-Migration [MigrationName]
```

## Design

Tippy's page design is based on [mazipan/bulma-admin-dashboard-template](https://github.com/mazipan/bulma-admin-dashboard-template).
