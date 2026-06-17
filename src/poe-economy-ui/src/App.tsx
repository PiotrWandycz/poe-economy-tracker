import { useEffect } from 'react'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

function App() {
  useEffect(() => {
    fetch(`${API_URL}/api/economy`)
      .then(r => r.json())
      .then(data => console.log('[economy]', data))
  }, [])

  return <div>PoE Economy Tracker</div>
}

export default App
