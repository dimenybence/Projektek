import { useCallback, useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  advanceQuiz,
  closeQuestion,
  getQuizAdmin,
  openQuestion,
  startQuiz,
} from '../api/client'
import type { QuizAdminDetailResponseDto } from '../api/types'
import {
  isQuizEditable,
  QuizPhase,
  type QuizPhaseDto,
  QuizRunStatus,
  quizDisplayStatus,
} from '../api/types'
import { useAdminQuizHub } from '../hooks/useAdminQuizHub'

function phaseLabel(p: QuizPhaseDto) {
  switch (p) {
    case QuizPhase.Lobby:
      return 'Várakozás (lobby)'
    case QuizPhase.QuestionOpen:
      return 'Kérdés nyitva'
    case QuizPhase.QuestionResults:
      return 'Eredmények megjelenítve'
    case QuizPhase.Completed:
      return 'Kör befejezve'
    default:
      return String(p)
  }
}

export function AdminQuizPage() {
  const { id } = useParams()
  const quizId = Number(id)
  const [detail, setDetail] = useState<QuizAdminDetailResponseDto | null>(null)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const load = useCallback(async () => {
    if (!Number.isFinite(quizId)) return
    try {
      const d = await getQuizAdmin(quizId)
      setDetail(d)
      setLoadError(null)
    } catch (e) {
      setLoadError(e instanceof Error ? e.message : 'Betöltési hiba')
    }
  }, [quizId])

  useEffect(() => {
    void load()
  }, [load])

  useAdminQuizHub(quizId, {
    onPlayStateUpdated: () => {
      void load()
    },
    onQuizSessionReset: () => {
      void load()
    },
  }, Number.isFinite(quizId))

  async function run(action: () => Promise<void>) {
    setBusy(true)
    setActionError(null)
    try {
      await action()
      await load()
    } catch (e) {
      setActionError(e instanceof Error ? e.message : 'Művelet sikertelen')
    } finally {
      setBusy(false)
    }
  }

  if (!Number.isFinite(quizId)) {
    return <p className="error-text">Érvénytelen kvíz azonosító.</p>
  }

  if (!detail && !loadError) {
    return (
      <div className="card muted">
        <p>Kvíz betöltése…</p>
      </div>
    )
  }

  if (!detail) {
    return (
      <div className="card">
        <p className="error-text">{loadError}</p>
        <Link to="/admin/quizzes">Vissza a listához</Link>
      </div>
    )
  }

  const canEdit = isQuizEditable(detail.runStatus, detail.phase)
  const canStart = detail.runStatus === QuizRunStatus.NotInProgress
  const inLobby =
    detail.runStatus === QuizRunStatus.InProgress && detail.phase === QuizPhase.Lobby
  const canOpenFirst =
    inLobby && detail.currentQuestionIndex === null && detail.questions.length > 0
  const questionOpen = detail.runStatus === QuizRunStatus.InProgress && detail.phase === QuizPhase.QuestionOpen
  const resultsPhase =
    detail.runStatus === QuizRunStatus.InProgress && detail.phase === QuizPhase.QuestionResults

  const currentIdx = detail.currentQuestionIndex
  const currentQuestion =
    currentIdx != null && currentIdx >= 0 && currentIdx < detail.questions.length
      ? detail.questions[currentIdx]
      : null

  return (
    <div className="stack loose">
      <div className="page-head">
        <div>
          <Link to="/admin/quizzes" className="muted small">
            ← Kvízeim
          </Link>
          <h1>{detail.title}</h1>
          <p className="muted">
            PIN: <code className="pin">{detail.pin}</code> · {quizDisplayStatus(detail.runStatus, detail.phase)} · {phaseLabel(detail.phase)}
            {currentIdx != null && ` · aktuális: ${currentIdx + 1}. / ${detail.questions.length}`}
          </p>
        </div>
      </div>

      {loadError && <p className="error-text">{loadError}</p>}
      {actionError && <p className="error-text">{actionError}</p>}

      {canEdit && (
        <p>
          <Link className="btn secondary" to={`/admin/quiz/${quizId}/edit`}>
            Kvíz szerkesztése
          </Link>
        </p>
      )}

      <section className="card">
        <h2>Vezérlés</h2>
        <div className="button-row">
          {canStart && (
            <button
              type="button"
              className="btn primary"
              disabled={busy}
              onClick={() => void run(() => startQuiz(quizId))}
            >
              Kvíz indítása / új kör
            </button>
          )}
          {canOpenFirst && (
            <button
              type="button"
              className="btn primary"
              disabled={busy}
              onClick={() => void run(() => openQuestion(quizId, 0))}
            >
              Első kérdés megnyitása
            </button>
          )}
          {questionOpen && (
            <button
              type="button"
              className="btn secondary"
              disabled={busy}
              onClick={() => void run(() => closeQuestion(quizId))}
            >
              Aktuális kérdés lezárása
            </button>
          )}
          {resultsPhase && (
            <button
              type="button"
              className="btn secondary"
              disabled={busy}
              onClick={() => void run(() => advanceQuiz(quizId))}
            >
              Következő kérdés / kvíz befejezése
            </button>
          )}
        </div>
        {questionOpen && detail.remainingSeconds != null && (
          <p className="timer-hint">Hátralévő idő (becsült): {detail.remainingSeconds} mp</p>
        )}
        <p className="muted small">
          Az első kérdést a lobby fázisból nyitod meg. A további kérdésekhez az eredményfázis után a „Következő
          kérdés” lépés szükséges.
        </p>
      </section>

      {currentQuestion && (
        <section className="card">
          <h2>Aktuális kérdés (admin)</h2>
          <p>{currentQuestion.text}</p>
          <ul className="answer-preview">
            {currentQuestion.options.map((o) => (
              <li key={o.id} className={o.isCorrect ? 'correct' : ''}>
                {o.text}
                {o.isCorrect && <span className="badge">helyes</span>}
              </li>
            ))}
          </ul>
        </section>
      )}

      <section className="card">
        <h2>Összes kérdés{canEdit ? ' (szerkeszthető)' : ' (nem szerkeszthető)'}</h2>
        <ol className="admin-questions">
          {detail.questions.map((q) => (
            <li key={q.id}>
              <strong>{q.text}</strong>
              {q.timeLimitSeconds != null && (
                <span className="muted small"> · időkorlát: {q.timeLimitSeconds}s</span>
              )}
              <ul>
                {q.options.map((o) => (
                  <li key={o.id} className={o.isCorrect ? 'correct' : ''}>
                    {o.text}
                    {o.isCorrect ? ' ✓' : ''}
                  </li>
                ))}
              </ul>
            </li>
          ))}
        </ol>
      </section>
    </div>
  )
}
