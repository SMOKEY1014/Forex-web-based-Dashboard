import { create } from "zustand";
import { TOKEN_STORAGE_KEY } from "../services/http";

type AuthUser = {
  uid: string;
  email: string;
  displayName: string;
};

type AuthState = {
  user: AuthUser | null;
  token: string | null;
  initialized: boolean;
  setSession: (session: { user: AuthUser; token: string } | null) => void;
  setInitialized: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  token: null,
  initialized: false,
  setSession: (session) =>
    set(() => {
      if (session?.token) {
        localStorage.setItem(TOKEN_STORAGE_KEY, session.token);
      } else {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
      }

      return {
        user: session?.user ?? null,
        token: session?.token ?? null
      };
    }),
  setInitialized: () => set({ initialized: true })
}));
