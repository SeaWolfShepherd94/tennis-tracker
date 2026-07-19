import OpponentBreakdown from '../components/OpponentBreakdown'
import RecentForm from '../components/RecentForm'
import StatCard from '../components/StatCard'
import WinLossChart, { type WinLossPoint } from '../components/WinLossChart'
import { useMatches } from '../hooks/useMatches'
import type { Match, MatchSet } from '../types'

const TRACKED_PLAYER_ID = '6bd7ca81-9e95-4190-bce1-53d39e573c45'
const TRACKED_PLAYER_NAME = 'Rafael Nadal'

function isTrackedCompletedMatch(match: Match): boolean {
  const isTrackedPlayer = match.playerOneId === TRACKED_PLAYER_ID || match.playerTwoId === TRACKED_PLAYER_ID
  const hasValidWinner =
    match.winnerId != null &&
    (match.winnerId === match.playerOneId || match.winnerId === match.playerTwoId)

  return match.status === 'Completed' && isTrackedPlayer && hasValidWinner
}

function didTrackedPlayerWinSet(match: Match, set: MatchSet): boolean {
  const isPlayerOne = match.playerOneId === TRACKED_PLAYER_ID
  const trackedGames = isPlayerOne ? set.playerOneGames : set.playerTwoGames
  const opponentGames = isPlayerOne ? set.playerTwoGames : set.playerOneGames

  if (trackedGames !== opponentGames) return trackedGames > opponentGames

  const trackedTiebreak = isPlayerOne ? set.playerOneTiebreakPoints : set.playerTwoTiebreakPoints
  const opponentTiebreak = isPlayerOne ? set.playerTwoTiebreakPoints : set.playerOneTiebreakPoints
  return trackedTiebreak != null && opponentTiebreak != null && trackedTiebreak > opponentTiebreak
}

export default function StatsPage() {
  const { matches, loading, error } = useMatches()

  if (loading) return <p className="loading">Loading…</p>
  if (error != null) return <p className="error-msg">Error: {error}</p>

  const completedMatches = matches
    .filter(isTrackedCompletedMatch)
    .sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime())

  let wins = 0
  let losses = 0
  const chartData: WinLossPoint[] = completedMatches.map((match) => {
    if (match.winnerId === TRACKED_PLAYER_ID) wins++
    else losses++

    return {
      date: new Date(match.startTime).toLocaleDateString(),
      wins,
      losses,
    }
  })

  const setsWon = completedMatches.reduce(
    (total, match) => total + match.sets.filter((set) => didTrackedPlayerWinSet(match, set)).length,
    0,
  )
  const winPercentage = completedMatches.length === 0 ? 0 : (wins / completedMatches.length) * 100

  return (
    <div>
      <div className="page-header">
        <h1>{TRACKED_PLAYER_NAME}&apos;s Stats</h1>
      </div>

      <div className="stats-grid">
        <StatCard label="Total Matches" value={String(completedMatches.length)} />
        <StatCard label="Win %" value={`${winPercentage.toFixed(0)}%`} />
        <StatCard label="Sets Won" value={String(setsWon)} />
      </div>

      <RecentForm matches={completedMatches} trackedPlayerId={TRACKED_PLAYER_ID} />

      {completedMatches.length === 0 ? (
        <p className="empty">No completed matches with a recorded winner for {TRACKED_PLAYER_NAME} yet.</p>
      ) : (
        <>
          <WinLossChart data={chartData} />
          <OpponentBreakdown matches={completedMatches} trackedPlayerId={TRACKED_PLAYER_ID} />
        </>
      )}
    </div>
  )
}
