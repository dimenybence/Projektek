import 'dart:math';
import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:sembast/sembast_io.dart';

class CoinDao {
  // Private constructor for singleton pattern
  CoinDao._privateConstructor();
  static final CoinDao instance = CoinDao._privateConstructor();

  // Database instance
  Database? _database;

  // Key for storing the coin value
  static const String _storeKey = 'coin_store';
  static const int _recordKey = 0; // using a single record

  final StoreRef<int, Map<String, dynamic>> _coinStore = intMapStoreFactory
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
    final dbPath = join(appDocDir.path, 'coin_database.db');
    final database = await databaseFactoryIo.openDatabase(dbPath);
    return database;
  }

  Future<int> getCoin() async {
    final db = await database;
    final record = await _coinStore.record(_recordKey).get(db);
    return record != null ? record['value'] as int : 0;
  }

  Future<void> addCoin(int amount) async {
    final db = await database;
    final current = await getCoin();
    final newValue = max(0, current + amount);
    await _coinStore.record(_recordKey).put(db, {'value': newValue});
  }

  /// Close the database
  Future<void> close() async {
    if (_database != null) {
      await _database!.close();
      _database = null;
    }
  }
}
