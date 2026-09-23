package hu.aut.bme.sflab.ConcertTickets.repository;

import hu.aut.bme.sflab.ConcertTickets.entities.Purchase;
import hu.aut.bme.sflab.ConcertTickets.entities.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface PurchaseRepository extends JpaRepository<Purchase, Long> {
    List<Purchase> findByUser(User user);
}
