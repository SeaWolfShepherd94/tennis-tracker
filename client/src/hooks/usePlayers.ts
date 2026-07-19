import { useCallback, useEffect, useState } from 'react'
import { playersApi } from '../services/api'
import type { Player } from '../types'

export function usePlayers() {
  const [players, setPlayers] = useState<Player[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    setLoading(true)
    playersApi
      .getAll()
      .then(setPlayers)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  const createPlayer = async (player: Partial<Player>): Promise<Player> => {
    const created = await playersApi.create(player)
    setPlayers((prev) => [...prev, created].sort((a, b) => a.name.localeCompare(b.name)))
    return created
  }

  const deletePlayer = async (id: string): Promise<void> => {
    await playersApi.delete(id)
    setPlayers((prev) => prev.filter((p) => p.id !== id))
  }

  return { players, loading, error, createPlayer, deletePlayer, reload: load }
}
