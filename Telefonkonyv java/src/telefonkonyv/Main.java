package telefonkonyv;

import java.util.Scanner;
/**
 * Innen történik a menüben navigálás, amely a létrehozott Phonebook objektumban végez módosításokat vagy
 * írat ki adatokat belőle. 
*/
public class Main {
	public static void main(String[] args) {
		Phonebook phonebook = new Phonebook();
		Scanner sc = new Scanner(System.in);
		
		Commands.menu();
		
		while(true) {
			String cmd = sc.nextLine();
			if(cmd.equals("1")) {
				Commands.listContacts(phonebook);
			}
			else if(cmd.equals("2")) {
				try {
					Commands.addContact(phonebook);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
			else if(cmd.equals("3")) {
				Commands.removeContact(phonebook);
			}
			else if(cmd.equals("4")) {
				System.out.print("Add meg az elmentendo fajl nevet: ");
				String filename = sc.nextLine();
				filename = filename+".txt";
				Commands.saveFile(filename, phonebook);
			}
			else if(cmd.equals("5")) {
				try {
					System.out.print("Add meg a beolvasando fajl nevet: ");
					String filename = sc.nextLine();
					filename = filename + ".txt";
					Commands.readFile(filename, phonebook);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
			else if(cmd.equals("6")) {
				System.out.println("Biztosan be szeretne zarni(i/n)?");
				String ex = sc.nextLine();
				if(ex.equals("i")){
					System.out.println("Viszlat!");
					break;
				}
				else if(ex.equals("n")) {
					continue;
				}
				else {
					System.out.println("Hibas bemenet");
				}
			}
			else {
				System.out.println("Hibas bemenet!");
			}
		}
	}
}
