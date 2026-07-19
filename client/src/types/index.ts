export interface Player {
  id: string
  name: string
  nationality?: string
  ranking?: number
  createdAt: string
}

export interface MatchSet {
  setNumber: number
  playerOneGames: number
  playerTwoGames: number
  playerOneTiebreakPoints?: number
  playerTwoTiebreakPoints?: number
}

export interface Match {
  id: string
  playerOneId: string
  playerTwoId: string
  playerOneName: string
  playerTwoName: string
  startTime: string
  endTime?: string
  surface: 'Hard' | 'Clay' | 'Grass' | 'Carpet'
  status: 'Scheduled' | 'InProgress' | 'Completed' | 'Cancelled'
  winnerId?: string
  winnerName?: string
  sets: MatchSet[]
}
