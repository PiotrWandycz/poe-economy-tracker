import { formatValue } from './formatValue'
import type { EconomyData } from './types'

const GROUP_BORDER: Record<string, string> = {
  General:   'border-blue-800',
  Equipment: 'border-[#af6025]',
  Atlas:     'border-purple-800',
}

const GROUP_BADGE: Record<string, string> = {
  General:   'bg-blue-900 text-blue-300',
  Equipment: 'bg-[#af6025]/20 text-[#af6025]',
  Atlas:     'bg-purple-900 text-purple-300',
}

export function EconomyOverview({ data }: { data: EconomyData }) {
  const visible = data.sections.filter(s => s.items.length > 0)

  return (
    <div className="flex flex-col h-screen overflow-hidden">
      <div className="flex items-baseline gap-4 px-4 py-4 shrink-0">
        <h1 className="text-3xl font-bold text-amber-400">PoE Economy</h1>
        <span className="text-zinc-500 text-sm">{data.divineOrbRate} Ex = 1 Divine</span>
      </div>

      <div className="flex flex-row gap-3 overflow-x-auto overflow-y-hidden px-4 pb-4 flex-1 min-h-0">
        {visible.map(section => (
          <div
            key={section.name}
            className={`shrink-0 w-56 flex flex-col rounded-lg border ${GROUP_BORDER[section.group] ?? 'border-zinc-700'} bg-zinc-900 p-4`}
          >
            <div className="flex items-center justify-between mb-3 shrink-0">
              <h2 className="font-semibold text-zinc-100 text-sm">{section.name}</h2>
              <span className={`text-xs px-2 py-0.5 rounded-full ${GROUP_BADGE[section.group] ?? 'bg-zinc-800 text-zinc-400'}`}>
                {section.group}
              </span>
            </div>
            <ul className="space-y-1.5 overflow-y-auto flex-1 min-h-0">
              {section.items.map((item, i) => {
                const inDiv = item.valueInExalts >= data.divineOrbRate
                const prevInDiv = i > 0 && section.items[i - 1].valueInExalts >= data.divineOrbRate
                const showSeparator = i > 0 && !inDiv && prevInDiv
                return (
                  <>
                    {showSeparator && <hr key={`sep-${section.name}`} className="border-zinc-600 my-1" />}
                    <li key={item.name} className="flex justify-between items-center text-sm">
                      <span className="text-zinc-300 truncate mr-2">{item.name}</span>
                      <span className="font-mono text-amber-300 shrink-0">{formatValue(item.valueInExalts, data.divineOrbRate)}</span>
                    </li>
                  </>
                )
              })}
            </ul>
          </div>
        ))}
      </div>
    </div>
  )
}
