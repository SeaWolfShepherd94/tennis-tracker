import { useCallback, useEffect, useState } from 'react'
import { matchesApi } from '../services/api'
import type { Match, MatchSet } from '../types'

export function useMatches() {
  const [matches, setMatches] = useState<Match[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    matchesApi
      .getAll()
      .then(setMatches)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const createMatch = async (match: Partial<Match>): Promise<Match> => {
    const created = await matchesApi.create(match)
    setMatches((prev) => [created, ...prev])
    return created
  }

  const updateMatch = async (id: string, match: Partial<Match>): Promise<Match> => {
    const updated = await matchesApi.update(id, match)
    setMatches((prev) => prev.map((m) => (m.id === id ? updated : m)))
    return updated
  }

  const saveScore = async (id: string, sets: MatchSet[]): Promise<Match> => {
    const updated = await matchesApi.saveScore(id, sets)
    setMatches((prev) => prev.map((m) => (m.id === id ? updated : m)))
    return updated
  }

  const deleteMatch = async (id: string): Promise<void> => {
    await matchesApi.delete(id)
    setMatches((prev) => prev.filter((m) => m.id !== id))
  }

  const importWimbledon = async (): Promise<number> => {
    const result = await matchesApi.importWimbledon()
    load()
    return result.matchesCreated
  }

  return { matches, loading, error, createMatch, updateMatch, saveScore, deleteMatch, importWimbledon, reload: load }
}
