import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { User } from '../classes/user';
import { UserService } from '../services/user.service';
import { LocalStorageService } from '../services/local-storage.service';
import { NavBarComponent } from '../nav-bar/nav-bar.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, NavBarComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  user: User = new User(0, '', '', '');

  constructor( private userService: UserService, private localStorageService: LocalStorageService, private router: Router ) { }

  ngOnInit(): void {
    const storedUser = this.localStorageService.getItem('user');
    if (storedUser && storedUser.id) {
      this.userService.getUserById(storedUser.id).subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => {
          console.error('Error loading user data:', err);
          alert('Failed to load user data.');
        }
      });
    } else {
      alert('User not logged in.');
      this.router.navigate(['/login']);
    }
  }

  updateProfile(): void {
    this.userService.updateUser(this.user).subscribe({
      next: () => {
        this.localStorageService.setItem('user', this.user);
        console.log('User updated');
        alert('Felhasználó frissítve');
      },
      error: (error) => {
        console.error('Error updating user:', error);
        alert(`Error updating user. Error: ${error.message}`);
      }
    });
  }

  navigateToHome(): void {
    this.router.navigate(['/']);
  }
}
