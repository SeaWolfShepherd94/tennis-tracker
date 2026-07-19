import {
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'

export interface WinLossPoint {
  date: string
  wins: number
  losses: number
}

interface WinLossChartProps {
  data: WinLossPoint[]
}

export default function WinLossChart({ data }: WinLossChartProps) {
  return (
    <div className="card chart-card">
      <h2>Win/Loss Record Over Time</h2>
      <p className="chart-subtitle">Cumulative record by match date</p>
      <div className="win-loss-chart">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={data} margin={{ top: 8, right: 12, left: -18, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="var(--gray-200)" />
            <XAxis dataKey="date" tick={{ fontSize: 12, fill: '#5f6368' }} />
            <YAxis allowDecimals={false} tick={{ fontSize: 12, fill: '#5f6368' }} />
            <Tooltip />
            <Legend />
            <Line type="stepAfter" dataKey="wins" name="Wins" stroke="#2d6a4f" strokeWidth={2} />
            <Line type="stepAfter" dataKey="losses" name="Losses" stroke="#c0392b" strokeWidth={2} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}
