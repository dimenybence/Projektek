import 'package:flutter/foundation.dart';
import 'dart:async';
import 'dart:math';
import 'package:walking_buddy/data/dao/last_login_dao.dart';
import 'package:walking_buddy/data/dao/satiety_dao.dart';

class PetSatiety extends ChangeNotifier {
  int satiety = 51;
  bool isEating = false;
  Timer? _eatingTimer;

  /// 1-50 hungry, 51-100 not hungry
  /// 14 minutes per satiety tick, so 50 hunger drops in 11,6 hours

  PetSatiety() {
    timing();
  }

  @override
  void dispose() {
    _eatingTimer?.cancel();
    super.dispose();
  }

  void triggerEating() {
    _eatingTimer?.cancel();
    isEating = true;
    notifyListeners();
    _eatingTimer = Timer(const Duration(seconds: 3), () {
      isEating = false;
      notifyListeners();
    });
  }

  Future<void> timing() async {
    final SatietyDao satietyDao = SatietyDao.instance;
    satiety = await satietyDao.getSatiety();
    notifyListeners();
    DateTime? lastLogin = await loadLastTime();
    int timeToWait = 14;
    if (lastLogin != null) {
      final now = DateTime.now();
      final elapsed = now.difference(lastLogin);

      int fullPasses = (elapsed.inMinutes / 14).toInt();
      timeToWait = 14 - (elapsed.inMinutes % 14).toInt();

      satiety = max(1, satiety - fullPasses);

      saveLastTime(lastLogin.add(Duration(minutes: fullPasses * 14)));
      notifyListeners();
    } else {
      saveLastTime(DateTime.now());
    }
    runTimer(timeToWait);
  }

  void runTimer(int timeToWait) async {
    while (true) {
      await Future.delayed(Duration(minutes: timeToWait));
      if (satiety > 1) {
        satiety -= 1;

        saveLastTime();
        notifyListeners();
      }
      timeToWait = 14;
    }
  }

  void feed(int amount) {
    satiety = min(100, satiety + amount);
    notifyListeners();
    final SatietyDao satietyDao = SatietyDao.instance;
    satietyDao.saveSatiety(satiety);
  }

  Future<DateTime?> loadLastTime() async {
    final lastLoginDao = LastLoginDao.instance;
    DateTime? lastLogin = await lastLoginDao.getLastLogin();
    return lastLogin;
  }

  Future<void> saveLastTime([DateTime? timeToSave]) async {
    final lastLoginDao = LastLoginDao.instance;
    await lastLoginDao.setLastLogin(timeToSave ?? DateTime.now());
    final SatietyDao satietyDao = SatietyDao.instance;
    satietyDao.saveSatiety(satiety);
  }
}
