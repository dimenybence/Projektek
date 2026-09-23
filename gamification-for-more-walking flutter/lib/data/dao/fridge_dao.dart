import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';
import 'package:sembast/sembast_io.dart';
import 'package:walking_buddy/data/models/fridge_item.dart';

class FridgeDao {
  // Private constructor for singleton pattern
  FridgeDao._privateConstructor();
  static final FridgeDao instance = FridgeDao._privateConstructor();

  // Database instance
  Database? _database;

  // Key for storing the fridge items
  static const String _itemsKey = 'items';

  // Store reference - using the main store for simplicity
  final StoreRef<int, Map<String, dynamic>> _itemsStore = intMapStoreFactory
      .store(_itemsKey);

  /// Initialize and open the database
  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  /// Initialize the database
  Future<Database> _initDatabase() async {
    // Get the application documents directory
    final appDocDir = await getApplicationDocumentsDirectory();

    // Ensure the directory exists
    await appDocDir.create(recursive: true);

    // Build the database path
    final dbPath = join(appDocDir.path, 'fridge_database.db');

    // Open the database
    final database = await databaseFactoryIo.openDatabase(dbPath);

    return database;
  }

  /// Read the fridge's items from the database
  /// Returns list of items (empty list if no items yet)
  Future<List<FridgeItem>> getItems() async {
    final db = await database;
    final snapshot = await _itemsStore.find(db);
    final list = snapshot.map((snap) {
      return FridgeItem.fromMap(snap.value);
    });

    return list.toList();
  }

  Future<void> addItem(FridgeItem item) async {
    await _itemsStore.add(await database, item.toMap());
  }

  Future<void> removeItem(String itemId) async {
    final db = await database;
    final finder = Finder(filter: Filter.equals('id', itemId));
    await _itemsStore.delete(db, finder: finder);
  }

  Future<void> deleteAllItems() async {
    final db = await database;
    await _itemsStore.delete(db);
  }

  /// Close the database
  Future<void> close() async {
    if (_database != null) {
      await _database!.close();
      _database = null;
    }
  }
}
