# Installation Guide

## Prerequisites
- Node.js 22+
- .NET SDK 9
- MongoDB Atlas URI (or local MongoDB)

## Setup
1. Create `.env` from `.env.example`.
2. Set `MONGODB_CONNECTION_STRING`, `MONGODB_DATABASE_NAME`, and API keys.
3. Backend:
   ```bash
   cd backend
   dotnet restore
   dotnet run
   ```
4. Frontend:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
