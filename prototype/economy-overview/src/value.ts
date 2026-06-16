// Denomination-aware Value display: Exalted Orbs below 1 Divine Orb, Divine Orbs at or above.
export function formatValue(valueInExalts: number, divineOrbRate: number): string {
  if (valueInExalts >= divineOrbRate) {
    const divines = valueInExalts / divineOrbRate;
    return `${divines % 1 === 0 ? divines : divines.toFixed(1)} Divine`;
  }
  return `${Math.round(valueInExalts)} Exalt${valueInExalts !== 1 ? 's' : ''}`;
}

export function filterAndSort(
  items: { name: string; valueInExalts: number }[],
  threshold: number,
) {
  return items
    .filter((i) => i.valueInExalts >= threshold)
    .sort((a, b) => b.valueInExalts - a.valueInExalts);
}
