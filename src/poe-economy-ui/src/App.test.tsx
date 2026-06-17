import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'

const MOCK_RESPONSE = {
  sections: [{ name: 'Currency', group: 'General', items: [{ name: 'Divine Orb', valueInExalts: 1000 }] }],
  divineOrbRate: 200,
}

describe('App', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
      json: () => Promise.resolve(MOCK_RESPONSE),
    }))
  })

  it('fetches /api/economy on mount', async () => {
    render(<App />)
    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(expect.stringContaining('/api/economy'))
    })
  })

  it('renders section data returned from the API', async () => {
    render(<App />)
    expect(await screen.findByRole('heading', { name: 'Currency' })).toBeInTheDocument()
  })
})
