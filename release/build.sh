#!/bin/bash


COMMAND="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="../Unity Free Flight"
LOG_FILE="build.out"
OPTIONS="-quit -batchmode -logFile $LOG_FILE -projectPath=$PROJECT_PATH"

PACKAGER="./package.sh"

BUILD_PATH="../release/FreeFlight"

BUILD_OSX="-buildOSX64Player $BUILD_PATH"
BUILD_WINDOWS="-buildWindows64Player $BUILD_PATH.exe"
BUILD_LINUX="-buildLinux64Player $BUILD_PATH.x86_64"
BUILD_WEB="-buildWebPlayerStreamed $BUILD_PATH"

touch $LOG_FILE
tail -n 0 -f "$LOG_FILE" & 
tail_pid=$!

$COMMAND $OPTIONS $BUILD_OSX &&
$PACKAGER "osx" &&

$COMMAND $OPTIONS $BUILD_WINDOWS &&
$PACKAGER "windows" &&

$COMMAND $OPTIONS $BUILD_LINUX &&
$PACKAGER "linux" &&

$COMMAND $OPTIONS $BUILD_WEB &&
$PACKAGER "web" &&

if [ "$?" -eq "0" ]
then
	echo "SUCCESS! All builds pass!!!"
else
	echo "FAIL! Stopping build process..."
fi

kill $tail_pid &> /dev/null
wait $tail_pid
rm $LOG_FILE
