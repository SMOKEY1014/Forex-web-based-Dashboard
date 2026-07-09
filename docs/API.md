# API Documentation

## Health
- `GET /health`

## Markets
- `GET /api/markets/latest` - latest snapshot by market
- `GET /api/markets/{market}/history?limit=60` - historical snapshots

## Watchlists
- `GET /api/watchlists`
- `PUT /api/watchlists/{market}`

## Auth
- `GET /api/auth/status`

## SignalR
- Hub: `/hubs/markets`
- Event: `market.snapshot`
