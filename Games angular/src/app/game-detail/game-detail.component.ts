import { Component, OnInit } from '@angular/core';
import { GameService } from '../game.service';
import { Game } from '../game.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-game-detail',
  templateUrl: './game-detail.component.html',
  styleUrl: './game-detail.component.scss'
})
export class GameDetailComponent implements OnInit {
  game: Game = { id: 0, name: '', genre: '', developer: '', platform: '' };

  constructor(private route:ActivatedRoute, private gameService: GameService, private router: Router) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.game = this.gameService.getGameDetails(id);
  }

  isImageExists(gameName: string): boolean{
    const imgPath = `assets/${gameName}.jpg`;
    const http = new XMLHttpRequest();
    http.open('HEAD', imgPath, false);
    http.send();
    return http.status !== 404;
  }

  showConfirmationModal(): void {
    const modal = document.getElementById('confirmDeleteModal');
    if (modal) {
      modal.style.display = 'block';
    }
  }

  cancelDelete(): void {
    const modal = document.getElementById('confirmDeleteModal');
    if(modal) {
      modal.style.display = 'none';
    }
  }

  deleteGame(): void {
    this.gameService.deleteGame(this.game.id);
    this.router.navigate(['/games']);
  }
}
