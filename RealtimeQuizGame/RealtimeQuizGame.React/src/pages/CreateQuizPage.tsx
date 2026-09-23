import { useNavigate } from 'react-router-dom'
import { createQuiz } from '../api/client'
import { emptyQuestion, QuizEditorForm } from '../components/QuizEditorForm'

export function CreateQuizPage() {
  const navigate = useNavigate()

  return (
    <QuizEditorForm
      pageTitle="Új kvíz"
      headerNote="A kvíz a kvíz indításáig szerkeszthető. Minden kérdéshez legalább két válasz és pontosan egy helyes megjelölés szükséges."
      initialTitle=""
      initialDefaultSeconds={30}
      initialQuestions={[emptyQuestion()]}
      submitLabel="Kvíz létrehozása"
      pendingLabel="Kvíz mentése…"
      onSubmit={async (body) => {
        const created = await createQuiz(body)
        navigate(`/admin/quiz/${created.id}`, { replace: true })
      }}
    />
  )
}
