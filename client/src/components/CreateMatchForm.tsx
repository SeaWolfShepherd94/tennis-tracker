import { useState } from 'react'
import type { Match, Player } from '../types'

interface CreateMatchFormProps {
  players: Player[]
  onSave: (match: Partial<Match>) => Promise<unknown>
  onCancel: () => void
}

const SURFACES = ['Hard', 'Clay', 'Grass', 'Carpet'] as const

export default function CreateMatchForm({ players, onSave, onCancel }: CreateMatchFormProps) {
  const [playerOneId, setPlayerOneId] = useState('')
  const [playerTwoId, setPlayerTwoId] = useState('')
  const [surface, setSurface] = useState<typeof SURFACES[number]>('Hard')
  const [startDate, setStartDate] = useState(new Date().toISOString().slice(0, 10))
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!playerOneId || !playerTwoId) return
    if (playerOneId === playerTwoId) {
      setError('Player 1 and Player 2 must be different.')
      return
    }
    setSaving(true)
    setError(null)
    try {
      await onSave({
        playerOneId,
        playerTwoId,
        surface,
        startTime: `${startDate}T00:00:00`,
        status: 'Scheduled',
      })
      onCancel()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save')
    } finally {
      setSaving(false)
    }
  }

  if (players.length < 2) {
    return (
      <div className="form-panel">
        <p className="player-meta">Add at least 2 players before creating a match.</p>
        <div className="form-actions" style={{ marginTop: 12 }}>
          <button type="button" className="btn-secondary" onClick={onCancel}>Close</button>
        </div>
      </div>
    )
  }

  return (
    <div className="form-panel">
      <h2>New Match</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-row">
          <div className="form-field">
            <label htmlFor="p1">Player 1 *</label>
            <select id="p1" value={playerOneId} onChange={(e) => setPlayerOneId(e.target.value)} required>
              <option value="">Select player…</option>
              {players.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
          </div>
          <div className="form-field">
            <label htmlFor="p2">Player 2 *</label>
            <select id="p2" value={playerTwoId} onChange={(e) => setPlayerTwoId(e.target.value)} required>
              <option value="">Select player…</option>
              {players.map((p) => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="form-row">
          <div className="form-field">
            <label htmlFor="surface">Surface</label>
            <select
              id="surface"
              value={surface}
              onChange={(e) => setSurface(e.target.value as typeof SURFACES[number])}
            >
              {SURFACES.map((s) => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>
          <div className="form-field">
            <label htmlFor="match-date">Date</label>
            <input
              id="match-date"
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
            />
          </div>
        </div>
        {error != null && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving || !playerOneId || !playerTwoId}>
            {saving ? 'Creating…' : 'Create Match'}
          </button>
          <button type="button" className="btn-secondary" onClick={onCancel}>Cancel</button>
        </div>
      </form>
    </div>
  )
}
