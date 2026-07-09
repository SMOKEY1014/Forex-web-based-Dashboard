import { create } from "zustand";
import type { MarketAlert } from "../types/market";

type AlertState = {
  alerts: MarketAlert[];
  pushAlert: (alert: MarketAlert) => void;
};

export const useAlertStore = create<AlertState>((set) => ({
  alerts: [],
  pushAlert: (alert) =>
    set((state) => ({
      alerts: [alert, ...state.alerts].slice(0, 50)
    }))
}));
