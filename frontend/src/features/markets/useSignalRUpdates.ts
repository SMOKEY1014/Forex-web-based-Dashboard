import { useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { API_BASE, TOKEN_STORAGE_KEY } from "../../services/http";
import type { MarketAlert, MarketSnapshot } from "../../types/market";
import { useAlertStore } from "../../store/alertStore";

export const useSignalRUpdates = () => {
  const queryClient = useQueryClient();
  const pushAlert = useAlertStore((state) => state.pushAlert);

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${API_BASE}/hubs/markets`, {
        accessTokenFactory: () => localStorage.getItem(TOKEN_STORAGE_KEY) ?? ""
      })
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

    connection.on("market.alert", (alert: MarketAlert) => {
      pushAlert(alert);
      if (typeof Notification !== "undefined" && Notification.permission === "granted") {
        new Notification(`${alert.market}: ${alert.type}`, {
          body: alert.message
        });
      }
    });

    connection.start().catch(() => undefined);

    return () => {
      connection.stop().catch(() => undefined);
    };
  }, [pushAlert, queryClient]);
};
