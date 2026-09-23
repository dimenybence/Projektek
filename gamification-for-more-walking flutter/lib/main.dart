import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:google_mobile_ads/google_mobile_ads.dart';
import 'package:walking_buddy/bloc/navigation_cubit.dart';
import 'package:walking_buddy/data/repositories/coin_repository.dart';
import 'package:walking_buddy/data/repositories/health_repository.dart';
import 'package:walking_buddy/data/repositories/reward_repository.dart';
import 'package:walking_buddy/data/repositories/ads_repository.dart';
import 'package:walking_buddy/presentation/main_navigation.dart';
import 'package:provider/provider.dart';
import 'package:walking_buddy/data/models/pet_satiety.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  // Initialize Google Mobile Ads SDK
  final RequestConfiguration requestConfiguration = RequestConfiguration(
    tagForChildDirectedTreatment: TagForChildDirectedTreatment.yes,
  );
  MobileAds.instance.updateRequestConfiguration(requestConfiguration);
  await MobileAds.instance.initialize();
  runApp(
    MultiProvider(
      providers: [ChangeNotifierProvider(create: (_) => PetSatiety())],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiRepositoryProvider(
      providers: [
        RepositoryProvider(create: (context) => HealthRepository()),
        RepositoryProvider(create: (context) => RewardRepository()),
        RepositoryProvider(create: (context) => CoinRepository()),
        RepositoryProvider(create: (context) => AdsRepository()),
      ],
      child: MaterialApp(
        title: 'Walking Buddy - Three-Tier Architecture',
        debugShowCheckedModeBanner: false,
        darkTheme: ThemeData(
          colorScheme: ColorScheme.fromSeed(
            seedColor: Colors.deepPurple,
            brightness: Brightness.dark,
          ),
          useMaterial3: true,
        ),
        themeMode: ThemeMode.dark,
        home: BlocProvider(
          create: (context) => NavigationCubit(),
          child: const MainNavigation(),
        ),
      ),
    );
  }
}
