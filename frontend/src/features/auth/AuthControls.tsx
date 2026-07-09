import { LogIn, LogOut } from "lucide-react";
import { hasFirebaseConfig, signInWithGoogle, signOutUser } from "../../services/firebaseAuth";
import { useAuthStore } from "../../store/authStore";

export const AuthControls = () => {
  const user = useAuthStore((state) => state.user);

  if (!hasFirebaseConfig()) {
    return <span className="text-xs text-zinc-500">Firebase sign-in not configured</span>;
  }

  return user ? (
    <div className="flex items-center gap-2">
      <span className="text-xs text-zinc-400">{user.displayName}</span>
      <button
        type="button"
        onClick={() => signOutUser().catch(() => undefined)}
        className="inline-flex items-center gap-1 rounded-md border border-zinc-700 px-2 py-1 text-xs hover:border-cyan-500"
      >
        <LogOut size={12} /> Sign out
      </button>
    </div>
  ) : (
    <button
      type="button"
      onClick={() => signInWithGoogle().catch(() => undefined)}
      className="inline-flex items-center gap-1 rounded-md border border-zinc-700 px-2 py-1 text-xs hover:border-cyan-500"
    >
      <LogIn size={12} /> Google sign-in
    </button>
  );
};
