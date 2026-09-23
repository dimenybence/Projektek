import 'package:health/health.dart';
import 'package:permission_handler/permission_handler.dart';

/// Repository layer that handles all health-related data operations.
/// This repository manages platform-specific logic using the health package.
class HealthRepository {
  final Health _health;

  // Define the health data types we want to request access to
  static final List<HealthDataType> _types = [HealthDataType.STEPS];

  // Define the permissions (read access for steps)
  static final List<HealthDataAccess> _permissions = [HealthDataAccess.READ];

  HealthRepository({Health? health}) : _health = health ?? Health();

  /// Configure the health plugin
  Future<void> initialize() async {
    await _health.configure();
  }

  /// Request necessary permissions to access step count data
  /// Returns true if permissions are granted, false otherwise
  Future<bool> requestPermissions() async {
    try {
      // First, request activity recognition permission (required on Android)
      final activityStatus = await Permission.activityRecognition.request();
      if (activityStatus.isDenied || activityStatus.isPermanentlyDenied) {
        return false;
      }

      // Then request health data permissions
      final bool? hasPermissions = await _health.hasPermissions(
        _types,
        permissions: _permissions,
      );

      if (hasPermissions == true) {
        return true;
      }

      // Request authorization
      final bool authorized = await _health.requestAuthorization(
        _types,
        permissions: _permissions,
      );
      return authorized;
    } catch (e) {
      throw Exception('Failed to request permissions: $e');
    }
  }

  /// Check if we have the necessary permissions
  Future<bool> hasPermissions() async {
    try {
      final activityStatus = await Permission.activityRecognition.status;
      if (!activityStatus.isGranted) {
        return false;
      }

      final bool? hasHealthPermissions = await _health.hasPermissions(
        _types,
        permissions: _permissions,
      );
      return hasHealthPermissions ?? false;
    } catch (e) {
      throw Exception('Failed to check permissions: $e');
    }
  }

  /// Query the daily total step count for the current day
  /// Returns the step count as an integer, or throws an exception on error
  Future<int> getTodayStepCount() async {
    try {
      // Get start and end of today
      final now = DateTime.now();
      final midnight = DateTime(now.year, now.month, now.day);

      // Use the getTotalStepsInInterval method for accurate daily step count
      final int? steps = await _health.getTotalStepsInInterval(midnight, now);

      return steps ?? 0;
    } catch (e) {
      throw Exception('Failed to fetch step count: $e');
    }
  }

  /// Get detailed step data points for today (optional - for more granular data)
  Future<List<HealthDataPoint>> getTodayStepDataPoints() async {
    try {
      final now = DateTime.now();
      final midnight = DateTime(now.year, now.month, now.day);

      final List<HealthDataPoint> healthData = await _health
          .getHealthDataFromTypes(
            types: _types,
            startTime: midnight,
            endTime: now,
          );

      return healthData;
    } catch (e) {
      throw Exception('Failed to fetch step data points: $e');
    }
  }

  /// Clean up resources
  Future<void> dispose() async {
    // Health plugin doesn't require explicit disposal
  }
}
