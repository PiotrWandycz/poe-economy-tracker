export function formatValue(valueInExalts: number, divineOrbRate: number): string {
  if (valueInExalts >= divineOrbRate) {
    const divines = valueInExalts / divineOrbRate
    const display = divines % 1 === 0 ? divines : divines.toFixed(1)
    return `${display} Divine Orb${divines === 1 ? '' : 's'}`
  }
  return `${Math.round(valueInExalts)} Exalted Orbs`
}
