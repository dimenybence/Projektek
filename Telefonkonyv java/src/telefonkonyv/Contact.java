package telefonkonyv;

import java.util.Objects;

/**
 * Ez az ősosztály, amelyben Stringekként vannak tárolva az attribútumok.
 */
public class Contact {
    protected String lastName;
    protected String firstName;
    protected String nickName;
    protected String privateNumber;
/**
 * Contact konstruktora.
 * @param lastName
 * @param firstName
 * @param nickName
 * @param privateNumber
 */
    public Contact(String lastName, String firstName, String nickName, String privateNumber) {
        this.lastName = lastName;
        this.firstName = firstName;
        this.nickName = nickName;
        this.privateNumber = privateNumber;
    }	
    
    public String getFullName() {
		return lastName + " " + firstName;
	}
    
    public String getPrivateNumber() {
    	return privateNumber;
    }
    //Contact toString metodusa
    public String toString() {
    	if (nickName.equals("")) {
			return lastName + " " + firstName + ": " + privateNumber;
		}
		else {
			return lastName + " " + firstName + " (" + nickName + "): " + privateNumber;
		}
    }
    /**
     * A JUnit teszthez szükséges, ahhoz hogy összehasonlítson 2 Contactot, hogy ugyanazok-e.
     */
    public boolean equals(Object o) {
        Contact c = (Contact)o;
        return (lastName.equals(c.lastName) && firstName.equals(c.firstName) && 
        		nickName.equals(c.nickName) && privateNumber.equals(c.privateNumber));
    }
}
