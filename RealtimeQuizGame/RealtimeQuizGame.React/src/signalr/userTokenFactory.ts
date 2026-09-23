import { loadUserAuth } from '../api/client'

export function userTokenFactory(): string {
  return loadUserAuth()?.authToken ?? ''
}
