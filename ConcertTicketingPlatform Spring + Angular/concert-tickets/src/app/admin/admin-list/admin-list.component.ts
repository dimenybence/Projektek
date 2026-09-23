import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Event } from '../../classes/event';
import { EventsService } from '../../services/events.service';
import { NavBarComponent } from '../../nav-bar/nav-bar.component';
import { Ticket, TicketCategory } from '../../classes/ticket';
import { Router } from '@angular/router';
import { TimeFormatPipe } from '../../time-format-pipe/time-format.pipe';

@Component({
  selector: 'app-admin-list',
  standalone: true,
  imports: [FormsModule, CommonModule, NavBarComponent, TimeFormatPipe],
  templateUrl: './admin-list.component.html',
  styleUrl: './admin-list.component.css'
})
export class AdminListComponent implements OnInit {

  events: Event[] = [];
  tickets: Ticket[] = [];
  newEvent: Event = new Event(0, '', '', '', '');
  newTicket: Ticket = new Ticket();
  ticketCategories = Object.values(TicketCategory);

  constructor( private eventService: EventsService, private router: Router) { }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getEvents().subscribe(events => {
      this.events = events;
    });
  }

  createNewEvent(): void {
    this.router.navigate(['/admin', 'new']);
  }

  createEvent(): void {
    this.eventService.createEvent(this.newEvent).subscribe(event => {
      this.events.push(event);
      this.newEvent = new Event(0, '', '', '', '');
      this.newEvent.tickets = [];
    });
  }

  editEvent(eventId: number): void {
    this.router.navigate(['/admin', eventId]);
  }

  deleteEvent(eventId: number): void {
    this.eventService.deleteEvent(eventId).subscribe(() => {
      this.loadEvents();
    });
  }
}
