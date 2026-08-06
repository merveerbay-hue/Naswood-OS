import { useI18n } from '@/i18n';
import { ProcessWizard } from './ProcessWizard';

export function ReceivingWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-RCV-001"
      title={t('wizard.receivingTitle')}
      description={t('wizard.receivingDesc')}
      finishLabel={t('wizard.post')}
      libraryPath="/inventory/operations/goods-receipts"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="GR-… · LOT-…"
      steps={[
        {
          title: t('wizard.rcv.po'),
          hint: t('wizard.rcv.poHint'),
          fields: [{ key: 'po', label: t('wizard.rcv.po'), placeholder: 'PO-2026-… (referans)' }],
        },
        { title: t('wizard.rcv.lines'), hint: t('wizard.rcv.linesHint') },
        {
          title: t('wizard.rcv.qty'),
          hint: t('wizard.rcv.qtyHint'),
          fields: [{ key: 'qty', label: t('wizard.rcv.qty'), type: 'number' }],
        },
        {
          title: t('wizard.rcv.wh'),
          hint: t('wizard.rcv.whHint'),
          fields: [{ key: 'wh', label: t('wizard.rcv.wh'), placeholder: 'Ana Depo' }],
        },
        {
          title: t('wizard.rcv.loc'),
          hint: t('wizard.rcv.locHint'),
          fields: [{ key: 'loc', label: t('wizard.rcv.loc'), placeholder: 'A-01' }],
        },
        { title: t('wizard.rcv.lot'), hint: t('wizard.rcv.lotHint') },
        { title: t('wizard.rcv.qi'), hint: t('wizard.rcv.qiHint') },
        { title: t('wizard.rcv.label'), hint: t('wizard.rcv.labelHint') },
        { title: t('wizard.rcv.post'), hint: t('wizard.rcv.postHint') },
      ]}
    />
  );
}

export function IssueWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-ISS-001"
      title={t('wizard.issueTitle')}
      description={t('wizard.issueDesc')}
      finishLabel={t('wizard.post')}
      libraryPath="/inventory/operations/goods-issues"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="GI-…"
      steps={[
        { title: t('wizard.iss.ref'), fields: [{ key: 'ref', label: t('wizard.iss.ref') }] },
        { title: t('wizard.iss.lines') },
        { title: t('wizard.iss.source'), fields: [{ key: 'wh', label: t('wizard.iss.source'), placeholder: 'Ana Depo' }] },
        { title: t('wizard.iss.lot'), hint: t('wizard.nameFirstHint') },
        { title: t('wizard.iss.qty'), fields: [{ key: 'qty', label: t('wizard.iss.qty'), type: 'number' }] },
        { title: t('wizard.post') },
      ]}
    />
  );
}

export function TransferWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-TRF-001"
      title={t('wizard.transferTitle')}
      description={t('wizard.transferDesc')}
      finishLabel={t('wizard.post')}
      libraryPath="/inventory/operations/transfers"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="TR-…"
      steps={[
        { title: t('wizard.trf.material'), fields: [{ key: 'mat', label: t('wizard.trf.material'), placeholder: t('wizard.nameFirstHint') }] },
        { title: t('wizard.trf.from'), fields: [{ key: 'from', label: t('wizard.trf.from') }] },
        { title: t('wizard.trf.to'), fields: [{ key: 'to', label: t('wizard.trf.to') }] },
        { title: t('wizard.trf.qty'), fields: [{ key: 'qty', label: t('wizard.trf.qty'), type: 'number' }] },
        { title: t('wizard.post') },
      ]}
    />
  );
}

export function CycleCountWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-CNT-001"
      title={t('wizard.countTitle')}
      description={t('wizard.countDesc')}
      finishLabel={t('wizard.closeSession')}
      libraryPath="/inventory/counts/cycle-counts"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="CNT-…"
      steps={[
        { title: t('wizard.cnt.scope'), fields: [{ key: 'scope', label: t('wizard.cnt.scope'), placeholder: 'Ana Depo / Zone A' }] },
        { title: t('wizard.cnt.open') },
        { title: t('wizard.cnt.count') },
        { title: t('wizard.cnt.variance') },
        { title: t('wizard.cnt.close') },
      ]}
    />
  );
}

export function MaterialDefinePage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-MAT-001"
      title={t('wizard.materialTitle')}
      description={t('wizard.materialDesc')}
      finishLabel={t('wizard.saveRelease')}
      libraryPath="/inventory/master-data/materials"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="MAT-…"
      steps={[
        {
          title: t('wizard.mat.general'),
          fields: [
            { key: 'name', label: t('wizard.mat.name'), placeholder: 'Oak Lamelle 26×140' },
            { key: 'type', label: t('wizard.mat.type') },
            { key: 'group', label: t('wizard.mat.group') },
          ],
        },
        {
          title: t('wizard.mat.species'),
          fields: [
            { key: 'species', label: t('wizard.mat.species') },
            { key: 'dims', label: t('wizard.mat.dims'), placeholder: '26×140×3000' },
            { key: 'uom', label: t('wizard.mat.uom'), placeholder: 'm³' },
          ],
        },
        { title: t('wizard.mat.capability'), fields: [{ key: 'cap', label: t('wizard.mat.capability') }] },
        { title: t('wizard.saveRelease') },
      ]}
    />
  );
}

export function WarehouseDefinePage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="INV-WH-001"
      title={t('wizard.warehouseTitle')}
      description={t('wizard.warehouseDesc')}
      finishLabel={t('wizard.saveRelease')}
      libraryPath="/inventory/master-data/warehouses"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="WH-…"
      steps={[
        {
          title: t('wizard.wh.general'),
          fields: [
            { key: 'name', label: t('wizard.wh.name'), placeholder: 'Ana Depo' },
            { key: 'type', label: t('wizard.wh.type') },
            { key: 'plant', label: t('wizard.wh.plant') },
            { key: 'owner', label: t('wizard.wh.owner') },
          ],
        },
        { title: t('wizard.saveRelease') },
      ]}
    />
  );
}

export function PlanningWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-101"
      title={t('wizard.planningTitle')}
      description={t('wizard.planningDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/planning/orders"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="PO-2026-…"
      steps={[
        {
          title: t('wizard.prd.product'),
          hint: t('wizard.nameFirstHint'),
          fields: [{ key: 'product', label: t('wizard.prd.product'), placeholder: 'Thermowood Deck 26×140×3000' }],
        },
        { title: t('wizard.prd.revision'), fields: [{ key: 'rev', label: t('wizard.prd.revision') }] },
        { title: t('wizard.prd.dims'), fields: [{ key: 'dims', label: t('wizard.prd.dims') }] },
        { title: t('wizard.prd.tech') },
        { title: t('wizard.prd.wood'), fields: [{ key: 'wood', label: t('wizard.prd.wood') }] },
        { title: t('wizard.prd.bom') },
        { title: t('wizard.prd.routing') },
        { title: t('wizard.prd.line'), fields: [{ key: 'line', label: t('wizard.prd.line') }] },
        { title: t('wizard.prd.capacity') },
        { title: t('wizard.prd.due'), fields: [{ key: 'due', label: t('wizard.prd.due'), type: 'date' }] },
        { title: t('wizard.prd.release') },
      ]}
    />
  );
}

export function BomBuilderPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-501"
      title={t('wizard.bomTitle')}
      description={t('wizard.bomDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/boms"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="BOM-…"
      steps={[
        {
          title: t('wizard.bom.product'),
          hint: t('wizard.nameFirstHint'),
          fields: [{ key: 'product', label: t('wizard.bom.product'), placeholder: 'Thermowood Deck 26×140×3000' }],
        },
        { title: t('wizard.bom.revision'), fields: [{ key: 'rev', label: t('wizard.bom.revision') }] },
        { title: t('wizard.bom.tree'), hint: t('wizard.bom.treeHint') },
        { title: t('wizard.bom.alt') },
        { title: t('wizard.bom.scrap'), fields: [{ key: 'scrap', label: t('wizard.bom.scrap'), type: 'number', placeholder: '%' }] },
        { title: t('wizard.bom.ops') },
        { title: t('wizard.bom.compare') },
        { title: t('wizard.bom.impact') },
        { title: t('wizard.bom.approve') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function RoutingDesignerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-502"
      title={t('wizard.routingTitle')}
      description={t('wizard.routingDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/routings"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="RT-…"
      steps={[
        { title: t('wizard.rt.flow') },
        { title: t('wizard.rt.machine'), fields: [{ key: 'mc', label: t('wizard.rt.machine'), placeholder: t('wizard.nameFirstHint') }] },
        { title: t('wizard.rt.wc'), fields: [{ key: 'wc', label: t('wizard.rt.wc') }] },
        { title: t('wizard.rt.setup'), fields: [{ key: 'setup', label: t('wizard.rt.setup'), type: 'number' }] },
        { title: t('wizard.rt.cycle'), fields: [{ key: 'cycle', label: t('wizard.rt.cycle'), type: 'number' }] },
        { title: t('wizard.rt.labor') },
        { title: t('wizard.rt.qc') },
        { title: t('wizard.rt.parallel') },
        { title: t('wizard.rt.sim') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function MachineStudioPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-503"
      title={t('wizard.machineTitle')}
      description={t('wizard.machineDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/machines"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="MC-…"
      steps={[
        {
          title: t('wizard.mc.general'),
          fields: [
            { key: 'name', label: t('wizard.mc.name'), placeholder: 'Weinmann WBS 120' },
            { key: 'mfr', label: t('wizard.mc.mfr') },
            { key: 'model', label: t('wizard.mc.model') },
          ],
        },
        { title: t('wizard.mc.tech'), fields: [{ key: 'axes', label: t('wizard.mc.axes'), type: 'number' }] },
        {
          title: t('wizard.mc.capacity'),
          fields: [
            { key: 'w', label: t('wizard.mc.maxW'), type: 'number' },
            { key: 'h', label: t('wizard.mc.maxH'), type: 'number' },
            { key: 't', label: t('wizard.mc.maxT'), type: 'number' },
          ],
        },
        { title: t('wizard.mc.ops') },
        { title: t('wizard.mc.magazine') },
        { title: t('wizard.mc.maint') },
        { title: t('wizard.mc.sensors') },
        { title: t('wizard.mc.docs') },
        { title: t('wizard.mc.iot') },
        { title: t('wizard.mc.commission') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function WorkCenterDesignerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-504"
      title={t('wizard.wcTitle')}
      description={t('wizard.wcDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/work-centers"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="WC-…"
      steps={[
        { title: t('wizard.wc.line'), fields: [{ key: 'line', label: t('wizard.wc.line') }] },
        { title: t('wizard.wc.layout') },
        { title: t('wizard.wc.capacity'), fields: [{ key: 'cap', label: t('wizard.wc.capacity'), type: 'number' }] },
        { title: t('wizard.wc.rules') },
        { title: t('wizard.wc.ops') },
        { title: t('wizard.wc.calendar') },
        { title: t('wizard.wc.shifts') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function LineDesignerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-505"
      title={t('wizard.lineTitle')}
      description={t('wizard.lineDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/lines"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="LINE-…"
      steps={[
        { title: t('wizard.line.def'), fields: [{ key: 'name', label: t('wizard.line.name'), placeholder: 'Profil Hattı 1' }] },
        { title: t('wizard.line.stations') },
        { title: t('wizard.line.machines') },
        { title: t('wizard.line.flow') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function OperationDesignerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-506"
      title={t('wizard.opTitle')}
      description={t('wizard.opDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/operations"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="OP-…"
      steps={[
        { title: t('wizard.op.def'), fields: [{ key: 'name', label: t('wizard.op.name') }] },
        { title: t('wizard.op.params') },
        { title: t('wizard.op.machines') },
        { title: t('wizard.op.tool') },
        { title: t('wizard.op.qc') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function ShiftPlannerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-507"
      title={t('wizard.shiftTitle')}
      description={t('wizard.shiftDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/shifts"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="SHIFT-…"
      steps={[
        { title: t('wizard.shift.template'), fields: [{ key: 'name', label: t('wizard.shift.template'), placeholder: 'Gündüz' }] },
        { title: t('wizard.shift.hours'), fields: [{ key: 'from', label: 'Başlangıç' }, { key: 'to', label: 'Bitiş' }] },
        { title: t('wizard.shift.breaks') },
        { title: t('wizard.shift.operators') },
        { title: t('wizard.shift.wc') },
        { title: t('wizard.shift.calendar') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function CalendarPlannerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-508"
      title={t('wizard.calTitle')}
      description={t('wizard.calDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/calendars"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="CAL-…"
      steps={[
        { title: t('wizard.cal.work') },
        { title: t('wizard.cal.holiday') },
        { title: t('wizard.cal.maint') },
        { title: t('wizard.cal.ot') },
        { title: t('wizard.cal.close') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function ToolLibraryManagerPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PRD-509"
      title={t('wizard.toolTitle')}
      description={t('wizard.toolDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/production/master-data/toolings"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="TL-…"
      steps={[
        { title: t('wizard.tool.type'), fields: [{ key: 'name', label: t('wizard.tool.type') }] },
        { title: t('wizard.tool.machines') },
        { title: t('wizard.tool.life'), fields: [{ key: 'life', label: t('wizard.tool.life'), type: 'number' }] },
        { title: t('wizard.tool.rev') },
        { title: t('wizard.tool.stock') },
        { title: t('wizard.tool.cal') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function PurchaseOrderWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="PUR-PO-001"
      title={t('wizard.poTitle')}
      description={t('wizard.poDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/purchasing/purchase-orders"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="PO-…"
      steps={[
        { title: t('wizard.pur.supplier'), fields: [{ key: 'sup', label: t('wizard.pur.supplier'), placeholder: t('wizard.nameFirstHint') }] },
        { title: t('wizard.pur.source') },
        { title: t('wizard.pur.lines') },
        { title: t('wizard.pur.wh'), fields: [{ key: 'wh', label: t('wizard.pur.wh'), placeholder: 'Ana Depo' }] },
        { title: t('wizard.pur.terms') },
        { title: t('wizard.pur.approve') },
        { title: t('wizard.release') },
      ]}
    />
  );
}

export function SalesOrderWizardPage() {
  const { t } = useI18n();
  return (
    <ProcessWizard
      screenId="SAL-SO-001"
      title={t('wizard.soTitle')}
      description={t('wizard.soDesc')}
      finishLabel={t('wizard.release')}
      libraryPath="/sales/sales-orders"
      libraryLabel={t('wizard.backToLibrary')}
      autoCodeHint="SO-…"
      steps={[
        { title: t('wizard.sal.customer'), fields: [{ key: 'cus', label: t('wizard.sal.customer'), placeholder: t('wizard.nameFirstHint') }] },
        { title: t('wizard.sal.source') },
        { title: t('wizard.sal.lines') },
        { title: t('wizard.sal.atp') },
        { title: t('wizard.sal.reserve') },
        { title: t('wizard.sal.ship') },
        { title: t('wizard.release') },
      ]}
    />
  );
}
