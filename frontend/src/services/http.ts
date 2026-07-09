const API_BASE = import.meta.env.VITE_API_URL ?? "http://localhost:5000";
const TOKEN_STORAGE_KEY = "firebaseIdToken";

type RequestOptions = {
  method?: string;
  body?: unknown;
  headers?: HeadersInit;
};

const requestJson = async <T>(path: string, options: RequestOptions = {}): Promise<T> => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  const response = await fetch(`${API_BASE}${path}`, {
    method: options.method ?? "GET",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: "Bearer " + token } : {}),
      ...options.headers
    },
    body: options.body === undefined ? undefined : JSON.stringify(options.body)
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Failed request to ${path}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
};

export const getJson = <T>(path: string) => requestJson<T>(path);
export const postJson = <T>(path: string, body: unknown) => requestJson<T>(path, { method: "POST", body });
export const putJson = <T>(path: string, body: unknown) => requestJson<T>(path, { method: "PUT", body });
export const deleteJson = <T>(path: string) => requestJson<T>(path, { method: "DELETE" });

export { API_BASE, TOKEN_STORAGE_KEY };
