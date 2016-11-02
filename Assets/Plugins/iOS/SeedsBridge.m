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


- (void)seedsInAppMessageClicked:(NSString*)messageId
{
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageClicked", [messageId UTF8String]);
}

- (void)seedsInAppMessageClicked:(NSString *)messageId withDynamicPrice:(double)price {
    NSString* msg = [NSString stringWithFormat:@"%@ %1.2f", messageId, price];
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageClickedWithDynamicPrice", [msg UTF8String]);
}

- (void)seedsInAppMessageDismissed:(NSString *)messageId
{
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageDismissed", [messageId UTF8String]);
}

- (void)seedsInAppMessageLoadSucceeded:(NSString*)messageId
{
    UnitySendMessage([self.gameObjectName UTF8String], "inAppMessageLoadSucceeded", [messageId UTF8String]);
}

- (void)seedsInAppMessageShown:(NSString*)messageId withSuccess:(BOOL)success
{
    UnitySendMessage([self.gameObjectName UTF8String],
                     success ? "inAppMessageShownSuccessfully" : "inAppMessageShownInsuccessfully",
                     [messageId UTF8String]);
}

- (void)seedsNoInAppMessageFound:(NSString*)messageId
{
    UnitySendMessage([self.gameObjectName UTF8String], "noInAppMessageFound", [messageId UTF8String]);
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

//BOOL Seeds_GetABTestingOn()
//{
//    return Seeds.sharedInstance.inAppMessageABTestingEnabled;
//}
//
//void Seeds_SetABTestingOn(BOOL abTestingOn)
//{
//    Seeds.sharedInstance.inAppMessageABTestingEnabled = abTestingOn;
//}

void Seeds_RequestInAppMessage(const char* pcsMessageId, const char* pcsManualLocalizedPrice)
{
    NSString* messageId = pcsMessageId ? [NSString stringWithUTF8String:pcsMessageId] : nil;
    NSString* manualLocalizedPrice = pcsManualLocalizedPrice ? [NSString stringWithUTF8String:pcsManualLocalizedPrice] : nil;
    [Seeds.sharedInstance requestInAppMessage:messageId withManualLocalizedPrice: manualLocalizedPrice];
}


BOOL Seeds_IsInAppMessageLoaded(const char* pcsMessageId)
{
    NSString* messageId = pcsMessageId ? [NSString stringWithUTF8String:pcsMessageId] : nil;
    return [Seeds.sharedInstance isInAppMessageLoaded:messageId];
}

void Seeds_ShowInAppMessage(const char* pcsMessageId, const char* pcsContext)
{
    NSString* messageId = pcsMessageId ? [NSString stringWithUTF8String:pcsMessageId] : nil;
    NSString* context = pcsContext ? [NSString stringWithUTF8String:pcsContext] : nil;
    [Seeds.sharedInstance showInAppMessage:messageId in:UnityGetGLViewController() withContext:context];
}

void Seeds_RequestInAppPurchaseCount(const char* pcsKey)
{
    NSString* key = pcsKey ? [NSString stringWithUTF8String:pcsKey] : nil;
    [Seeds.sharedInstance requestInAppPurchaseCount:^(NSString* errorMessage, int purchasesCount) {
        NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:
                            errorMessage ? errorMessage : @"", @"errorMessage",
                            key ? key : @"", @"key",
                            [NSString stringWithFormat:@"%d", purchasesCount], @"purchasesCount",
                            nil];

        NSError *err;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

        UnitySendMessage(
            [SeedsInAppMessageDelegateProxy.sharedInstance.gameObjectName UTF8String],
            "onInAppPurchaseCount",
            [jsonString UTF8String]);
    }
                                                 of:key];
}

void Seeds_RequestInAppMessageShowCount(const char* pcsMessageId)
{
    NSString* messageId = pcsMessageId ? [NSString stringWithUTF8String:pcsMessageId] : nil;
    [Seeds.sharedInstance requestInAppMessageShowCount:^(NSString* errorMessage, int showCount) {
        NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:
                              errorMessage ? errorMessage : @"", @"errorMessage",
                              messageId ? messageId : @"", @"messageId",
                              [NSString stringWithFormat:@"%d", showCount], @"showCount",
                              nil];

        NSError *err;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

        UnitySendMessage(
            [SeedsInAppMessageDelegateProxy.sharedInstance.gameObjectName UTF8String],
            "onInAppMessageShowCount",
            [jsonString UTF8String]);
    }
                                                 of:messageId];
}

void Seeds_RequestGenericUserBehaviorQuery(const char* pcsQueryPath)
{
    NSString* queryPath = pcsQueryPath ? [NSString stringWithUTF8String:pcsQueryPath] : nil;
    [Seeds.sharedInstance requestGenericUserBehaviorQuery:^(NSString* errorMessage, id result) {
        NSDictionary *dict = [NSDictionary dictionaryWithObjectsAndKeys:
                              errorMessage ? errorMessage : @"", @"errorMessage",
                              queryPath ? queryPath : @"", @"queryPath",
                              [result stringValue], @"result",
                              nil];

        NSError *err;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&err];
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

        UnitySendMessage(
                         [SeedsInAppMessageDelegateProxy.sharedInstance.gameObjectName UTF8String],
                         "onGenericUserBehaviorQuery",
                         [jsonString UTF8String]);
    }
                                                    of:queryPath];
}
