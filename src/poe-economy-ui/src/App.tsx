import { useEffect, useState } from 'react'
import { EconomyOverview } from './EconomyOverview'
import type { EconomyData } from './types'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

function App() {
  const [data, setData] = useState<EconomyData | null>(null)

  useEffect(() => {
    fetch(`${API_URL}/api/economy`)
      .then(r => r.json())
      .then(setData)
  }, [])

  if (!data) return <div className="p-8 text-zinc-500">Loading…</div>

  return <EconomyOverview data={data} />
}

export default App
