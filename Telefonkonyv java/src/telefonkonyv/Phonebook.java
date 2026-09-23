package telefonkonyv;

import java.util.ArrayList;
import java.util.Objects;
/**
 * A Phonebook osztály heterogén kollekciót alkot a Contact osztállyal.
 */
public class Phonebook {
    private ArrayList<Contact> contacts;

    public Phonebook() {
        this.contacts = new ArrayList<Contact>();
    }
    /**
     * Hozzáadja a Phonebookban megadott ArrayListhez a megadott Contact-ot.
     * @param contact
     */
    public void addContact(Contact contact) {
        contacts.add(contact);
    }
    
    /**
     * Kitöröli a Phonebookban megadott ArrayListből a megadott Contact-ot.
     * @param contact
     */
    public void removeContact(Contact contact) {
        contacts.remove(contact);
    }
    /**
     * Visszaadja a Contactokat tartalmazó ArrayListet. 
     * @return contacts
     */
    public ArrayList<Contact> getContacts() {
        return contacts;
    }
}
