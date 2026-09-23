import { useState, type FormEvent } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { joinQuiz, loadUserAuth, setParticipantToken } from '../api/client'
import { useAuth } from '../context/AuthContext'

export function JoinQuizPage() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const notice = (location.state as { notice?: string } | null)?.notice
  const [pin, setPin] = useState('')
  const [displayName, setDisplayName] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [pending, setPending] = useState(false)

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(null)
    setPending(true)
    try {
      const auth = loadUserAuth()
      const token = user && auth?.authToken ? auth.authToken : undefined
      const res = await joinQuiz(pin.trim().toUpperCase(), displayName.trim(), token)
      setParticipantToken(res.participationToken)
      navigate('/play', { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Csatlakozás sikertelen')
    } finally {
      setPending(false)
    }
  }

  return (
    <div className="narrow-card">
      <h1>Csatlakozás kvízhez</h1>
      {notice && <p className="success-text">{notice}</p>}
      <p className="muted">
        Add meg a kvíz PIN kódját és a megjelenő nevedet. Bejelentkezés nem kötelező; bejelentkezve a részvételed
        opcionálisan a fiókodhoz kapcsolódik.
      </p>
      <form className="stack" onSubmit={(e) => void onSubmit(e)}>
        <label className="field">
          <span>PIN kód</span>
          <input
            value={pin}
            onChange={(e) => setPin(e.target.value)}
            minLength={4}
            maxLength={16}
            required
            autoCapitalize="characters"
          />
        </label>
        <label className="field">
          <span>Megjelenő név</span>
          <input value={displayName} onChange={(e) => setDisplayName(e.target.value)} minLength={1} maxLength={40} required />
        </label>
        {error && <p className="error-text">{error}</p>}
        <button type="submit" className="btn primary" disabled={pending}>
          {pending ? 'Csatlakozás…' : 'Belépés a kvízbe'}
        </button>
      </form>
    </div>
  )
}
