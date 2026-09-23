import 'dart:math';
import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:sembast/sembast_io.dart';

class SatietyDao {
  // Private constructor for singleton pattern
  SatietyDao._privateConstructor();
  static final SatietyDao instance = SatietyDao._privateConstructor();

  // Database instance
  Database? _database;

  // Key for storing the satiety value
  static const String _storeKey = 'satiety_store';
  static const int _recordKey = 0; // using a single record

  final StoreRef<int, Map<String, dynamic>> _satietyStore = intMapStoreFactory
      .store(_storeKey);

  /// Initialize and open the database
  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  /// Initialize the database
  Future<Database> _initDatabase() async {
    final appDocDir = await getApplicationDocumentsDirectory();
    await appDocDir.create(recursive: true);
    final dbPath = join(appDocDir.path, 'satiety_database.db');
    final database = await databaseFactoryIo.openDatabase(dbPath);
    return database;
  }

  Future<int> getSatiety() async {
    final db = await database;
    final record = await _satietyStore.record(_recordKey).get(db);
    return record != null
        ? record['value'] as int
        : 100; // Default to 100 (full satiety)
  }

  Future<void> saveSatiety(int value) async {
    final db = await database;
    // Ensure value is within reasonable bounds if needed, e.g., 0 to 100
    final newValue = max(0, min(100, value));
    await _satietyStore.record(_recordKey).put(db, {'value': newValue});
  }
}
