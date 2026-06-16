import { useEffect } from 'react';

const VARIANTS = ['A', 'B', 'C'] as const;
const LABELS: Record<string, string> = {
  A: 'A — Dense tables',
  B: 'B — Card grid',
  C: 'C — Accordion',
};

export function PrototypeSwitcher({ current }: { current: string }) {
  const idx = VARIANTS.indexOf(current as typeof VARIANTS[number]);

  function go(delta: number) {
    const next = VARIANTS[(idx + delta + VARIANTS.length) % VARIANTS.length];
    const url = new URL(window.location.href);
    url.searchParams.set('variant', next);
    window.history.replaceState(null, '', url);
    window.location.reload();
  }

  useEffect(() => {
    function onKey(e: KeyboardEvent) {
      const tag = (e.target as HTMLElement).tagName;
      if (tag === 'INPUT' || tag === 'TEXTAREA') return;
      if (e.key === 'ArrowLeft') go(-1);
      if (e.key === 'ArrowRight') go(1);
    }
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  });

  return (
    <div className="fixed bottom-5 left-1/2 -translate-x-1/2 flex items-center gap-3 bg-zinc-900 border border-zinc-700 rounded-full px-4 py-2 shadow-xl text-sm font-mono z-50">
      <button onClick={() => go(-1)} className="text-zinc-400 hover:text-white px-1">←</button>
      <span className="text-zinc-200 min-w-36 text-center">{LABELS[current] ?? current}</span>
      <button onClick={() => go(1)} className="text-zinc-400 hover:text-white px-1">→</button>
      <span className="text-zinc-500 text-xs ml-2">↔ arrow keys</span>
    </div>
  );
}
