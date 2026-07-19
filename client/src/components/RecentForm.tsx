import type { Match } from '../types'

interface RecentFormProps {
  matches: Match[]
  trackedPlayerId: string
}

export default function RecentForm({ matches, trackedPlayerId }: RecentFormProps) {
  const recentMatches = matches.slice(-5)

  if (recentMatches.length === 0) return null

  return (
    <div className="recent-form" aria-label="Recent form">
      <span className="recent-form-label">Recent form</span>
      <div className="recent-form-badges">
        {recentMatches.map((match) => {
          const won = match.winnerId === trackedPlayerId
          return (
            <span
              key={match.id}
              className={`form-badge ${won ? 'form-win' : 'form-loss'}`}
              title={`${new Date(match.startTime).toLocaleDateString()}: ${won ? 'Win' : 'Loss'}`}
            >
              {won ? 'W' : 'L'}
            </span>
          )
        })}
      </div>
    </div>
  )
}
