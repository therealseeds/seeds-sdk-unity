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

- (void)seedsInAppMessageClicked:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId;
- (void)seedsInAppMessageClosed:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId andCompleted:(BOOL)completed;
- (void)seedsInAppMessageLoadSucceeded:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId;
- (void)seedsInAppMessageShown:(SeedsInAppMessage*)inAppMessage withMessageId:(NSString*)messageId withSuccess:(BOOL)success;
- (void)seedsNoInAppMessageFound:(NSString*)messageId;

@end

#endif
