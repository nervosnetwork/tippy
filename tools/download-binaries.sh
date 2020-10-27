#!/bin/bash

CKB_VERSION=$(cat .ckb-version)
ROOT_DIR=$(pwd) # Be sure to run this from root directory!

function download_ckb_macos() {
  CKB_FILENAME="ckb_${CKB_VERSION}_x86_64-apple-darwin"
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/mac

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/${CKB_VERSION}/${CKB_FILENAME}.zip"
  unzip ${CKB_FILENAME}.zip
  cp ${CKB_FILENAME}/ckb ./
  cp ${CKB_FILENAME}/ckb-cli ./
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.zip
}

function download_ckb_linux() {
  CKB_FILENAME="ckb_${CKB_VERSION}_x86_64-unknown-linux-gnu"
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/linux

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/${CKB_VERSION}/${CKB_FILENAME}.tar.gz"
  tar xvzf ${CKB_FILENAME}.tar.gz
  cp ${CKB_FILENAME}/ckb ./
  cp ${CKB_FILENAME}/ckb-cli ./
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.tar.gz
}

function download_ckb_windows() {
  CKB_FILENAME="ckb_${CKB_VERSION}_x86_64-pc-windows-msvc"
  cd $ROOT_DIR/src/Tippy.Ctrl/BinDeps/win

  curl -O -L "https://github.com/nervosnetwork/ckb/releases/download/${CKB_VERSION}/${CKB_FILENAME}.zip"
  unzip ${CKB_FILENAME}.zip
  cp ${CKB_FILENAME}/ckb.exe ./
  cp ${CKB_FILENAME}/ckb-cli.exe ./
  rm -rf $CKB_FILENAME
  rm ${CKB_FILENAME}.zip
}

case $1 in
  mac)    download_ckb_macos ;;
  linux)  download_ckb_linux ;;
  win)    download_ckb_windows ;;
  *)
    if [[ "$OSTYPE" == "darwin"* ]]; then
      download_ckb_macos
    elif [[ "$OSTYPE" == "linux-gnu" ]]; then
      download_ckb_linux
    else
      download_ckb_windows
    fi
  ;;
esac
