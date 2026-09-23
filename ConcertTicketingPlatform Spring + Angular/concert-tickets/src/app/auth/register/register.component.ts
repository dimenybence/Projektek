import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RegisterService } from '../../services/register.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  name: string = '';
  email: string = '';
  password: string = '';

  constructor(private router: Router, private registerService: RegisterService) { }

  onSubmit(form: any): void {
    if (form.valid) {
      this.registerService.register(this.name, this.email, this.password).subscribe({
        next: response => {
          console.log('Regisztráció sikeres!', response);
          this.router.navigate(['/login']);        
        },
        error: error => {
          console.error('Regisztráció sikertelen!', error);
          alert('Regisztráció sikertelen!');
        }
      });
    }
  }
}
