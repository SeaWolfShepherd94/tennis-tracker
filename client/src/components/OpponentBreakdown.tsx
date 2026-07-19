import type { Match } from '../types'

interface OpponentRecord {
  id: string
  name: string
  matchesPlayed: number
  wins: number
  losses: number
}

interface OpponentBreakdownProps {
  matches: Match[]
  trackedPlayerId: string
}

export default function OpponentBreakdown({ matches, trackedPlayerId }: OpponentBreakdownProps) {
  const opponents = matches.reduce<Map<string, OpponentRecord>>((records, match) => {
    const isPlayerOne = match.playerOneId === trackedPlayerId
    const opponentId = isPlayerOne ? match.playerTwoId : match.playerOneId
    const opponentName = isPlayerOne ? match.playerTwoName : match.playerOneName
    const record = records.get(opponentId) ?? {
      id: opponentId,
      name: opponentName ?? 'Unknown opponent',
      matchesPlayed: 0,
      wins: 0,
      losses: 0,
    }

    record.matchesPlayed++
    if (match.winnerId === trackedPlayerId) record.wins++
    else record.losses++
    records.set(opponentId, record)
    return records
  }, new Map())

  const sortedOpponents = [...opponents.values()].sort((a, b) =>
    b.matchesPlayed - a.matchesPlayed || a.name.localeCompare(b.name),
  )

  if (sortedOpponents.length === 0) return null

  return (
    <section className="card opponent-breakdown">
      <h2>Opponent Breakdown</h2>
      <div className="opponent-table-wrap">
        <table>
          <thead>
            <tr>
              <th>Opponent</th>
              <th>Played</th>
              <th>Wins</th>
              <th>Losses</th>
            </tr>
          </thead>
          <tbody>
            {sortedOpponents.map((opponent) => (
              <tr key={opponent.id}>
                <td>{opponent.name}</td>
                <td>{opponent.matchesPlayed}</td>
                <td>{opponent.wins}</td>
                <td>{opponent.losses}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  )
}
