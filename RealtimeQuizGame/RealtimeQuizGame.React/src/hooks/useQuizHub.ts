import type { HubConnection } from '@microsoft/signalr'
import { useEffect, useRef } from 'react'
import { getParticipantToken } from '../api/client'
import { createQuizHubConnection } from '../signalr/createQuizHubConnection'
import { participantTokenFactory } from '../signalr/participantTokenFactory'

export interface QuizHubHandlers {
  onPlayStateUpdated: (quizId: number) => void
  onQuizSessionReset: (quizId: number) => void
}

export function useQuizHub(
  handlers: QuizHubHandlers,
  enabled: boolean,
  quizId: number | null,
) {
  const handlersRef = useRef(handlers)
  handlersRef.current = handlers

  useEffect(() => {
    if (!enabled || !getParticipantToken() || quizId == null || !Number.isFinite(quizId)) {
      return
    }

    const connection = createQuizHubConnection(participantTokenFactory)

    connection.on('PlayStateUpdated', (id: number) => {
      handlersRef.current.onPlayStateUpdated(id)
    })
    connection.on('QuizSessionReset', (id: number) => {
      handlersRef.current.onQuizSessionReset(id)
    })

    let cancelled = false

    async function connect(conn: HubConnection, id: number) {
      try {
        await conn.start()
        if (!cancelled) {
          await conn.invoke('JoinQuizGroup', id)
        }
      } catch {
        /* connection errors surface on next play-state fetch */
      }
    }

    void connect(connection, quizId)

    return () => {
      cancelled = true
      void connection.invoke('LeaveQuizGroup', quizId).finally(() => connection.stop())
    }
  }, [enabled, quizId])
}
