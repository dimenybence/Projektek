import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Event } from '../../classes/event';
import { EventsService } from '../../services/events.service';
import { FormsModule } from '@angular/forms';
import { NavBarComponent } from '../../nav-bar/nav-bar.component';
import { Ticket, TicketCategory } from '../../classes/ticket';

@Component({
  selector: 'app-event-edit',
  standalone: true,
  imports: [FormsModule, NavBarComponent],
  templateUrl: './admin-edit.component.html',
  styleUrls: ['./admin-edit.component.css']
})
export class AdminEditComponent implements OnInit {
  event: Event = new Event(0, '', '', '', '');
  standardTicket: Ticket = new Ticket();
  vipTicket: Ticket = new Ticket();
  ticketCategories = Object.values(TicketCategory);
  isNewEvent: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private eventService: EventsService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const eventId = +this.route.snapshot.paramMap.get('id')!;
    if (isNaN(eventId) || eventId === 0) {
      this.isNewEvent = true;
      this.initializeTickets();
    } else {
    this.loadEvent(eventId);
    }
  }

  loadEvent(eventId: number): void {
    this.eventService.getEvent(eventId).subscribe(events => {
      this.event = events;
      this.initializeTickets();
    });
  }

  initializeTickets(): void {
    this.standardTicket = this.event.tickets.find(t => t.category === TicketCategory.STANDARD) || new Ticket();
    this.vipTicket = this.event.tickets.find(t => t.category === TicketCategory.VIP) || new Ticket();

    if (!this.standardTicket.id) {
      this.standardTicket.category = TicketCategory.STANDARD;
      this.event.tickets.push(this.standardTicket);
    }

    if (!this.vipTicket.id) {
      this.vipTicket.category = TicketCategory.VIP;
      this.event.tickets.push(this.vipTicket);
    }
  }

  saveEvent(): void {
    if (this.isNewEvent) {
      this.eventService.createEvent(this.event).subscribe({
        next: () => {
          console.log('Event created successfully');
          this.router.navigate(['/admin']);
        },
        error: (err) => {
          console.error('Error creating event:', err);
          alert(`Failed to create event. Error: ${err.message}`);
        }
      });
    } else {
      this.eventService.updateEvent(this.event).subscribe({
        next: () => {
          console.log('Event updated successfully');
          this.router.navigate(['/admin']);
        },
        error: (err) => {
          console.error('Error updating event:', err);
          alert(`Failed to update event. Error: ${err.message}`);
        }
      });
    }
  }

  cancelEdit(): void {
    this.router.navigate(['/admin']);
  }
}