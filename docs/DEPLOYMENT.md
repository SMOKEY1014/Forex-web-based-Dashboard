# Deployment Guide

## Backend (Render)
- Uses `render.yaml` and `backend/Dockerfile`.
- Set env vars: `MONGODB_CONNECTION_STRING`, `MONGODB_DATABASE_NAME`, `FIREBASE_PROJECT_ID`.
- Health check endpoint: `/health`.

## Frontend (Vercel)
- Uses root `vercel.json`.
- Build command installs/builds frontend.
- Set `VITE_API_URL` and `VITE_SIGNALR_URL`.

## Docker
```bash
docker compose up --build
```
