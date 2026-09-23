export class Ticket {
    id!: number;
    category: TicketCategory = TicketCategory.STANDARD;
    price: number = 0;
    stock: number = 0;
    event!: Event;
}

export enum TicketCategory {
    VIP = 'VIP',
    STANDARD = 'STANDARD',
}
