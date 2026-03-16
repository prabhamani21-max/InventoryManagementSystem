# Exchange Schema Migration Plan

## Goal

Align exchange persistence with the product model:

- Save a locked valuation snapshot when an exchange order is created.
- Keep `exchange_order` as the transaction header.
- Keep `exchange_item` as the item-level valuation snapshot.
- Defer destructive schema cleanup until API and downstream consumers stop depending on redundant item fields.

## Deployment Order

1. Deploy the application code changes first.
2. Validate that new `exchange_order` and `exchange_item` rows are saving non-zero valuation fields.
3. Audit existing historical rows for missing snapshot values.
4. Apply optional schema cleanup only after confirming no consumers depend on item-level `status_id`, `updated_by`, or stored `total_deduction_percent`.

## Phase 1: No-Breaking-Change Release

This phase requires no DB change.

Expected behavior after deployment:

- `exchange_order.total_*` values are populated from the same valuation logic used by preview.
- `exchange_item.purity_percentage`, `pure_weight`, `current_rate_per_gram`, `market_value`, `deduction_amount`, and `credit_amount` are persisted at create time.
- order completion and cancellation keep item status aligned with the order status.

## Validation Queries

Use these queries after deployment to confirm new rows are correct:

```sql
select
    eo.id,
    eo.order_number,
    eo.total_market_value,
    eo.total_deduction_amount,
    eo.total_credit_amount,
    count(ei.id) as item_count,
    sum(ei.market_value) as item_market_value,
    sum(ei.deduction_amount) as item_deduction_amount,
    sum(ei.credit_amount) as item_credit_amount
from exchange_order eo
join exchange_item ei on ei.exchange_order_id = eo.id
group by eo.id, eo.order_number, eo.total_market_value, eo.total_deduction_amount, eo.total_credit_amount
order by eo.id desc;
```

```sql
select
    id,
    exchange_order_id,
    purity_percentage,
    pure_weight,
    current_rate_per_gram,
    market_value,
    deduction_amount,
    credit_amount
from exchange_item
where created_date >= now() - interval '7 days'
order by id desc;
```

## Historical Data Audit

Rows created before the code fix may contain zeroed snapshot fields. Identify them with:

```sql
select
    id,
    exchange_order_id,
    purity_id,
    purity_percentage,
    pure_weight,
    current_rate_per_gram,
    market_value,
    deduction_amount,
    credit_amount
from exchange_item
where purity_percentage = 0
   or pure_weight = 0
   or current_rate_per_gram = 0
   or market_value = 0
   or credit_amount = 0
order by id;
```

Do not backfill these rows from current metal rates unless the business accepts that historical values will become approximations.

## Phase 2: Optional Schema Cleanup

Apply this only after:

- API contracts stop exposing item-level `status_id`, or clients no longer depend on it.
- the application stops reading or writing `exchange_item.updated_by`.
- the application derives `total_deduction_percent` at runtime instead of storing it.

### Target Cleanup

- drop `exchange_item.total_deduction_percent`
- drop `exchange_item.status_id`
- drop `exchange_item.updated_by`

### PostgreSQL SQL

```sql
begin;

drop index if exists "IX_exchange_item_status_id";
drop index if exists "IX_exchange_item_updated_by";

alter table exchange_item drop constraint if exists "FK_exchange_item_generic_status_status_id";
alter table exchange_item drop constraint if exists "FK_exchange_item_users_updated_by";

alter table exchange_item drop column if exists total_deduction_percent;
alter table exchange_item drop column if exists status_id;
alter table exchange_item drop column if exists updated_by;

commit;
```

## Rollback

If Phase 1 code deployment causes issues, rollback the application only. No schema rollback is required.

If Phase 2 schema cleanup is applied, rollback requires re-adding the dropped columns and constraints before running the older application build.
