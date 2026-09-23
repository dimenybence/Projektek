import beolvas
import osztaly


def hasonlit(dat1, dat2):
    """Összehasonlít két dátumot, és ha az első nagyobb mint a második akkor igazt ad vissza, ha nem hamist."""
    ev1 = int(dat1[0:4])
    honap1 = int(dat1[5:7])
    nap1 = int(dat1[8:10])

    ev2 = int(dat2[0:4])
    honap2 = int(dat2[5:7])
    nap2 = int(dat2[8:10])

    if ev1 > ev2:
        return True
    elif ev1 == ev2 and honap1 > honap2:
        return True
    elif ev1 == ev2 and honap1 == honap2 and nap1 > nap2:
        return True
    else:
        return False


def datum_rendez(esemenyek):
    """Buuble-sorttal rendezi az osztály modulban tárolt esemének listában lévő elemeket dátum szerint."""
    for i in range(0, len(esemenyek)):
        for j in range(0, len(esemenyek) - i - 1):
            if hasonlit(esemenyek[j].datum,esemenyek[j + 1].datum):
                esemenyek[j], esemenyek[j + 1] = esemenyek[j + 1], esemenyek[j]

def intervallum(esemenyek):
    """Beolvassa az intervallum elejét és végét, aztán rendezi a már létező listát. Kiírja az intervallumba eső dátumokat."""
    if len(esemenyek) == 0:
        print("Még nem töltöttél fel eseményeket.")
        return
    eleje = osztaly.Esemeny()
    vege = osztaly.Esemeny()
    datum_rendez(esemenyek)
    print("Add meg a kezdeti dátumot, ahonnan listázni szeretnéd az eseményeket!")
    eleje.datum = beolvas.datum_be()
    print("Add meg a listázandó dátum végét!")
    vege.datum = beolvas.datum_be()
    print()
    i = 0
    while True:     #Addig megy a while, amíg az eleje el nem éri az adott dátumot
        if hasonlit(eleje.datum, esemenyek[i].datum) == False:
            break
        else:
            i += 1
    while True:     #Az i-től számítva kiírja az elemeket
        if i == len(esemenyek):
            break
        elif hasonlit(vege.datum, esemenyek[i].datum) == False:
            break
        else:
            print(f"[{i}]: ")
            kiir(esemenyek[i])
            print()
            i += 1

def kiir(e):
    """Kiírja egy esemény tulajdonságait."""
    print(f"Dátum: {e.datum}")
    print(f"Idő: {e.ido}")
    print(f"Helyszín: {e.helyszin}")
    print(f"Elnevezés: {e.elnevezes}")
    print(f"Megjegyzés: {e.megjegyzes}")

def keres(esemenyek):
    """Rákeres a beadott rekord nevére, ha van ilyen kiírja, ha nem talál kiírja a, hogy nem létezik ilyen."""
    be = input("Add meg az esemény pontos nevét amelyet keresel: ")
    szoveg = "Nincs ilyen nevű esemény."
    van = False
    for i in range(0,len(esemenyek)):
        if be == esemenyek[i].elnevezes:
            van = True
            kiir(esemenyek[i])
    if van == False:
        print(szoveg)