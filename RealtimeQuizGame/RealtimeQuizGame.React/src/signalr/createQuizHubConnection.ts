import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

export function createQuizHubConnection(
  accessTokenFactory: () => string | Promise<string>,
): HubConnection {
  return new HubConnectionBuilder()
    .withUrl('/QuizHub', {
      accessTokenFactory: () => Promise.resolve(accessTokenFactory()),
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build()
}
