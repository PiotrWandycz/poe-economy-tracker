import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { EconomyOverview } from './EconomyOverview'
import type { EconomyData } from './types'

const BASE: EconomyData = {
  sections: [
    {
      name: 'Currency',
      group: 'General',
      items: [
        { name: 'Divine Orb', valueInExalts: 1000 },
        { name: 'Orb of Annulment', valueInExalts: 400 },
      ],
    },
  ],
  divineOrbRate: 200,
}

describe('EconomyOverview', () => {
  it('shows each section name as a heading', () => {
    render(<EconomyOverview data={BASE} />)
    expect(screen.getByRole('heading', { name: 'Currency' })).toBeInTheDocument()
  })

  it('shows Divine Orbs for items at or above the divine rate', () => {
    render(<EconomyOverview data={BASE} />)
    // 1000 Ex at rate 200 = 5 Divine Orbs
    expect(screen.getByText('Divine Orb')).toBeInTheDocument()
    expect(screen.getByText('5 Divine Orbs')).toBeInTheDocument()
  })

  it('shows Exalted Orbs for items below the divine rate', () => {
    const data = {
      sections: [{ name: 'Currency', group: 'General', items: [{ name: 'Chaos Orb', valueInExalts: 50 }] }],
      divineOrbRate: 200,
    }
    render(<EconomyOverview data={data} />)
    expect(screen.getByText('50 Exalted Orbs')).toBeInTheDocument()
  })

  it('hides sections with no items', () => {
    const data: EconomyData = {
      ...BASE,
      sections: [
        { name: 'Empty', group: 'General', items: [] },
        BASE.sections[0],
      ],
    }
    render(<EconomyOverview data={data} />)
    expect(screen.queryByRole('heading', { name: 'Empty' })).not.toBeInTheDocument()
    expect(screen.getByRole('heading', { name: 'Currency' })).toBeInTheDocument()
  })

  it('renders items in the order received (descending by value)', () => {
    render(<EconomyOverview data={BASE} />)
    const items = screen.getAllByRole('listitem')
    expect(items[0]).toHaveTextContent('Divine Orb')
    expect(items[1]).toHaveTextContent('Orb of Annulment')
  })
})
