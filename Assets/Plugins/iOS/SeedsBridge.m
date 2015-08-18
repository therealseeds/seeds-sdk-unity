#import <Foundation/Foundation.h>
#import <SeedsSDK/SeedsSDK.h>

void Seeds_Init(const char* pcsServerUrl, const char* pcsAppKey)
{
    NSString* serverUrl = [NSString stringWithUTF8String:pcsServerUrl];
    NSString* appKey = [NSString stringWithUTF8String:pcsAppKey];
    [Seeds.sharedInstance start:appKey withHost:serverUrl];
}

void Seeds_InitWithDeviceId(const char* pcsServerUrl, const char* pcsAppKey, const char* pcsDeviceId)
{
    NSString* serverUrl = [NSString stringWithUTF8String:pcsServerUrl];
    NSString* appKey = [NSString stringWithUTF8String:pcsAppKey];
    NSString* deviceId = [NSString stringWithUTF8String:pcsDeviceId];
    [Seeds.sharedInstance start:appKey withHost:serverUrl andDeviceId:deviceId];
}
