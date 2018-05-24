// 
// iOSユーティリティープラグイン
//
#import <Foundation/Foundation.h>

extern "C"
{
    float iOS_GetiOSVersion();
    int iOS_GetStatusBarWidth();
    int iOS_GetStatusBarHeight();
    float iOS_ScreenScaleFactor();
    int iOS_StorageAvailableMb();
}

// iOSのバージョンを取得
float iOS_GetiOSVersion()
{
    return [[[UIDevice currentDevice] systemVersion] floatValue];
}

// ステータスバーの高さ
int iOS_GetStatusBarHeight()
{
    CGRect statusBarViewRect = [[UIApplication sharedApplication] statusBarFrame];
    
    CGFloat height = statusBarViewRect.size.height;

    return (int)height;
}

// ステータスバーの幅
int iOS_GetStatusBarWidth()
{
    CGRect statusBarViewRect = [[UIApplication sharedApplication] statusBarFrame];
    
    CGFloat width = statusBarViewRect.size.width;
    
    return (int)width;
}

float iOS_ScreenScaleFactor()
{
    UIScreen* screen = [UIScreen mainScreen];
    
    // we should query nativeScale if available to get the true device resolution
    // this way we avoid unnecessarily large frame buffers and downscaling.
    // e.g. iPhone 6+ pretends to be a x3 device, while its physical screen is x2.6 something.
    if([screen respondsToSelector:@selector(nativeScale)])
    {
        // On AppleTV screen.nativeScale returns NaN when device is in sleep mode
        if (isnan(screen.nativeScale))
            return 1.0f;
        else
            return screen.nativeScale;
    }
    
    return screen.scale;
}

int iOS_StorageAvailableMb()
{
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES);
    NSDictionary *dict = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error:nil];

    if (dict)
    {
        int Mb = 1024 * 1024;
        unsigned long long free = [[dict objectForKey: NSFileSystemFreeSize] unsignedLongLongValue] / Mb;
        return (int)free;
    }

    return -1;
}