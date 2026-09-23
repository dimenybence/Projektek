import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/ads_cubit.dart';
import 'package:walking_buddy/presentation/widgets/shop/shop_item_card.dart';
import 'package:walking_buddy/presentation/widgets/shop/wallet_card.dart';

class ShopPage extends StatefulWidget {
  const ShopPage({super.key});

  @override
  State<ShopPage> createState() => _ShopPageState();
}

class _ShopPageState extends State<ShopPage> {
  @override
  void initState() {
    super.initState();
    context.read<AdsCubit>().loadRewardedAd();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          child: Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const WalletCard(),
                const SizedBox(height: 24),
                const Text(
                  'Available foods',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: Colors.white70,
                  ),
                ),
                const SizedBox(height: 16),
                const ShopItemCard(
                  name: 'Apple',
                  emoji: '🍎',
                  price: 50,
                  boostPercentage: 10,
                ),
                const ShopItemCard(
                  name: 'Banana',
                  emoji: '🍌',
                  price: 60,
                  boostPercentage: 12,
                ),
                const ShopItemCard(
                  name: 'Carrot',
                  emoji: '🥕',
                  price: 40,
                  boostPercentage: 8,
                ),
                const ShopItemCard(
                  name: 'Orange',
                  emoji: '🍊',
                  price: 55,
                  boostPercentage: 11,
                ),
                const ShopItemCard(
                  name: 'Grapes',
                  emoji: '🍇',
                  price: 70,
                  boostPercentage: 15,
                ),
                const ShopItemCard(
                  name: 'Watermelon',
                  emoji: '🍉',
                  price: 80,
                  boostPercentage: 18,
                ),
                const ShopItemCard(
                  name: 'Avocado',
                  emoji: '🥑',
                  price: 90,
                  boostPercentage: 20,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
