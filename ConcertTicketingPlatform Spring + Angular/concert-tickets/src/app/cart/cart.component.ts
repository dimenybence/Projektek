import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LocalStorageService } from '../services/local-storage.service';
import { NavBarComponent } from '../nav-bar/nav-bar.component';
import { PurchaseService } from '../services/purchase.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, NavBarComponent, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: any[] = [];
  totalPrice: number = 0;
  paymentMethod: string = '';

  constructor(
    private localStorageService: LocalStorageService,
    private purchaseService: PurchaseService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.cart = this.localStorageService.getItem('cart') || [];
    this.paymentMethod = this.localStorageService.getItem('paymentMethod') || '';
    this.calculateTotalPrice();
  }

  calculateTotalPrice(): void {
    this.totalPrice = this.cart.reduce((total, item) => total + item.price, 0);
  }

  deleteFromCart(index: number): void {
    this.cart.splice(index, 1);
    this.localStorageService.setItem('cart', this.cart);
    this.calculateTotalPrice();
  }

  selectPaymentMethod(method: string): void {
    this.paymentMethod = method;
    this.localStorageService.setItem('paymentMethod', method);
  }

  checkout(): void {
    const user = this.localStorageService.getItem('user');
    const userId = user.id;
    const purchaseRequests = this.cart.map(item => {
      if (!item.ticket || !item.ticket.id) {
        console.error('Rossz jegyadatok:', item);
        return Promise.reject('Rossz jegyadatok');
      }
      const requestData = {
        user: { id: userId },
        ticket: { id: item.ticket.id },
        quantity: item.quantity,
        paymentMethod: this.paymentMethod
      };
      return this.purchaseService.createPurchase(requestData).toPromise();
    });

    Promise.all(purchaseRequests).then(() => {
      alert('Sikeres vásárlás!');
      this.localStorageService.removeItem('cart');
      this.router.navigate(['/']);
    }).catch(error => {
      console.error('Purchase failed', error);
      alert('Vásárlás sikertelen. Kérjük próbálja újra később.');
    });
  }
}