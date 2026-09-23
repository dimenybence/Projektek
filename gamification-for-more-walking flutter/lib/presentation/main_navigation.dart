import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/coin_cubit.dart';
import 'package:walking_buddy/bloc/reward_cubit.dart';
import 'package:walking_buddy/bloc/step_count_cubit.dart';
import 'package:walking_buddy/bloc/ads_cubit.dart';
import 'package:walking_buddy/data/repositories/coin_repository.dart';
import 'package:walking_buddy/data/repositories/health_repository.dart';
import 'package:walking_buddy/data/repositories/reward_repository.dart';
import 'package:walking_buddy/data/repositories/ads_repository.dart';
import 'package:walking_buddy/bloc/navigation_cubit.dart';
import 'package:walking_buddy/bloc/navigation_state.dart';
import 'package:walking_buddy/bloc/fridge_cubit.dart';
import 'package:walking_buddy/data/repositories/fridge_repository.dart';
import 'package:walking_buddy/presentation/pages/home_page.dart';
import 'package:walking_buddy/presentation/pages/awards_page.dart';
import 'package:walking_buddy/presentation/pages/shop_page.dart';
import 'package:walking_buddy/presentation/widgets/custom_app_bar.dart';
import 'package:walking_buddy/presentation/widgets/home/fridge_modal.dart';
import 'package:walking_buddy/data/models/pet_satiety.dart';

class MainNavigation extends StatefulWidget {
  const MainNavigation({super.key});

  @override
  State<MainNavigation> createState() => _MainNavigationState();
}

class _MainNavigationState extends State<MainNavigation> {
  static final List<Widget> _pages = [
    BlocProvider(
      create: (context) =>
          StepCountCubit(repository: context.read<HealthRepository>()),
      child: const HomePage(),
    ),
    MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (context) =>
              StepCountCubit(repository: context.read<HealthRepository>()),
        ),
        BlocProvider(
          create: (context) => RewardCubit(context.read<RewardRepository>()),
        ),
        BlocProvider(
          create: (context) =>
              AdsCubit(repository: context.read<AdsRepository>()),
        ),
      ],
      child: const AwardsPage(),
    ),
    BlocProvider(
      create: (context) => AdsCubit(repository: context.read<AdsRepository>()),
      child: const ShopPage(),
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(create: (context) => FridgeCubit(FridgeRepository())),
        BlocProvider(
          create: (context) => CoinCubit(context.read<CoinRepository>()),
        ),
      ],
      child: BlocBuilder<NavigationCubit, NavigationState>(
        builder: (context, state) {
          return Scaffold(
            appBar: CustomAppBar(currentPageIndex: state.selectedIndex),
            body: _pages[state.selectedIndex],
            floatingActionButton: state.selectedIndex == 0
                ? Container(
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.purple.withValues(alpha: 0.3),
                          blurRadius: 20,
                          spreadRadius: 5,
                        ),
                        BoxShadow(
                          color: Colors.pinkAccent.withValues(alpha: 0.1),
                          blurRadius: 30,
                          spreadRadius: 10,
                        ),
                      ],
                    ),
                    child: Container(
                      decoration: BoxDecoration(
                        gradient: const LinearGradient(
                          colors: [
                            Colors.purple,
                            Colors.purpleAccent,
                            Colors.pinkAccent,
                          ],
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                        ),
                        borderRadius: BorderRadius.circular(16),
                      ),
                      child: FloatingActionButton(
                        onPressed: () async {
                          final petSatiety = context.read<PetSatiety>();
                          final initialSatiety = petSatiety.satiety;
                          await showDialog(
                            context: context,
                            builder: (dialogContext) => BlocProvider.value(
                              value: context.read<FridgeCubit>(),
                              child: const FridgeModal(),
                            ),
                          );
                          if (petSatiety.satiety > initialSatiety) {
                            petSatiety.triggerEating();
                          }
                        },
                        backgroundColor: Colors.transparent,
                        elevation: 0,
                        child: const Icon(
                          Icons.kitchen_outlined,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  )
                : null,
            bottomNavigationBar: Theme(
              data: Theme.of(context).copyWith(
                navigationBarTheme: NavigationBarThemeData(
                  indicatorColor: Colors.transparent,
                  elevation: 8,
                  height: 80,
                  labelBehavior: NavigationDestinationLabelBehavior.alwaysShow,
                ),
                splashFactory: NoSplash.splashFactory,
                highlightColor: Colors.transparent,
              ),
              child: NavigationBar(
                selectedIndex: state.selectedIndex,
                onDestinationSelected: (int index) {
                  context.read<NavigationCubit>().navigateToPage(index);
                },
                destinations: [
                  _buildDestination(
                    icon: Icons.home_outlined,
                    selectedIcon: Icons.home,
                    label: 'Home',
                    isSelected: state.selectedIndex == 0,
                  ),
                  _buildDestination(
                    icon: Icons.emoji_events_outlined,
                    selectedIcon: Icons.emoji_events,
                    label: 'Awards',
                    isSelected: state.selectedIndex == 1,
                  ),
                  _buildDestination(
                    icon: Icons.shopping_bag_outlined,
                    selectedIcon: Icons.shopping_bag,
                    label: 'Shop',
                    isSelected: state.selectedIndex == 2,
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  NavigationDestination _buildDestination({
    required IconData icon,
    required IconData selectedIcon,
    required String label,
    required bool isSelected,
  }) {
    return NavigationDestination(
      icon: _AnimatedIndicatorIcon(icon: icon, isSelected: isSelected),
      selectedIcon: _AnimatedIndicatorIcon(
        icon: selectedIcon,
        isSelected: true,
      ),
      label: label,
    );
  }
}

class _AnimatedIndicatorIcon extends StatelessWidget {
  final IconData icon;
  final bool isSelected;

  const _AnimatedIndicatorIcon({required this.icon, required this.isSelected});

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeInOut,
          height: 3,
          width: isSelected ? 40 : 0,
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [Colors.purple, Colors.purpleAccent, Colors.pinkAccent],
              begin: Alignment.centerLeft,
              end: Alignment.centerRight,
            ),
            borderRadius: BorderRadius.circular(2),
          ),
        ),
        const SizedBox(height: 8),
        Icon(icon),
      ],
    );
  }
}
