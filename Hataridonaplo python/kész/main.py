import sys
import beolvas
import listaz
import modosit
import fajlkezeles

def menu():
    """Kiírja a program menüjét a konzolra."""
    print()
    print("Add meg mit szeretnél:")
    print("[1]: új esemény")
    print("[2]: esemény módosítása")
    print("[3]: esemény törlése")
    print("[4]: események listázása")
    print("[5]: név szerinti keresés")
    print("[6]: mentés")
    print("[7]: importálás")
    print("[0]: kilépés")

def main():
    """Innen lehet a menüből választani amely meghívja az adott funkciókat, majd kilép ha a felhasználó 0-t ad meg"""
    esemenyek = []  # ebben a listában tárolom az eseményeket amikben Esemeny() tipusú adatok lesznek
    menu()
    while True:
        be = beolvas.olvas()
        if be == 0:
            print("Biztosan be akarod zárni?(i/n): ")
            while True:
                try:
                    bezaras = input()
                    if bezaras == "i":
                        sys.exit()
                    elif bezaras == "n":
                        break
                    else:
                        raise ValueError
                except ValueError:
                    print("Hibás bemenet! Próbáld újra (i/n): ")
        if be == 1:
            beolvas.uj_rekord(esemenyek)
            menu()
        if be == 2:
            modosit.atir(esemenyek)
            menu()
        if be == 3:
            modosit.torol(esemenyek)
            menu()
        if be == 4:
            listaz.intervallum(esemenyek)
            menu()
        if be == 5:
            listaz.keres(esemenyek)
            menu()
        if be == 6:
            fajlkezeles.mentes(esemenyek)
            menu()
        if be == 7:
            fajlkezeles.importalas(esemenyek)
            menu()
main()