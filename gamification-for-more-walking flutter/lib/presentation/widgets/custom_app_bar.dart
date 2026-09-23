import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/coin_cubit.dart';
import 'package:walking_buddy/bloc/coin_state.dart';

class CustomAppBar extends StatelessWidget implements PreferredSizeWidget {
  final int currentPageIndex;

  const CustomAppBar({super.key, required this.currentPageIndex});

  @override
  Size get preferredSize => const Size.fromHeight(110);

  AppBarConfig _getAppBarConfig() {
    switch (currentPageIndex) {
      case 0: // Home
        return AppBarConfig(
          icon: Icons.directions_walk,
          backgroundColor: Colors.purple,
          title: 'PetStep',
          subtitle: 'Step & care',
        );
      case 1: // Awards
        return AppBarConfig(
          icon: Icons.emoji_events,
          backgroundColor: Colors.purpleAccent,
          title: 'Awards',
          subtitle: 'Milestones',
        );
      case 2: // Shop
        return AppBarConfig(
          icon: Icons.shopping_bag,
          backgroundColor: Colors.blue,
          title: 'Shop',
          subtitle: 'Buy food',
        );
      default:
        return AppBarConfig(
          icon: Icons.apps,
          backgroundColor: Colors.grey,
          title: 'App',
          subtitle: 'Navigation',
        );
    }
  }

  @override
  Widget build(BuildContext context) {
    final config = _getAppBarConfig();

    return BlocConsumer<CoinCubit, CoinState>(
      listener: (context, state) {},
      builder: (context, state) {
        return Container(
          height: preferredSize.height,
          padding: const EdgeInsets.fromLTRB(16, 48, 16, 12),
          child: Row(
            children: [
              // Left: Icon with colored background
              Container(
                width: 56,
                height: 56,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    colors: [
                      config.backgroundColor,
                      config.backgroundColor.withValues(alpha: 0.7),
                    ],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                  borderRadius: BorderRadius.circular(16),
                  boxShadow: [
                    BoxShadow(
                      color: config.backgroundColor.withValues(alpha: 0.3),
                      blurRadius: 8,
                      offset: const Offset(0, 2),
                    ),
                  ],
                ),
                child: Icon(config.icon, color: Colors.white, size: 32),
              ),
              const SizedBox(width: 16),
              // Middle: Title and subtitle
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      config.title,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(
                      config.subtitle,
                      style: TextStyle(
                        color: Colors.white.withValues(alpha: 0.7),
                        fontSize: 14,
                      ),
                    ),
                  ],
                ),
              ),
              // Right: Currency display
              Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 8,
                ),
                decoration: BoxDecoration(
                  gradient: const LinearGradient(
                    colors: [Colors.orange, Colors.amber],
                    begin: Alignment.centerLeft,
                    end: Alignment.centerRight,
                  ),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Container(
                      width: 24,
                      height: 24,
                      decoration: const BoxDecoration(
                        color: Colors.amberAccent,
                        shape: BoxShape.circle,
                      ),
                      child: const Center(
                        child: Icon(
                          Icons.monetization_on,
                          color: Colors.white,
                          size: 16,
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Text(
                      '${state.coin}',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}

class AppBarConfig {
  final IconData icon;
  final Color backgroundColor;
  final String title;
  final String subtitle;

  AppBarConfig({
    required this.icon,
    required this.backgroundColor,
    required this.title,
    required this.subtitle,
  });
}
