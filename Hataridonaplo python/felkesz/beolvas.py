import osztaly

def olvas():
    """Beolvas egy értéket 1-től 7-ig. Ha rossz az input hibát dob."""
    while True:
        try:
            print()
            be = int(input("Érték megadása 0-tól 7-ig: "))
            print()
            if be > 7 or be < 0:
                raise ValueError("0,7")
            break
        except ValueError:
            print("Sikertelen. Írd be újra! ")
    return be

def datum_be():
    """Bekéri a helyes dátumot. Ha hibás akkor azt nem fogadja el."""
    while True:
        try:
            datum = input("Add meg a dátumot (ÉÉÉÉ.HH.NN formátumba!): ")

            if len(datum) != 10:
                raise ValueError("Hibás formátum. Írd be újra! ")

            if datum[4] != "." and datum[7] != ".":
                raise ValueError("Hibás formátum. Írd be újra! ")

            ev = int(datum[0:4])
            honap = int(datum[5:7])
            nap = int(datum[8:10])
            if ev < 1 or (honap > 12 or honap < 1) or (nap > 31 or nap < 1):
                raise ValueError("Hibás formátum. Írd be újra! ")

            break
        except ValueError:
            print("Hibás formátum. Írd be újra! ")
    return datum

def ido_be():
    """Beolvassa az időt óra:másodperc formátumba, ha helytelen hibát dob."""
    while True:
        try:
            ido = input("Add meg az időt (ÓÓ:PP): ")

            if ido[2] != ":":
                raise ValueError("Hibás formátum. Írd be újra! ")

            if len(ido) != 5:
                raise ValueError("Hibás formátum. Írd be újra! ")

            ora = int(ido[0:2])
            perc = int(ido[3:5])
            if (ora > 23 or ora < 1) or (perc > 59 or perc < 1):
                raise ValueError("Hibás formátum. Írd be újra! ")
            break
        except ValueError:
            print("Hibás formátum. Írd be újra! ")
    return ido

def helyszin_be():
    hely = input("Add meg az helyszínt: ")
    return hely

def elnevezes_be():
    elnevezes = input("Add meg az elnevezést: ")
    return elnevezes

def megjegyzes_be():
    megjegyzes = input("Adj az megjegyzést: ")
    return megjegyzes

def uj_rekord():
    """Beolvassa egy rekord adatait"""
    e = osztaly.Esemeny()
    e.datum = datum_be()
    e.ido = ido_be()
    e.helyszin = helyszin_be()
    e.elnevezes = elnevezes_be()
    e.megjegyzes = megjegyzes_be()
    osztaly.esemenyek.append(e)