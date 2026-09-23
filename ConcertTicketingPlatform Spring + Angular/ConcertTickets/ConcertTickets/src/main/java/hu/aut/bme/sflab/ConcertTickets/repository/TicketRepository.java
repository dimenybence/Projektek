package hu.aut.bme.sflab.ConcertTickets.repository;

import hu.aut.bme.sflab.ConcertTickets.entities.Ticket;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface TicketRepository extends JpaRepository<Ticket, Long> {
}