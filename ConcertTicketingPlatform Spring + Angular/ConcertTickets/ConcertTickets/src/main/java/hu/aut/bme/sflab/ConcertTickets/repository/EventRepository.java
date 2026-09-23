package hu.aut.bme.sflab.ConcertTickets.repository;

import hu.aut.bme.sflab.ConcertTickets.entities.Event;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface EventRepository extends JpaRepository<Event, Long> {
    List<Event> findAll();
}