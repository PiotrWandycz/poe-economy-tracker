import { describe, expect, it } from 'vitest'
import { formatValue } from './formatValue'

describe('formatValue', () => {
  it('shows Ex when value is below 1 Divine', () => {
    expect(formatValue(50, 200)).toBe('50 Ex')
  })

  it('shows 1 Div when value equals the divine rate', () => {
    expect(formatValue(200, 200)).toBe('1 Div')
  })

  it('shows whole multiples in Div', () => {
    expect(formatValue(400, 200)).toBe('2 Div')
  })

  it('shows one decimal place for fractional Divines >= 1', () => {
    expect(formatValue(300, 200)).toBe('1.5 Div')
  })
})
