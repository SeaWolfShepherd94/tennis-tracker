import { useState } from 'react'
import type { Player } from '../types'

interface CreatePlayerFormProps {
  onSave: (player: Partial<Player>) => Promise<unknown>
  onCancel: () => void
}

export default function CreatePlayerForm({ onSave, onCancel }: CreatePlayerFormProps) {
  const [name, setName] = useState('')
  const [nationality, setNationality] = useState('')
  const [ranking, setRanking] = useState('')
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!name.trim()) return
    setSaving(true)
    setError(null)
    try {
      await onSave({
        name: name.trim(),
        nationality: nationality.trim() || undefined,
        ranking: ranking ? parseInt(ranking, 10) : undefined,
      })
      setName('')
      setNationality('')
      setRanking('')
      onCancel()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="form-panel">
      <h2>Add Player</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-row">
          <div className="form-field">
            <label htmlFor="player-name">Name *</label>
            <input
              id="player-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Full name"
              required
            />
          </div>
          <div className="form-field">
            <label htmlFor="player-nationality">Nationality</label>
            <input
              id="player-nationality"
              value={nationality}
              onChange={(e) => setNationality(e.target.value)}
              placeholder="e.g. USA"
            />
          </div>
          <div className="form-field" style={{ maxWidth: 120 }}>
            <label htmlFor="player-ranking">Ranking</label>
            <input
              id="player-ranking"
              type="number"
              min={1}
              value={ranking}
              onChange={(e) => setRanking(e.target.value)}
              placeholder="e.g. 42"
            />
          </div>
        </div>
        {error != null && <p className="error-msg">{error}</p>}
        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={saving || !name.trim()}>
            {saving ? 'Saving…' : 'Add Player'}
          </button>
          <button type="button" className="btn-secondary" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}
