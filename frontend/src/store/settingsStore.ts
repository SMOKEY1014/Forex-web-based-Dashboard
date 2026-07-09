import { create } from "zustand";

type SettingsState = {
  useExtendedHours: boolean;
  toggleExtendedHours: () => void;
};

export const useSettingsStore = create<SettingsState>((set) => ({
  useExtendedHours: false,
  toggleExtendedHours: () => set((state) => ({ useExtendedHours: !state.useExtendedHours }))
}));
