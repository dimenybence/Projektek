package hu.aut.bme.sflab.ConcertTickets.service;

import hu.aut.bme.sflab.ConcertTickets.entities.Ticket;
import hu.aut.bme.sflab.ConcertTickets.repository.TicketRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.*;

@Service
public class TicketService {

    @Autowired
    private TicketRepository ticketRepository;

    public List<Ticket> getAllTickets() {
        return ticketRepository.findAll();
    }

    public Ticket createTicket(Ticket ticket) {
        return ticketRepository.save(ticket);
    }

    public void deleteTicket(Long id) {
        ticketRepository.deleteById(id);
    }

    public Ticket updateTicket(Long id, Ticket ticket) {
        Ticket existingTicket = ticketRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Ticket not found"));

        existingTicket.setCategory(ticket.getCategory());
        existingTicket.setPrice(ticket.getPrice());
        existingTicket.setStock(ticket.getStock());
        existingTicket.setEvent(ticket.getEvent());

        return ticketRepository.save(existingTicket);
    }

}
