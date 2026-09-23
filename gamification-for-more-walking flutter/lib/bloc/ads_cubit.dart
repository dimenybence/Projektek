import 'dart:ui';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/data/repositories/ads_repository.dart';
import 'ads_state.dart';

/// Cubit that manages the ads state and business logic.
/// This layer does NOT contain any direct ad loading code or UI code.
/// It communicates only with the Repository layer.
class AdsCubit extends Cubit<AdsCombinedState> {
  final AdsRepository _repository;
  static const int _maxFailedLoadAttempts = 3;
  int _rewardedLoadAttempts = 0;

  AdsCubit({required AdsRepository repository})
    : _repository = repository,
      super(const AdsCombinedState()) {
    _initializeAds();
  }

  /// Initialize the Mobile Ads SDK
  Future<void> _initializeAds() async {
    try {
      await _repository.initializeAds();
      emit(state.copyWith(isInitialized: true));
    } catch (e) {
      emit(
        state.copyWith(
          isInitialized: false,
          rewardedError: 'Failed to initialize ads: $e',
        ),
      );
    }
  }

  /// Load a rewarded ad
  Future<void> loadRewardedAd() async {
    if (state.isRewardedLoading) return;

    emit(state.copyWith(isRewardedLoading: true, clearRewardedError: true));

    try {
      await _repository.loadRewardedAd(
        onAdLoaded: (ad) {
          _rewardedLoadAttempts = 0;
          emit(
            state.copyWith(
              rewardedAd: ad,
              isRewardedLoading: false,
              clearRewardedError: true,
            ),
          );
        },
        onAdFailedToLoad: (error) {
          _rewardedLoadAttempts++;
          emit(
            state.copyWith(
              isRewardedLoading: false,
              rewardedError: 'Rewarded ad failed to load: ${error.message}',
              clearRewardedAd: true,
            ),
          );

          // Retry loading if under max attempts
          if (_rewardedLoadAttempts < _maxFailedLoadAttempts) {
            Future.delayed(const Duration(seconds: 2), () {
              if (!isClosed) {
                loadRewardedAd();
              }
            });
          }
        },
      );
    } catch (e) {
      emit(
        state.copyWith(
          isRewardedLoading: false,
          rewardedError: 'Failed to load rewarded ad: $e',
        ),
      );
    }
  }

  /// Show the rewarded ad
  void showRewardedAd({VoidCallback? onRewardEarned}) {
    if (state.rewardedAd == null) {
      emit(
        state.copyWith(
          rewardedError: 'No rewarded ad loaded',
          clearRewardedError: false,
        ),
      );
      // Try to load one for next time
      loadRewardedAd();
      return;
    }

    _repository.showRewardedAd(
      ad: state.rewardedAd!,
      onUserEarnedReward: (ad, reward) {
        onRewardEarned?.call();
        emit(
          state.copyWith(
            totalRewards: state.totalRewards + reward.amount.toInt(),
            lastRewardType: reward.type,
            clearRewardedAd: true, // Ad is consumed
          ),
        );
        // Load the next ad
        loadRewardedAd();
      },
      onAdDismissed: (ad) {
        // Ad dismissed without reward (or after reward)
        // If reward was earned, it's already handled in onUserEarnedReward
        // Just ensure we clear the ad reference if not already done
        if (state.rewardedAd != null) {
          emit(state.copyWith(clearRewardedAd: true));
          loadRewardedAd();
        }
      },
      onAdFailedToShow: (ad, error) {
        emit(
          state.copyWith(
            rewardedError: 'Ad failed to show: ${error.message}',
            clearRewardedAd: true,
          ),
        );
        loadRewardedAd();
      },
    );
  }
}
