import { Ticket } from "./ticket";

export class Event {
    id!: number;
    name: string = '';
    date: string = '';
    time: string = '';
    location: string = '';
    tickets: Ticket[] = [];

    constructor(id: number, name: string, date: string, time: string, location: string, tickets: Ticket[] = []) {
        this.id = id;
        this.name = name;
        this.date = date;
        this.time = time;
        this.location = location;
        this.tickets = tickets;
    }
}
