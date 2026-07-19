interface StatCardProps {
  label: string
  value: string
}

export default function StatCard({ label, value }: StatCardProps) {
  return (
    <div className="card stat-card">
      <span className="stat-card-label">{label}</span>
      <strong className="stat-card-value">{value}</strong>
    </div>
  )
}
