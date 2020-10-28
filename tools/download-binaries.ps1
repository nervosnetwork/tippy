$CkbVersion = Get-Content ".ckb-version"
$CkbIndexerVersion= Get-Content ".ckb-indexer-version"
$RootDir = Get-Location # Be sure to run this from root directory!

function GotoFolder() {
  New-Item -Path $RootDir/src/Tippy.Ctrl/BinDeps/win -ItemType Directory
  Set-Location $RootDir/src/Tippy.Ctrl/BinDeps/win
}
function DownloadCkb {
  $CkbFilename = "ckb_v${CkbVersion}_x86_64-pc-windows-msvc"

  Invoke-WebRequest "https://github.com/nervosnetwork/ckb/releases/download/v${CkbVersion}/${CkbFilename}.zip" -OutFile "${CkbFilename}.zip"
  Expand-Archive -Force "${CkbFilename}.zip" -DestinationPath ./
  Copy-Item $CkbFilename/ckb.exe ./
  Copy-Item $CkbFilename/ckb-cli.exe ./
  Remove-Item -Force -Recurse $CkbFilename
  Remove-Item "${CkbFilename}.zip"
}

function DownloadCkbIndexer() {
  $Filename = "ckb-indexer-${CkbIndexerVersion}-windows"
  $InnerZipFilename = "ckb-indexer-windows-x86_64"

  Invoke-WebRequest "https://github.com/nervosnetwork/ckb-indexer/releases/download/v${CkbIndexerVersion}/${Filename}.zip" -OutFile "${Filename}.zip"
  Expand-Archive -Force "${FILENAME}.zip" -DestinationPath ./
  Expand-Archive -Force "${InnerZipFilename}.zip"
  Copy-Item $InnerZipFilename/ckb-indexer.exe ./
  Remove-Item "${Filename}.zip"
  Remove-Item -Force -Recurse $InnerZipFilename
  Remove-Item "${InnerZipFilename}.zip"
}

GotoFolder
DownloadCkb
DownloadCkbIndexer
Set-Location $RootDir