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
    final List<int> milestones = [1000, 2500, 5000, 10000, 15000, 20000];
    final List<int> displayMilestones = [0, ...milestones];

    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.blue.withValues(alpha: 0.2),
        borderRadius: BorderRadius.circular(24),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.2),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
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
          final int currentSteps = (state is StepCountLoaded) ? state.steps : 0;
          final bool isCompleted = currentSteps >= milestones.last;

          return Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Header with Icon and Steps Count
              Row(
                children: [
                  Container(
                    width: 60,
                    height: 60,
                    decoration: BoxDecoration(
                      color: isCompleted ? Colors.amber : Colors.cyan,
                      borderRadius: BorderRadius.circular(16),
                    ),
                    child: isCompleted
                        ? const Center(
                            child: Text('🏆', style: TextStyle(fontSize: 32)),
                          )
                        : const Icon(
                            Icons.directions_walk,
                            color: Colors.white,
                            size: 32,
                          ),
                  ),
                  const SizedBox(width: 16),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Daily Steps',
                        style: TextStyle(color: Colors.grey, fontSize: 16),
                      ),
                      Text(
                        '$currentSteps',
                        style: TextStyle(
                          color: Colors.white,
                          fontSize: 40,
                          fontWeight: FontWeight.bold,
                          height: 1.0,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
              if (!isCompleted) ...[
                const SizedBox(height: 24),
                // Next Milestone Text
                Row(
                  children: [
                    const Text(
                      'Next Milestone: ',
                      style: TextStyle(color: Colors.grey, fontSize: 16),
                    ),
                    Text(
                      _getNextMilestoneLabel(currentSteps, milestones),
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ],
              const SizedBox(height: 20),

              // Milestones Progress
              LayoutBuilder(
                builder: (context, constraints) {
                  final double totalWidth = constraints.maxWidth;
                  final double availableWidth = totalWidth - 16;
                  final double progress = _calculateProgress(
                    currentSteps,
                    displayMilestones,
                  );

                  double lineWidth = availableWidth * progress;

                  for (int i = 0; i < displayMilestones.length - 1; i++) {
                    if (currentSteps >= displayMilestones[i] &&
                        currentSteps < displayMilestones[i + 1]) {
                      final double segmentWidth =
                          availableWidth / (displayMilestones.length - 1);
                      final double nextMilestonePos = (i + 1) * segmentWidth;
                      if (lineWidth > nextMilestonePos - 12) {
                        lineWidth = nextMilestonePos - 12;
                      }
                      break;
                    }
                  }

                  return Stack(
                    children: [
                      // Line Layer
                      Positioned(
                        top: 7,
                        left: 8,
                        right: 8,
                        child: Stack(
                          children: [
                            // Background Line
                            Container(
                              height: 2,
                              color: Colors.grey.withValues(alpha: 0.3),
                            ),
                            // Progress Line
                            Container(
                              height: 2,
                              width: lineWidth,
                              color: Colors.cyan,
                            ),
                          ],
                        ),
                      ),
                      // Milestones
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: displayMilestones.map((milestone) {
                          if (milestone == 0) {
                            return const SizedBox(width: 16, height: 16);
                          }
                          return _buildMilestoneDot(
                            _formatMilestone(milestone),
                            currentSteps >= milestone,
                          );
                        }).toList(),
                      ),
                    ],
                  );
                },
              ),
            ],
          );
        },
      ),
    );
  }

  String _getNextMilestoneLabel(int currentSteps, List<int> milestones) {
    if (currentSteps >= milestones.last) {
      return 'Completed';
    }
    for (final milestone in milestones) {
      if (currentSteps < milestone) {
        return '$milestone';
      }
    }
    return '${milestones.last}';
  }

  String _formatMilestone(int milestone) {
    if (milestone >= 1000) {
      double value = milestone / 1000;
      if (value == value.toInt()) {
        return '${value.toInt()}K';
      }
      String val = value.toStringAsFixed(2);
      if (val.endsWith('0')) val = val.substring(0, val.length - 1);
      if (val.endsWith('0')) val = val.substring(0, val.length - 1);
      if (val.endsWith('.')) val = val.substring(0, val.length - 1);
      return '${val}K';
    }
    return '$milestone';
  }

  double _calculateProgress(int currentSteps, List<int> milestones) {
    if (currentSteps < milestones.first) return 0.0;
    if (currentSteps >= milestones.last) return 1.0;

    for (int i = 0; i < milestones.length - 1; i++) {
      final start = milestones[i];
      final end = milestones[i + 1];
      if (currentSteps >= start && currentSteps < end) {
        final double segmentProgress = (currentSteps - start) / (end - start);
        final double segmentWidth = 1.0 / (milestones.length - 1);
        return (i * segmentWidth) + (segmentProgress * segmentWidth);
      }
    }
    return 0.0;
  }

  Widget _buildMilestoneDot(String label, bool isCompleted) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 16,
          height: 16,
          decoration: BoxDecoration(
            color: isCompleted
                ? Colors.cyan
                : Colors.grey.withValues(alpha: 0.3),
            shape: BoxShape.circle,
            border: Border.all(
              color: isCompleted
                  ? Colors.cyan
                  : Colors.black.withValues(alpha: 0.3),
              width: 2,
            ),
          ),
        ),
        const SizedBox(height: 8),
        Text(
          label,
          style: TextStyle(
            color: isCompleted
                ? Colors.white
                : Colors.grey.withValues(alpha: 0.7),
            fontSize: 12,
            fontWeight: isCompleted ? FontWeight.bold : FontWeight.normal,
          ),
        ),
      ],
    );
  }
}
