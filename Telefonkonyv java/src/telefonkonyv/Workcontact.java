package telefonkonyv;
/**
 * A Contact leszármazottja, hozzáadja a workNumber attributum Stringet.
 */
public class Workcontact extends Contact {
    private String workNumber;
    /**
     * Workcontact konstruktora.
     * @param lastName
     * @param firstName
     * @param nickName
     * @param privateNumber
     * @param workNumber
     */
    public Workcontact(String lastName, String firstName, String nickName, String privateNumber, String workNumber) {
        super(lastName, firstName, nickName, privateNumber);
        this.workNumber = workNumber;
    }

    public String getWorkNumber() {
        return workNumber;
    }
    
    //Workcontact toString metodusa
    public String toString() {
    	return super.toString() +", " + workNumber;
    }
    
    /**
     * A JUnit teszthez szükséges, ahhoz hogy összehasonlítson 2 Workcontactot, hogy ugyanazok-e.
     */
    public boolean equals(Object o) {
        Workcontact c = (Workcontact)o;
        return (lastName.equals(c.lastName) && firstName.equals(c.firstName) && 
        		nickName.equals(c.nickName) && privateNumber.equals(c.privateNumber) == workNumber.equals(c.workNumber));
    }
}

