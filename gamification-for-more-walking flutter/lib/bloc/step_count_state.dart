import 'package:equatable/equatable.dart';

/// Base class for all step count states
/// Using Equatable for value equality comparisons
abstract class StepCountState extends Equatable {
  const StepCountState();

  @override
  List<Object> get props => [];
}

/// Initial state when the step counter hasn't been initialized yet
class StepCountInitial extends StepCountState {
  const StepCountInitial();
}

/// Loading state when fetching step count data
class StepCountLoading extends StepCountState {
  const StepCountLoading();
}

/// Success state with the current step count
class StepCountLoaded extends StepCountState {
  final int steps;

  const StepCountLoaded(this.steps);

  @override
  List<Object> get props => [steps];
}

/// State when permissions are denied
class StepCountPermissionDenied extends StepCountState {
  final String message;

  const StepCountPermissionDenied(this.message);

  @override
  List<Object> get props => [message];
}

/// Error state when something goes wrong
class StepCountError extends StepCountState {
  final String message;

  const StepCountError(this.message);

  @override
  List<Object> get props => [message];
}
