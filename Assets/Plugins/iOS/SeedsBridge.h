#import <Foundation/Foundation.h>

@interface SeedsDeepLinks : NSObject

+ (instancetype)sharedInstance;

- (BOOL)openDeepLink:(NSURL*)url;

@end
