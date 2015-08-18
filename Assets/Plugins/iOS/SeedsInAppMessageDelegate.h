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

- (void)seedsInAppMessageClicked:(SeedsInAppMessage*)inAppMessage;
- (void)seedsInAppMessageClosed:(SeedsInAppMessage*)inAppMessage andCompleted:(BOOL)completed;
- (void)seedsInAppMessageLoadSucceeded:(SeedsInAppMessage*)inAppMessage;
- (void)seedsInAppMessageShown:(SeedsInAppMessage*)inAppMessage withSuccess:(BOOL)success;
- (void)seedsNoInAppMessageFound;

@end

#endif
