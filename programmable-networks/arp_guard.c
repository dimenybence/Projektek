#include <linux/bpf.h>
#include <linux/if_ether.h>
#include <linux/if_arp.h>
#include <bcc/proto.h>

struct mac_addr {
    __u8 addr[6];
};

BPF_HASH(trusted_macs, __u32, struct mac_addr);
BPF_ARRAY(drop_count, __u64, 1);

int arp_guard(struct xdp_md *ctx) {
    void *data     = (void *)(long)ctx->data;
    void *data_end = (void *)(long)ctx->data_end;

    struct ethhdr *eth = data;
    // Bounds check
    if ((void *)(eth + 1) > data_end)
        return XDP_PASS;
    // Ez ARP csomag? -- NEM --► PASS
    if (eth->h_proto != htons(ETH_P_ARP))
        return XDP_PASS;

    struct arphdr *arp = (void *)(eth + 1);
    // Bounds check
    if ((void *)(arp + 1) > data_end)
        return XDP_PASS;
    // Ez ARP válasz (reply)? -- NEM --► PASS
    if (arp->ar_op != htons(ARPOP_REPLY))
        return XDP_PASS;

    unsigned char *payload = (unsigned char *)(arp + 1);
    // Bounds check
    if (payload + 20 > (unsigned char *)data_end)
        return XDP_PASS;

    __u8  *sender_mac = payload;
    __u32 *sender_ip  = (__u32 *)(payload + 6);

    // Az IP a megbízható listában? -- NEM --► PASS
    struct mac_addr *trusted = trusted_macs.lookup(sender_ip);
    if (!trusted)
        return XDP_PASS;

    for (int i = 0; i < 6; i++) {
        if (trusted->addr[i] != sender_mac[i]) {
            __u32 idx = 0;
            __u64 *cnt = drop_count.lookup(&idx);
            if (cnt)
                __sync_fetch_and_add(cnt, 1);
            // A MAC egyezik a bizalmival? -- NEM --► DROP + számlál
            return XDP_DROP;
        }
    }
    // A MAC egyezik a bizalmival? -- IGEN --► PASS
    return XDP_PASS;
}