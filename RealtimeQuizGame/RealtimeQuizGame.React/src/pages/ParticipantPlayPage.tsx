import { useCallback, useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import {
  getParticipantPlayState,
  getParticipantToken,
  setParticipantToken,
  submitParticipantAnswer,
} from '../api/client'
import type { LeaderboardEntryDto, ParticipantPlayStateDto } from '../api/types'
import {
  isQuizFinished,
  isQuizNotStarted,
  QuizPhase,
  type QuizPhaseDto,
  QuizRunStatus,
} from '../api/types'
import { useQuizHub } from '../hooks/useQuizHub'

function isParticipationRevokedError(e: unknown): boolean {
  return e instanceof Error && e.message.includes('új kvízkört indítottak')
}

function LeaderboardSection({ entries }: { entries: LeaderboardEntryDto[] }) {
  return (
    <section className="card leaderboard-panel">
      <h2>Ranglista</h2>
      <ol className="leaderboard">
        {entries.map((entry) => (
          <li
            key={`${entry.rank}-${entry.displayName}`}
            className={`leaderboard-row${entry.isCurrentParticipant ? ' you' : ''}`}
          >
            <span className="leaderboard-rank">{entry.rank}.</span>
            <span className="leaderboard-name">
              {entry.displayName}
              {entry.isCurrentParticipant && <span className="badge you">te</span>}
            </span>
            <span className="leaderboard-score">
              {entry.correctAnswerCount} helyes
            </span>
          </li>
        ))}
      </ol>
    </section>
  )
}

function phaseHint(phase: QuizPhaseDto) {
  switch (phase) {
    case QuizPhase.Lobby:
      return 'Várakozás a kvízvezetőre…'
    case QuizPhase.QuestionOpen:
      return 'Válassz egy választ az időkorláton belül.'
    case QuizPhase.QuestionResults:
      return 'Eredmények'
    case QuizPhase.Completed:
      return 'A kvíz véget ért.'
    default:
      return ''
  }
}

export function ParticipantPlayPage() {
  const navigate = useNavigate()
  const [state, setState] = useState<ParticipantPlayStateDto | null>(null)
  const [displayRemaining, setDisplayRemaining] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const [hubEnabled, setHubEnabled] = useState(false)
  const [newRoundNotice, setNewRoundNotice] = useState<string | null>(null)

  const redirectToJoinAfterReset = useCallback(
    (notice: string) => {
      setParticipantToken(null)
      navigate('/join', { replace: true, state: { notice } })
    },
    [navigate],
  )

  const poll = useCallback(async () => {
    if (!getParticipantToken()) return
    try {
      const s = await getParticipantPlayState()
      setState(s)
      setDisplayRemaining(s.remainingSeconds ?? null)
      setError(null)
      setHubEnabled(true)
      if (
        s.runStatus === QuizRunStatus.InProgress &&
        s.phase === QuizPhase.Lobby &&
        !s.currentQuestion
      ) {
        setNewRoundNotice(null)
      }
    } catch (e) {
      if (isParticipationRevokedError(e)) {
        redirectToJoinAfterReset('Új kör indult — csatlakozz újra a PIN kóddal.')
        return
      }
      setError(e instanceof Error ? e.message : 'Nem sikerült lekérni az állapotot')
    }
  }, [redirectToJoinAfterReset])

  useQuizHub(
    {
      onPlayStateUpdated: () => {
        void poll()
      },
      onQuizSessionReset: () => {
        setNewRoundNotice('Új kör indult — várakozás a következő kérdésre.')
        void poll()
      },
    },
    hubEnabled,
    state?.quizId ?? null,
  )

  useEffect(() => {
    if (!getParticipantToken()) {
      navigate('/join', { replace: true })
      return
    }
    void poll()
  }, [navigate, poll])

  useEffect(() => {
    if (state?.phase !== QuizPhase.QuestionOpen || displayRemaining == null || displayRemaining <= 0) {
      return
    }
    const id = window.setInterval(() => {
      setDisplayRemaining((r) => (r != null && r > 0 ? r - 1 : r))
    }, 1000)
    return () => window.clearInterval(id)
  }, [state?.phase, state?.currentQuestionIndex])

  async function onPick(optionId: number) {
    if (!state || submitting || state.hasAnsweredCurrentQuestion) return
    if (state.phase !== QuizPhase.QuestionOpen) return
    const remaining = displayRemaining ?? state.remainingSeconds ?? 0
    if (remaining <= 0) return
    setSubmitting(true)
    setError(null)
    try {
      await submitParticipantAnswer(optionId)
      await poll()
    } catch (e) {
      if (isParticipationRevokedError(e)) {
        redirectToJoinAfterReset('Új kör indult — csatlakozz újra a PIN kóddal.')
        return
      }
      setError(e instanceof Error ? e.message : 'Válasz küldése sikertelen')
    } finally {
      setSubmitting(false)
    }
  }

  if (!state && !error) {
    return (
      <div className="card muted">
        <p>Játékállapot betöltése…</p>
      </div>
    )
  }

  if (!state) {
    return (
      <div className="narrow-card">
        <p className="error-text">{error}</p>
        <Link to="/join">Új csatlakozás</Link>
      </div>
    )
  }

  const waitingNotStarted = isQuizNotStarted(state.runStatus, state.phase)
  const finished = isQuizFinished(state.runStatus, state.phase)
  const inLobby =
    state.runStatus === QuizRunStatus.InProgress && state.phase === QuizPhase.Lobby && !state.currentQuestion
  const remaining = displayRemaining ?? state.remainingSeconds ?? 0
  const canAnswer =
    state.phase === QuizPhase.QuestionOpen &&
    state.currentQuestion &&
    !state.hasAnsweredCurrentQuestion &&
    remaining > 0

  const maxVotes =
    state.resultBreakdown?.reduce((m, o) => Math.max(m, o.answerCount), 0) ?? 1

  return (
    <div className="play-layout stack loose">
      <header>
        <h1>{state.quizTitle}</h1>
        <p className="muted">{phaseHint(state.phase)}</p>
      </header>

      {error && <p className="error-text">{error}</p>}
      {newRoundNotice && <p className="success-text">{newRoundNotice}</p>}

      {waitingNotStarted && (
        <div className="card">
          <p>A kvíz még nem indult. Várj a szervezőre.</p>
        </div>
      )}

      {finished && (
        <div className="card">
          <p>A kvíz véget ért. Köszönjük a részvételt.</p>
          <Link to="/join" className="btn secondary">
            Másik kvízhez csatlakozás
          </Link>
        </div>
      )}

      {inLobby && !finished && !waitingNotStarted && (
        <div className="card">
          <p>Lobby: a következő kérdésre a kvízvezető indul.</p>
        </div>
      )}

      {state.phase === QuizPhase.QuestionOpen && state.currentQuestion && (
        <section className="card question-panel">
          <div className="question-meta">
            {state.currentQuestionIndex != null && (
              <span className="badge">Kérdés {state.currentQuestionIndex + 1}</span>
            )}
            {(displayRemaining != null || state.remainingSeconds != null) && (
              <span className={`timer ${remaining <= 5 ? 'urgent' : ''}`}>{remaining} mp</span>
            )}
          </div>
          <h2>{state.currentQuestion.text}</h2>
          {state.hasAnsweredCurrentQuestion && (
            <p className="success-text">Válaszod rögzítettük. Várakozás a kérdés lezárására…</p>
          )}
          <ul className="option-buttons">
            {state.currentQuestion.options.map((o) => (
              <li key={o.id}>
                <button
                  type="button"
                  className="btn option"
                  disabled={!canAnswer || submitting}
                  onClick={() => void onPick(o.id)}
                >
                  {o.text}
                </button>
              </li>
            ))}
          </ul>
        </section>
      )}

      {state.phase === QuizPhase.QuestionResults && state.resultBreakdown && (
        <section className="card results-panel">
          <h2>Eredmények</h2>
          <ul className="result-bars">
            {state.resultBreakdown.map((o) => {
              const pct = maxVotes > 0 ? Math.round((o.answerCount / maxVotes) * 100) : 0
              const yours = state.selectedAnswerOptionId === o.id
              return (
                <li key={o.id} className={o.isCorrect ? 'correct' : ''}>
                  <div className="result-row">
                    <span>
                      {o.text}
                      {o.isCorrect && <span className="badge ok">helyes</span>}
                      {yours && <span className="badge you">a te válaszod</span>}
                    </span>
                    <span className="count">{o.answerCount}</span>
                  </div>
                  <div className="bar-track">
                    <div className="bar-fill" style={{ width: `${pct}%` }} />
                  </div>
                </li>
              )
            })}
          </ul>
        </section>
      )}

      {state.leaderboard && state.leaderboard.length > 0 && (
        <LeaderboardSection entries={state.leaderboard} />
      )}
    </div>
  )
}
