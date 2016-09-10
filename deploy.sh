set -euo pipefail
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

if [ $# -le 0 ]
then
  echo "A handy helper for automating all steps of the Unity SDK deployment workflow. \n"
  echo "deploy.sh mobile-sdks      Builds iOS and/or Android SDKs for Unity and moves the artifacts to"
  echo "                           the correct locations in Unity SDK. Overwrites the old artifacts."
  echo "                           You must set SEEDS_ANDROID_SDK, SEEDS_IOS_SDK, ANDROID_SDK envs.\n"

  echo "deploy.sh android-bridge   Builds the Android bridge code. You must set ANDROID_SDK env."

  echo "deploy.sh packages <ver>   Builds the normal & legacy packages (from SeedsBuild class)\n"
  echo "                           and names them after the specified version (e.g. '1.4.5').\n"

  echo "deploy.sh bintray <ver>    Deploys the packages to Bintray, using the specified version."
	exit 1
fi

operation=$1

if [[ $operation = "mobile-sdks" ]]; then
  # Don't open the directories in Finder when running the scripts
  export NOT_INTERACTIVE=1

  if [ "${SEEDS_ANDROID_SDK+1}" ]; then
    $SEEDS_ANDROID_SDK/BuildUnity3D.sh
    rsync -av $SEEDS_ANDROID_SDK/build/Unity3D/Assets/ $DIR/Assets/
  fi

  if [ "${SEEDS_IOS_SDK+1}" ]; then
    $SEEDS_IOS_SDK/BuildUnity3D.sh
    rsync -av $SEEDS_IOS_SDK/build/Unity3D/Assets/ $DIR/Assets/
  fi

  echo "\nSDKs builded and moved to the Unity SDK."
fi

if [[ $operation = "android-bridge" ]]; then
  ANDROID_HOME=$ANDROID_SDK $DIR/AndroidBridge/BuildUnity3D.sh
fi

if [[ $operation = "packages" ]]; then
  rm -rf SeedsSDK_Legacy.unitypackage

  rm -rf SeedsSDK.unitypackage

  /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath "$DIR" -executeMethod SeedsBuild.BuildPackage -quit -logFile /dev/stdout
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath "$DIR" -executeMethod SeedsBuild.BuildLegacyPackage -quit -logFile /dev/stdout

  echo "\nPackage creation completed! You can find them from the SDK root."
fi

if [[ $operation = "bintray" ]]; then
  echo "To be implemented"

  echo "Remember to update links and versions in the Unity SDK documentation page!"
fi
