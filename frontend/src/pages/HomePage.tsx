import { AnimatePresence, motion } from "framer-motion";
import { MarketCard } from "../components/MarketCard";
import { useSignalRUpdates } from "../features/markets/useSignalRUpdates";
import { useMarkets } from "../hooks/useMarkets";

export const HomePage = () => {
  const { data, isLoading, isError } = useMarkets();
  useSignalRUpdates();

  if (isLoading) {
    return <p className="text-zinc-400">Loading market cards…</p>;
  }

  if (isError || !data) {
    return <p className="text-rose-400">Unable to load market data.</p>;
  }

  return (
    <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      <AnimatePresence>
        {data.map((market) => (
          <motion.div
            key={market.market}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
          >
            <MarketCard market={market} />
          </motion.div>
        ))}
      </AnimatePresence>
    </section>
  );
};
