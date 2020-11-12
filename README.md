# tippy

![tippy](https://github.com/nervosnetwork/tippy/workflows/tippy/badge.svg)

> Tippy is still under active development and considered to be a work in progress.

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
4. Start the embed CKB explorer frontend:
  ```shell
  cd src/Tippy/ClientApp
  npm install
  npm start
  ```
5. Open `Tippy.sln` with Visual Studio 2019 (v16.8 or later), Visual Studio 2019 for Mac (v8.8 or later), or Visual Studio Code
6. Select `Tippy` as startup project for the solution, then start debugging it
7. Browse `http://localhost:5000/home` in your browser
