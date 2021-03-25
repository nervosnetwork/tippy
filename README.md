# tippy

![tippy](https://github.com/nervosnetwork/tippy/workflows/tippy/badge.svg)

> Tippy is still under active development and considered to be a work in progress.

## Install Dependencies

Transaction debugger requires `ttyd` and `gdb 10`

For Linux

```bash
brew install ttyd gdb
```

For macOS, must build gdb from source

```bash
brew install gdb --build-from-source
brew install ttyd
```

And debugger not support Windows.

## Getting Started

1. Fetch the codebase: `git clone --recursive https://github.com/nervosnetwork/tippy.git`
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
6. Browse `http://localhost:5000/home` in your browser (if it's not opened automatically)

## Add Database Migration

```shell
dotnet ef migrations add [MigrationName]  --project src/Tippy.Core --startup-project src/Tippy
```

## Design

Tippy's page design is baded on [mazipan/bulma-admin-dashboard-template](https://github.com/mazipan/bulma-admin-dashboard-template).
