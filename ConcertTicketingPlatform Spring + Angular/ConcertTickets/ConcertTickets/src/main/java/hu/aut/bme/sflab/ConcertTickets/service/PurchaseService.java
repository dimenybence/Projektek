package hu.aut.bme.sflab.ConcertTickets.service;

import hu.aut.bme.sflab.ConcertTickets.entities.Purchase;
import hu.aut.bme.sflab.ConcertTickets.entities.Ticket;
import hu.aut.bme.sflab.ConcertTickets.entities.User;
import hu.aut.bme.sflab.ConcertTickets.repository.PurchaseRepository;
import hu.aut.bme.sflab.ConcertTickets.repository.TicketRepository;
import hu.aut.bme.sflab.ConcertTickets.repository.UserRepository;
import jakarta.transaction.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import java.util.*;

import java.time.LocalDateTime;

@Service
@Transactional
public class PurchaseService {

    @Autowired
    private PurchaseRepository purchaseRepository;

    @Autowired
    private UserRepository userRepository;

    @Autowired
    private TicketRepository ticketRepository;

    public Purchase createPurchase(Long userId, Long ticketId, int quantity, String paymentMethod) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new IllegalArgumentException("User not found"));

        Ticket ticket = ticketRepository.findById(ticketId)
            .orElseThrow(() -> new IllegalArgumentException("Ticket not found"));

        if(ticket.getStock() < quantity) {
            throw new IllegalArgumentException("Not enough tickets available");
        }

        Purchase purchase = new Purchase();
        purchase.setUser(user);
        purchase.setTicket(ticket);
        purchase.setQuantity(quantity);
        purchase.setPurchaseDate(LocalDateTime.now());
        purchase.setPaymentMethod(paymentMethod);


        ticket.setStock(ticket.getStock() - quantity);
        ticketRepository.save(ticket);

        return purchaseRepository.save(purchase);
    }

    public List<Purchase> getUserPurchases(Long userId) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new IllegalArgumentException("User not found"));

        return purchaseRepository.findByUser(user);
    }

    public List<Purchase> getAllPurchases() {
        return purchaseRepository.findAll();
    }
}
