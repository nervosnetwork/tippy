# tippy

![tippy](https://github.com/nervosnetwork/tippy/workflows/tippy/badge.svg)

> Tippy is still under active development and considered to be a work in progress.

## Getting Started

1. Install [.NET Core SDK](https://www.microsoft.com/net/download) 5.0
2. Install CKB related binary dependencies:
  ```shell
  ./tools/download-binaries.ps1
  ```
  or
  ```shell
  ./tools/download-binaries.sh
  ```
3. Start the embed CKB explorer frontend:
  ```shell
  cd src/Tippy/ClientApp
  npm install
  npm start
  ```
4. Run the .NET solution with Visual Studio 2019 (v16.8 or later), Visual Studio 2019 for Mac (v8.8 or later), or Visual Studio Code
5. Browse `http://localhost:5000/home` in your browser
