const API_BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5000";

export const getJson = async <T>(path: string): Promise<T> => {
  const response = await fetch(`${API_BASE}${path}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch ${path}`);
  }

  return response.json() as Promise<T>;
};

export { API_BASE };
