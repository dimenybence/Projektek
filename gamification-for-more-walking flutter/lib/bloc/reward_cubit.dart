import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/reward_config.dart';
import 'package:walking_buddy/bloc/reward_state.dart';
import 'package:walking_buddy/data/repositories/reward_repository.dart';

class RewardCubit extends Cubit<RewardState> {
  final RewardRepository repository;

  RewardCubit(this.repository) : super(const RewardState({})) {
    loadTodayRewards(stepGoalToReward.keys.toList());
  }

  Future<void> loadTodayRewards(Iterable<int> rewardIds) async {
    final claimed = <int>{};

    for (final id in rewardIds) {
      final available = await repository.isRewardAvailable(id);
      if (!available) claimed.add(id);
    }

    emit(RewardState(claimed));
  }

  Future<void> claimReward(int rewardId) async {
    await repository.claimReward(rewardId);

    final updated = Set<int>.from(state.claimedToday)..add(rewardId);
    emit(RewardState(updated));
  }
}
