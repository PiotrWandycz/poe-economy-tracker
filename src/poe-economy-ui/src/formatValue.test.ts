import { describe, expect, it } from 'vitest'
import { formatValue } from './formatValue'

describe('formatValue', () => {
  it('shows Exalted Orbs when value is below 1 Divine', () => {
    expect(formatValue(50, 200)).toBe('50 Exalted Orbs')
  })

  it('shows 1 Divine Orb when value equals the divine rate', () => {
    expect(formatValue(200, 200)).toBe('1 Divine Orb')
  })

  it('shows plural Divine Orbs for whole multiples', () => {
    expect(formatValue(400, 200)).toBe('2 Divine Orbs')
  })

  it('shows one decimal place for fractional Divines >= 1', () => {
    expect(formatValue(300, 200)).toBe('1.5 Divine Orbs')
  })
})
