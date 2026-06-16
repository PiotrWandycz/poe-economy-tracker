// Variant A — Dense tables
// All sections stacked vertically as compact tables. Maximum information density.
// Good for power users who want to scan the full economy fast.

import type { EconomyData } from './mockData';
import { filterAndSort, formatValue } from './value';

export function VariantA({ data, threshold }: { data: EconomyData; threshold: number }) {
  const visible = data.sections
    .map((s) => ({ ...s, items: filterAndSort(s.items, threshold) }))
    .filter((s) => s.items.length > 0);

  return (
    <div className="max-w-3xl mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold text-amber-400 mb-1">PoE Economy</h1>
      <p className="text-zinc-500 text-sm mb-8">Threshold: {threshold} Exalts · {data.divineOrbRate} Ex = 1 Divine</p>

      <div className="space-y-8">
        {visible.map((section) => (
          <div key={section.name}>
            <div className="flex items-center gap-3 mb-2">
              <h2 className="text-sm font-semibold text-zinc-300 uppercase tracking-widest">
                {section.name}
              </h2>
              <span className="text-xs text-zinc-600 bg-zinc-800 px-2 py-0.5 rounded">
                {section.group}
              </span>
            </div>
            <table className="w-full text-sm border-collapse">
              <tbody>
                {section.items.map((item) => (
                  <tr key={item.name} className="border-b border-zinc-800 hover:bg-zinc-900">
                    <td className="py-1.5 pr-4 text-zinc-200">{item.name}</td>
                    <td className="py-1.5 text-right font-mono text-amber-300">
                      {formatValue(item.valueInExalts, data.divineOrbRate)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ))}
      </div>
    </div>
  );
}
