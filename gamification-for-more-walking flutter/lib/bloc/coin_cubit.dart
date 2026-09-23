import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/coin_state.dart';
import 'package:walking_buddy/data/repositories/coin_repository.dart';

class CoinCubit extends Cubit<CoinState> {
  final CoinRepository repository;

  CoinCubit(this.repository) : super(const CoinState(0)) {
    loadCoin();
  }

  Future<void> loadCoin() async {
    emit(CoinState(await repository.getCoin()));
  }

  Future<void> addCoin(int amount) async {
    await repository.addCoin(amount);
    emit(CoinState(await repository.getCoin()));
  }
}
