export function formatValue(valueInExalts: number, divineOrbRate: number): string {
  if (valueInExalts >= divineOrbRate) {
    const divines = valueInExalts / divineOrbRate
    const display = divines % 1 === 0 ? divines : divines.toFixed(1)
    return `${display} Div`
  }
  return `${Math.round(valueInExalts)} Ex`
}
