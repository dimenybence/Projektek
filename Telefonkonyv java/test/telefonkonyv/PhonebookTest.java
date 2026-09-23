package telefonkonyv;

import static org.junit.Assert.*;

import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintStream;
import java.util.ArrayList;

import org.junit.Before;
import org.junit.Test;


public class PhonebookTest {
	PrintStream standardOut = System.out;
	ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
	
	Phonebook p;
	Contact c, c2;
	Workcontact w;
	
	@Before
	public void setUp() {
		p = new Phonebook();
		c = new Contact("Rend", "Elek", "Rendi", "208510161");
		c2 = new Contact("Vicc", "Elek", "Vici", "987654321");
		w = new Workcontact("Vegh", "Bela", "VB", "123456789", "111121314");
	}
	
	@Test
	public void testContactGetFullName() {
		assertEquals("Rend Elek", c.getFullName());
	}
	
	@Test
	public void testContactGetPrivateNumber() {
		assertEquals("208510161", c.getPrivateNumber());
	}
	
	@Test
	public void testWorkContactGetWorkNumber() {
		assertEquals("111121314", w.getWorkNumber());
	}
	
	@Test
	public void testContactToString() {
		assertEquals("Rend Elek (Rendi): 208510161", c.toString());
	}
	
	@Test
	public void testWorkContactToString() {
		assertEquals("Vegh Bela (VB): 123456789, 111121314", w.toString());
	}
	
	@Test
	public void testListContacts() {
		p.addContact(c);
		p.addContact(c2);
		p.addContact(w);
		
		System.setOut(new PrintStream(outputStream));
        
        Commands.listContacts(p);
        
        
        String expectedOutput = "Rend Elek (Rendi): 208510161 - szemelyes kontakt\n" + 
                				"Vicc Elek (Vici): 987654321 - szemelyes kontakt\n" + 
                				"Vegh Bela (VB): 123456789, 111121314 - munkahelyi kontakt\n";

        assertEquals(expectedOutput, outputStream.toString());
	}
	
	@Test
	public void testAddContact() {
		p.addContact(c);
		assertEquals(1, p.getContacts().size());
		assertTrue(p.getContacts().contains(c));
	}
	
	@Test
	public void testRemoveContact(){
		p.addContact(c);
        assertEquals(1, p.getContacts().size());
        p.removeContact(c);
        assertEquals(0, p.getContacts().size());
        assertFalse(p.getContacts().contains(c));
	}
	
	@Test
    public void testSaveFile() {
        p.addContact(c);
        p.addContact(c2);

        Commands.saveFile("test.txt", p);

        String fileContent = "";
        try {
        	FileReader f = new FileReader("test.txt");
        	BufferedReader reader = new BufferedReader(f);
            String line;
            while ((line = reader.readLine()) != null) {
                fileContent += line+"\n";
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        String expectedFileContent = "Rend Elek (Rendi): 208510161\n" + "Vicc Elek (Vici): 987654321\n";
        assertEquals(fileContent, expectedFileContent);
        
        File testDataFile = new File("test.txt");
        testDataFile.delete();
    }
	
	@Test
    public void testReadFile() throws Exception {
		FileWriter fileWriter = new FileWriter("test.txt");
		String testfilecontent = "Rend Elek (Rendi): 208510161\n" + "Vicc Elek (Vici): 987654321\n";
		fileWriter.write(testfilecontent);
		fileWriter.close();
		
		Phonebook expectedContacts = new Phonebook();
		
		Commands.readFile("test.txt", p);

		expectedContacts.addContact(new Contact("Rend", "Elek", "Rendi", "208510161"));
		expectedContacts.addContact(new Contact("Vicc", "Elek", "Vici", "987654321"));
		
		assertEquals(expectedContacts.getContacts().size(), p.getContacts().size());
		assertEquals(p.getContacts(), expectedContacts.getContacts());
		
		File file = new File("test.txt");
		file.delete();
	}
}
