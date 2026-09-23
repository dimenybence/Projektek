import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/ads_cubit.dart';
import 'package:walking_buddy/bloc/ads_state.dart';
import 'package:walking_buddy/bloc/coin_cubit.dart';
import 'package:walking_buddy/bloc/reward_cubit.dart';
import 'package:walking_buddy/bloc/reward_state.dart';

class RewardCard extends StatelessWidget {
  final int stepGoal;
  final int rewardAmount;
  final int currentSteps;

  const RewardCard({
    super.key,
    required this.stepGoal,
    required this.rewardAmount,
    required this.currentSteps,
  });

  @override
  Widget build(BuildContext context) {
    final bool isUnlocked = currentSteps >= stepGoal;
    return BlocConsumer<RewardCubit, RewardState>(
      listener: (context, state) {},
      builder: (context, state) {
        return Container(
          margin: const EdgeInsets.only(bottom: 16),
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: isUnlocked
                ? Colors.deepPurple.withValues(alpha: 0.2)
                : Colors.grey.shade900,
            borderRadius: BorderRadius.circular(16),
            border: isUnlocked
                ? Border.all(color: Colors.purpleAccent.withValues(alpha: 0.3))
                : null,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: isUnlocked ? Colors.purple : Colors.white10,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Icon(
                      isUnlocked ? Icons.emoji_events : Icons.lock,
                      color: Colors.white,
                      size: 24,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '$stepGoal steps',
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          const Icon(
                            Icons.monetization_on,
                            color: Colors.amber,
                            size: 16,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            '+$rewardAmount',
                            style: const TextStyle(
                              color: Colors.amber,
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ],
              ),
              const SizedBox(height: 16),
              // Progress Bar
              ClipRRect(
                borderRadius: BorderRadius.circular(4),
                child: LinearProgressIndicator(
                  value: isUnlocked
                      ? 1.0
                      : (currentSteps / stepGoal).clamp(0.0, 1.0),
                  backgroundColor: Colors.white10,
                  valueColor: AlwaysStoppedAnimation<Color>(
                    isUnlocked ? Colors.purpleAccent : Colors.grey,
                  ),
                  minHeight: 8,
                ),
              ),
              const SizedBox(height: 16),
              if (isUnlocked)
                Row(
                  children: [
                    Expanded(
                      child: Container(
                        decoration: BoxDecoration(
                          gradient: const LinearGradient(
                            colors: [Colors.purpleAccent, Colors.deepPurple],
                            begin: Alignment.topLeft,
                            end: Alignment.bottomRight,
                          ),
                          borderRadius: BorderRadius.circular(12),
                          boxShadow: [
                            BoxShadow(
                              color: Colors.purple.withValues(alpha: 0.3),
                              blurRadius: 8,
                              offset: const Offset(0, 4),
                            ),
                          ],
                        ),
                        child: ElevatedButton.icon(
                          onPressed: () {
                            context.read<RewardCubit>().claimReward(stepGoal);
                            context.read<CoinCubit>().addCoin(rewardAmount);
                          },
                          icon: const Icon(Icons.emoji_events, size: 18),
                          label: const Text('Claim'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.transparent,
                            shadowColor: Colors.transparent,
                            foregroundColor: Colors.white,
                            padding: const EdgeInsets.symmetric(vertical: 12),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: BlocBuilder<AdsCubit, AdsCombinedState>(
                        builder: (context, adsState) {
                          final isAdLoaded = adsState.rewardedAd != null;
                          return Container(
                            decoration: BoxDecoration(
                              gradient: isAdLoaded
                                  ? const LinearGradient(
                                      colors: [
                                        Colors.greenAccent,
                                        Colors.green,
                                      ],
                                      begin: Alignment.topLeft,
                                      end: Alignment.bottomRight,
                                    )
                                  : LinearGradient(
                                      colors: [
                                        Colors.grey.shade700,
                                        Colors.grey.shade800,
                                      ],
                                      begin: Alignment.topLeft,
                                      end: Alignment.bottomRight,
                                    ),
                              borderRadius: BorderRadius.circular(12),
                              boxShadow: isAdLoaded
                                  ? [
                                      BoxShadow(
                                        color: Colors.green.withValues(
                                          alpha: 0.3,
                                        ),
                                        blurRadius: 8,
                                        offset: const Offset(0, 4),
                                      ),
                                    ]
                                  : [],
                            ),
                            child: ElevatedButton.icon(
                              onPressed: isAdLoaded
                                  ? () {
                                      context.read<AdsCubit>().showRewardedAd(
                                        onRewardEarned: () {
                                          context
                                              .read<RewardCubit>()
                                              .claimReward(stepGoal);
                                          context.read<CoinCubit>().addCoin(
                                            rewardAmount * 2,
                                          );
                                        },
                                      );
                                    }
                                  : null,
                              icon: isAdLoaded
                                  ? const Icon(Icons.play_arrow, size: 18)
                                  : const SizedBox(
                                      width: 18,
                                      height: 18,
                                      child: CircularProgressIndicator(
                                        strokeWidth: 2,
                                        color: Colors.white,
                                      ),
                                    ),
                              label: Text(
                                isAdLoaded ? '2x with ad' : 'Loading...',
                              ),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.transparent,
                                shadowColor: Colors.transparent,
                                foregroundColor: Colors.white,
                                disabledForegroundColor: Colors.white70,
                                padding: const EdgeInsets.symmetric(
                                  vertical: 12,
                                ),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(12),
                                ),
                              ),
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                )
              else
                Text(
                  '${(stepGoal - currentSteps).clamp(0, stepGoal)} steps remaining',
                  style: TextStyle(color: Colors.grey.shade400, fontSize: 14),
                ),
            ],
          ),
        );
      },
    );
  }
}
