import { useState, type FormEvent } from 'react'
import type { CreateAnswerOptionDto, CreateQuestionDto, CreateQuizRequestDto } from '../api/types'

export interface DraftOption extends CreateAnswerOptionDto {
  key: string
}

export interface DraftQuestion {
  key: string
  text: string
  timeLimitSeconds: string
  options: DraftOption[]
}

function newKey() {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`
}

export function emptyQuestion(): DraftQuestion {
  return {
    key: newKey(),
    text: '',
    timeLimitSeconds: '',
    options: [
      { key: newKey(), text: '', isCorrect: true },
      { key: newKey(), text: '', isCorrect: false },
    ],
  }
}

export function draftFromAdminQuestions(
  questions: { text: string; timeLimitSeconds: number | null; options: { text: string; isCorrect: boolean }[] }[],
): DraftQuestion[] {
  return questions.map((q) => ({
    key: newKey(),
    text: q.text,
    timeLimitSeconds: q.timeLimitSeconds != null ? String(q.timeLimitSeconds) : '',
    options: q.options.map((o) => ({
      key: newKey(),
      text: o.text,
      isCorrect: o.isCorrect,
    })),
  }))
}

export interface QuizEditorFormProps {
  pageTitle: string
  headerNote: string
  pin?: string
  initialTitle: string
  initialDefaultSeconds: number
  initialQuestions: DraftQuestion[]
  submitLabel: string
  pendingLabel: string
  onSubmit: (body: CreateQuizRequestDto) => Promise<void>
}

export function QuizEditorForm({
  pageTitle,
  headerNote,
  pin,
  initialTitle,
  initialDefaultSeconds,
  initialQuestions,
  submitLabel,
  pendingLabel,
  onSubmit,
}: QuizEditorFormProps) {
  const [title, setTitle] = useState(initialTitle)
  const [defaultSeconds, setDefaultSeconds] = useState(initialDefaultSeconds)
  const [questions, setQuestions] = useState<DraftQuestion[]>(initialQuestions)
  const [error, setError] = useState<string | null>(null)
  const [pending, setPending] = useState(false)

  function addQuestion() {
    setQuestions((q) => [...q, emptyQuestion()])
  }

  function removeQuestion(index: number) {
    setQuestions((q) => (q.length <= 1 ? q : q.filter((_, i) => i !== index)))
  }

  function updateQuestion(index: number, patch: Partial<DraftQuestion>) {
    setQuestions((q) => q.map((item, i) => (i === index ? { ...item, ...patch } : item)))
  }

  function addOption(qIndex: number) {
    setQuestions((q) =>
      q.map((item, i) =>
        i === qIndex
          ? { ...item, options: [...item.options, { key: newKey(), text: '', isCorrect: false }] }
          : item,
      ),
    )
  }

  function removeOption(qIndex: number, oIndex: number) {
    setQuestions((q) =>
      q.map((item, i) => {
        if (i !== qIndex) return item
        if (item.options.length <= 2) return item
        const next = item.options.filter((_, j) => j !== oIndex)
        if (!next.some((o) => o.isCorrect)) next[0] = { ...next[0], isCorrect: true }
        return { ...item, options: next }
      }),
    )
  }

  function setCorrectOption(qIndex: number, oIndex: number) {
    setQuestions((q) =>
      q.map((item, i) => {
        if (i !== qIndex) return item
        return {
          ...item,
          options: item.options.map((o, j) => ({ ...o, isCorrect: j === oIndex })),
        }
      }),
    )
  }

  function validate(): string | null {
    if (!title.trim()) return 'Add meg a kvíz címét.'
    if (defaultSeconds < 5 || defaultSeconds > 600) return 'Az alapértelmezett időkorlát 5–600 másodperc lehet.'
    for (let qi = 0; qi < questions.length; qi++) {
      const q = questions[qi]
      if (!q.text.trim()) return `${qi + 1}. kérdés szövege kötelező.`
      if (q.options.length < 2) return `${qi + 1}. kérdéshez legalább 2 válasz kell.`
      const correct = q.options.filter((o) => o.isCorrect)
      if (correct.length !== 1) return `${qi + 1}. kérdésnél pontosan egy helyes választ jelölj meg.`
      for (let oi = 0; oi < q.options.length; oi++) {
        if (!q.options[oi].text.trim()) return `${qi + 1}. kérdés, ${oi + 1}. válasz: üres szöveg.`
      }
      if (q.timeLimitSeconds.trim()) {
        const n = Number(q.timeLimitSeconds)
        if (Number.isNaN(n) || n < 5 || n > 600) return `${qi + 1}. kérdés időkorlátja 5–600 másodperc lehet.`
      }
    }
    return null
  }

  function buildBody(): CreateQuizRequestDto {
    return {
      title: title.trim(),
      defaultSecondsPerQuestion: defaultSeconds,
      questions: questions.map(
        (q): CreateQuestionDto => ({
          text: q.text.trim(),
          timeLimitSeconds: q.timeLimitSeconds.trim() ? Number(q.timeLimitSeconds) : null,
          options: q.options.map((o) => ({
            text: o.text.trim(),
            isCorrect: o.isCorrect,
          })),
        }),
      ),
    }
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const v = validate()
    if (v) {
      setError(v)
      return
    }
    setError(null)
    setPending(true)
    try {
      await onSubmit(buildBody())
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Mentés sikertelen')
    } finally {
      setPending(false)
    }
  }

  return (
    <div className="wide-card">
      <h1>{pageTitle}</h1>
      <p className="muted">{headerNote}</p>
      {pin != null && (
        <p className="muted small">
          PIN: <code className="pin">{pin}</code> (nem módosítható)
        </p>
      )}
      <form className="stack loose" onSubmit={(e) => void handleSubmit(e)}>
        <label className="field">
          <span>Cím</span>
          <input value={title} onChange={(e) => setTitle(e.target.value)} maxLength={500} required />
        </label>
        <label className="field">
          <span>Alapértelmezett időkorlát (mp, 5–600)</span>
          <input
            type="number"
            min={5}
            max={600}
            value={defaultSeconds}
            onChange={(e) => setDefaultSeconds(Number(e.target.value))}
          />
        </label>

        <div className="questions-editor">
          {questions.map((q, qi) => (
            <fieldset key={q.key} className="card question-card">
              <legend>Kérdés {qi + 1}</legend>
              <div className="question-head">
                {questions.length > 1 && (
                  <button type="button" className="btn ghost small" onClick={() => removeQuestion(qi)}>
                    Kérdés törlése
                  </button>
                )}
              </div>
              <label className="field">
                <span>Kérdés szövege</span>
                <textarea
                  rows={3}
                  value={q.text}
                  onChange={(e) => updateQuestion(qi, { text: e.target.value })}
                  maxLength={2000}
                />
              </label>
              <label className="field">
                <span>Saját időkorlát (mp, opcionális)</span>
                <input
                  type="number"
                  min={5}
                  max={600}
                  placeholder="üres = alapértelmezett"
                  value={q.timeLimitSeconds}
                  onChange={(e) => updateQuestion(qi, { timeLimitSeconds: e.target.value })}
                />
              </label>
              <div className="options-block">
                <p className="small muted">Jelöld meg a helyes választ. A kiválasztott sor kiemelve jelenik meg.</p>
                {q.options.map((o, oi) => (
                  <div key={o.key} className={`option-row ${o.isCorrect ? 'correct' : ''}`}>
                    <label className="correct-toggle">
                      <input
                        type="radio"
                        name={`correct-${q.key}`}
                        checked={o.isCorrect}
                        onChange={() => setCorrectOption(qi, oi)}
                        aria-label="Helyes válasz"
                      />
                      <span>Helyes</span>
                    </label>
                    <input
                      type="text"
                      placeholder={`Válasz ${oi + 1}`}
                      value={o.text}
                      onChange={(e) =>
                        setQuestions((qs) =>
                          qs.map((qq, i) =>
                            i === qi
                              ? {
                                  ...qq,
                                  options: qq.options.map((oo, j) =>
                                    j === oi ? { ...oo, text: e.target.value } : oo,
                                  ),
                                }
                              : qq,
                          ),
                        )
                      }
                      maxLength={500}
                    />
                    {q.options.length > 2 && (
                      <button type="button" className="btn ghost small" onClick={() => removeOption(qi, oi)}>
                        ×
                      </button>
                    )}
                  </div>
                ))}
                <button type="button" className="btn secondary small" onClick={() => addOption(qi)}>
                  + Válasz
                </button>
              </div>
            </fieldset>
          ))}
        </div>

        <button type="button" className="btn secondary" onClick={addQuestion}>
          + Új kérdés
        </button>

        {error && <p className="error-text">{error}</p>}
        <button type="submit" className="btn primary" disabled={pending}>
          {pending ? pendingLabel : submitLabel}
        </button>
      </form>
    </div>
  )
}
