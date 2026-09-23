package hu.aut.bme.sflab.ConcertTickets.controller;

import hu.aut.bme.sflab.ConcertTickets.entities.Purchase;
import hu.aut.bme.sflab.ConcertTickets.service.EmailService;
import hu.aut.bme.sflab.ConcertTickets.service.PurchaseService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.format.DateTimeFormatter;
import java.util.Collections;
import java.util.List;

@RestController
@RequestMapping("/api/purchase")
public class PurchaseController {

    @Autowired
    private PurchaseService purchaseService;

    private final EmailService emailService;

    public PurchaseController(EmailService emailService) {
        this.emailService = emailService;
    }

    @GetMapping
    public List<Purchase> getAllPurchases() {
        return purchaseService.getAllPurchases();
    }

    @PostMapping
    public ResponseEntity<?> createPurchase(@RequestBody Purchase purchase) {
        try {
            if (purchase.getUser() == null || purchase.getTicket() == null || purchase.getQuantity() == 0) {
                throw new RuntimeException("Missing required fields: user, ticket, or quantity");
            }

            Purchase createdPurchase = purchaseService.createPurchase(
                    purchase.getUser().getId(),
                    purchase.getTicket().getId(),
                    purchase.getQuantity(),
                    purchase.getPaymentMethod()
            );
            DateTimeFormatter formatter = java.time.format.DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm");

            String emailBody = String.format(
                    "Vásárlás részletei:\n\nNév: %s\nEmail: %s\nKoncert: %s\nJegykategória: %s\nMennyiség: %d\nÁr: %s Ft\nFizetési mód: %s\nVásárlás dátuma: %s",
                    createdPurchase.getUser().getName(),
                    createdPurchase.getUser().getEmail(),
                    createdPurchase.getTicket().getEvent().getName(),
                    createdPurchase.getTicket().getCategory(),
                    createdPurchase.getQuantity(),
                    createdPurchase.getTicket().getPrice(),
                    createdPurchase.getPaymentMethod(),
                    createdPurchase.getPurchaseDate().format(formatter)
            );

            emailService.sendEmail(createdPurchase.getUser().getEmail(), "Vásárlás megerősítése", emailBody);
            return ResponseEntity.ok(createdPurchase);
        } catch (RuntimeException e) {
            return ResponseEntity
                    .badRequest()
                    .body(Collections.singletonMap("message", e.getMessage()));
        }
    }

    @GetMapping("/{userId}")
    public ResponseEntity<?> getUserPurchases(@PathVariable Long userId){
        try {
            List<Purchase> purchases = purchaseService.getUserPurchases(userId);
            return ResponseEntity.ok(purchases);
        } catch (RuntimeException e) {
            return ResponseEntity
                    .badRequest()
                    .body(Collections.singletonMap("message", e.getMessage()));
        }
    }
}
