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

export type MarketAlert = {
  market: string;
  severity: string;
  type: string;
  message: string;
  biasScore: number;
  confidence: number;
  triggeredAtUtc: string;
};

export type EconomicCalendarEvent = {
  id: string;
  eventCode: string;
  market: string;
  region: string;
  title: string;
  impact: string;
  eventTimeUtc: string;
  forecast: number;
  previous: number;
  actual: number;
  sentimentScore: number;
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
  factorScores: Record<string, number>;
  alerts: MarketAlert[];
  economicEventsCount: number;
  providerUsed: string;
  updatedAtUtc: string;
  isExtendedHours: boolean;
};

export type Watchlist = {
  id: string;
  name: string;
  market: string;
  companies: CompanyEntry[];
  maximumCompanies: number;
  notes: string;
  autoUpdateEnabled: boolean;
  autoWeightRefreshEnabled: boolean;
  refreshIntervalSeconds: number;
  lastUpdatedUtc: string;
  createdUtc: string;
  modifiedUtc: string;
  version: number;
};

export type WatchlistVersion = {
  id: string;
  market: string;
  version: number;
  operation: string;
  snapshot: Watchlist | null;
  changedAtUtc: string;
};
