#!/bin/sh

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR

./gradlew clean build || true
echo "NOTICE: If you just saw a Gradle com.android.dex.DexIndexOverflowException error, it doesn't mean that this build has failed, because we are only using the intermediate jar file and moving it to ../Assets/Plugins/Android/SeedsBridge.jar."
cp ./library/build/intermediates/bundles/release/classes.jar ../Assets/Plugins/Android/SeedsBridge.jar
