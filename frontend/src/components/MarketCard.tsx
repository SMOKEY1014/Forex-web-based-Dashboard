import { Link } from "react-router-dom";
import { Activity, ArrowDownRight, ArrowUpRight } from "lucide-react";
import type { MarketSnapshot } from "../types/market";
import { formatPercent } from "../utils/format";

type Props = {
  market: MarketSnapshot;
};

export const MarketCard = ({ market }: Props) => {
  const strongestPositive = [...market.companies].sort((a, b) => b.contribution - a.contribution)[0];
  const strongestNegative = [...market.companies].sort((a, b) => a.contribution - b.contribution)[0];

  return (
    <Link
      to={`/market/${market.market}`}
      className="glass-card block rounded-xl border border-zinc-800 p-4 transition hover:border-cyan-500"
    >
      <div className="mb-3 flex items-center justify-between">
        <h2 className="text-lg font-semibold text-zinc-100">{market.market}</h2>
        <span className="rounded-full bg-zinc-800 px-2 py-1 text-xs text-zinc-300">{market.biasLabel}</span>
      </div>
      <div className="space-y-2 text-sm text-zinc-300">
        <p className="flex items-center gap-2"><Activity size={14} /> Pressure: {market.totalPressure.toFixed(2)}</p>
        <p>Bias score: {market.marketBiasScore.toFixed(1)} / 100</p>
        <p>Confidence: {formatPercent(market.confidence)}</p>
        <p>Bullish/Bearish/Neutral: {market.bullishPercentage}% / {market.bearishPercentage}% / {market.neutralPercentage}%</p>
        <p className="flex items-center gap-1 text-emerald-400"><ArrowUpRight size={14} /> {strongestPositive?.ticker ?? "-"}</p>
        <p className="flex items-center gap-1 text-rose-400"><ArrowDownRight size={14} /> {strongestNegative?.ticker ?? "-"}</p>
      </div>
      <p className="mt-3 text-xs text-zinc-500">Updated {new Date(market.updatedAtUtc).toLocaleTimeString()}</p>
    </Link>
  );
};
