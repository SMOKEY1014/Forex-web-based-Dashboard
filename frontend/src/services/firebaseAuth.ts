import { initializeApp } from "firebase/app";
import {
  GoogleAuthProvider,
  getAuth,
  onAuthStateChanged,
  signInWithPopup,
  signOut,
  type User
} from "firebase/auth";

const firebaseConfig = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID
};

const isConfigured = Object.values(firebaseConfig).every((value) => Boolean(value));
const app = isConfigured ? initializeApp(firebaseConfig) : null;
const auth = app ? getAuth(app) : null;
const provider = new GoogleAuthProvider();

export const hasFirebaseConfig = () => isConfigured;

export const subscribeAuth = (onChanged: (user: User | null) => void) => {
  if (!auth) {
    onChanged(null);
    return () => undefined;
  }

  return onAuthStateChanged(auth, onChanged);
};

export const signInWithGoogle = async () => {
  if (!auth) {
    throw new Error("Firebase config is missing.");
  }

  await signInWithPopup(auth, provider);
};

export const signOutUser = async () => {
  if (!auth) {
    return;
  }

  await signOut(auth);
};
