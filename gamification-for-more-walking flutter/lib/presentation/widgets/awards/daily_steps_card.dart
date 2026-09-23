import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/step_count_cubit.dart';
import 'package:walking_buddy/bloc/step_count_state.dart';

class DailyStepsCard extends StatefulWidget {
  const DailyStepsCard({super.key});

  @override
  State<DailyStepsCard> createState() => _DailyStepsCardState();
}

class _DailyStepsCardState extends State<DailyStepsCard> {
  @override
  void initState() {
    super.initState();
    // Initialize and fetch steps when the widget is created
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<StepCountCubit>().initialize();
      context.read<StepCountCubit>().fetchSteps();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 20),
      decoration: BoxDecoration(
        color: Colors.deepPurple.shade700.withValues(alpha: 0.4),
        borderRadius: BorderRadius.circular(28),
      ),
      child: RefreshIndicator(
        onRefresh: () => context.read<StepCountCubit>().refreshSteps(),
        child: BlocConsumer<StepCountCubit, StepCountState>(
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
            return Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                if (state is StepCountLoaded)
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Daily steps',
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 18,
                          fontWeight: FontWeight.w400,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        state.steps.toString(),
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 48,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                const Text('👟', style: TextStyle(fontSize: 60)),
              ],
            );
          },
        ),
      ),
    );
  }
}
