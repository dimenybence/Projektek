import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/ads_cubit.dart';
import 'package:walking_buddy/bloc/reward_config.dart';
import 'package:walking_buddy/bloc/reward_cubit.dart';
import 'package:walking_buddy/bloc/reward_state.dart';
import 'package:walking_buddy/bloc/step_count_cubit.dart';
import 'package:walking_buddy/bloc/step_count_state.dart';
import 'package:walking_buddy/presentation/widgets/awards/daily_steps_card.dart';
import 'package:walking_buddy/presentation/widgets/awards/reward_card.dart';

class AwardsPage extends StatefulWidget {
  const AwardsPage({super.key});

  @override
  State<AwardsPage> createState() => _AwardsPageState();
}

class _AwardsPageState extends State<AwardsPage> {
  @override
  void initState() {
    super.initState();
    // Load rewarded ad when entering the page
    context.read<AdsCubit>().loadRewardedAd();
  }

  @override
  Widget build(BuildContext context) {
    return BlocConsumer<StepCountCubit, StepCountState>(
      listener: (context, state) {
        // Show error messages using SnackBar
        if (state is StepCountError) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(state.message),
              backgroundColor: Colors.red,
              duration: const Duration(seconds: 3),
              action: SnackBarAction(
                label: 'Retry',
                textColor: Colors.white,
                onPressed: () {
                  context.read<StepCountCubit>().fetchSteps();
                },
              ),
            ),
          );
        }
      },
      builder: (context, state) {
        final int currentSteps = (state is StepCountLoaded) ? state.steps : 0;

        return BlocConsumer<RewardCubit, RewardState>(
          listener: (context, state) {},
          builder: (context, state) {
            // goal is used as id
            // so you can't have more than one reward for a goal
            final availableIds = <int>{};

            for (final key in stepGoalToReward.keys) {
              if (state.claimedToday.contains(key)) continue;
              availableIds.add(key);
            }

            return Scaffold(
              body: SafeArea(
                child: SingleChildScrollView(
                  child: Padding(
                    padding: const EdgeInsets.all(16.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const DailyStepsCard(),
                        const SizedBox(height: 24),
                        const Text(
                          'Available rewards',
                          style: TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Colors.white70,
                          ),
                        ),
                        const SizedBox(height: 16),
                        ...availableIds.map(
                          (entry) => RewardCard(
                            stepGoal: entry,
                            rewardAmount: stepGoalToReward[entry] ?? 0,
                            currentSteps: currentSteps,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            );
          },
        );
      },
    );
  }
}
