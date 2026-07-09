import { deleteJson, getJson, postJson, putJson } from "./http";
import type { Watchlist, WatchlistVersion } from "../types/market";

export const getAdminWatchlists = () => getJson<Watchlist[]>("/api/admin/watchlists");

export const createWatchlist = (payload: Watchlist) => postJson<Watchlist>("/api/admin/watchlists", payload);

export const updateWatchlist = (payload: Watchlist) => putJson<Watchlist>(`/api/admin/watchlists/${payload.market}`, payload);

export const deleteWatchlist = (market: string) => deleteJson<void>(`/api/admin/watchlists/${market}`);

export const getWatchlistVersions = (market: string, limit = 20) =>
  getJson<WatchlistVersion[]>(`/api/admin/watchlists/${market}/versions?limit=${limit}`);

export const rollbackWatchlist = (market: string, version: number) =>
  postJson<Watchlist>(`/api/admin/watchlists/${market}/rollback/${version}`, {});

export const importWatchlists = (payload: Watchlist[]) => postJson<Watchlist[]>("/api/admin/watchlists/import", payload);

export const exportWatchlists = () => getJson<Watchlist[]>("/api/admin/watchlists/export");
