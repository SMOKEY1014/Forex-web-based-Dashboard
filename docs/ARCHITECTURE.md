# Architecture Diagram

```text
Frontend (React 19)
  ├─ Pages/Components/Hooks/Store
  ├─ TanStack Query + SignalR Client
  └─ Recharts dashboard views

Backend (ASP.NET Core 9)
  ├─ Presentation (Controllers)
  ├─ Services (Market Intelligence + Watchlist)
  ├─ Infrastructure (Mongo Repositories + Provider Selection)
  ├─ BackgroundServices (30s refresh loop)
  └─ SignalR Hub (real-time updates)

MongoDB Atlas
  ├─ watchlists
  └─ marketSnapshots (indexed by market + updatedAt)
```
