import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import {
  fetchCurrentUser,
  loadUserAuth,
  loginUser,
  logoutUser,
  registerUser,
  saveUserAuth,
  type StoredUserAuth,
} from '../api/client'
import type { UserResponseDto } from '../api/types'

interface AuthState {
  user: UserResponseDto | null
  loading: boolean
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>
  register: (name: string, email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserResponseDto | null>(null)
  const [loading, setLoading] = useState(true)

  const hydrate = useCallback(async (stored: StoredUserAuth) => {
    const stillSameSession = () => loadUserAuth()?.userId === stored.userId

    try {
      const profile = await fetchCurrentUser()
      if (!stillSameSession()) return
      setUser(profile)
    } catch {
      if (!stillSameSession()) return
      saveUserAuth(null)
      setUser(null)
    }
  }, [])

  useEffect(() => {
    const stored = loadUserAuth()
    if (!stored) {
      setLoading(false)
      return
    }
    let cancelled = false
    void hydrate(stored).finally(() => {
      if (!cancelled) setLoading(false)
    })
    return () => {
      cancelled = true
    }
  }, [hydrate])

  const login = useCallback(async (email: string, password: string) => {
    const res = await loginUser({ email, password })
    const stored: StoredUserAuth = {
      userId: res.userId,
      authToken: res.authToken,
      refreshToken: res.refreshToken,
    }
    saveUserAuth(stored)
    const profile = await fetchCurrentUser()
    setUser(profile)
  }, [])

  const register = useCallback(async (name: string, email: string, password: string) => {
    await registerUser({ name, email, password })
    await login(email, password)
  }, [login])

  const logout = useCallback(async () => {
    try {
      await logoutUser()
    } catch {
      /* token may already be invalid */
    }
    saveUserAuth(null)
    setUser(null)
  }, [])

  const value = useMemo(
    () => ({
      user,
      loading,
      login,
      register,
      logout,
    }),
    [user, loading, login, register, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
