import type { HubConnection } from '@microsoft/signalr'
import { useEffect, useRef } from 'react'
import { loadUserAuth } from '../api/client'
import { createQuizHubConnection } from '../signalr/createQuizHubConnection'
import { userTokenFactory } from '../signalr/userTokenFactory'
import type { QuizHubHandlers } from './useQuizHub'

export function useAdminQuizHub(
  quizId: number,
  handlers: QuizHubHandlers,
  enabled: boolean,
) {
  const handlersRef = useRef(handlers)
  handlersRef.current = handlers

  useEffect(() => {
    if (!enabled || !Number.isFinite(quizId) || !loadUserAuth()?.authToken) {
      return
    }

    const connection = createQuizHubConnection(userTokenFactory)

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
        /* admin state refresh falls back to manual reload */
      }
    }

    void connect(connection, quizId)

    return () => {
      cancelled = true
      void connection.invoke('LeaveQuizGroup', quizId).finally(() => connection.stop())
    }
  }, [enabled, quizId])
}
