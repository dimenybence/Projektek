import 'package:flutter/material.dart';
import 'package:walking_buddy/presentation/widgets/home/character_status_card.dart';
import 'package:walking_buddy/presentation/widgets/home/daily_steps_card.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return const SingleChildScrollView(
      child: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          children: [
            CharacterStatusCard(),
            SizedBox(height: 16),
            DailyStepsCard(),
          ],
        ),
      ),
    );
  }
}
