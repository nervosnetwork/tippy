#!/bin/bash

CKB_VERSION=$(cat .ckb-version)
CKB_INDEXER_VERSION=$(cat .ckb-indexer-version)
CKB_DEBUGGER_VERSION=$(cat .ckb-debugger-version)
ROOT_DIR=$(pwd) # Be sure to run this from root directory!

function download_ckb_macos() {
  CKB_FILENAME="ckb_v${CKB_VERSION}_x86_64-apple-darwin"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/v${CKB_VERSION}/${CKB_FILENAME}.zip"
  unzip -o ${CKB_FILENAME}.zip ${CKB_FILENAME}/ckb ${CKB_FILENAME}/ckb-cli
  cp ${CKB_FILENAME}/ckb ./
  chmod +x ./ckb
  # cp ${CKB_FILENAME}/ckb-cli ./
  # chmod +x ./ckb-cli
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.zip
}

function download_ckb_linux() {
  CKB_FILENAME="ckb_v${CKB_VERSION}_x86_64-unknown-linux-gnu"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/v${CKB_VERSION}/${CKB_FILENAME}.tar.gz"
  tar xvzf ${CKB_FILENAME}.tar.gz ${CKB_FILENAME}/ckb ${CKB_FILENAME}/ckb-cli
  cp ${CKB_FILENAME}/ckb ./
  chmod +x ./ckb
  # cp ${CKB_FILENAME}/ckb-cli ./
  # chmod +x ./ckb-cli
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.tar.gz
}

function download_ckb_windows() {
  CKB_FILENAME="ckb_v${CKB_VERSION}_x86_64-pc-windows-msvc"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/win
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/win

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/v${CKB_VERSION}/${CKB_FILENAME}.zip"
  unzip -o ${CKB_FILENAME}.zip ${CKB_FILENAME}/ckb.exe ${CKB_FILENAME}/ckb-cli.exe
  cp ${CKB_FILENAME}/ckb.exe ./
  # cp ${CKB_FILENAME}/ckb-cli.exe ./
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.zip
}

function download_ckb_indexer_macos() {
  FILENAME="ckb-indexer-${CKB_INDEXER_VERSION}-macos"
  INNER_ZIP_FILENAME="ckb-indexer-mac-x86_64.zip"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac

  curl -O -L "https://github.com/nervosnetwork/ckb-indexer/releases/download/v${CKB_INDEXER_VERSION}/${FILENAME}.zip"
  unzip -o ${FILENAME}.zip
  unzip -o ${INNER_ZIP_FILENAME} ckb-indexer
  chmod +x ./ckb-indexer
  rm ${FILENAME}.zip
  rm ${INNER_ZIP_FILENAME}
}

function download_ckb_indexer_linux() {
  FILENAME="ckb-indexer-${CKB_INDEXER_VERSION}-linux"
  TAR_FILENAME="ckb-indexer-linux-x86_64.tar.gz"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux

  curl -O -L "https://github.com/nervosnetwork/ckb-indexer/releases/download/v${CKB_INDEXER_VERSION}/${FILENAME}.zip"
  unzip -o ${FILENAME}.zip
  tar xvzf $TAR_FILENAME ckb-indexer
  chmod +x ./ckb-indexer
  rm -rf $TAR_FILENAME
  rm ${FILENAME}.zip
}

function download_ckb_indexer_windows() {
  FILENAME="ckb-indexer-${CKB_INDEXER_VERSION}-windows"
  INNER_ZIP_FILENAME="ckb-indexer-windows-x86_64.zip"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/win
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/win

  curl -O -L "https://github.com/nervosnetwork/ckb-indexer/releases/download/v${CKB_INDEXER_VERSION}/${FILENAME}.zip"
  unzip -o ${FILENAME}.zip
  unzip -o ${INNER_ZIP_FILENAME} ckb-indexer.exe
  rm ${FILENAME}.zip
  rm ${INNER_ZIP_FILENAME}
}

function download_ckb_debugger_macos() {
  FILENAME="ckb-debugger-macos-x64.tar.gz"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac

  curl -O -L "https://github.com/nervosnetwork/ckb-standalone-debugger/releases/download/v${CKB_DEBUGGER_VERSION}/${FILENAME}"
  tar xvzf $FILENAME ckb-debugger
  chmod +x ./ckb-debugger
  rm -rf $FILENAME
  chmod +x ./ckb-debugger
  rm $FILENAME
}

function download_ckb_debugger_linux() {
  FILENAME="ckb-debugger-linux-x64.tar.gz"
  mkdir -p $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux

  curl -O -L "https://github.com/nervosnetwork/ckb-standalone-debugger/releases/download/v${CKB_DEBUGGER_VERSION}/${FILENAME}"
  tar xvzf $FILENAME ckb-debugger
  chmod +x ./ckb-debugger
  rm -rf $FILENAME
  chmod +x ./ckb-debugger
  rm $FILENAME
}

function download_ckb_debugger_windows() {
  # TODO: debugger not supported on win yet.
}

function download_macos() {
  download_ckb_macos
  download_ckb_indexer_macos
  download_ckb_debugger_macos
}

function download_linux() {
  download_ckb_linux
  download_ckb_indexer_linux
  download_ckb_debugger_linux
}

function download_windows() {
  download_ckb_windows
  download_ckb_indexer_windows
  download_ckb_debugger_windows
}

case $1 in
  mac)    download_macos ;;
  linux)  download_linux ;;
  win)    download_windows ;;
  *)
    if [[ "$OSTYPE" == "darwin"* ]]; then
      download_macos
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
      download_linux
    else
      download_windows
    fi
  ;;
esac
