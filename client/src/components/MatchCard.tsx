import { useState } from 'react'
import type { Match, MatchSet } from '../types'

interface MatchCardProps {
  match: Match
  onSaveScore: (id: string, sets: MatchSet[]) => Promise<Match>
  onDelete: (id: string) => void
}

const statusClass: Record<string, string> = {
  Completed: 'status-completed',
  InProgress: 'status-inprogress',
  Scheduled: 'status-scheduled',
  Cancelled: 'status-cancelled',
}

const badgeClass: Record<string, string> = {
  Completed: 'badge-completed',
  InProgress: 'badge-inprogress',
  Scheduled: 'badge-scheduled',
  Cancelled: 'badge-cancelled',
}

function scoreString(sets: MatchSet[]): string {
  return sets
    .map((s) => {
      const tb =
        s.playerOneTiebreakPoints != null && s.playerTwoTiebreakPoints != null
          ? ` (${s.playerOneTiebreakPoints}/${s.playerTwoTiebreakPoints})`
          : ''
      return `${s.playerOneGames}-${s.playerTwoGames}${tb}`
    })
    .join(', ')
}

interface SetRow {
  p1: string
  p2: string
  tb1: string
  tb2: string
}

function emptyRow(): SetRow { return { p1: '', p2: '', tb1: '', tb2: '' } }

function needsTiebreak(row: SetRow): boolean {
  const p1 = parseInt(row.p1 || '0', 10)
  const p2 = parseInt(row.p2 || '0', 10)
  return p1 >= 6 && p2 >= 6
}

function validateBestOfFive(rows: SetRow[]): string | null {
  const complete = rows.filter((r) => r.p1 !== '' && r.p2 !== '')
  if (complete.length === 0) return 'Enter at least one set score.'

  let p1Sets = 0, p2Sets = 0
  for (const row of complete) {
    const p1 = parseInt(row.p1, 10)
    const p2 = parseInt(row.p2, 10)
    const tb = needsTiebreak(row)
    const p1Wins =
      p1 > p2 ||
      (tb && row.tb1 !== '' && row.tb2 !== '' && parseInt(row.tb1, 10) > parseInt(row.tb2, 10))
    if (p1Wins) p1Sets++; else p2Sets++
  }

  if (p1Sets < 3 && p2Sets < 3) return 'A player must win 3 sets (best of 5).'
  return null
}

export default function MatchCard({ match, onSaveScore, onDelete }: MatchCardProps) {
  const [showScores, setShowScores] = useState(false)
  const [rows, setRows] = useState<SetRow[]>([emptyRow()])
  const [saving, setSaving] = useState(false)
  const [scoreError, setScoreError] = useState<string | null>(null)

  const updateRow = (i: number, field: keyof SetRow, value: string) => {
    setRows((prev) => prev.map((r, idx) => (idx === i ? { ...r, [field]: value } : r)))
  }

  const openScoreEntry = () => {
    setRows([emptyRow()])
    setScoreError(null)
    setShowScores(true)
  }

  const saveScores = async () => {
    const validationError = validateBestOfFive(rows)
    if (validationError) {
      setScoreError(validationError)
      return
    }

    const sets: MatchSet[] = rows
      .filter((r) => r.p1 !== '' && r.p2 !== '')
      .map((r, i) => ({
        setNumber: i + 1,
        playerOneGames: parseInt(r.p1, 10),
        playerTwoGames: parseInt(r.p2, 10),
        playerOneTiebreakPoints:
          needsTiebreak(r) && r.tb1 !== '' ? parseInt(r.tb1, 10) : undefined,
        playerTwoTiebreakPoints:
          needsTiebreak(r) && r.tb2 !== '' ? parseInt(r.tb2, 10) : undefined,
      }))

    setSaving(true)
    setScoreError(null)
    try {
      await onSaveScore(match.id, sets)
      setShowScores(false)
    } catch (err) {
      setScoreError(err instanceof Error ? err.message : 'Failed to save')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className={`card match-card ${statusClass[match.status] ?? ''}`}>
      <div className="match-meta">
        <span>{match.surface}</span>
        <span>·</span>
        <time dateTime={match.startTime}>{new Date(match.startTime).toLocaleDateString()}</time>
        <span className={`badge ${badgeClass[match.status] ?? ''}`}>{match.status}</span>
      </div>

      <div className="match-players">
        {match.playerOneName ?? 'Player 1'}{' '}
        <span style={{ color: 'var(--gray-400)', fontWeight: 400 }}>vs</span>{' '}
        {match.playerTwoName ?? 'Player 2'}
      </div>

      {match.sets.length > 0 && (
        <div className="match-score">{scoreString(match.sets)}</div>
      )}

      {match.winnerName && (
        <div className="match-winner">Winner: {match.winnerName}</div>
      )}

      <div className="match-actions">
        {match.status === 'Scheduled' && !showScores && (
          <button className="btn-secondary btn-sm" onClick={openScoreEntry}>
            Enter Score
          </button>
        )}
        {!showScores && (
          <button className="btn-danger btn-sm" onClick={() => onDelete(match.id)}>
            Delete
          </button>
        )}
      </div>

      {showScores && match.status !== 'Completed' && (
        <div className="score-entry">
          <h3>
            <span style={{ fontWeight: 400 }}>{match.playerOneName ?? 'P1'}</span>
            {' vs '}
            <span style={{ fontWeight: 400 }}>{match.playerTwoName ?? 'P2'}</span>
          </h3>
          {rows.map((row, i) => (
            <div className="score-row" key={i}>
              <label>Set {i + 1}</label>
              <input
                className="score-input"
                type="number"
                min={0}
                max={7}
                value={row.p1}
                placeholder="—"
                onChange={(e) => updateRow(i, 'p1', e.target.value)}
              />
              <span className="score-sep">–</span>
              <input
                className="score-input"
                type="number"
                min={0}
                max={7}
                value={row.p2}
                placeholder="—"
                onChange={(e) => updateRow(i, 'p2', e.target.value)}
              />
              {needsTiebreak(row) && (
                <>
                  <span style={{ fontSize: '.75rem', color: 'var(--gray-400)', marginLeft: 8 }}>
                    TB:
                  </span>
                  <input
                    className="score-input"
                    type="number"
                    min={0}
                    value={row.tb1}
                    placeholder="—"
                    onChange={(e) => updateRow(i, 'tb1', e.target.value)}
                  />
                  <span className="score-sep">–</span>
                  <input
                    className="score-input"
                    type="number"
                    min={0}
                    value={row.tb2}
                    placeholder="—"
                    onChange={(e) => updateRow(i, 'tb2', e.target.value)}
                  />
                </>
              )}
              {rows.length > 1 && (
                <button
                  className="btn-danger btn-sm"
                  onClick={() => setRows((prev) => prev.filter((_, idx) => idx !== i))}
                >
                  ✕
                </button>
              )}
            </div>
          ))}
          {rows.length < 5 && (
            <button
              className="btn-secondary btn-sm"
              style={{ marginTop: 4 }}
              onClick={() => setRows((prev) => [...prev, emptyRow()])}
            >
              + Add Set
            </button>
          )}
          {scoreError != null && <p className="error-msg">{scoreError}</p>}
          <div className="form-actions" style={{ marginTop: 12 }}>
            <button className="btn-primary btn-sm" onClick={saveScores} disabled={saving}>
              {saving ? 'Saving…' : 'Save Score'}
            </button>
            <button className="btn-secondary btn-sm" onClick={() => setShowScores(false)}>
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  )
}
