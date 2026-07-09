import { getJson } from "./http";
import type { EconomicCalendarEvent, MarketSnapshot } from "../types/market";

export const getLatestMarkets = () => getJson<MarketSnapshot[]>("/api/markets/latest");

export const getMarketHistory = (market: string, limit = 60) =>
  getJson<MarketSnapshot[]>(`/api/markets/${market}/history?limit=${limit}`);

export const getEconomicCalendar = (market: string, hours = 6) =>
  getJson<EconomicCalendarEvent[]>(`/api/economic-calendar/${market}?hours=${hours}`);
