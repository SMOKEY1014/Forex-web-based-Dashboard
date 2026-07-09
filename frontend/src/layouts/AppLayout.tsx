import { BellRing } from "lucide-react";
import { Outlet } from "react-router-dom";
import { useSettingsStore } from "../store/settingsStore";

export const AppLayout = () => {
  const { useExtendedHours, toggleExtendedHours } = useSettingsStore();

  return (
    <div className="min-h-screen bg-zinc-950 text-zinc-100">
      <header className="sticky top-0 z-10 border-b border-zinc-800/80 bg-zinc-950/80 backdrop-blur">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-3">
          <h1 className="text-xl font-bold tracking-wide">Market Pulse Dashboard</h1>
          <button
            type="button"
            onClick={toggleExtendedHours}
            className="rounded-md border border-zinc-700 px-3 py-1 text-sm hover:border-cyan-500"
          >
            {useExtendedHours ? "Extended" : "Regular"}
          </button>
        </div>
      </header>
      <main className="mx-auto max-w-7xl px-4 py-6">
        <Outlet />
      </main>
      <footer className="mx-auto flex max-w-7xl items-center justify-end gap-2 px-4 pb-4 text-xs text-zinc-500">
        <BellRing size={14} /> Browser alerts supported
      </footer>
    </div>
  );
};
