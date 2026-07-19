import type { Match, MatchSet, Player } from '../types'

const BASE_URL = '/api'

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options?.headers },
    ...options,
  })
  if (!response.ok) {
    const body = await response.json().catch(() => ({ error: response.statusText }))
    throw new Error((body as { error?: string }).error ?? 'Request failed')
  }
  return response.json() as Promise<T>
}

export const matchesApi = {
  getAll: () => request<Match[]>('/matches'),
  getById: (id: string) => request<Match>(`/matches/${id}`),
  create: (match: Partial<Match>) =>
    request<Match>('/matches', { method: 'POST', body: JSON.stringify(match) }),
  update: (id: string, match: Partial<Match>) =>
    request<Match>(`/matches/${id}`, { method: 'PUT', body: JSON.stringify(match) }),
  saveScore: (id: string, sets: MatchSet[]) =>
    request<Match>(`/matches/${id}/score`, { method: 'PUT', body: JSON.stringify({ sets }) }),
  delete: (id: string) => request<void>(`/matches/${id}`, { method: 'DELETE' }),
  importWimbledon: () =>
    request<{ matchesCreated: number }>('/matches/import-wimbledon', { method: 'POST' }),
}

export const playersApi = {
  getAll: () => request<Player[]>('/players'),
  getById: (id: string) => request<Player>(`/players/${id}`),
  create: (player: Partial<Player>) =>
    request<Player>('/players', { method: 'POST', body: JSON.stringify(player) }),
  update: (id: string, player: Partial<Player>) =>
    request<Player>(`/players/${id}`, { method: 'PUT', body: JSON.stringify(player) }),
  delete: (id: string) => request<void>(`/players/${id}`, { method: 'DELETE' }),
}
