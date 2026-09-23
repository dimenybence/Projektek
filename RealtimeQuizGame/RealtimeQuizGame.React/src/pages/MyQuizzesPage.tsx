import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { listMyQuizzes } from '../api/client'
import type { QuizSummaryResponseDto } from '../api/types'
import { Pagination } from '../components/Pagination'
import {
  isQuizEditable,
  QuizPhase,
  type QuizPhaseDto,
  quizDisplayStatus,
} from '../api/types'

const PAGE_SIZE = 10

function phaseLabel(p: QuizPhaseDto) {
  switch (p) {
    case QuizPhase.Lobby:
      return 'Várakozás'
    case QuizPhase.QuestionOpen:
      return 'Aktív kérdés'
    case QuizPhase.QuestionResults:
      return 'Eredmények'
    case QuizPhase.Completed:
      return 'Befejezve'
    default:
      return String(p)
  }
}

export function MyQuizzesPage() {
  const [list, setList] = useState<QuizSummaryResponseDto[]>([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(0)
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async (pageToLoad: number) => {
    setLoading(true)
    setError(null)
    try {
      const data = await listMyQuizzes(pageToLoad, PAGE_SIZE)
      setList(data.items)
      setPage(data.currentPage)
      setTotalPages(data.totalPages)
      setTotal(data.total)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Betöltési hiba')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void load(page)
  }, [load, page])

  function onPageChange(nextPage: number) {
    setPage(nextPage)
  }

  if (loading && list.length === 0) {
    return (
      <div className="card muted">
        <p>Kvízek betöltése…</p>
      </div>
    )
  }

  return (
    <div className="stack loose">
      <div className="page-head">
        <h1>Kvízeim</h1>
        <Link className="btn primary" to="/admin/create">
          Új kvíz
        </Link>
      </div>
      {error && <p className="error-text">{error}</p>}
      {!list.length ? (
        <div className="card">
          <p className="muted">Még nincs kvíz. Hozz létre egyet.</p>
        </div>
      ) : (
        <>
          <ul className="quiz-list">
            {list.map((q) => (
              <li key={q.id} className="card quiz-row">
                <div>
                  <h2>{q.title}</h2>
                  <p className="muted small">
                    PIN: <code className="pin">{q.pin}</code> · {q.questionCount} kérdés ·{' '}
                    {quizDisplayStatus(q.runStatus, q.phase)} · {phaseLabel(q.phase)}
                    {q.currentQuestionIndex != null && ` · kérdés #${q.currentQuestionIndex + 1}`}
                  </p>
                </div>
                <div className="button-row">
                  {isQuizEditable(q.runStatus, q.phase) && (
                    <Link className="btn secondary" to={`/admin/quiz/${q.id}/edit`}>
                      Szerkesztés
                    </Link>
                  )}
                  <Link className="btn secondary" to={`/admin/quiz/${q.id}`}>
                    Vezérlés
                  </Link>
                </div>
              </li>
            ))}
          </ul>
          <Pagination
            currentPage={page}
            totalPages={totalPages}
            total={total}
            pageSize={PAGE_SIZE}
            onPageChange={onPageChange}
            disabled={loading}
          />
        </>
      )}
    </div>
  )
}
