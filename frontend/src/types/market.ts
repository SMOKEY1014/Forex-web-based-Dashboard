export type CompanyEntry = {
  ticker: string;
  companyName: string;
  officialWeight: number;
  price: number;
  percentageChange: number;
  contribution: number;
  isLocked: boolean;
  updatedAtUtc: string;
};

export type MarketSnapshot = {
  id: string;
  market: string;
  companies: CompanyEntry[];
  totalPressure: number;
  bullishPercentage: number;
  bearishPercentage: number;
  neutralPercentage: number;
  marketBiasScore: number;
  confidence: number;
  biasLabel: string;
  explanation: string;
  factorBreakdown: Record<string, number>;
  updatedAtUtc: string;
  isExtendedHours: boolean;
};
