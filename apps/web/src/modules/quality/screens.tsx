import { Link } from '@tanstack/react-router';
import { useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { ProcessWizard } from '@/modules/shared/process/ProcessWizard';

function StubPage({
  screenId,
  title,
  desc,
}: {
  screenId: string;
  title: string;
  desc: string;
}) {
  const { t } = useI18n();
  return (
    <div className="space-y-3">
      <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
      <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
      <p className="text-sm text-[var(--text-secondary)]">{desc}</p>
      <p className="text-xs text-[var(--text-muted)]">{t('quality.stubHint')}</p>
    </div>
  );
}

export function InspectionStartPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="QLT-INSP-001"
      title={t('quality.inspTitle')}
      description={t('quality.inspDesc')}
      finishLabel={t('quality.inspFinish')}
      libraryPath="/quality/dashboard"
      libraryLabel={t('quality.backDash')}
      autoCodeHint="INSP-…"
      steps={[
        {
          title: t('quality.insp.ctx'),
          hint: t('quality.insp.ctxHint'),
          fields: [
            { key: 'source', label: t('quality.insp.source'), placeholder: 'GR / PO / WO…' },
            { key: 'material', label: t('quality.insp.material'), placeholder: t('wizard.nameFirstHint') },
          ],
        },
        {
          title: t('quality.insp.plan'),
          fields: [{ key: 'plan', label: t('quality.insp.plan'), placeholder: 'Incoming timber AQL…' }],
        },
        { title: t('quality.insp.sample'), hint: t('quality.insp.sampleHint') },
        { title: t('quality.insp.result'), fields: [{ key: 'result', label: t('quality.insp.result') }] },
        { title: t('quality.insp.disposition'), hint: t('quality.insp.dispositionHint') },
        { title: t('quality.inspFinish') },
      ]}
    />
  );
}

export function NcrWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="QLT-NCR-001"
      title={t('quality.ncrTitle')}
      description={t('quality.ncrDesc')}
      finishLabel={t('quality.ncrFinish')}
      libraryPath="/quality/operations/ncrs"
      libraryLabel={t('quality.ncrLibrary')}
      autoCodeHint="NCR-…"
      steps={[
        {
          title: t('quality.ncr.detect'),
          fields: [
            { key: 'where', label: t('quality.ncr.where'), placeholder: 'Incoming / Process / Final / Lab' },
            { key: 'mi', label: t('quality.ncr.mi'), placeholder: 'MI display (scan) — not typed invent' },
          ],
        },
        {
          title: t('quality.ncr.defect'),
          fields: [
            { key: 'defect', label: t('quality.ncr.defect') },
            { key: 'qty', label: t('quality.ncr.qty'), type: 'number' },
          ],
        },
        { title: t('quality.ncr.evidence'), hint: t('quality.ncr.evidenceHint') },
        {
          title: t('quality.ncr.disposition'),
          fields: [{ key: 'disp', label: t('quality.ncr.disposition'), placeholder: 'Hold / Scrap / Rework / Return…' }],
        },
        { title: t('quality.ncr.capa'), hint: t('quality.ncr.capaHint') },
        { title: t('quality.ncrFinish') },
      ]}
    />
  );
}

export function NcrLibraryPage() {
  const { t } = useI18n();
  const rows = [
    { code: 'NCR-2026-00041', mi: 'LOG-…-00045', status: 'Open', defect: 'Blue stain' },
    { code: 'NCR-2026-00038', mi: 'FG-TWDECK-…', status: 'Disposition', defect: 'Crack' },
    { code: 'NCR-2026-00022', mi: 'PKG-00254', status: 'Closed', defect: 'Moisture out' },
  ];
  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">QLT-NCR-LIB</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('quality.ncrLibTitle')}</h2>
          <p className="text-sm text-[var(--text-secondary)]">{t('quality.ncrLibDesc')}</p>
        </div>
        <Link
          to="/quality/operations/ncr"
          className="rounded-md bg-[var(--color-primary)] px-3 py-2 text-sm font-medium text-white"
        >
          {t('quality.jobNcr')}
        </Link>
      </div>
      <div className="overflow-x-auto rounded-lg border border-[var(--border-default)]">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-[var(--color-surface-hover)] text-xs uppercase text-[var(--text-muted)]">
            <tr>
              <th className="px-3 py-2">{t('quality.colCode')}</th>
              <th className="px-3 py-2">{t('quality.colMi')}</th>
              <th className="px-3 py-2">{t('quality.colDefect')}</th>
              <th className="px-3 py-2">{t('quality.colStatus')}</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.code} className="border-t border-[var(--border-default)]">
                <td className="px-3 py-2 font-mono text-xs font-medium">{r.code}</td>
                <td className="px-3 py-2 font-mono text-xs">{r.mi}</td>
                <td className="px-3 py-2">{r.defect}</td>
                <td className="px-3 py-2">{r.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export function HoldDeskPage() {
  const { t } = useI18n();
  const [mi, setMi] = useState('PKG-00254 / FG-TWDECK-…');
  const [action, setAction] = useState<'hold' | 'release' | 'scrap' | 'rework'>('hold');
  const [reason, setReason] = useState('');
  const [logged, setLogged] = useState<string[]>([]);

  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">QLT-HOLD</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('quality.holdTitle')}</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('quality.holdDesc')}</p>
      </div>
      <Card>
        <CardHeader>
          <CardTitle className="text-base">{t('quality.holdDecision')}</CardTitle>
          <CardDescription>{t('quality.holdLaw')}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          <label className="block space-y-1 text-sm">
            <span className="text-[var(--text-muted)]">{t('quality.holdTarget')}</span>
            <Input value={mi} onChange={(e) => setMi(e.target.value)} className="font-mono" />
          </label>
          <div className="flex flex-wrap gap-2">
            {(
              [
                ['hold', t('quality.disp.hold')],
                ['release', t('quality.disp.release')],
                ['scrap', t('quality.disp.scrap')],
                ['rework', t('quality.disp.rework')],
              ] as const
            ).map(([id, label]) => (
              <Button
                key={id}
                type="button"
                variant={action === id ? 'default' : 'secondary'}
                onClick={() => setAction(id)}
              >
                {label}
              </Button>
            ))}
          </div>
          <label className="block space-y-1 text-sm">
            <span className="text-[var(--text-muted)]">{t('quality.holdReason')}</span>
            <Input value={reason} onChange={(e) => setReason(e.target.value)} placeholder={t('quality.holdReasonPh')} />
          </label>
          <Button
            type="button"
            disabled={!reason.trim()}
            onClick={() => {
              setLogged((h) => [
                `${new Date().toISOString().slice(11, 19)} ${action.toUpperCase()} → ${mi} · ${reason}`,
                ...h,
              ]);
              setReason('');
            }}
          >
            {t('quality.holdPost')}
          </Button>
          <p className="text-xs text-[var(--text-muted)]">{t('quality.holdInventoryNote')}</p>
        </CardContent>
      </Card>
      {logged.length ? (
        <Card>
          <CardHeader>
            <CardTitle className="text-base">{t('quality.holdHistory')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1 font-mono text-xs text-[var(--text-secondary)]">
            {logged.map((l) => (
              <p key={l}>{l}</p>
            ))}
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}

export function CapaPage() {
  const { t } = useI18n();
  return (
    <StubPage screenId="QLT-CAPA-001" title={t('quality.capaTitle')} desc={t('quality.capaDesc')} />
  );
}

export function MoistureLabPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-LAB" title={t('quality.labTitle')} desc={t('quality.labDesc')} />;
}

export function TraceabilityPage() {
  const { t } = useI18n();
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">QLT-TRACE</p>
        <h2 className="text-xl font-semibold tracking-tight">{t('quality.traceTitle')}</h2>
        <p className="text-sm text-[var(--text-secondary)]">{t('quality.traceDesc')}</p>
      </div>
      <Card>
        <CardContent className="space-y-2 pt-4 text-sm text-[var(--text-secondary)]">
          <p>{t('quality.traceChain')}</p>
          <p className="font-mono text-xs">
            Supplier → Receiving → MI → Package → Issue / Production → FG → Customer
          </p>
          <p>{t('quality.traceLaw')}</p>
        </CardContent>
      </Card>
    </div>
  );
}

export function CocPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-COC" title={t('quality.cocTitle')} desc={t('quality.cocDesc')} />;
}

export function CertificatesPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-CERT" title={t('quality.certTitle')} desc={t('quality.certDesc')} />;
}

export function InspectionPlansPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-PLAN" title={t('quality.planTitle')} desc={t('quality.planDesc')} />;
}

export function SpecsPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-SPEC" title={t('quality.specTitle')} desc={t('quality.specDesc')} />;
}

export function QualityReportsPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-RPT" title={t('quality.rptTitle')} desc={t('quality.rptDesc')} />;
}

export function QualitySettingsPage() {
  const { t } = useI18n();
  return <StubPage screenId="QLT-SET" title={t('quality.setTitle')} desc={t('quality.setDesc')} />;
}
