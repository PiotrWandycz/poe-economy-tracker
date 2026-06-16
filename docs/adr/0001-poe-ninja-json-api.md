# Use the poe.ninja JSON API, not HTML scraping

poe.ninja is a client-side SPA with no data in the initial HTML. It exposes a JSON API at:

`GET /poe2/api/economy/exchange/current/overview?league={league}&type={type}`

All section types share the same endpoint and response shape: `core.items[]` (id, name), `core.rates.exalted` (Divine Orb → Exalted Orb rate), and `lines[]` (id, primaryValue in Divine Orbs). Value in Exalted Orbs = `primaryValue × core.rates.exalted`. This API is undocumented but stable in practice; we call it directly rather than rendering the page with a headless browser.
