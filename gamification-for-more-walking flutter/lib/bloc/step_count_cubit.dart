import 'package:flutter_bloc/flutter_bloc.dart';
import '../data/repositories/health_repository.dart';
import 'step_count_state.dart';

/// Cubit that manages the step count state and business logic.
/// This layer does NOT contain any direct data access or UI code.
/// It communicates only with the HealthRepository layer.
class StepCountCubit extends Cubit<StepCountState> {
  final HealthRepository _repository;

  StepCountCubit({required HealthRepository repository})
    : _repository = repository,
      super(const StepCountInitial());

  /// Initialize the health repository
  Future<void> initialize() async {
    try {
      await _repository.initialize();
    } catch (e) {
      emit(StepCountError('Failed to initialize: $e'));
    }
  }

  /// Check and request permissions, then fetch step count
  Future<void> fetchSteps() async {
    try {
      emit(const StepCountLoading());

      // Check if we have permissions
      final hasPermissions = await _repository.hasPermissions();

      if (!hasPermissions) {
        // Request permissions
        final granted = await _repository.requestPermissions();

        if (!granted) {
          emit(
            const StepCountPermissionDenied(
              'Permission denied. Please grant access to Health data in your device settings.',
            ),
          );
          return;
        }
      }

      // Fetch the step count
      final steps = await _repository.getTodayStepCount();
      emit(StepCountLoaded(steps));
    } catch (e) {
      emit(StepCountError('Failed to fetch steps: $e'));
    }
  }

  /// Refresh step count (useful for pull-to-refresh)
  Future<void> refreshSteps() async {
    try {
      // Check permissions first
      final hasPermissions = await _repository.hasPermissions();
      if (!hasPermissions) {
        emit(
          const StepCountPermissionDenied(
            'Permission denied. Please grant access to Health data.',
          ),
        );
        return;
      }

      // Fetch fresh step count
      final steps = await _repository.getTodayStepCount();
      emit(StepCountLoaded(steps));
    } catch (e) {
      // On error, emit error state
      emit(StepCountError('Failed to refresh steps: $e'));
    }
  }

  @override
  Future<void> close() {
    // Clean up resources when the cubit is closed
    return super.close();
  }
}
