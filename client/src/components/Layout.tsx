import type { ReactNode } from 'react'
import { Link, useLocation } from 'react-router-dom'

interface LayoutProps {
  children: ReactNode
}

export default function Layout({ children }: LayoutProps) {
  const { pathname } = useLocation()

  return (
    <>
      <nav>
        <strong>Tennis Tracker</strong>
        <Link to="/matches" aria-current={pathname === '/' || pathname.startsWith('/matches') ? 'page' : undefined}>
          Matches
        </Link>
        <Link to="/players" aria-current={pathname === '/players' ? 'page' : undefined}>
          Players
        </Link>
        <Link to="/stats" aria-current={pathname === '/stats' ? 'page' : undefined}>
          Stats
        </Link>
      </nav>
      <main>{children}</main>
    </>
  )
}
