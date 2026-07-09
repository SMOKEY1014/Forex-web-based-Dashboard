import { useQuery } from "@tanstack/react-query";
import { getLatestMarkets, getMarketHistory } from "../services/marketService";

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
