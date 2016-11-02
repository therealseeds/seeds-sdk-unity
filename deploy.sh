set -euo pipefail
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

if [ $# -le 0 ]
then
  echo "A handy helper for automating all steps of the Unity SDK deployment workflow. \n"
  echo "deploy.sh mobile-sdks      Builds iOS and/or Android SDKs for Unity and moves the artifacts to"
  echo "                           the correct locations in Unity SDK. Overwrites the old artifacts."
  echo "                           You must set SEEDS_ANDROID_SDK, SEEDS_IOS_SDK, ANDROID_SDK envs.\n"

  echo "deploy.sh android-bridge   Builds the Android bridge code. You must set ANDROID_SDK env.\n"

  echo "deploy.sh packages <ver>   Builds the normal & legacy packages (from SeedsBuild class)"
  echo "                           and names them after the specified version (e.g. 0.3.12).\n"

  echo "deploy.sh bintray <ver>    Deploys the packages to Bintray, using the specified version."
  echo "                           You must set BINTRAY_USER and BINTRAY_API_KEY envs.\n"
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

if [ $operation = "packages" ] || [ $operation = "bintray" ]; then
  PCK_VERSION=$2
  PCK_VERSION_STRIPPED="${PCK_VERSION//.}"
  SDK_FILE=SeedsSDK${PCK_VERSION_STRIPPED}.unitypackage
  SDK_LEGACY_FILE=SeedsSDK_Legacy${PCK_VERSION_STRIPPED}.unitypackage
fi

if [[ $operation = "packages" ]]; then
  rm -rf $SDK_FILE
  rm -rf $SDK_LEGACY_FILE

  /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath "$DIR" -executeMethod SeedsBuild.BuildPackage -quit -logFile /dev/stdout
  /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -projectPath "$DIR" -executeMethod SeedsBuild.BuildLegacyPackage -quit -logFile /dev/stdout

  mv SeedsSDK.unitypackage $SDK_FILE
  mv SeedsSDK_Legacy.unitypackage $SDK_LEGACY_FILE

  echo "\nPackage creation completed! You can find them from the SDK root."
fi

deploy_bintray() {
    CURL="curl -u${BINTRAY_USER}:${BINTRAY_API_KEY} -H Content-Type:application/json -H Accept:application/json"

  if (upload_bintray); then
    echo "Publishing ${FILE}..."
    ${CURL} -X POST ${API}/content/${BINTRAY_REPO}/${PCK_NAME}/${PCK_VERSION}/publish/ -d "{ \"discard\": \"false\" }"
  else
    echo "[SEVERE] First you should upload your rpm ${FILE}"
  fi
}

upload_bintray() {
  echo "Uploading ${FILE}..."

  uploaded=` [ $(${CURL} --write-out %{http_code} --silent --output /dev/null -T ${FILE} -H X-Bintray-Package:${PCK_NAME} -H X-Bintray-Version:${PCK_VERSION} ${API}/content/${BINTRAY_REPO}/${FILE}) -eq 201 ] `
  echo "File ${FILE} uploaded? y:1/N:0 ${uploaded}"
  return ${uploaded}
}

if [[ $operation = "bintray" ]]; then
  # Template: https://github.com/bintray/bintray-examples/blob/master/bash-example/pushToBintray.sh

  API=https://api.bintray.com
  PACKAGE_DESCRIPTOR=bintray-package.json
  BINTRAY_REPO='seedsinc/unity-sdk'

  FILE=$SDK_FILE; PCK_NAME='unity5-sdk'
  deploy_bintray

  FILE=$SDK_LEGACY_FILE; PCK_NAME='unity4-sdk'
  deploy_bintray

  echo "Remember to update links and versions in the Unity SDK documentation page!"
fi
