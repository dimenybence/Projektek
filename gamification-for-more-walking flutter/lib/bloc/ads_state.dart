import 'package:equatable/equatable.dart';
import 'package:google_mobile_ads/google_mobile_ads.dart';

/// Base class for all Ad states
abstract class AdsState extends Equatable {
  const AdsState();

  @override
  List<Object?> get props => [];
}

/// Combined state for the AdsCubit to manage multiple ad types simultaneously
class AdsCombinedState extends Equatable {
  final bool isInitialized;
  final bool isRewardedLoading;
  final RewardedAd? rewardedAd;
  final String? rewardedError;
  final int totalRewards;
  final String? lastRewardType;

  const AdsCombinedState({
    this.isInitialized = false,
    this.isRewardedLoading = false,
    this.rewardedAd,
    this.rewardedError,
    this.totalRewards = 0,
    this.lastRewardType,
  });

  AdsCombinedState copyWith({
    bool? isInitialized,
    bool? isRewardedLoading,
    RewardedAd? rewardedAd,
    String? rewardedError,
    int? totalRewards,
    String? lastRewardType,
    bool clearRewardedAd = false,
    bool clearRewardedError = false,
  }) {
    return AdsCombinedState(
      isInitialized: isInitialized ?? this.isInitialized,
      isRewardedLoading: isRewardedLoading ?? this.isRewardedLoading,
      rewardedAd: clearRewardedAd ? null : (rewardedAd ?? this.rewardedAd),
      rewardedError: clearRewardedError
          ? null
          : (rewardedError ?? this.rewardedError),
      totalRewards: totalRewards ?? this.totalRewards,
      lastRewardType: lastRewardType ?? this.lastRewardType,
    );
  }

  @override
  List<Object?> get props => [
    isInitialized,
    isRewardedLoading,
    rewardedAd,
    rewardedError,
    totalRewards,
    lastRewardType,
  ];
}
