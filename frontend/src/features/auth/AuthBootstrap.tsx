import { useEffect } from "react";
import { subscribeAuth } from "../../services/firebaseAuth";
import { useAuthStore } from "../../store/authStore";

export const AuthBootstrap = () => {
  const setSession = useAuthStore((state) => state.setSession);
  const setInitialized = useAuthStore((state) => state.setInitialized);

  useEffect(() => {
    const unsubscribe = subscribeAuth((user) => {
      if (!user) {
        setSession(null);
        setInitialized();
        return;
      }

      user
        .getIdToken()
        .then((token) =>
          setSession({
            token,
            user: {
              uid: user.uid,
              email: user.email ?? "",
              displayName: user.displayName ?? user.email ?? "Signed In"
            }
          }))
        .finally(setInitialized);
    });

    return () => {
      unsubscribe();
    };
  }, [setInitialized, setSession]);

  return null;
};
