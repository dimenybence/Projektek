import osztaly

def mentes(esemenyek):
    """Az osztály modulban lévő listát elmenti egy szöveges fájlba, adatiat soronként menti el."""
    fajlnev = input("Mi legyen a fájé neve: ")
    fajlnev = fajlnev + ".txt"
    with open(fajlnev, "wt", encoding = "utf-8") as f:
        for i in range(0,len(esemenyek)):
            f.write(esemenyek[i].datum)
            f.write("\n")
            f.write(esemenyek[i].ido)
            f.write("\n")
            f.write(esemenyek[i].helyszin)
            f.write("\n")
            f.write(esemenyek[i].elnevezes)
            f.write("\n")
            f.write(esemenyek[i].megjegyzes)
            f.write("\n")

def beolvas():
    """A fájlnév megadása után egy listában tárolja, soronként a fájl adatait."""
    fajlnev = input("Mi a fájl neve: ")
    fajlnev = fajlnev + ".txt"
    tmp = []
    with open(fajlnev, "rt", encoding="utf-8") as f:
        for sor in f:
            sor = sor.rstrip("\n")
            tmp.append(sor)
        return tmp

def importalas(esemenyek):
    """A beolvasott lista elemeit rendszerezi osztályba és beleteszi az osztály modulban tárolt listába."""
    lista = beolvas()
    i = 0
    while i < len(lista):
        e = osztaly.Esemeny()
        esemenyek.append(e)
        e.datum = lista[i]
        e.ido = lista[i+1]
        e.helyszin = lista[i+2]
        e.elnevezes = lista[i+3]
        e.megjegyzes = lista[i+4]
        i+=5






