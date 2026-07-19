import { useState } from 'react'
import CreatePlayerForm from '../components/CreatePlayerForm'
import PlayerCard from '../components/PlayerCard'
import { usePlayers } from '../hooks/usePlayers'

export default function PlayersPage() {
  const { players, loading, error, createPlayer, deletePlayer } = usePlayers()
  const [showForm, setShowForm] = useState(false)

  if (loading) return <p className="loading">Loading…</p>
  if (error != null) return <p className="error-msg">Error: {error}</p>

  return (
    <div>
      <div className="page-header">
        <h1>Players</h1>
        {!showForm && (
          <button className="btn-primary" onClick={() => setShowForm(true)}>
            + Add Player
          </button>
        )}
      </div>

      {showForm && (
        <CreatePlayerForm onSave={createPlayer} onCancel={() => setShowForm(false)} />
      )}

      {players.length === 0 && !showForm && (
        <p className="empty">No players yet. Add one to get started.</p>
      )}

      {players.map((player) => (
        <PlayerCard key={player.id} player={player} onDelete={deletePlayer} />
      ))}
    </div>
  )
}
