import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:sembast/sembast_io.dart';

class LastLoginDao {
  // Private constructor for singleton pattern
  LastLoginDao._privateConstructor();
  static final LastLoginDao instance = LastLoginDao._privateConstructor();

  // Database instance
  Database? _database;

  // Store reference for key-value storage
  final StoreRef<String, dynamic> _store = stringMapStoreFactory.store(
    'user_session',
  );

  // Key for last login time
  static const String _lastLoginKey = 'last_login';

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
    final dbPath = join(appDocDir.path, 'user_session.db');
    final db = await databaseFactoryIo.openDatabase(dbPath);
    return db;
  }

  /// Get the last login time
  /// Returns null if not set
  Future<DateTime?> getLastLogin() async {
    final db = await database;
    final record = await _store.record(_lastLoginKey).get(db);
    if (record != null && record is String) {
      return DateTime.tryParse(record);
    }
    return null;
  }

  /// Set the last login time
  Future<void> setLastLogin(DateTime dateTime) async {
    final db = await database;
    await _store.record(_lastLoginKey).put(db, dateTime.toIso8601String());
  }

  /// Delete the last login time
  Future<void> deleteLastLogin() async {
    final db = await database;
    await _store.record(_lastLoginKey).delete(db);
  }

  /// Close the database
  Future<void> close() async {
    if (_database != null) {
      await _database!.close();
      _database = null;
    }
  }
}
