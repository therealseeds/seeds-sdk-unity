
// Seeds.h
//
// This code is provided under the MIT License.
//
// Please visit www.count.ly for more information.
//
// Changed by Oleksii Pelykh
//
// Changes: renamed to 'Seeds';
//

#import <Foundation/Foundation.h>
#if (TARGET_OS_IPHONE || TARGET_IPHONE_SIMULATOR) && (!SEEDS_TARGET_WATCHKIT)
#import <UIKit/UIKit.h>
#endif
@class SeedsEventQueue;
@protocol SeedsInAppMessageDelegate;

@interface Seeds : NSObject
{
	double unsentSessionLength;
	NSTimer *timer;
    time_t startTime;
	double lastTime;
	BOOL isSuspended;
    SeedsEventQueue *eventQueue;
}

@property (readonly, nonatomic, copy) NSString* deviceId;
@property (atomic, retain) id<SeedsInAppMessageDelegate> inAppMessageDelegate;

@property (atomic, assign) BOOL inAppMessageDoNotShow;
@property (atomic, assign) BOOL adClicked;
@property (atomic, copy) NSString* inAppMessageVariantName;

+ (instancetype)sharedInstance;

- (void)start:(NSString *)appKey withHost:(NSString *)appHost;

- (void)start:(NSString *)appKey withHost:(NSString *)appHost andDeviceId:(NSString *)deviceId;

- (BOOL)isStarted;

- (void)recordEvent:(NSString *)key count:(int)count;

- (void)recordEvent:(NSString *)key count:(int)count sum:(double)sum;

- (void)recordEvent:(NSString *)key segmentation:(NSDictionary *)segmentation count:(int)count;

- (void)recordEvent:(NSString *)key segmentation:(NSDictionary *)segmentation count:(int)count sum:(double)sum;

- (void)recordUserDetails:(NSDictionary *)userDetails;
extern NSString* const kCLYUserName;
extern NSString* const kCLYUserUsername;
extern NSString* const kCLYUserEmail;
extern NSString* const kCLYUserOrganization;
extern NSString* const kCLYUserPhone;
extern NSString* const kCLYUserGender;
extern NSString* const kCLYUserPicture;
extern NSString* const kCLYUserPicturePath;
extern NSString* const kCLYUserBirthYear;
extern NSString* const kCLYUserCustom;

- (void)recordIAPEvent:(NSString *)key price:(double)price;

- (void)recordSeedsIAPEvent:(NSString *)key price:(double)price;

- (void)trackPurchase:(NSString *)key price:(double)price;

- (void)requestInAppMessage;

- (BOOL)isInAppMessageLoaded;

- (void)showInAppMessageIn:(UIViewController*)viewController;

#pragma mark - Seeds Messaging
#if (TARGET_OS_IPHONE || TARGET_IPHONE_SIMULATOR) && (!SEEDS_TARGET_WATCHKIT)
/**
 * Seeds Messaging support
 */
- (void)startWithMessagingUsing:(NSString *)appKey withHost:(NSString *)appHost andOptions:(NSDictionary *)options;

/**
 * Make this device a test device, so only messages with test checkbox will arrive on it.
 */
- (void)startWithTestMessagingUsing:(NSString *)appKey withHost:(NSString *)appHost andOptions:(NSDictionary *)options;

- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken;

- (void)didFailToRegisterForRemoteNotifications;

/**
 * Records user's location and sends it to the server with next updateSession. This value will be used instead of geoip lookup on the server when sending geolocation-aware push notifications.
 */
- (void)setLocation:(double)latitude longitude:(double)longitude;

/**
 * Create a set of UIMutableUserNotificationCategory'ies which you can register in addition to your ones to enable iOS 8 actions.
 */
- (NSMutableSet *) seedsNotificationCategories;

/**
 * Create a set of UIMutableUserNotificationCategory'ies which you can register in addition to your ones to enable iOS 8 actions. This method gives you ability to provide localized or just different versions of your action titles.
 * @param titles Array of NSString objects in following order: Cancel, Open, Update, Review
 */
- (NSMutableSet *) seedsNotificationCategoriesWithActionTitles:(NSArray *)actions;

/**
 * Method that does automatic processing for Seeds Messaging:
 * - It records that the message has been received.
 * - In case of standard Seeds messages (Message, URL, Update, Review) it displays alert with app name as a title:
 * --- for Message - just alert with message text and button OK;
 * --- for URL - Cancel & Open buttons;
 * --- for Update - Cancel & Update buttons;
 * --- for Review - Cancel & Review buttons.
 * Whenever user presses one of (Open, Update, Review) buttons Seeds performs corresponding action (opens up a Safari for URL type, opens your app page in App Store for Update and review section of your app in App Store for Review) and records Action event so you could then see conversion rates in Seeds Dashboard.
 * @param info Dictionary you got from application:didReceiveRemoteNotification:
 * @param titles Array of NSString objects in following order: Cancel, Open, Update, Review
 * @return TRUE When Seeds has successfully handled notification and presented alert, FALSE otherwise.
 */
- (BOOL)handleRemoteNotification:(NSDictionary *)info withButtonTitles:(NSArray *)titles;

/**
 * Same as previous method, but with default button titles Cancel, OK, URL, Update, Review.
 */
- (BOOL)handleRemoteNotification:(NSDictionary *)info;

/**
 * Method records push opened event. Quite handy if you do not call handleRemoteNotification: method, but still want to see conversions in Seeds Dashboard.
 * @param c NSDictionary of @"c" userInfo key.
 */
- (void)recordPushOpenForSeedsDictionary:(NSDictionary *)c;

/**
 * Method records push action event. Quite handy if you do not call handleRemoteNotification: method, but still want to see conversions in Seeds Dashboard.
 * @param c NSDictionary of @"c" userInfo key.
 */
- (void)recordPushActionForSeedsDictionary:(NSDictionary *)c;
#endif

#pragma mark - Seeds CrashReporting
/*
 This SDK can be used for Seeds CrashReporting service in addition to Seeds Analytics.
 If the only thing you need is Seeds Analytics, you can skip this section. 
 For Seeds CrashReporting, you'll need to add one more line of Seeds code to your application:
 
 For iOS, inside `application:didFinishLaunchingWithOptions:` method, just after the line you started Seeds, add this line:
 `[Seeds.sharedInstance startCrashReporting];`
 
 With this one line of code, Seeds iOS SDK will generate a crash report if your application crashes due to an exception, and send it to Seeds Server for further inspection. If a crash report can not be delivered to server (i.e. no internet connection, unavailable server), the SDK stores the crash report locally in order to try again later.
 
 And you can use `SeedsCrashLog()` (just like `NSLog()`) to get custom logs with the crash reports. Logs generated by `SeedsCrashLog()` are stored in a non-persistent structure, and delivered to server only in case of a crash.
 
 A crash report includes following information in addition to Seeds Analytics already provides:
 
 - Exception Name
 - Exception Description
 - Stack Trace
 - Used RAM
 - Total RAM
 - Used Disk
 - Total Disk
 - Battery Level 
 - Device Orientation
 - Connection Type
 - OpenGL ES Version
 - Jailbrake State
 - Background State
 - Time Since Launch
 - Custom Logs generated by `SeedsCrashLog()`
 */
#if (TARGET_OS_IPHONE || TARGET_IPHONE_SIMULATOR) && (!SEEDS_TARGET_WATCHKIT)
- (void)startCrashReporting;
- (void)startCrashReportingWithSegments:(NSDictionary *)segments;
- (void)recordHandledException:(NSException *)exception;

- (void)crashTest;
- (void)crashTest2;
- (void)crashTest3;
- (void)crashTest4;

void SCL(const char* function, NSUInteger line, NSString* message);
#define SeedsCrashLog(format, ...) SCL(__FUNCTION__,__LINE__, [NSString stringWithFormat:(format), ##__VA_ARGS__])
#endif


#pragma mark - Seeds Background Fetch Session Ending
#if (TARGET_OS_IPHONE || TARGET_IPHONE_SIMULATOR) && (!SEEDS_TARGET_WATCHKIT)
- (void)endBackgroundSessionWithCompletionHandler:(void (^)(UIBackgroundFetchResult result))completionHandler;
#endif

@end