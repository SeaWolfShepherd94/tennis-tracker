import type { Player } from '../types'

interface PlayerCardProps {
  player: Player
  onDelete: (id: string) => void
}

export default function PlayerCard({ player, onDelete }: PlayerCardProps) {
  return (
    <div className="card player-card">
      <div className="player-info">
        <div>
          <div className="player-name">{player.name}</div>
          <div className="player-meta">
            {player.nationality && <span>{player.nationality}</span>}
            {player.nationality && player.ranking != null && <span> · </span>}
            {player.ranking != null && <span>Ranked #{player.ranking}</span>}
          </div>
        </div>
      </div>
      <button
        className="btn-danger btn-sm"
        onClick={() => onDelete(player.id)}
      >
        Delete
      </button>
    </div>
  )
}
