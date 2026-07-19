import { useState } from 'react'
import CreateMatchForm from '../components/CreateMatchForm'
import MatchCard from '../components/MatchCard'
import { useMatches } from '../hooks/useMatches'
import { usePlayers } from '../hooks/usePlayers'

export default function MatchesPage() {
  const { matches, loading, error, createMatch, saveScore, deleteMatch, importWimbledon } = useMatches()
  const { players } = usePlayers()
  const [showForm, setShowForm] = useState(false)
  const [importing, setImporting] = useState(false)
  const [importMessage, setImportMessage] = useState<{ text: string; isError: boolean } | null>(null)

  const handleImport = async () => {
    setImporting(true)
    setImportMessage(null)
    try {
      const count = await importWimbledon()
      setImportMessage({ text: `${count} matches imported`, isError: false })
    } catch (err) {
      setImportMessage({ text: `Import failed: ${(err as Error).message}`, isError: true })
    } finally {
      setImporting(false)
    }
  }

  if (loading) return <p className="loading">Loading…</p>
  if (error != null) return <p className="error-msg">Error: {error}</p>

  return (
    <div>
      <div className="page-header">
        <h1>Matches</h1>
        <div style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
          {importMessage && (
            <span className={importMessage.isError ? 'error-msg' : ''} style={importMessage.isError ? {} : { fontSize: '.875rem', color: 'var(--green)' }}>
              {importMessage.text}
            </span>
          )}
          <button className="btn-secondary" onClick={handleImport} disabled={importing || showForm}>
            {importing ? 'Importing…' : 'Import Wimbledon Draw'}
          </button>
          {!showForm && (
            <button className="btn-primary" onClick={() => setShowForm(true)}>
              + New Match
            </button>
          )}
        </div>
      </div>

      {showForm && (
        <CreateMatchForm
          players={players}
          onSave={createMatch}
          onCancel={() => setShowForm(false)}
        />
      )}

      {matches.length === 0 && !showForm && (
        <p className="empty">No matches yet. Create one to get started.</p>
      )}

      {matches.map((match) => (
        <MatchCard
          key={match.id}
          match={match}
          onSaveScore={saveScore}
          onDelete={deleteMatch}
        />
      ))}
    </div>
  )
}
