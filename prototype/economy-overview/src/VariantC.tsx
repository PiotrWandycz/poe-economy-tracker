// Variant C — Grouped accordion
// Sections collapsed by default under General / Atlas group headings.
// Shows a count badge when collapsed. Expands inline on click.
// Prioritises vertical compactness; best when most sections are noise and you want to drill in.

import { useState } from 'react';
import type { EconomyData, Section } from './mockData';
import { filterAndSort, formatValue } from './value';

function SectionRow({ section, divineOrbRate }: { section: Section & { items: ReturnType<typeof filterAndSort> }; divineOrbRate: number }) {
  const [open, setOpen] = useState(false);
  const top = section.items[0];

  return (
    <div className="border-b border-zinc-800 last:border-0">
      <button
        onClick={() => setOpen((v) => !v)}
        className="w-full flex items-center justify-between px-4 py-3 hover:bg-zinc-800/50 text-left"
      >
        <div className="flex items-center gap-3">
          <span className="text-zinc-200 font-medium">{section.name}</span>
          <span className="text-xs text-zinc-500">{section.items.length} item{section.items.length !== 1 ? 's' : ''}</span>
        </div>
        <div className="flex items-center gap-3">
          {!open && top && (
            <span className="text-xs text-zinc-400">
              top: <span className="text-amber-300 font-mono">{formatValue(top.valueInExalts, divineOrbRate)}</span>
            </span>
          )}
          <span className="text-zinc-600 text-xs">{open ? '▲' : '▼'}</span>
        </div>
      </button>

      {open && (
        <div className="px-4 pb-3">
          <table className="w-full text-sm">
            <tbody>
              {section.items.map((item) => (
                <tr key={item.name} className="border-t border-zinc-800/50 first:border-0">
                  <td className="py-1.5 text-zinc-300">{item.name}</td>
                  <td className="py-1.5 text-right font-mono text-amber-300">
                    {formatValue(item.valueInExalts, divineOrbRate)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export function VariantC({ data, threshold }: { data: EconomyData; threshold: number }) {
  const byGroup: Record<string, (Section & { items: ReturnType<typeof filterAndSort> })[]> = {};

  for (const section of data.sections) {
    const items = filterAndSort(section.items, threshold);
    if (items.length === 0) continue;
    if (!byGroup[section.group]) byGroup[section.group] = [];
    byGroup[section.group].push({ ...section, items });
  }

  return (
    <div className="max-w-2xl mx-auto px-4 py-8">
      <div className="flex items-baseline gap-4 mb-8">
        <h1 className="text-2xl font-bold text-amber-400">PoE Economy</h1>
        <span className="text-zinc-500 text-sm">≥ {threshold} Ex · {data.divineOrbRate} Ex/Divine</span>
      </div>

      {Object.entries(byGroup).map(([group, sections]) => (
        <div key={group} className="mb-6">
          <h2 className="text-xs uppercase tracking-widest text-zinc-500 font-semibold mb-1 px-4">
            {group}
          </h2>
          <div className="rounded-lg border border-zinc-800 bg-zinc-900 overflow-hidden">
            {sections.map((section) => (
              <SectionRow key={section.name} section={section} divineOrbRate={data.divineOrbRate} />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
