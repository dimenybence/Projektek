import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Game } from '../game.service';
import { GameService } from '../game.service';

@Component({
  selector: 'app-add-edit-game',
  templateUrl: './add-edit-game.component.html',
  styleUrl: './add-edit-game.component.scss'
})
export class AddEditGameComponent implements OnInit{
  newGame: Game = { id: 0, name: '', genre: '', developer: '', platform: '' };
  isEditMode = false;

  constructor(private router: Router, private gameService:GameService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    const gameId = this.route.snapshot.paramMap.get('id');
    if (gameId) {
      this.isEditMode = true;
      this.newGame = this.gameService.getGameDetails(gameId);
    }
  }

  onSubmit(): void {
    if (this.isEditMode) {
      this.gameService.updateGame(this.newGame);
      this.router.navigate(['/games']);
    } else {
      this.gameService.addGame(this.newGame);
      this.router.navigate(['/games']);
    }
  }
}
