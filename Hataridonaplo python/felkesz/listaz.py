import beolvas
import osztaly

def hasonlit(dat1, dat2):
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
    elif ev1 == ev2 and honap1 == honap2 and nap1 >= nap2:
        return True
    else:
        return False

def intervallum():
    eleje = beolvas.datum_be()
    vege = beolvas.datum_be()

