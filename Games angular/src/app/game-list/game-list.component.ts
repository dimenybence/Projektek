import { Component, OnInit } from '@angular/core';
import { Game, GameService } from '../game.service';

@Component({
  selector: 'app-game-list',
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.scss']
})
export class GameListComponent implements OnInit {
  games: Game[] = [];
  selectedGame: Game =
    { id: 0, name: '', genre: '', developer: '', platform: '' };
  selectedGameIndex: number | null = null;

  isListView: boolean = true;

  constructor(private gameService: GameService) { }

  ngOnInit(): void {
    this.games = this.gameService.getGames();
  }

  toggleView(): void {
    this.isListView = !this.isListView;
  }
}

