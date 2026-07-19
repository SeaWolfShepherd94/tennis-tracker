import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import MatchesPage from './pages/MatchesPage'
import PlayersPage from './pages/PlayersPage'
import StatsPage from './pages/StatsPage'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<MatchesPage />} />
        <Route path="/matches" element={<MatchesPage />} />
        <Route path="/players" element={<PlayersPage />} />
        <Route path="/stats" element={<StatsPage />} />
      </Routes>
    </Layout>
  )
}
