import { useParams } from "react-router-dom";
import { PressureChart } from "../components/PressureChart";
import { useMarketHistory, useMarkets } from "../hooks/useMarkets";
import { formatNumber, formatPercent } from "../utils/format";

export const MarketDetailsPage = () => {
  const { market = "" } = useParams();
  const { data: markets } = useMarkets();
  const { data: history } = useMarketHistory(market);

  const current = markets?.find((entry) => entry.market.toLowerCase() === market.toLowerCase());

  if (!current) {
    return <p className="text-zinc-400">Loading market details…</p>;
  }

  return (
    <div className="space-y-6">
      <div className="glass-card rounded-xl border border-zinc-800 p-4">
        <h2 className="mb-1 text-xl font-semibold">{current.market}</h2>
        <p className="text-sm text-zinc-400">{current.explanation}</p>
      </div>

      <PressureChart data={history ?? []} />

      <div className="overflow-x-auto rounded-xl border border-zinc-800">
        <table className="min-w-full divide-y divide-zinc-800 text-sm">
          <thead className="bg-zinc-900">
            <tr>
              <th className="px-3 py-2 text-left">Ticker</th>
              <th className="px-3 py-2 text-left">Price</th>
              <th className="px-3 py-2 text-left">% Change</th>
              <th className="px-3 py-2 text-left">Official Weight</th>
              <th className="px-3 py-2 text-left">Contribution</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-800">
            {current.companies.map((company) => (
              <tr key={company.ticker}>
                <td className="px-3 py-2">{company.ticker}</td>
                <td className="px-3 py-2">{formatNumber(company.price)}</td>
                <td className={`px-3 py-2 ${company.percentageChange >= 0 ? "text-emerald-400" : "text-rose-400"}`}>
                  {formatPercent(company.percentageChange)}
                </td>
                <td className="px-3 py-2">{formatPercent(company.officialWeight * 100)}</td>
                <td className="px-3 py-2">{company.contribution.toFixed(4)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};
