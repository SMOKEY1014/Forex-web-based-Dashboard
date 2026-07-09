export const formatPercent = (value: number) => `${value.toFixed(2)}%`;

export const formatNumber = (value: number) => value.toLocaleString("en-US", { maximumFractionDigits: 2 });
