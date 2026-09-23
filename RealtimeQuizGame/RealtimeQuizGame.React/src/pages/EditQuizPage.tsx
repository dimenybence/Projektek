import { useCallback, useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { getQuizAdmin, updateQuiz } from '../api/client'
import type { QuizAdminDetailResponseDto } from '../api/types'
import { isQuizEditable } from '../api/types'
import { draftFromAdminQuestions, emptyQuestion, QuizEditorForm } from '../components/QuizEditorForm'

export function EditQuizPage() {
  const { id } = useParams()
  const quizId = Number(id)
  const navigate = useNavigate()
  const [detail, setDetail] = useState<QuizAdminDetailResponseDto | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)

  const load = useCallback(async () => {
    if (!Number.isFinite(quizId)) return
    setLoading(true)
    setLoadError(null)
    try {
      const d = await getQuizAdmin(quizId)
      setDetail(d)
    } catch (e) {
      setLoadError(e instanceof Error ? e.message : 'Betöltési hiba')
    } finally {
      setLoading(false)
    }
  }, [quizId])

  useEffect(() => {
    void load()
  }, [load])

  if (!Number.isFinite(quizId)) {
    return <p className="error-text">Érvénytelen kvíz azonosító.</p>
  }

  if (loading) {
    return (
      <div className="card muted">
        <p>Kvíz betöltése…</p>
      </div>
    )
  }

  if (loadError || !detail) {
    return (
      <div className="narrow-card">
        <p className="error-text">{loadError ?? 'A kvíz nem található.'}</p>
        <Link to="/admin/quizzes">Vissza a listához</Link>
      </div>
    )
  }

  if (!isQuizEditable(detail.runStatus, detail.phase)) {
    return (
      <div className="narrow-card stack">
        <h1>Kvíz nem szerkeszthető</h1>
        <p className="muted">
          A kvíz csak indítás előtt vagy befejezett állapotban szerkeszthető; futó kör nem módosítható.
        </p>
        <div className="button-row">
          <Link className="btn secondary" to="/admin/quizzes">
            Kvízeim
          </Link>
          <Link className="btn primary" to={`/admin/quiz/${quizId}`}>
            Vezérlés
          </Link>
        </div>
      </div>
    )
  }

  const initialQuestions =
    detail.questions.length > 0 ? draftFromAdminQuestions(detail.questions) : [emptyQuestion()]

  return (
    <QuizEditorForm
      pageTitle="Kvíz szerkesztése"
      headerNote="A kvíz aktív futása alatt nem módosítható. Új kör indítása előtt szerkesztheted. A PIN kód változatlan marad."
      pin={detail.pin}
      initialTitle={detail.title}
      initialDefaultSeconds={detail.defaultSecondsPerQuestion}
      initialQuestions={initialQuestions}
      submitLabel="Változások mentése"
      pendingLabel="Mentés…"
      onSubmit={async (body) => {
        await updateQuiz(quizId, body)
        navigate(`/admin/quiz/${quizId}`, { replace: true })
      }}
    />
  )
}
