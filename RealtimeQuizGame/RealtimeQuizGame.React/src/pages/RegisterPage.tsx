import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function RegisterPage() {
  const { register } = useAuth()
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [pending, setPending] = useState(false)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setPending(true)
    try {
      await register(name, email, password)
      navigate('/admin/quizzes', { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Sikertelen regisztráció')
    } finally {
      setPending(false)
    }
  }

  return (
    <div className="narrow-card">
      <h1>Regisztráció</h1>
      <form className="stack" onSubmit={(e) => void onSubmit(e)}>
        <label className="field">
          <span>Név</span>
          <input value={name} onChange={(e) => setName(e.target.value)} required minLength={1} />
        </label>
        <label className="field">
          <span>E-mail</span>
          <input type="email" autoComplete="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
        </label>
        <label className="field">
          <span>Jelszó</span>
          <input
            type="password"
            autoComplete="new-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={6}
          />
        </label>
        {error && <p className="error-text">{error}</p>}
        <button type="submit" className="btn primary" disabled={pending}>
          {pending ? 'Fiók létrehozása…' : 'Fiók létrehozása'}
        </button>
      </form>
      <p className="muted small">
        Van már fiókod? <Link to="/login">Bejelentkezés</Link>
      </p>
    </div>
  )
}
