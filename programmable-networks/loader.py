import ctypes
import os
import socket
import time
from bcc import BPF

INTERFACE  = os.environ.get("ARP_GUARD_IFACE", "eth0")
TRUST_FILE = os.environ.get("ARP_GUARD_TRUST", "/work/trusted.txt")
POLL_SEC   = 2


class MacAddr(ctypes.Structure):
    _fields_ = [("addr", ctypes.c_ubyte * 6)]


def ip_to_key(ip_str):
    return ctypes.c_uint32(int.from_bytes(socket.inet_aton(ip_str), "little"))


def mac_to_value(mac_str):
    parts = [int(p, 16) for p in mac_str.split(":")]
    return MacAddr((ctypes.c_ubyte * 6)(*parts))


def load_trust_file(path):
    entries = []
    with open(path) as f:
        for raw in f:
            line = raw.strip()
            if not line or line.startswith("#"):
                continue
            parts = line.split()
            if len(parts) != 2:
                print(f"[!] Skipping malformed line: {raw!r}")
                continue
            entries.append((parts[0], parts[1]))
    return entries


def main():
    b = BPF(src_file="arp_guard.c")
    fn = b.load_func("arp_guard", BPF.XDP)

    trust = load_trust_file(TRUST_FILE)
    if not trust:
        print(f"[!] No trusted entries loaded from {TRUST_FILE}; nothing to guard.")
    for ip, mac in trust:
        b["trusted_macs"][ip_to_key(ip)] = mac_to_value(mac)
        print(f"[+] Trusted: {ip} -> {mac}")

    b.attach_xdp(INTERFACE, fn, BPF.XDP_FLAGS_SKB_MODE)
    print(f"[+] ARP Guard attached to {INTERFACE}. Press Ctrl+C to stop.")

    counter = b["drop_count"]
    idx = ctypes.c_uint32(0)
    last = 0
    try:
        while True:
            time.sleep(POLL_SEC)
            current = counter[idx].value
            if current != last:
                delta = current - last
                print(f"[!] Dropped {delta} spoofed ARP reply/replies (total: {current})")
                last = current
    except KeyboardInterrupt:
        pass
    finally:
        b.remove_xdp(INTERFACE, BPF.XDP_FLAGS_SKB_MODE)
        print(f"\n[+] Detached. Total spoofs blocked: {last}. Bye!")


if __name__ == "__main__":
    main()
