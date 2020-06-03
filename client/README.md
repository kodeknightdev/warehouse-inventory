# Warehouse Pulse - client

Angular 9 front end for the Warehouse Pulse API.

## Running locally

Needs Node 12 LTS and the API running in IIS Express (`https://localhost:44358`, see `server/`).

```
npm install
npm start
```

Open http://localhost:4200. Calls to `/api` are proxied to the API by `proxy.conf.json`,
so no CORS setup is needed in dev.

## Pages

- **Dashboard** - totals, shipments by state, stock value per category
- **Inventory** - search / filter / page items, create + edit, item page with stock adjustments and history
- **Shipments** - list with status/carrier filters, new shipment form, detail page with tracking timeline
  and status updates

Low stock alerts are polled every minute and shown as a banner + the bell badge in the toolbar.

## Tests

```
npm test
```

Headless (no Chrome window, e.g. on a build agent):

```
npx ng test --watch=false --browsers=ChromeHeadlessCI
```

## Build

```
npm run build -- --prod
```

Output goes to `dist/warehouse-pulse`. In production the app expects the API on the same host under `/api`.
