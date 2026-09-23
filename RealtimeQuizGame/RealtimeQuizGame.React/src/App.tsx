import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Layout } from './components/Layout'
import { ProtectedRoute } from './components/ProtectedRoute'
import { AuthProvider } from './context/AuthContext'
import { AdminQuizPage } from './pages/AdminQuizPage'
import { CreateQuizPage } from './pages/CreateQuizPage'
import { EditQuizPage } from './pages/EditQuizPage'
import { HomePage } from './pages/HomePage'
import { JoinQuizPage } from './pages/JoinQuizPage'
import { LoginPage } from './pages/LoginPage'
import { MyQuizzesPage } from './pages/MyQuizzesPage'
import { ParticipantPlayPage } from './pages/ParticipantPlayPage'
import { RegisterPage } from './pages/RegisterPage'

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<HomePage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="register" element={<RegisterPage />} />
            <Route path="join" element={<JoinQuizPage />} />
            <Route path="play" element={<ParticipantPlayPage />} />
            <Route
              path="admin/quizzes"
              element={
                <ProtectedRoute>
                  <MyQuizzesPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="admin/create"
              element={
                <ProtectedRoute>
                  <CreateQuizPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="admin/quiz/:id/edit"
              element={
                <ProtectedRoute>
                  <EditQuizPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="admin/quiz/:id"
              element={
                <ProtectedRoute>
                  <AdminQuizPage />
                </ProtectedRoute>
              }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
