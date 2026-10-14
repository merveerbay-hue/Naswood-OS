# Preview — bu run

Uygulama **agent VM içinde** çalışıyor (senin laptop’unda değil).

| Servis | Adres (VM) | Durum |
|--------|------------|--------|
| Vite   | `0.0.0.0:5173` | Login UI |
| API    | `0.0.0.0:5080` | `/health` → Healthy |

Worktree: `/tmp/naswood-web` · Login: `admin` / `Naswood!Admin1`

## ERR_CONNECTION_REFUSED / “Disconnected” ne demek?

Simple Browser’da **Disconnected** görüyorsan Cursor, bu cloud agent’a tunnel kurmamış demektir.
O anda `http://localhost:5173` **senin bilgisayarındaki** boş porta gider → `ERR_CONNECTION_REFUSED`.
Agent’ta Vite ayakta olsa bile yenilemek bunu düzeltmez.

## Doğru açılış (Cursor Desktop · Agents Window)

1. Sol/üst **Agents** listesinden bu run’ı aç:  
   [NOS architectural principles](https://cursor.com/agents/bc-45ccde4c-87aa-44c8-820c-d438da7ace37)
2. Sekme **aktif** olsun; üstte **Disconnected** yazıyorsa önce bağlantı gelsin (yenile / agent’a tekrar tıkla).
3. Editör panelinin **sağ üstündeki fiş (plug) ikonu** → Ports.
4. Listede **5173** (ve tercihen **5080**) görünsün.
   - Yoksa: **Forward a Port** → `5173`
   - Varsa: satıra sağ tık → **Open in Browser** (elle `localhost` yazma)
5. Başka cloud agent sekmeleri açıksa **yalnızca aktif sekme** `localhost:5173` forward’unu alır; diğer sekmeleri kapat veya plug menüsünden stale forward’ları Stop et.

## Hâlâ Disconnected ise

- Bu agent’ı kapatıp aynı linkten yeniden aç.
- Cursor’ı tamamen kapat/aç (tunnel client tarafında).
- `remote.autoForwardPorts` kapalıysa aç (Settings).
- Kod/sunucu sorunu değil: VM’de 5173/5080 şu an Healthy.
