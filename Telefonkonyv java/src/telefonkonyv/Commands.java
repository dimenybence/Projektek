package telefonkonyv;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Scanner;
/**
 * Ez az osztály tárolja a menüben megjelenített parancsok forráskódját.
 */
public class Commands {
	/**
	 * Kiírja a kimenetre a választható opciókat.
	 */
	public static void menu() {
		System.out.println("A kovetkezo opciok erhetok el:");
		System.out.println("1 - Listazas");
		System.out.println("2 - Kontakt felvetele");
		System.out.println("3 - Kontakt torlese");
		System.out.println("4 - Adatok mentese");
		System.out.println("5 - Adatok betoltese");
		System.out.println("6 - Kilepes");
		System.out.println("Add meg mit szeretnel: ");
	}
	/**
	 * Kilistázza a Contactokat formázva, a végére írva a kontaktok fajtáját. Ha üres lenne az ArrayList kiírja az üzenetet.
	 * @param phonebook
	 */
    public static void listContacts(Phonebook phonebook) {
    	if(phonebook.getContacts().isEmpty()) {
    		System.out.println("Nem talalhato nevjegy!");
    		return;
    	}
        for (Contact contact : phonebook.getContacts()) {
        	if(contact instanceof Workcontact) {
        		System.out.print(contact + " - munkahelyi kontakt\n");
        	}
        	else {
        		System.out.print(contact + " - szemelyes kontakt\n");
        	}
        }
    }
    /**
     * Hozzáadja az ArrayListhez a kontaktot a bekért információkkal, ha valami hibába ütközik Exception-t dob.
     * @param phonebook
     */
    public static void addContact(Phonebook phonebook) {
        Scanner sc = new Scanner(System.in);
        
        while (true) {
            try {
            	System.out.println("Ha szemelyes kontaktot szeretne hozzaadni nyomja meg az 1-es gomot");
            	System.out.println("Ha munkahelyi kontaktot szeretne magadni nyomja meg a 2-es gombot");
                System.out.print("Valasszon : ");
            	
            	String contactType = sc.nextLine();
            	if(!contactType.equals("1") && !contactType.equals("2")) {
            		throw new Exception("Ilyen opcio nem letezik!");
            	}
            	
            	System.out.print("Add meg a vezeteknevet: ");
                String lastName = sc.nextLine();
                if (lastName.isEmpty()) {
                    throw new Exception("A vezeteknev nem lehet ures");
                }
                
                System.out.print("Add meg a keresztnevet: ");
                String firstName = sc.nextLine();
                if (firstName.isEmpty()) {
                    throw new Exception("A keresztnev nem lehet ures");
                }
                
                System.out.print("Add meg a becenevet (opcionalis, ha nem szeretne nyomjon entert): ");
                String nickName = sc.nextLine();
                
                System.out.print("Add meg a telefonszamot: ");
                String privateNumber = sc.nextLine();
                if (privateNumber.isEmpty() || privateNumber.length() < 6) {
                    throw new Exception("A telefonszam nem lehet ures es minimum 6 karakteres");
                }
                try {
                    Long.parseLong(privateNumber);
                } catch (NumberFormatException e) {
                    throw new Exception("A telefonszam csak szamokat tartalmazhat");
                }
                if(contactType.equals("1")) {
                	Contact contact = new Contact(lastName, firstName, nickName, privateNumber);
                    phonebook.addContact(contact);
                }
                else if(contactType.equals("2")) {
                	System.out.print("Add meg a munkahelyi telefonszamot: ");
                    String workNumber = sc.nextLine();
                    if (workNumber.isEmpty() || workNumber.length() < 6) {
                        throw new Exception("A telefonszam nem lehet ures es minimum 6 karakteres");
                    }
                    try {
                        Long.parseLong(workNumber);
                    } catch (NumberFormatException e) {
                        throw new Exception("A telefonszam csak szamokat tartalmazhat");
                    }
                    Workcontact workcontact = new Workcontact(lastName, firstName, nickName, privateNumber, workNumber);
                    phonebook.addContact(workcontact);
				}
                
                System.out.println("Kontakt sikeresen hozzaadva.");
                break;
            } catch (Exception e) {
                System.out.println("Hiba: " + e.getMessage());
            }
        }
    }
    /**
     * Kitörli az ArrayListből a rákeresett kontaktot.
     * @param phonebook
     */
    public static void removeContact(Phonebook phonebook) {
        Scanner sc = new Scanner(System.in);
        System.out.print("Add meg a kontakt teljes nevet vagy privat telefonszamat vagy munkahelyi telefonszamat a torleshez: ");
        String search = sc.nextLine();

        Contact contactToRemove = null;
        for (Contact contact : phonebook.getContacts()) {
            if (contact instanceof Workcontact) {
                if (contact.getFullName().equals(search) || contact.getPrivateNumber().equals(search) || ((Workcontact) contact).getWorkNumber().equals(search)) {
                    contactToRemove = contact;
                    break;
                }
            } else {
                if (contact.getFullName().equals(search) || contact.getPrivateNumber().equals(search)) {
                    contactToRemove = contact;
                    break;
                }
            }
        }
        if (contactToRemove != null) {
            phonebook.removeContact(contactToRemove);
            System.out.println("Kontakt sikeresen eltavolitva.");
        } else {
            System.out.println("Ilyen kontakt nem talalhato.");
        }
    }
    /**
     * Elmenti az ArrayListben tárolt kontaktokat soronként egy txt fajlba, ha üres üzenetet dob és visszatér.
     * @param phonebook
     */
    public static void saveFile(String filename, Phonebook phonebook) {
    	if(phonebook.getContacts().isEmpty()) {
    		System.out.println("Adj hozza kontaktokat a fajl metesehez.");
    		return;
    	}
        try {
			FileWriter f = new FileWriter(filename);
			for(Contact i : phonebook.getContacts()) {
				f.write(i+"\n");
			}
			f.close();
			System.out.println("Fajl mentve " + filename + " neven.");
		} catch (IOException e) {
			e.printStackTrace();
		}
    }
    /**
     * A bekért txt fájl nevét megadva visszaolvassa a fájlban tárolt kontaktokat.
     * @param phonebook
     * @throws Exception
     */
    public static void readFile(String filename, Phonebook phonebook) throws Exception {
        try {
            Scanner fileScanner = new Scanner(new File(filename));
            while (fileScanner.hasNextLine()) {
                String line = fileScanner.nextLine();
                String[] fields = line.split(": ");
                
                String name = fields[0].trim();
                String phone = fields[1].trim();
                String[] names = name.split(" ");
                String lastName = names[0].trim();
                String firstName = names[1].trim();
                String nickName = "";
                if (names.length > 2) {
                    nickName = names[2].substring(1, names[2].length() - 1);
                }
                
                if (phone.contains(",")) {
                    String[] numbers = phone.split(",");
                    String privateNumber = numbers[0].trim();
                    String workNumber = numbers[1].trim();
                    Workcontact contact = new Workcontact(lastName, firstName, nickName, privateNumber, workNumber);
                    phonebook.addContact(contact);
                } else {
                    Contact contact = new Contact(lastName, firstName, nickName, phone);
                    phonebook.addContact(contact);
                }
            }
            fileScanner.close();
            System.out.println("Adatok visszaolvasva a " + filename + " fajlbol.");
        } catch (FileNotFoundException e) {
            System.out.println("A fajl nem talalhato.");
            e.printStackTrace();
        } catch (Exception e) {
            System.out.println("Hiba tortent az adatok visszaolvasasa kozben.");
            e.printStackTrace();
        }
    }
}
