import 'package:flutter/material.dart';
import 'package:walking_buddy/data/models/pet_satiety.dart';
import 'package:provider/provider.dart';

class CharacterStatusCard extends StatelessWidget {
  const CharacterStatusCard({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.deepPurple.withValues(alpha: 0.5),
        borderRadius: BorderRadius.circular(24),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          // Header
          Row(
            children: [
              const Icon(Icons.favorite_border, color: Colors.pinkAccent),
              const SizedBox(width: 8),
              const Text(
                'Satiety',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 18,
                  fontWeight: FontWeight.w500,
                ),
              ),
              const Spacer(),
              Consumer<PetSatiety>(
                builder: (context, petSatiety, _) {
                  return Text(
                    '${petSatiety.satiety}%',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 18,
                      fontWeight: FontWeight.w500,
                    ),
                  );
                },
              ),
            ],
          ),
          const SizedBox(height: 12),
          // Progress Bar
          ClipRRect(
            borderRadius: BorderRadius.circular(10),
            child: Consumer<PetSatiety>(
              builder: (context, petSatiety, _) {
                return LinearProgressIndicator(
                  value: petSatiety.satiety / 100,
                  minHeight: 10,
                  backgroundColor: Colors.black.withValues(alpha: 0.6),
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.cyanAccent),
                );
              },
            ),
          ),
          const SizedBox(height: 20),
          // Character
          SizedBox(
            height: 200,
            width: 200,
            child: Consumer<PetSatiety>(
              builder: (context, petSatiety, _) {
                String assetName;
                if (petSatiety.isEating) {
                  assetName = 'assets/Husky Eating.gif';
                } else if (petSatiety.satiety >= 51) {
                  assetName = 'assets/Husky Happy.gif';
                } else {
                  assetName = 'assets/Husky Sad.gif';
                }
                return Image.asset(assetName, fit: BoxFit.contain);
              },
            ),
          ),
          const SizedBox(height: 20),
          // Status Chip
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            decoration: BoxDecoration(
              color: Colors.black.withValues(alpha: 0.3),
              borderRadius: BorderRadius.circular(20),
            ),
            child: Consumer<PetSatiety>(
              builder: (context, petSatiety, _) {
                final isHappy = petSatiety.satiety >= 51;
                return Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      isHappy ? Icons.wb_sunny : Icons.sentiment_dissatisfied,
                      color: isHappy ? Colors.amber : Colors.blueGrey,
                      size: 20,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      isHappy ? 'Happy!' : 'Sad',
                      style: TextStyle(
                        color: isHappy ? Colors.amber : Colors.blueGrey,
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
