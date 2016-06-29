#!/bin/sh

./gradlew clean build
cp ./library/build/intermediates/bundles/release/classes.jar ../Assets/Plugins/Android/SeedsBridge.jar