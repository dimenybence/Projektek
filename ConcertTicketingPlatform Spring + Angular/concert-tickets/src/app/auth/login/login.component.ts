import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { CommonModule } from '@angular/common';
import { LocalStorageService } from '../../services/local-storage.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  constructor(private router: Router, private loginService: LoginService, private localStorageService: LocalStorageService) { }

  onSubmit(form: any): void {
    if (form.valid) {
      if (this.email === 'login@admin.com' && this.password === 'password') {
        console.log('Admin bejelentkezve');
        this.localStorageService.setItem('user', { email: this.email });
        this.router.navigate(['/admin']);
      }
      else {
        this.loginService.login(this.email, this.password).subscribe({
          next: response => {
            console.log('Bejelentkezés sikeres!', response);
            this.localStorageService.setItem('user', { id: response.id, email: this.email });
            this.router.navigate(['/']);
          },
          error: error => {
            console.error('Bejelentkezés sikertelen!', error);
            alert('Bejelentkezés sikertelen!');
          }
        });
      }
    }
  }

  navigateToRegister(): void {
    this.router.navigate(['/register']);
  }
}
