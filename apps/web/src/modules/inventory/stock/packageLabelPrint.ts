import type { PackagePassport } from './packagePassportApi';

function esc(s: string) {
  return s.replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!));
}

export function buildPackageLabelHtml(docs: PackagePassport[], printedAt = new Date()) {
  const when = printedAt.toLocaleString('tr-TR', { timeZone: 'Europe/Istanbul' });
  const cards = docs
    .map((p) => {
      const qr = encodeURIComponent(`${window.location.origin}${p.qrPath}`);
      const rows = (p.contents ?? []).slice(0, 4);
      const extra = (p.contents?.length ?? 0) > 4 ? `<div class="muted">+${p.contents.length - 4} ölçü — QR ile detay</div>` : '';
      const measures = rows
        .map(
          (c) =>
            `<tr><td>${esc(c.measurement || '—')}</td><td>${c.pieceCount ?? '—'}</td><td>${c.quantity} ${esc(c.unitOfMeasure)}</td></tr>`,
        )
        .join('');
      return `<article class="label">
        <header>NASWOOD</header>
        <h1>${esc(p.materialName || p.materialCode)}</h1>
        <div class="code">${esc(p.materialCode)}</div>
        <div class="meta">${esc([p.woodSpecies, p.materialType, p.quality].filter(Boolean).join(' · ') || '—')}</div>
        <table>${measures || `<tr><td>—</td><td>—</td><td>${p.quantity} ${esc(p.unitOfMeasure)}</td></tr>`}</table>
        ${extra}
        <div class="total">TOPLAM ${p.totalPieceCount != null ? `${p.totalPieceCount} PCS · ` : ''}${p.quantity} ${esc(p.stockUnit || p.unitOfMeasure)}</div>
        <div class="ids">LOT ${esc(p.lotNumber)}<br/>PAKET ${esc(p.packageNo)}</div>
        <div class="loc">${esc(p.factory)} · ${esc(p.warehouseCode)} / ${esc(p.locationCode)} · ${esc(p.status)}</div>
        <div class="bc">${esc(p.barcode)}</div>
        <img alt="QR" src="https://api.qrserver.com/v1/create-qr-code/?size=110x110&data=${qr}" />
        <footer>Etiket: ${esc(when)}</footer>
      </article>`;
    })
    .join('');
  return `<!doctype html><html><head><meta charset="utf-8"><title>NASWOOD etiket</title>
<style>
@page { size: 100mm 70mm; margin: 3mm; }
body { font-family: Arial, sans-serif; margin: 0; color: #111; }
.label { width: 94mm; min-height: 64mm; border: 1px solid #111; padding: 3mm; page-break-after: always; }
header { font-size: 10px; letter-spacing: .2em; }
h1 { font-size: 16px; margin: 2px 0; text-transform: uppercase; }
.code { font-family: monospace; font-size: 12px; }
.meta, .loc, .muted, footer { font-size: 10px; }
table { width: 100%; font-size: 11px; margin: 3px 0; }
.total, .ids { font-size: 11px; font-weight: 700; }
.bc { font-family: monospace; font-size: 13px; letter-spacing: .08em; margin: 4px 0; }
img { width: 22mm; height: 22mm; }
</style></head><body>${cards}<script>window.onload=()=>window.print()</script></body></html>`;
}

export function printPackageLabels(docs: PackagePassport[]) {
  const w = window.open('', '_blank', 'noopener,noreferrer,width=480,height=640');
  if (!w) throw new Error('Yazdırma penceresi engellendi.');
  w.document.write(buildPackageLabelHtml(docs));
  w.document.close();
}
