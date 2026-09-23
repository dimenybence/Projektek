import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/fridge_state.dart';
import 'package:walking_buddy/data/models/fridge_item.dart';
import 'package:walking_buddy/data/repositories/fridge_repository.dart';

class FridgeCubit extends Cubit<FridgeState> {
  final FridgeRepository _repository;

  FridgeCubit(this._repository) : super(FridgeState(items: [])) {
    loadItems();
  }

  Future<void> loadItems() async {
    try {
      emit(state.copyWith(isLoading: true));
      final items = await _repository.getFridgeItems();
      emit(FridgeState(items: items, isLoading: false));
    } catch (e) {
      emit(
        state.copyWith(isLoading: false, errorMessage: 'Failed to load items'),
      );
    }
  }

  Future<void> addItem(FridgeItem item) async {
    try {
      await _repository.addItem(item);
      await loadItems();
    } catch (e) {
      emit(state.copyWith(errorMessage: 'Failed to add item'));
    }
  }

  Future<void> consumeItem(String itemId) async {
    try {
      await _repository.consumeItem(itemId);
      await loadItems();
    } catch (e) {
      emit(state.copyWith(errorMessage: 'Failed to consume item'));
    }
  }

  Future<void> addDemoItems() async {
    try {
      await _repository.addDemoItems();
      await loadItems();
    } catch (e) {
      emit(state.copyWith(errorMessage: 'Failed to add demo items'));
    }
  }
}
