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

  it('lists items with name and value in Exalts', () => {
    render(<EconomyOverview data={BASE} />)
    expect(screen.getByText('Divine Orb')).toBeInTheDocument()
    expect(screen.getByText('1000 Ex')).toBeInTheDocument()
    expect(screen.getByText('Orb of Annulment')).toBeInTheDocument()
    expect(screen.getByText('400 Ex')).toBeInTheDocument()
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
