// PROTOTYPE — economy overview UI variants. Delete or absorb when done.
// Three variants switchable via ?variant=A|B|C (default: A)
// Threshold hardcoded to 50 Exalts for prototyping purposes.

import { MOCK_DATA } from './mockData';
import { PrototypeSwitcher } from './PrototypeSwitcher';
import { VariantA } from './VariantA';
import { VariantB } from './VariantB';
import { VariantC } from './VariantC';

const THRESHOLD = 50; // Exalted Orbs

function App() {
  const params = new URLSearchParams(window.location.search);
  const variant = params.get('variant') ?? 'A';

  return (
    <>
      {variant === 'A' && <VariantA data={MOCK_DATA} threshold={THRESHOLD} />}
      {variant === 'B' && <VariantB data={MOCK_DATA} threshold={THRESHOLD} />}
      {variant === 'C' && <VariantC data={MOCK_DATA} threshold={THRESHOLD} />}
      <PrototypeSwitcher current={variant} />
    </>
  );
}

export default App;
