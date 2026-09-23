import { Ticket } from './ticket';
import { User } from './user';

export class Purchase {
  id!: number;
  user!: User;
  ticket!: Ticket;
  quantity!: number;
  purchaseDate!: string;
  price!: number;
  paymentMethod!: string;
}