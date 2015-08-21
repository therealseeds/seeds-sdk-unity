#import <Foundation/Foundation.h>
#import "SeedsBridge.h"
#import "Seeds.h"
#import "SeedsInAppMessageDelegate.h"
#import "UnityInterface.h"
#include "SeedsConfig.h"

@interface SeedsDeepLinks ()

@property (nonatomic, copy) NSString *gameObjectName;

- (void)onOpenURL:(NSNotification*)notification;

@end

@implementation SeedsDeepLinks

@synthesize gameObjectName;

+ (instancetype)sharedInstance
{
    static SeedsDeepLinks *s_sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{s_sharedInstance = self.new;});
    return s_sharedInstance;
}

- (instancetype)init
{
    if (self = [super init])
    {
        self.gameObjectName = nil;
    }
    return self;
}

- (BOOL)openDeepLink:(NSURL *)url
{
    if (url == nil)
        return NO;

    if (![[url scheme] isEqualToString:SEEDS_DeepLinking_scheme])
        return NO;

    if (![[url host] isEqualToString:SEEDS_DeepLinking_host])
        return NO;

    if ([SEEDS_DeepLinking_pathPrefix length] > 1 && ![[url path] hasPrefix:SEEDS_DeepLinking_pathPrefix])
        return NO;

    UnitySendMessage([self.gameObjectName UTF8String], "onLinkArrived", [[url absoluteString] UTF8String]);
    return YES;
}

@end

@interface SeedsInAppMessageDelegateProxy : NSObject <SeedsInAppMessageDelegate>

@property (nonatomic, copy) NSString *gameObjectName;

+ (instancetype)sharedInstance;

@end

@implementation SeedsInAppMessageDelegateProxy

@synthesize gameObjectName;

+ (instancetype)sharedInstance
{
    static SeedsInAppMessageDelegateProxy *s_sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{s_sharedInstance = self.new;});
    return s_sharedInstance;
}

- (instancetype)init
{
    if (self = [super init])
    {
        self.gameObjectName = nil;
    }
    return self;
}

- (void)seedsInAppMessageClicked:(SeedsInAppMessage*)inAppMessage
{
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageClicked", "");
}

- (void)seedsInAppMessageClosed:(SeedsInAppMessage*)inAppMessage andCompleted:(BOOL)completed
{
    UnitySendMessage([self.gameObjectName UTF8String],
                     completed ? "inAppMessageClosedComplete" : "inAppMessageClosedIncomplete",
                     "");
}

- (void)seedsInAppMessageLoadSucceeded:(SeedsInAppMessage*)inAppMessage
{
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageLoadSucceeded", "");
}

- (void)seedsInAppMessageShown:(SeedsInAppMessage*)inAppMessage withSuccess:(BOOL)success
{
    UnitySendMessage([self.gameObjectName UTF8String],
                     success ? "inAppMessageShownSuccessfully" : "inAppMessageShownInsuccessfully",
                     "");
}

- (void)seedsNoInAppMessageFound
{
    UnitySendMessage([self.gameObjectName UTF8String], "noInAppMessageFound", "");
}

@end

void SeedsDeepLinks_SetGameObjectName(const char* pcsGameObjectName)
{
    NSString* gameObjectName = [NSString stringWithUTF8String:pcsGameObjectName];
    SeedsDeepLinks.sharedInstance.gameObjectName = gameObjectName;
}

void SeedsDeepLinks_Setup(BOOL registerAsPlugin)
{
    if (registerAsPlugin)
        [SeedsDeepLinks.sharedInstance performSelector:@selector(registerAsPlugin)];
}

void Seeds_SetGameObjectName(const char* pcsGameObjectName)
{
    NSString* gameObjectName = [NSString stringWithUTF8String:pcsGameObjectName];
    SeedsInAppMessageDelegateProxy.sharedInstance.gameObjectName = gameObjectName;
}

void Seeds_Setup()
{
    [Seeds sharedInstance].inAppMessageDelegate = SeedsInAppMessageDelegateProxy.sharedInstance;
}

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

BOOL Seeds_IsStarted()
{
    return [Seeds.sharedInstance isStarted];
}

void Seeds_RecordEvent1(const char* pcsKey)
{
    NSString* key = [NSString stringWithUTF8String:pcsKey];
    [Seeds.sharedInstance recordEvent:key count:1];
}

void Seeds_RecordEvent2(const char* pcsKey, int count)
{
    NSString* key = [NSString stringWithUTF8String:pcsKey];
    [Seeds.sharedInstance recordEvent:key count:count];
}

void Seeds_RecordEvent3(const char* pcsKey, int count, double sum)
{
    NSString* key = [NSString stringWithUTF8String:pcsKey];
    [Seeds.sharedInstance recordEvent:key count:count sum:sum];
}

void Seeds_RecordIAPEvent(const char* pcsKey, double price)
{
    NSString* key = [NSString stringWithUTF8String:pcsKey];
    [Seeds.sharedInstance recordIAPEvent:key price:price];
}

void Seeds_RecordSeedsIAPEvent(const char* pcsKey, double price)
{
    NSString* key = [NSString stringWithUTF8String:pcsKey];
    [Seeds.sharedInstance recordSeedsIAPEvent:key price:price];
}

void Seeds_SetLocation(double lat, double lon)
{
    [Seeds.sharedInstance setLocation:lat longitude:lon];
}

void Seeds_EnableCrashReporting()
{
    [Seeds.sharedInstance startCrashReporting];
}

BOOL Seeds_GetABTestingOn()
{
    return Seeds.sharedInstance.inAppMessageABTestingEnabled;
}

void Seeds_SetABTestingOn(BOOL abTestingOn)
{
    Seeds.sharedInstance.inAppMessageABTestingEnabled = abTestingOn;
}

void Seeds_SetMessageVariantName(const char* pcsMessageVariantName)
{
    NSString* messageVariantName = [NSString stringWithUTF8String:pcsMessageVariantName];
    Seeds.sharedInstance.inAppMessageVariantName = messageVariantName;
}

const char* Seeds_GetMessageVariantName()
{
    const char* pcsMessageVariantName = [Seeds.sharedInstance.inAppMessageVariantName UTF8String];
    return strdup(pcsMessageVariantName);
}

void Seeds_RequestInAppMessage()
{
    [Seeds.sharedInstance requestInAppMessage];
}

BOOL Seeds_IsInAppMessageLoaded()
{
    return [Seeds.sharedInstance isInAppMessageLoaded];
}

void Seeds_ShowInAppMessage()
{
    [Seeds.sharedInstance showInAppMessageIn:UnityGetGLViewController()];
}
