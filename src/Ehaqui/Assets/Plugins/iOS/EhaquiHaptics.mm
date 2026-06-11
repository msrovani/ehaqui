#import <UIKit/UIKit.h>

extern "C" void _PlayHaptic(int level) {
    if (@available(iOS 13.0, *)) {
        UIImpactFeedbackGenerator *gen;
        switch (level) {
            case 1:
                gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
                break;
            case 2:
                gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
                break;
            case 3:
                gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
                break;
            case 4:
                gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleRigid];
                break;
            case 5:
                gen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleSoft];
                break;
            default:
                return;
        }
        [gen prepare];
        [gen impactOccurred];
    } else {
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
    }
}
