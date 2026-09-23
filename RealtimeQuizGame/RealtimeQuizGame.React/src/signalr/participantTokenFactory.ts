import { getParticipantToken } from '../api/client'

export function participantTokenFactory(): string {
  return getParticipantToken() ?? ''
}
