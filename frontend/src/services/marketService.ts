import { getJson } from "./http";
import type { MarketSnapshot } from "../types/market";

export const getLatestMarkets = () => getJson<MarketSnapshot[]>("/api/markets/latest");

export const getMarketHistory = (market: string, limit = 60) =>
  getJson<MarketSnapshot[]>(`/api/markets/${market}/history?limit=${limit}`);
