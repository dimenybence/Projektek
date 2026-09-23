package hu.aut.bme.sflab.ConcertTickets.service;

import hu.aut.bme.sflab.ConcertTickets.entities.Event;
import hu.aut.bme.sflab.ConcertTickets.entities.Ticket;
import hu.aut.bme.sflab.ConcertTickets.repository.EventRepository;
import jakarta.transaction.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class EventService {

    @Autowired
    private EventRepository eventRepository;

    public List<Event> getAllEvents() {
        return eventRepository.findAll();
    }

    public Event getEventById(Long id) {
        return eventRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Event not found"));
    }

    public Event createEvent(Event event) {
        event.getTickets().forEach(ticket -> ticket.setEvent(event));
        return eventRepository.save(event);
    }

    public void deleteEvent(Long id) {
        eventRepository.deleteById(id);
    }

    @Transactional
    public Event updateEvent(Long eventId, Event updatedEvent) {
        Event existingEvent = eventRepository.findById(eventId)
                .orElseThrow(() -> new RuntimeException("Event not found"));

        existingEvent.setName(updatedEvent.getName());
        existingEvent.setDate(updatedEvent.getDate());
        existingEvent.setTime(updatedEvent.getTime());
        existingEvent.setLocation(updatedEvent.getLocation());
        
        mergeTickets(existingEvent, updatedEvent.getTickets());

        return eventRepository.save(existingEvent);
    }

    private void mergeTickets(Event existingEvent, List<Ticket> updatedTickets) {
        existingEvent.getTickets().removeIf(existingTicket -> {
            for (Ticket updatedTicket : updatedTickets) {
                if (updatedTicket.getId() != null && updatedTicket.getId().equals(existingTicket.getId())) {
                    existingTicket.setCategory(updatedTicket.getCategory());
                    existingTicket.setPrice(updatedTicket.getPrice());
                    existingTicket.setStock(updatedTicket.getStock());
                    return false;
                }
            }
            return true;
        });

        for (Ticket updatedTicket : updatedTickets) {
            if (updatedTicket.getId() == null) {
                updatedTicket.setEvent(existingEvent);
                existingEvent.getTickets().add(updatedTicket);
            }
        }
    }
}
