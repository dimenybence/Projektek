# ARP Guard – Projekt leírás

Egy **eBPF/XDP** alapú csomagszűrő, amely felismeri és eldobja
a hamis ARP válaszcsomagokat – így megakadályozza a klasszikus
**ARP gyorsítótár-mérgezést** (ARP cache poisoning / ARP spoofing).

A kernel-oldali szűrő C-ben íródott (a BCC eszközcsomag fordítja és
tölti be), egy Python szkript pedig user-space-ben felel a betöltésért,
a bizalmi lista feltöltéséért és a statisztikák kiírásáért. A
projekt egy háromkonténeres image-et hoz létre (gateway, victim, attacker),
amelyben élő ARP spoofing támadás indítható – és látszik, ahogy a
szűrő blokkolja azt.

---

## A probléma – ARP hamisítás

Amikor egy gép a helyi hálózaton (LAN) IP-csomagot szeretne küldeni egy
másik hostnak, először tudnia kell, hogy az adott IP-hez milyen
**MAC cím** (hardvercím) tartozik. Ezt az ARP (Address Resolution
Protocol) kérdezi le:

> *„Kié a 172.28.55.10-es IP? Mondja meg a 172.28.55.20-nak.”*

Az IP tulajdonosa válaszol:

> *„A 172.28.55.10-es IP a 02:42:ac:1c:37:0a MAC-en van.”*

Az áldozat ezt eltárolja a saját **ARP gyorsítótárában**, és a továbbiakban
ezt használja minden onnan érkező csomaghoz.

**A baj:** az ARP protokollnak nincs hitelesítése. Bármelyik gép a hálózaton
hazudhatja azt, hogy ő a 172.28.55.10 – ekkor az áldozat által küldött
forgalom mind a támadón keresztül megy.

```
        Támadás előtt                       Támadás után (mérgezve)
   áldozat ──► átjáró (valódi MAC)    áldozat ──► támadó (hamis MAC)
                                                ▲
                                       támadó hazudja: „én vagyok az átjáró”
```

---

## A megoldás – eBPF/XDP szűrő

A védelem közvetlenül a kernelben, a legkorábbi lehetséges pillanatban
fut le: már akkor, amikor egy csomag megérkezik a hálózati kártyához,
**még mielőtt** a kernel ARP-gyorsítótára frissülne.

### Felhasznált technológiák

| Fogalom | Jelentés |
|---|---|
| **eBPF** | "Extended Berkeley Packet Filter" – egy biztonságos, sandboxolt virtuális gép a Linux kernelen belül, amibe ellenőrzött C-programok tölthetők be. |
| **XDP** | "eXpress Data Path" – a kernel egyik legkorábbi hookja a hálózati csomag útján. Az itt futó program eldöntheti, hogy a csomag továbbhaladhat-e (`XDP_PASS`) vagy azonnal eldobandó (`XDP_DROP`). |
| **BCC** | "BPF Compiler Collection" – eszközkészlet, amellyel C-ben írhatjuk a kernel-oldali kódot, és Pythonból kezelhetjük (betöltés, attach, BPF mapek). |

### Mit csinál a szűrő?

Minden beérkező csomagnál a szűrő:

```
  ┌─────────────────────────────────────────────────────────┐
  │ 1. Ez ARP csomag?                  ── NEM ──► PASS      │
  │ 2. Ez ARP válasz (reply)?          ── NEM ──► PASS      │
  │ 3. Az IP a megbízható listában?    ── NEM ──► PASS      │
  │ 4. A MAC egyezik a bizalmival?     ── IGEN ──► PASS     │
  │    Nem egyezik?                    ── DROP + számlál ◄  │
  └─────────────────────────────────────────────────────────┘
```
### Miért pont ez a sorrend és ezek a döntések?

A szűrő egy **célzott védelem**, nem általános tűzfal. Csak egyetlen
támadási formát (ARP spoofing) blokkol, mindent mást zavartalanul átenged.
A négy döntés ezt fokozatosan szűkíti le:

1. **Nem ARP csomag → PASS.** A normál forgalmat (TCP, UDP, ICMP, IPv6…) nem
   érdekel minket.
2. **Nem ARP válasz → PASS.** Az ARP gyorsítótár-mérgezés a *válaszokon*
   keresztül történik, mert ezek írják felül az áldozat ARP tábláját. Az ARP
   *kérések* önmagukban nem mérgeznek, ezért nem foglalkozunk velük.
3. **Az IP nincs a bizalmi listában → PASS.** Csak azokat az IP-ket védjük,
   amelyeket explicit beírtunk a `trusted.txt`-be (pl. az átjárót). Ismeretlen
   IP-kre nem mondhatjuk meg, mi az "igazi" MAC – ezért nem hozhatunk döntést.
4. **MAC egyezik a bizalmival → PASS.** Ez egy valódi, legitim ARP válasz a
   valódi gazdagéptől, hagyjuk frissülni az ARP gyorsítótárat.
5. **MAC NEM egyezik → DROP + számlál.** Egy védett IP-ről érkezett válasz egy
   hamis MAC-cel: csakis támadás lehet. Eldobjuk a csomagot (még *mielőtt* a
   kernel ARP-cache-e látná), és atomikusan növeljük a drop számlálót.

Az eldobott csomagok száma egy BPF tömbben (counter) gyűlik, amit a
Python user-space szkript kétmásodpercenként kiolvas és kiír.

---

## Architektúra

```
┌─────────────────────────────────────────────────────────────┐
│                  loader.py (user-space)                     │
│   - beolvassa: trusted.txt                                  │
│   - lefordítja: arp_guard.c (BCC-vel)                       │
│   - betölti és csatolja az XDP programot az eth0-ra         │
│   - feltölti a `trusted_macs` mapet                         │
│   - figyeli a `drop_count` mapet, kiírja a statisztikát     │
└─────────────────────────┬───────────────────────────────────┘
                          │ BCC / bpf() rendszerhívás
┌─────────────────────────▼───────────────────────────────────┐
│                       Linux kernel                          │
│   ┌──────────────────────┐    ┌──────────────────────┐      │
│   │ BPF mapek            │    │ XDP program          │      │
│   │  trusted_macs (HASH) │◄──►│  arp_guard.c         │      │
│   │  drop_count  (ARRAY) │    │  eth0-hoz csatolva   │      │
│   └──────────────────────┘    └─────────▲────────────┘      │
└──────────────────────────────────────────┼──────────────────┘
                                           │ minden bejövő csomag
                                  [ hálózati interfész ]
```

### Teszt-környezet (Docker)

```
                  Docker bridge: 172.28.55.0/24
   .10                    .20                       .30
  ┌──────────┐         ┌─────────────────┐        ┌──────────┐
  │ gateway  │         │     victim      │        │ attacker │
  │ (alpine) │         │ Ubuntu+BCC+XDP  │        │ (alpine) │
  │ MAC: ...0a │       │ MAC: ...14      │        │ MAC: ...1e │
  └──────────┘         └─────────────────┘        └──────────┘
   valódi átjáró        ide csatolt a szűrő        spoofol
```

---

## Fájlstruktúra

```
.
├── arp_guard.c          # XDP program (kernel-oldali szűrő)
├── loader.py            # Python loader és monitor (BCC API)
├── trusted.txt          # IP → MAC megbízható lista (kézzel szerkeszthető)
├── attack.py            # Demó segédszkript: hamisított ARP-t küld scapyval
├── Dockerfile.victim    # Az áldozat image-e: Ubuntu 24.04 + BCC + eszközök
├── setup.sh             # Konténer entrypoint (kheaders, sysctl, loader.py)
├── docker-compose.yml   # 3-konténeres labor: gateway, victim, attacker
└── README.md           # Magyar nyelvű leírás
```

| Fájl | Szerep |
|---|---|
| `arp_guard.c` | A kernelben fut az XDP hookon. Minden ARP választ összevet a bizalmi listával, és eltér esetén `XDP_DROP`-pal eldobja. |
| `loader.py` | Lefordítja és csatolja az XDP programot, betölti a `trusted.txt`-et a BPF mapbe, majd 2 másodpercenként frissíti a számlálót. |
| `trusted.txt` | Soronként egy `<IP>  <MAC>` páros. |
| `attack.py` | Scapy-alapú segédszkript, amely a kívánt számú hamis ARP választ küld. |
| `Dockerfile.victim` | Az áldozat image-ének configja.
| `setup.sh` | Az áldozat konténer entrypointja: kibontja a kernel headereket, beállítja az `arp_accept`-et, majd elindítja a `loader.py`-t. |
| `docker-compose.yml` | Definiálja 3 konténert, fix IP- és MAC-címek, közös bridge hálózat. |

---

## Előfeltételek

- **Linux gazda** eBPF-támogatással (kernel 5.x+ ajánlott).
- **Docker** (Windowson Docker Desktop, WSL2 háttérrel).
- **Kernel headerek elérhetők**:
  - natív Linuxon: `linux-headers-$(uname -r)` telepítve, vagy
  - WSL2-n: `kheaders` kernelmodul betöltve (lásd lent).

### WSL2-specifikus megjegyzés (Docker Desktop Windowson)

A WSL2 kernele csak akkor teszi elérhetővé a fejléceit, ha betöltjük a
`kheaders` modult:

```powershell
wsl modprobe kheaders
```

Ezt **Windows-indításonként egyszer** kell lefuttatni. A `setup.sh` az
áldozat konténerben a futás során kicsomagolja a headerket a BCC számára.

---

## Gyors indítás

```bash
# (Csak WSL2-n, Windows-indításonként egyszer)
wsl modprobe kheaders

# 3 konténer indítása
docker compose up -d

# A szűrő élő logjának követése
docker logs -f arp_victim
```

Várt kimenet:

```
[*] Extracting kernel headers for <kernel-verzió> ...
[*] Starting ARP Guard ...
[+] Trusted: 172.28.55.10 -> 02:42:ac:1c:37:0a
[+] ARP Guard attached to eth0. Press Ctrl+C to stop.
```

A szűrő most aktív és vizsgálja az áldozat `eth0` interfészének minden
bejövő csomagját.

A környezet leállítása:

```bash
docker compose down
```

---

## Demó

```bash
# 0. Előkészület (Windows-indításonként egyszer)
wsl modprobe kheaders

# 1. Környezet indítása
docker compose up -d

# 2. Másik terminál: szűrő figyelése
docker logs -f arp_victim

# 3. Támadás indítása az attacker konténerből (10 hamis ARP)
docker exec arp_attacker python3 attack.py

# 4. ARP tábla ellenőrzése – tisztának kell lennie
docker exec arp_victim ip neigh show

# 5. (Opcionális) Szűrő kikapcsolása, támadás megismétlése
docker exec arp_victim pkill -f loader.py
docker exec arp_victim ip link set dev eth0 xdpgeneric off
docker exec arp_victim ip neigh flush dev eth0
docker exec arp_attacker python3 attack.py 3
docker exec arp_victim ip neigh show         # most már mérgezett!

# 6. Szűrő visszakapcsolása
docker compose restart victim

# 7. Környezet leállítása
docker compose down
```

### `attack.py` paraméterek

```bash
docker exec arp_attacker python3 attack.py              # 10 hamisítvány (alapértelmezett)
docker exec arp_attacker python3 attack.py 50           # 50 hamisítvány
docker exec arp_attacker python3 attack.py 1 --mac aa:bb:cc:dd:ee:ff
docker exec arp_attacker python3 attack.py --ip 172.28.55.10
```

---


## Záró gondolatok

A projekt egy valós, működő demó: az ARP egy klasszikus, máig
kihasználható gyengesége, az eBPF/XDP pedig egy modern, alacsony
overhead-ű megoldás a kivédésére.
