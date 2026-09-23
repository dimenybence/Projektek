set -e

KVER=$(uname -r)

if [ -f /sys/kernel/kheaders.tar.xz ] && [ ! -e "/lib/modules/$KVER/build/include/linux/bpf.h" ]; then
    echo "[*] Extracting kernel headers for $KVER ..."
    mkdir -p "/lib/modules/$KVER/build"
    tar -xf /sys/kernel/kheaders.tar.xz -C "/lib/modules/$KVER/build"
fi

sysctl -wq net.ipv4.conf.eth0.arp_accept=1 || true
sysctl -wq net.ipv4.conf.all.arp_accept=1  || true

echo "[*] Starting ARP Guard ..."
exec python3 -u /work/loader.py
