import { useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { API_BASE } from "../../services/http";
import type { MarketSnapshot } from "../../types/market";

export const useSignalRUpdates = () => {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${API_BASE}/hubs/markets`)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on("market.snapshot", (snapshot: MarketSnapshot) => {
      queryClient.setQueryData<MarketSnapshot[]>(["markets", "latest"], (current) => {
        if (!current?.length) {
          return [snapshot];
        }

        const filtered = current.filter((entry) => entry.market !== snapshot.market);
        return [...filtered, snapshot].sort((a, b) => a.market.localeCompare(b.market));
      });
    });

    connection.start().catch(() => undefined);

    return () => {
      connection.stop().catch(() => undefined);
    };
  }, [queryClient]);
};
