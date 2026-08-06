# Process Screens (job wizards & terminals)

**Authority for screen type:** [`docs/13_Design/Common/Screen_Types.md`](../../13_Design/Common/Screen_Types.md) § Create → Job CTA matrix  
**Job-first template:** [`JOB_FIRST_SCREEN_DESIGN.md`](../JOB_FIRST_SCREEN_DESIGN.md)

These PRDs replace shared **Create / Yeni** entity forms for operational jobs.

| File | CTA | Type |
|------|-----|------|
| [`INV_Receiving_Wizard.md`](./INV_Receiving_Wizard.md) | Receive goods / Mal kabul başlat | Wizard |
| [`INV_Issue_Wizard.md`](./INV_Issue_Wizard.md) | Issue goods / Mal çıkışı | Wizard |
| [`INV_Transfer_Wizard.md`](./INV_Transfer_Wizard.md) | Transfer stock / Stok transfer | Wizard |
| [`INV_Cycle_Count_Session.md`](./INV_Cycle_Count_Session.md) | Start count / Sayım başlat | Wizard |
| [`PRD_Production_Planning_Wizard.md`](./PRD_Production_Planning_Wizard.md) | Plan production / Üretim planla | Wizard |
| [`QLT_NCR_Wizard.md`](./QLT_NCR_Wizard.md) | Raise NCR / NCR aç | Wizard |
| [`MNT_Work_Order_Wizard.md`](./MNT_Work_Order_Wizard.md) | Open work order / İş emri aç | Wizard |
| [`PUR_Purchase_Order_Wizard.md`](./PUR_Purchase_Order_Wizard.md) | Place purchase order / Sipariş ver | Wizard |
| [`SAL_Sales_Order_Wizard.md`](./SAL_Sales_Order_Wizard.md) | Enter sales order / Sipariş gir | Wizard |

**FE rule:** CTA navigates here — never opens a shared `ResourcePage` / `EntityListScreen` create panel.
