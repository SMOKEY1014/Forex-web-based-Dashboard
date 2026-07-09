# Market Pulse Dashboard

Production-ready starter for a **real-time market intelligence dashboard** with:
- **Frontend**: React 19 + TypeScript + Vite + Tailwind + TanStack Query + Zustand + Recharts + SignalR client
- **Backend**: ASP.NET Core 9 Web API + SignalR + Background Services + MongoDB Repository/Service layers
- **Data**: MongoDB Atlas-compatible document model for watchlists and historical snapshots

## Quick Start

1. Copy `.env.example` to `.env` and fill API/Firebase/Mongo values.
2. Start backend:
   ```bash
   cd backend
   dotnet run
   ```
3. Start frontend:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

Or run with Docker:
```bash
docker compose up --build
```

## Documentation

- [Installation Guide](docs/INSTALLATION.md)
- [Deployment Guide](docs/DEPLOYMENT.md)
- [API Documentation](docs/API.md)
- [Architecture Diagram](docs/ARCHITECTURE.md)
- [Folder Structure](docs/FOLDER_STRUCTURE.md)
- [.env Guide](docs/ENV_GUIDE.md)

## Key Features Implemented

- Configuration-driven market support (`US100`, `US30`, `GER40`, `XAUUSD`)
- Official-weight-based contribution calculations
- Real-time SignalR broadcast (`market.snapshot`)
- Historical snapshot persistence in MongoDB
- Firebase JWT-ready authentication setup with guest mode support
- Background refresh loop with configurable interval
- CI workflow, Dockerfiles, docker-compose, Render/Vercel config
