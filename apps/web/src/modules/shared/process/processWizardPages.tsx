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
      steps={[
        { title: t('wizard.rcv.po'), hint: t('wizard.rcv.poHint') },
        { title: t('wizard.rcv.lines'), hint: t('wizard.rcv.linesHint') },
        { title: t('wizard.rcv.qty'), hint: t('wizard.rcv.qtyHint') },
        { title: t('wizard.rcv.wh'), hint: t('wizard.rcv.whHint') },
        { title: t('wizard.rcv.loc'), hint: t('wizard.rcv.locHint') },
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
      steps={[
        { title: t('wizard.iss.ref') },
        { title: t('wizard.iss.lines') },
        { title: t('wizard.iss.source') },
        { title: t('wizard.iss.lot') },
        { title: t('wizard.iss.qty') },
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
      steps={[
        { title: t('wizard.trf.material') },
        { title: t('wizard.trf.from') },
        { title: t('wizard.trf.to') },
        { title: t('wizard.trf.qty') },
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
      steps={[
        { title: t('wizard.cnt.scope') },
        { title: t('wizard.cnt.open') },
        { title: t('wizard.cnt.count') },
        { title: t('wizard.cnt.variance') },
        { title: t('wizard.cnt.close') },
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
      steps={[
        { title: t('wizard.prd.product') },
        { title: t('wizard.prd.revision') },
        { title: t('wizard.prd.dims') },
        { title: t('wizard.prd.tech') },
        { title: t('wizard.prd.wood') },
        { title: t('wizard.prd.bom') },
        { title: t('wizard.prd.routing') },
        { title: t('wizard.prd.line') },
        { title: t('wizard.prd.capacity') },
        { title: t('wizard.prd.due') },
        { title: t('wizard.prd.release') },
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
      steps={[
        { title: t('wizard.pur.supplier') },
        { title: t('wizard.pur.source') },
        { title: t('wizard.pur.lines') },
        { title: t('wizard.pur.wh') },
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
      steps={[
        { title: t('wizard.sal.customer') },
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
