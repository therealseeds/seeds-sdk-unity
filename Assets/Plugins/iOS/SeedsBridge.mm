#import <Foundation/Foundation.h>
#import "SeedsBridge.h"
#import "AppDelegateListener.h"

@interface SeedsDeepLinks (UnityPlugin) <AppDelegateListener>

- (void)registerAsPlugin;
- (void)onOpenURL:(NSNotification*)notification;

@end

@implementation SeedsDeepLinks (UnityPlugin)

- (void)registerAsPlugin
{
	UnityRegisterAppDelegateListener(self);
}

- (void)onOpenURL:(NSNotification*)notification
{
    NSURL *url = [notification.userInfo objectForKey:@"url"];
    [self openDeepLink:url];
}

@end
