import { Injectable } from '@angular/core';

export interface Game {
  id: number,
  name: string,
  genre: string,
  developer: string,
  platform: string
}

@Injectable({
  providedIn: 'root'
})
export class GameService {

  games: Game[] = 
    [
      // { id: 1, name: 'Witcher III', genre: 'Adventure', developer: 'CD Projekt Red', platform: 'PC, PS4, XBOX' },
      // { id: 2, name: 'God of War', genre: 'Action', developer: 'Santa Monica Studio', platform: 'PS4'},
      // { id: 3, name: 'Grand Theft Auto V', genre: 'Action', developer: 'Rockstar Games', platform: 'PC, PS4, XBOX' },
      // { id: 4, name: 'EA Sports FC 24', genre: 'Sport', developer: 'EA Sports', platform: 'PC, PS4, XBOX' }
    ];

  constructor() {
    this.loadGamesFromLocalStorage();
   }

  loadGamesFromLocalStorage(): void {
    const storedGames = localStorage.getItem('games');
    if (storedGames) {
      this.games = JSON.parse(storedGames);
    }  
  }

  saveGamesToLocalStorage(): void {
    localStorage.setItem('games', JSON.stringify(this.games));
  }

  getGames(): Game[] {
    return this.games;
  }

  getGameById(id: number): Game {
    return this.games.find(game => game.id === id) || { id: 0, name: '', genre: '', developer: '', platform: '' };
  }

  addGame(game: Game): void {
    this.games.push(game);
    this.games.forEach((game, index) => game.id = index + 1);
    this.saveGamesToLocalStorage();
  }
  
  getGameDetails(id: string | null): Game {
    return this.games.find(game => game.id === Number(id)) || { id: 0, name: '', genre: '', developer: '', platform: '' };
  }

  updateGame(updatedGame: Game) {
    const gameId = this.games.findIndex(game => game.id === updatedGame.id);
    this.games[gameId] = updatedGame;
    this.saveGamesToLocalStorage();
  }

  deleteGame(id: number) {
    const gameId = this.games.findIndex(game => game.id === id);
    this.games.splice(gameId, 1);
    this.saveGamesToLocalStorage();
  }
}
