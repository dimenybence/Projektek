package hu.aut.bme.sflab.ConcertTickets.controller;

import hu.aut.bme.sflab.ConcertTickets.entities.User;
import hu.aut.bme.sflab.ConcertTickets.service.EmailService;
import hu.aut.bme.sflab.ConcertTickets.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@RestController
@RequestMapping("api/authentification")
public class AuthentificationController {

    @Autowired
    private UserService userService;

    private final EmailService emailService;

    public AuthentificationController(EmailService emailService) {
        this.emailService = emailService;
    }

    @PostMapping("/register")
    public ResponseEntity<User> registerUser(@RequestBody Map<String, String> userData) {
        User user = userService.register(
                userData.get("name"),
                userData.get("email"),
                userData.get("password")
        );
        emailService.sendEmail(user.getEmail(), "Regisztráció", "Sikeres regisztráció!");
        return ResponseEntity.ok(user);
    }

    @PostMapping("/login")
    public ResponseEntity<User> login(@RequestBody Map<String, String> credentials) {
        User user = userService.login(
                credentials.get("email"),
                credentials.get("password")
        );
        return ResponseEntity.ok(user);
    }
}
