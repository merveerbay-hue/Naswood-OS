import { ResourcePage } from './ResourcePage';

/**
 * Work Center = üretim noktası (Kesim, Fırın, Planer, CNC…).
 * Depo veya stok lokasyonu değildir. WIP stok LocationType=WIP ile tutulur.
 */
export function WorkCenterPage() {
  return (
    <ResourcePage
      title="İş Merkezi (Work Center)"
      description="Üretim noktası masterı — depo/lokasyon değildir. Örnek: Kesim, Fırın, Planer, Finger Joint, Glulam Pres, CLT Pres, CNC, Paketleme. WIP stok ayrı Location kaydıdır."
      route="work-centers"
      kind="master"
      fields={[
        { key: 'Code', label: 'Kod', type: 'string' as const },
        { key: 'Name', label: 'Ad', type: 'string' as const },
        { key: 'CapacityPerHour', label: 'Kapasite/saat', type: 'number' as const },
        { key: 'PlantId', label: 'Tesis', type: 'string' as const },
        { key: 'Status', label: 'Durum', type: 'string' as const },
      ]}
    />
  );
}
