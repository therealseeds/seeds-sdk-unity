//
//  SeedsInAppMessageDelegate.h
//  Seeds
//
//  Created by Alexey Pelykh on 8/14/15.
//
//

#ifndef Seeds_SeedsInAppMessageDelegate_h
#define Seeds_SeedsInAppMessageDelegate_h

@class SeedsInAppMessage;

@protocol SeedsInAppMessageDelegate <NSObject>

@optional

// Callback signatures for an app with a single interstitial
- (void)seedsInAppMessageClicked:(SeedsInAppMessage*)inAppMessage;
- (void)seedsInAppMessageClosed:(SeedsInAppMessage*)inAppMessage andCompleted:(BOOL)completed;
- (void)seedsInAppMessageLoadSucceeded:(SeedsInAppMessage*)inAppMessage;
- (void)seedsInAppMessageShown:(SeedsInAppMessage*)inAppMessage withSuccess:(BOOL)success;
- (void)seedsNoInAppMessageFound;

// Callback signatures for an app with multiple interstitials
- (void)seedsInAppMessageClicked:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId;
- (void)seedsInAppMessageClosed:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId andCompleted:(BOOL)completed;
- (void)seedsInAppMessageLoadSucceeded:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId;
- (void)seedsInAppMessageShown:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId withSuccess:(BOOL)success;
- (void)seedsNoInAppMessageFound:(NSString*)messageId;

@end

#endif
