/**
 * Bulk material-card Excel/CSV template contract.
 * Run: npx tsx apps/web/src/modules/inventory/materials/materialCardBulk.selftest.ts
 */
import {
  EXAMPLE_TEMPLATE_ROWS,
  MATERIAL_CARD_TEMPLATE_COLUMNS,
  buildCsvTemplate,
  buildSpreadsheetMlTemplate,
  parseDelimitedTable,
  parseSpreadsheetMl,
  parseBoolCell,
  resolveTemplateColumn,
  rowToDraft,
  tableToDrafts,
} from './materialCardBulk';

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

{
  assert(MATERIAL_CARD_TEMPLATE_COLUMNS.includes('MainCategory'), 'main col');
  assert(!(MATERIAL_CARD_TEMPLATE_COLUMNS as readonly string[]).includes('MaterialCode'), 'no code col');
  assert(resolveTemplateColumn('Ürün grubu') === 'MainCategory', 'tr header');
  assert(resolveTemplateColumn('cins') === 'MaterialType', 'alias');
  assert(parseBoolCell('Evet', false) === true, 'bool evet');
  assert(parseBoolCell('Hayır', true) === false, 'bool hayir');
  console.log('TEST1 OK — template columns + aliases, no MaterialCode');
}

{
  const draft = rowToDraft({
    MainCategory: 'HM',
    MaterialType: 'KR',
    WoodToken: 'PIN',
    ThicknessMm: '50',
    WidthMode: 'single',
    WidthMm: '100',
    LengthMm: '4000',
    StockUom: 'M3',
    CountUom: 'PCS',
  });
  assert(draft.name === 'Çam Kereste', `name ${draft.name}`);
  assert(draft.isThermowood === false, 'thermo');
  const rows = tableToDrafts(EXAMPLE_TEMPLATE_ROWS, [], []);
  const ok = rows.filter((r) => r.ok);
  assert(ok.length === 3, `examples ${ok.length}`);
  assert(ok[0].ok && ok[0].previewCode === 'HM-KR-PIN-001', ok[0].ok ? ok[0].previewCode : 'fail');
  assert(ok[1].ok && ok[1].previewCode === 'HM-KR-PIN-002', 'second size next seq');
  assert(ok[2].ok && ok[2].previewCode === 'YM-LM-PIN-S-001', ok[2].ok ? ok[2].previewCode : 'fail');
  console.log('TEST2 OK — example rows mint sequential codes');
}

{
  const dupTable = [
    [...MATERIAL_CARD_TEMPLATE_COLUMNS],
    EXAMPLE_TEMPLATE_ROWS[1],
    EXAMPLE_TEMPLATE_ROWS[1],
  ];
  const rows = tableToDrafts(dupTable, [], []);
  assert(rows[0].ok === true, 'first ok');
  assert(rows[1].ok === false, 'intra-file duplicate blocked');
  console.log('TEST3 OK — intra-file duplicate blocked');
}

{
  const csv = buildCsvTemplate();
  const table = parseDelimitedTable(csv);
  assert(table[0][1] === 'MainCategory', `header ${table[0][1]}`);
  const xml = buildSpreadsheetMlTemplate();
  assert(xml.includes('ss:Name="Kartlar"'), 'kartlar sheet');
  assert(xml.includes('ss:Name="Kodlar"'), 'kodlar sheet');
  assert(!xml.includes('MaterialCode</Data>') || xml.includes('YAZMAYIN'), 'code not a data column');
  const fromXml = parseSpreadsheetMl(xml);
  assert(fromXml[0][0] === 'Name', 'xml header');
  assert(fromXml.length >= 4, 'xml example rows');
  console.log('TEST4 OK — csv + SpreadsheetML round-trip');
}

{
  const thermo = rowToDraft({
    MainCategory: 'HM',
    MaterialType: 'KR',
    WoodToken: 'PIN',
    IsThermowood: 'Evet',
    ThicknessMm: '50',
    WidthMm: '100',
    LengthMm: '4000',
  });
  const rows = tableToDrafts(
    [
      [...MATERIAL_CARD_TEMPLATE_COLUMNS],
      [
        '',
        'HM',
        'KR',
        'PIN',
        'Evet',
        '',
        '',
        '50',
        'single',
        '100',
        '',
        '',
        '',
        '4000',
        'M3',
        'PCS',
        'Evet',
        'Evet',
        'Active',
        '',
        'NORMAL_STOCK',
        '',
      ],
    ],
    [],
    [],
  );
  assert(thermo.isThermowood, 'thermo flag');
  assert(rows[0].ok && rows[0].previewCode === 'HM-KR-TPIN-001', rows[0].ok ? rows[0].previewCode : 'fail');
  console.log('TEST5 OK — thermowood T-prefix on minted code');
}

console.log('materialCardBulk.selftest: ALL OK');
