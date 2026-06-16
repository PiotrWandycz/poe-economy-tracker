// Variant B — Card grid
// Each section is a card in a responsive masonry-style grid.
// Emphasises section identity; easier to scan at a glance which section has valuable items.

import type { EconomyData } from './mockData';
import { filterAndSort, formatValue } from './value';

const GROUP_COLOUR: Record<string, string> = {
  General: 'border-blue-800',
  Atlas: 'border-purple-800',
};

const GROUP_BADGE: Record<string, string> = {
  General: 'bg-blue-900 text-blue-300',
  Atlas: 'bg-purple-900 text-purple-300',
};

export function VariantB({ data, threshold }: { data: EconomyData; threshold: number }) {
  const visible = data.sections
    .map((s) => ({ ...s, items: filterAndSort(s.items, threshold) }))
    .filter((s) => s.items.length > 0);

  return (
    <div className="max-w-6xl mx-auto px-4 py-8">
      <div className="flex items-baseline gap-4 mb-8">
        <h1 className="text-3xl font-bold text-amber-400">PoE Economy</h1>
        <span className="text-zinc-500 text-sm">≥ {threshold} Exalts · {data.divineOrbRate} Ex = 1 Divine</span>
      </div>

      <div className="columns-1 sm:columns-2 lg:columns-3 gap-4 space-y-4">
        {visible.map((section) => (
          <div
            key={section.name}
            className={`break-inside-avoid rounded-lg border ${GROUP_COLOUR[section.group]} bg-zinc-900 p-4`}
          >
            <div className="flex items-center justify-between mb-3">
              <h2 className="font-semibold text-zinc-100">{section.name}</h2>
              <span className={`text-xs px-2 py-0.5 rounded-full ${GROUP_BADGE[section.group]}`}>
                {section.group}
              </span>
            </div>
            <ul className="space-y-1.5">
              {section.items.map((item) => (
                <li key={item.name} className="flex justify-between items-center text-sm">
                  <span className="text-zinc-300 truncate mr-2">{item.name}</span>
                  <span className="font-mono text-amber-300 shrink-0">
                    {formatValue(item.valueInExalts, data.divineOrbRate)}
                  </span>
                </li>
              ))}
            </ul>
          </div>
        ))}
      </div>
    </div>
  );
}
