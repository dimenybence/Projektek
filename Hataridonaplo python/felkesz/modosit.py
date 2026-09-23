import beolvas
import osztaly


def index():
    """A listázott megindexelt adatokból kiválaszt egyet a felhasználó, majd visszaadja."""
    index = 0
    while True:
        try:
            index = int(input())
            if index > len(osztaly.esemenyek) or index < 0:
                raise ValueError("Ilyen index nincs. Próbáld újra! ")
            break
        except ValueError:
            print("Ilyen index nincs. Próbáld újra! ")
    return index

def atir():
    """Kilistázza az adott érték szerint az adatokat megindexelve, amelyet majd az index() függvényben lehet kiválasztani és új értéket rendel a lista már meglévő eleméhez"""
    if osztaly.esemenyek:
        print("Mit szeretnél módosítani? (dátum = d, idő = i, hely = h,elnevezés = e, megjegyzés = m)")
        szoveg = "Hányas értéket szeretnéd módosítani? "
        while True:
            try:
                be = input()
                if be == "d":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].datum}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek[h] = beolvas.datum_be()
                    break
                elif be == "i":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].ido}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek[h] = beolvas.ido_be()
                    break
                elif be == "h":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].hely}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek[h] = beolvas.helyszin_be()
                    break
                elif be == "e":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].elnevezes}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek[h] = beolvas.elnevezes_be()
                    break
                elif be == "m":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].megjegyzes}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek[h] = beolvas.megjegyzes_be()
                    break
                else:
                    raise ValueError("Ilyen nincs. Próbáld újra: ")
            except ValueError:
                print("Ilyen nincs. Próbáld újra: ")

    else:
        print("Még nem adtál hozzá eseményeket, amiket módosíthatnál. Kérlek adj hozzá értékeket!")


def torol():
    """Hasonlóan működik az átírhoz, viszont itt kitörli az adott elemet a listából."""
    if osztaly.esemenyek:
        print("Mi alapján szeretnél törölni? (dátum = d, idő = i, hely = h,elnevezés = e, megjegyzés = m)")
        szoveg = "Hányas értéket szeretnéd törölni? "
        while True:
            try:
                be = input()
                if be == "d":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].datum}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek.pop(h)
                    break
                elif be == "i":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].ido}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek.pop(h)
                    break
                elif be == "h":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].hely}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek.pop(h)
                    break
                elif be == "e":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].elnevezes}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek.pop(h)
                    break
                elif be == "m":
                    for i in range(0, len(osztaly.esemenyek)):
                        print(f"[{i}]. {osztaly.esemenyek[i].megjegyzes}")
                    print(szoveg)
                    h = index()
                    osztaly.esemenyek.pop(h)
                    break
                else:
                    raise ValueError("Ilyen nincs. Próbáld újra: ")
            except ValueError:
                print("Ilyen nincs. Próbáld újra: ")
    else:
        print("Még nem adtál hozzá eseményeket, amelyet törölhetnél. Kérlek adj hozzá értékeket!")