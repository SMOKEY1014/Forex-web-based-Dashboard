import { Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import type { MarketSnapshot } from "../types/market";

type Props = {
  data: MarketSnapshot[];
};

export const PressureChart = ({ data }: Props) => (
  <div className="glass-card h-64 rounded-xl border border-zinc-800 p-4">
    <h3 className="mb-4 text-sm font-semibold text-zinc-200">Pressure History</h3>
    <ResponsiveContainer width="100%" height="85%">
      <LineChart data={data}>
        <XAxis dataKey="updatedAtUtc" tickFormatter={(value: string) => new Date(value).toLocaleTimeString()} stroke="#71717a" />
        <YAxis stroke="#71717a" />
        <Tooltip labelFormatter={(value) => new Date(value).toLocaleString()} />
        <Line dataKey="totalPressure" stroke="#22d3ee" strokeWidth={2} dot={false} />
      </LineChart>
    </ResponsiveContainer>
  </div>
);
