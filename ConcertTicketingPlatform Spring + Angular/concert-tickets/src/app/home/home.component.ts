import { Component, OnInit } from '@angular/core';
import { NavBarComponent } from '../nav-bar/nav-bar.component';
import { CommonModule } from '@angular/common';
import { EventsService } from '../services/events.service';
import { Router } from '@angular/router';
import { Event } from '../classes/event';
import { TicketCategory } from '../classes/ticket';
import { FormsModule } from '@angular/forms';
import { TimeFormatPipe } from '../time-format-pipe/time-format.pipe';
import { LocalStorageService } from '../services/local-storage.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NavBarComponent, CommonModule, FormsModule, TimeFormatPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  events: Event[] = [];
  loggedIn: boolean = false;

  filteredEvents: Event[] = [];
  filterDate: string = '';
  filterTime: string = '';
  filterLocation: string = '';
  filterStock: number | null = null;
  filterPrice: number | null = null;
  filterCategory: string = '';
  ticketCategories = Object.values(TicketCategory);
  selectedEvent: Event | null = null;
  selectedCategory: string = '';
  quantity: number = 1;
  cart: any[] = [];
  isModalOpen: boolean = false;

  constructor(
    private eventsService: EventsService,
    private localStorageService: LocalStorageService,
    private router: Router
  ) { }
  
  ngOnInit(): void {
    this.loadEvents();
    const user = this.localStorageService.getItem('user');
    if (user) {
      this.loggedIn = true;
    }
  }

  loadEvents(): void {
    this.eventsService.getEvents().subscribe(data => {
      this.events = data;
      this.applyFilters();
    });
  }

  applyFilters(): void {
    if (this.filterTime!=='') {
      this.filterTime = this.filterTime + ':00';
    }
    this.filteredEvents = this.events.filter(event => {
      const matchesDate = !this.filterDate || event.date === this.filterDate;
      const matchesTime = !this.filterTime || event.time === this.filterTime;
      const matchesLocation = !this.filterLocation || event.location.toLowerCase().includes(this.filterLocation.toLowerCase());
      const matchesStock = this.filterStock === null || event.tickets.some(ticket => {
        if (this.filterCategory) {
          return ticket.category === this.filterCategory && ticket.stock >= (this.filterStock ?? 0);
        } else {
          return ticket.stock >= (this.filterStock ?? 0);
        }
      });
      const matchesPrice = this.filterPrice === null || event.tickets.some(ticket => {
        if (this.filterCategory) {
          return ticket.category === this.filterCategory && ticket.price <= (this.filterPrice ?? 0);
        } else {
          return ticket.price <= (this.filterPrice ?? 0);
        }
      });
      const matchesCategory = !this.filterCategory || event.tickets.some(ticket => ticket.category === this.filterCategory);

      return matchesDate && matchesLocation && matchesStock && matchesCategory && matchesPrice && matchesTime;
    });
  }

  openModal(event: Event): void {
    if (!this.loggedIn) {
      alert('Jelentkezz be hogy megvásárold a jegyed.');
      this.router.navigate(['/login']);
      return;
    }
    this.selectedEvent = event;
    this.selectedCategory = '';
    this.quantity = 0;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  addToCart(): void {
    if (this.selectedEvent && this.selectedCategory && this.quantity > 0) {
      const ticket = this.selectedEvent.tickets.find(t => t.category === this.selectedCategory);
      if (ticket) {
        const existingCart = this.localStorageService.getItem('cart') || [];
        existingCart.push({
          event: this.selectedEvent,
          ticket: ticket,
          category: this.selectedCategory,
          quantity: this.quantity,
          price: ticket.price * this.quantity
        });
        console.log('Added to cart:', existingCart);
        this.localStorageService.setItem('cart', existingCart);
        this.cart = existingCart;
        this.closeModal();
      } else {
        alert('Ticket not found for the selected category.');
      }
    } else {
      alert('Please select a category and quantity.');
    }
  }
}
