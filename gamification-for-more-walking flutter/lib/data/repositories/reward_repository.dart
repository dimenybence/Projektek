import 'package:walking_buddy/data/dao/reward_dao.dart';

class RewardRepository {
  final RewardDao _rewardDao;

  RewardRepository({RewardDao? rewardDao})
    : _rewardDao = rewardDao ?? RewardDao.instance;

  Future<void> claimReward(int rewardId) async {
    return await _rewardDao.confirmRewardToday(rewardId);
  }

  Future<bool> isRewardAvailable(int rewardId) async {
    return await _rewardDao.isRewardAvailableToday(rewardId);
  }
}
