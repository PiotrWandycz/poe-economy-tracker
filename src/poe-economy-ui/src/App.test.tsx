import { render, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'

describe('App', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
      json: () => Promise.resolve({ sections: [], divineOrbRate: 0 }),
    }))
  })

  it('renders without crashing', () => {
    render(<App />)
  })

  it('fetches /api/economy on mount', async () => {
    render(<App />)
    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/api/economy'))
    })
  })
})
