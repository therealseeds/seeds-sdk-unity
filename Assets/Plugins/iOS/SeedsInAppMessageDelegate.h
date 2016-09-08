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
- (void)seedsInAppMessageLoadSucceeded;
- (void)seedsInAppMessageShown:(BOOL)success;
- (void)seedsNoInAppMessageFound;
- (void)seedsInAppMessageClicked;
- (void)seedsInAppMessageDismissed;

// Callback signatures for an app with multiple interstitials
- (void)seedsInAppMessageLoadSucceeded:(NSString*)messageId;
- (void)seedsInAppMessageShown:(NSString*)messageId withSuccess:(BOOL)success;
- (void)seedsNoInAppMessageFound:(NSString*)messageId;
- (void)seedsInAppMessageClicked:(NSString*)messageId;
- (void)seedsInAppMessageDismissed:(NSString*)messageId;

// Callback signatures for an app with multiple interstitials with dynamic pricing
- (void)seedsInAppMessageClicked:(NSString *)messageId withDynamicPrice:(double)price;

@end

#endif
