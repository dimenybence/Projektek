import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/coin_cubit.dart';
import 'package:walking_buddy/bloc/coin_state.dart';
import 'package:walking_buddy/bloc/fridge_cubit.dart';
import 'package:walking_buddy/bloc/fridge_state.dart';
import 'package:walking_buddy/data/models/fridge_item.dart';

class ShopItemCard extends StatelessWidget {
  final String name;
  final String emoji;
  final int price;
  final int boostPercentage;

  const ShopItemCard({
    super.key,
    required this.name,
    required this.emoji,
    required this.price,
    required this.boostPercentage,
  });

  @override
  Widget build(BuildContext context) {
    return BlocConsumer<FridgeCubit, FridgeState>(
      listener: (context, state) {},
      builder: (context, fridgeState) {
        return BlocConsumer<CoinCubit, CoinState>(
          listener: (context, state) {},
          builder: (context, state) {
            final bool canAfford = state.coin >= price;

            return Container(
              margin: const EdgeInsets.only(bottom: 16),
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.blueGrey.shade900,
                borderRadius: BorderRadius.circular(20),
                border: Border.all(color: Colors.white.withValues(alpha: 0.05)),
              ),
              child: Row(
                children: [
                  Container(
                    width: 60,
                    height: 60,
                    decoration: BoxDecoration(
                      color: Colors.white.withValues(alpha: 0.05),
                      borderRadius: BorderRadius.circular(16),
                    ),
                    alignment: Alignment.center,
                    child: Text(emoji, style: const TextStyle(fontSize: 32)),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          name,
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Row(
                          children: [
                            const Icon(
                              Icons.monetization_on,
                              color: Colors.amber,
                              size: 16,
                            ),
                            const SizedBox(width: 4),
                            Text(
                              '$price',
                              style: const TextStyle(
                                color: Colors.amber,
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(width: 12),
                            const Icon(
                              Icons.auto_awesome,
                              color: Colors.greenAccent,
                              size: 16,
                            ),
                            const SizedBox(width: 4),
                            Text(
                              '+$boostPercentage%',
                              style: const TextStyle(
                                color: Colors.greenAccent,
                                fontSize: 16,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  Container(
                    decoration: BoxDecoration(
                      color: canAfford ? null : Colors.grey.shade400,
                      gradient: canAfford
                          ? const LinearGradient(
                              colors: [Colors.cyanAccent, Colors.cyan],
                              begin: Alignment.topLeft,
                              end: Alignment.bottomRight,
                            )
                          : null,
                      borderRadius: BorderRadius.circular(12),
                      boxShadow: canAfford
                          ? [
                              BoxShadow(
                                color: Colors.cyan.withValues(alpha: 0.3),
                                blurRadius: 8,
                                offset: const Offset(0, 4),
                              ),
                            ]
                          : [],
                    ),
                    child: ElevatedButton(
                      onPressed: canAfford
                          ? () {
                              final String id = DateTime.now()
                                  .millisecondsSinceEpoch
                                  .toString();

                              context.read<CoinCubit>().addCoin(-price);
                              context.read<FridgeCubit>().addItem(
                                FridgeItem(
                                  id: id,
                                  name: name,
                                  emoji: emoji,
                                  boostPercentage: boostPercentage,
                                ),
                              );
                            }
                          : null,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.transparent,
                        shadowColor: Colors.transparent,
                        foregroundColor: canAfford
                            ? Colors.white
                            : Colors.grey.shade700,
                        padding: const EdgeInsets.symmetric(
                          horizontal: 24,
                          vertical: 12,
                        ),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        disabledBackgroundColor: Colors.transparent,
                        disabledForegroundColor: Colors.grey.shade700,
                      ),
                      child: Text(
                        canAfford ? 'Buy' : "Can't buy",
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 16,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }
}
