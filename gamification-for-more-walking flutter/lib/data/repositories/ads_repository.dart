import 'package:google_mobile_ads/google_mobile_ads.dart';
import '../../keys.dart';

/// Repository layer that handles Google Mobile Ads operations.
/// This layer abstracts ad loading and management logic from the BLoC.
class AdsRepository {
  static const String _rewardedAdUnitIdAndroid = rewardedAdUnitIdAndroid;

  /// Get the appropriate rewarded ad unit ID for the current platform
  String getRewardedAdUnitId() {
    return _rewardedAdUnitIdAndroid;
  }

  /// Initialize the Mobile Ads SDK
  Future<void> initializeAds() async {
    await MobileAds.instance.initialize();
  }

  /// Load a rewarded ad
  Future<void> loadRewardedAd({
    required Function(RewardedAd) onAdLoaded,
    required Function(LoadAdError) onAdFailedToLoad,
  }) async {
    await RewardedAd.load(
      adUnitId: getRewardedAdUnitId(),
      request: const AdRequest(),
      rewardedAdLoadCallback: RewardedAdLoadCallback(
        onAdLoaded: (ad) {
          onAdLoaded(ad);
        },
        onAdFailedToLoad: (error) {
          onAdFailedToLoad(error);
        },
      ),
    );
  }

  /// Show a rewarded ad with callbacks
  Future<void> showRewardedAd({
    required RewardedAd ad,
    required Function(AdWithoutView, RewardItem) onUserEarnedReward,
    required Function(Ad) onAdDismissed,
    required Function(Ad, AdError) onAdFailedToShow,
  }) async {
    // Set up full screen content callback
    ad.fullScreenContentCallback = FullScreenContentCallback(
      onAdShowedFullScreenContent: (ad) {
        // Ad showed full screen content
      },
      onAdDismissedFullScreenContent: (ad) {
        ad.dispose();
        onAdDismissed(ad);
      },
      onAdFailedToShowFullScreenContent: (ad, error) {
        ad.dispose();
        onAdFailedToShow(ad, error);
      },
    );

    // Show the ad
    ad.show(onUserEarnedReward: onUserEarnedReward);
  }
}
