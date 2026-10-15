import { FOREIGN_SITE_MESSAGE, parseIdentityCode, resolveScan } from './identityCode';

function assert(cond: unknown, msg: string): void {
  if (!cond) throw new Error(msg);
}

for (const code of ['PKG-BUC-000001', 'LOT-BUC-000001', 'LAM-BUC-000001', 'AUTO-BUC-000001', 'TRF-BUC-000001', 'REST-BUC-000001']) {
  const p = parseIdentityCode(code);
  assert(p && p.isLocal && p.sequence === 1 && p.hasSitePrefix, `TEST1 local ${code}`);
}
const ant = parseIdentityCode('PKG-000245');
assert(ant && ant.siteCode === 'ANT' && !ant.hasSitePrefix && !ant.isLocal, 'TEST2 unprefixed is Antalya');
assert(parseIdentityCode('pkg-buc-1234567')?.sequence === 1234567, 'TEST3 lower case, 7 digits');
for (const bad of ['', 'PKG-BUC-00001', 'NW-PKG-F01-26-000001', 'NWPKG-F01-26-000001', 'LOT-OPEN-F01-260910-0001', 'LOT-PR-F01-260910-0001', 'LOT-2026-00101', 'PKG-IST-000001', 'https://x/inventory/packages/p/abc']) {
  assert(parseIdentityCode(bad) === null, `TEST4 reject ${bad}`);
}
assert(resolveScan('PKG-BUC-000001').kind === 'package', 'TEST5 package');
assert(resolveScan('LOT-BUC-000001').kind === 'lot', 'TEST6 lot');
assert(resolveScan('TRF-BUC-000001').kind === 'transfer', 'TEST7 transfer');
assert(resolveScan('LAM-BUC-000001').kind === 'unsupported', 'TEST8 lam not in NOS yet');
for (const foreign of ['PKG-000245', 'PKG-ANT-000245', 'LOT-000123']) {
  const r = resolveScan(foreign);
  assert(r.kind === 'foreign' && r.message === FOREIGN_SITE_MESSAGE, `TEST9 foreign ${foreign}`);
}
assert(resolveScan('NWPKG-F01-26-000001').kind === 'invalid', 'TEST10 invalid');
console.log('identityCode selftest OK');
