import { useQuery } from "@tanstack/react-query";
import { getEconomicCalendar, getLatestMarkets, getMarketHistory } from "../services/marketService";

export const useMarkets = () =>
  useQuery({
    queryKey: ["markets", "latest"],
    queryFn: getLatestMarkets,
    refetchInterval: 30_000
  });

export const useMarketHistory = (market: string) =>
  useQuery({
    queryKey: ["markets", market, "history"],
    queryFn: () => getMarketHistory(market),
    enabled: Boolean(market)
  });

export const useEconomicCalendar = (market: string) =>
  useQuery({
    queryKey: ["calendar", market],
    queryFn: () => getEconomicCalendar(market),
    enabled: Boolean(market),
    refetchInterval: 60_000
  });
