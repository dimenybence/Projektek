import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:walking_buddy/bloc/navigation_state.dart';

class NavigationCubit extends Cubit<NavigationState> {
  NavigationCubit() : super(NavigationState(selectedIndex: 0));

  void navigateToPage(int pageIndex) {
    emit(state.copyWith(selectedIndex: pageIndex));
  }
}
