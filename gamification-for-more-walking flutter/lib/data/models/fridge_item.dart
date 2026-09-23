class FridgeItem {
  final String id;
  final String name;
  final String emoji;
  final int boostPercentage;

  FridgeItem({
    required this.id,
    required this.name,
    required this.emoji,
    required this.boostPercentage,
  });

  Map<String, dynamic> toMap() => {
    'id': id,
    'name': name,
    'emoji': emoji,
    'boostPercentage': boostPercentage,
  };

  static FridgeItem fromMap(Map<String, dynamic> map) => FridgeItem(
    id: map['id'] as String,
    name: map['name'] as String,
    emoji: map['emoji'] as String,
    boostPercentage: map['boostPercentage'] as int,
  );

  FridgeItem copyWith({
    String? id,
    String? name,
    String? emoji,
    int? boostPercentage,
  }) {
    return FridgeItem(
      id: id ?? this.id,
      name: name ?? this.name,
      emoji: emoji ?? this.emoji,
      boostPercentage: boostPercentage ?? this.boostPercentage,
    );
  }
}
