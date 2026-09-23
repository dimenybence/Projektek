import { Link } from 'react-router-dom'

export function HomePage() {
  return (
    <div className="hero-block">
      <h1>Kvíz játék</h1>
      <p className="lede">
        Résztvevőként csatlakozhatsz PIN kóddal bejelentkezés nélkül. Kvízeket létrehozni és vezérelni csak
        regisztrált felhasználóként lehet.
      </p>
      <div className="cta-row">
        <Link className="btn primary" to="/join">
          Csatlakozás kvízhez
        </Link>
        <Link className="btn secondary" to="/admin/quizzes">
          Admin: kvízeim
        </Link>
      </div>
    </div>
  )
}
