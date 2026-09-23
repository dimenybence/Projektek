import 'package:walking_buddy/data/dao/coin_dao.dart';

class CoinRepository {
  final CoinDao _coinDao;

  CoinRepository({CoinDao? coinDao}) : _coinDao = coinDao ?? CoinDao.instance;

  Future<int> getCoin() async {
    return await _coinDao.getCoin();
  }

  Future<void> addCoin(int amount) async {
    await _coinDao.addCoin(amount);
  }
}
