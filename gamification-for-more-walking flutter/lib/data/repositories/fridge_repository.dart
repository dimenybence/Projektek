import 'package:walking_buddy/data/dao/fridge_dao.dart';
import 'package:walking_buddy/data/models/fridge_item.dart';

class FridgeRepository {
  final FridgeDao _fridgeDao;

  FridgeRepository({FridgeDao? fridgeDao})
    : _fridgeDao = fridgeDao ?? FridgeDao.instance;

  Future<List<FridgeItem>> getFridgeItems() async {
    return await _fridgeDao.getItems();
  }

  Future<void> addItem(FridgeItem item) async {
    await _fridgeDao.addItem(item);
  }

  Future<void> consumeItem(String itemId) async {
    await _fridgeDao.removeItem(itemId);
  }

  // Helper method to add demo items
  Future<void> addDemoItems() async {
    final demoItems = [
      FridgeItem(id: '1', name: 'Banana', emoji: '🍌', boostPercentage: 12),
      FridgeItem(id: '2', name: 'Apple', emoji: '🍎', boostPercentage: 10),
      FridgeItem(id: '3', name: 'Cookie', emoji: '🍪', boostPercentage: 15),
    ];

    for (var item in demoItems) {
      await addItem(item);
    }
  }
}
