import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:sembast/sembast_io.dart';

class RewardDao {
  RewardDao._privateConstructor();
  static final RewardDao instance = RewardDao._privateConstructor();

  Database? _database;

  static const String _storeKey = 'rewards';
  static const int _recordKey = 1;

  final StoreRef<int, Map<String, dynamic>> _store = intMapStoreFactory.store(
    _storeKey,
  );

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  Future<Database> _initDatabase() async {
    final dir = await getApplicationDocumentsDirectory();
    await dir.create(recursive: true);
    final path = join(dir.path, 'reward_database.db');
    return databaseFactoryIo.openDatabase(path);
  }

  DateTime _day(DateTime d) => DateTime(d.year, d.month, d.day);

  String _dayKey(DateTime d) => _day(d).toIso8601String();

  Future<Map<String, List<int>>> _loadMap() async {
    final db = await database;
    final data = await _store.record(_recordKey).get(db);

    if (data == null) return {};

    return data.map((k, v) => MapEntry(k, List<int>.from(v as List)));
  }

  Future<void> _saveMap(Map<String, List<int>> map) async {
    final db = await database;
    await _store.record(_recordKey).put(db, map);
  }

  Future<bool> isRewardAvailableToday(int rewardId) async {
    final map = await _loadMap();
    final key = _dayKey(DateTime.now());

    if (!map.containsKey(key)) return true;
    return !map[key]!.contains(rewardId);
  }

  Future<void> confirmRewardToday(int rewardId) async {
    final map = await _loadMap();
    final key = _dayKey(DateTime.now());

    final rewards = map[key]?.toSet() ?? <int>{};
    rewards.add(rewardId);

    map[key] = rewards.toList();
    await _saveMap(map);
  }

  Future<void> clearAll() async {
    final db = await database;
    await _store.record(_recordKey).delete(db);
  }

  Future<void> close() async {
    await _database?.close();
    _database = null;
  }
}
