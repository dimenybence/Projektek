import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageService } from '../services/local-storage.service';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent implements OnInit {
  loggedIn: boolean = false;
  isAdmin: boolean = false;

  constructor(private router: Router, private localStorageService: LocalStorageService) {  }

  ngOnInit(): void {
    const user = this.localStorageService.getItem('user');
    if (user) {
      this.loggedIn = true;
      this.isAdmin = user.email === 'login@admin.com';
    }
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  navigateToProfile(): void {
    this.router.navigate(['/profile']);
  }

  navigateToCart(): void {
    this.router.navigate(['/cart']);
  }

  logOut(): void {
    this.localStorageService.removeItem('user');
    this.loggedIn = false;
    this.isAdmin = false;
    this.router.navigate(['/']);
  }
}
