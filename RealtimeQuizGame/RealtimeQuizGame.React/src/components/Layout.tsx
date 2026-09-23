import { Link, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function Layout() {
  const { user, logout } = useAuth()

  return (
    <div className="app-shell">
      <header className="top-bar">
        <Link to="/" className="brand">
          Realtime Quiz
        </Link>
        <nav className="nav-links">
          <Link to="/join">Csatlakozás</Link>
          {user ? (
            <>
              <Link to="/admin/quizzes">Kvízeim</Link>
              <Link to="/admin/create">Új kvíz</Link>
              <span className="user-chip">{user.name}</span>
              <button type="button" className="btn ghost" onClick={() => void logout()}>
                Kijelentkezés
              </button>
            </>
          ) : (
            <>
              <Link to="/login">Bejelentkezés</Link>
              <Link to="/register">Regisztráció</Link>
            </>
          )}
        </nav>
      </header>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  )
}
