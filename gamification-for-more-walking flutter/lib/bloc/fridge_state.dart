import 'package:walking_buddy/data/models/fridge_item.dart';

class FridgeState {
  final List<FridgeItem> items;
  final bool isLoading;
  final String? errorMessage;

  FridgeState({required this.items, this.isLoading = false, this.errorMessage});

  FridgeState copyWith({
    List<FridgeItem>? items,
    bool? isLoading,
    String? errorMessage,
  }) {
    return FridgeState(
      items: items ?? this.items,
      isLoading: isLoading ?? this.isLoading,
      errorMessage: errorMessage ?? this.errorMessage,
    );
  }

  bool get isEmpty => items.isEmpty;
}
