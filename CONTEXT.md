# PoE Economy Tracker

A price-checking tool for Path of Exile 2's player-driven economy. Scrapes data from poe.ninja (no public API exists) and caches it server-side to show the most valuable tradeable items across Sections on a single page.

## Language

**League**:
The active seasonal game mode in Path of Exile 2. poe.ninja scopes all economy data to a specific League. The active League name is stored in a config value and updated manually when a new League season begins.
_Avoid_: Season, server, realm

**Exalted Orb**:
The base unit of value in the economy — the "small change." Used to express the price of items worth less than one Divine Orb.
_Avoid_: Exalt (shorthand only), currency (too broad)

**Divine Orb**:
The premium unit of value — the "pound" to Exalted Orb's "penny." Used to express the price of items worth one Divine Orb or more. Never shown as a fraction below 1 (e.g. 0.9 Divine is displayed as its Exalted Orb equivalent instead).
_Avoid_: Divine (shorthand only)

**Value**:
The price of a Stackable expressed in the appropriate denomination: Exalted Orbs when below one Divine Orb, Divine Orbs when at or above one Divine Orb. Sourced from poe.ninja's Value column.
_Avoid_: Price, cost, worth

**Group**:
The top-level navigation division on poe.ninja: General, Equipment, or Atlas. Each Group contains multiple Sections.
_Avoid_: Category (too vague), tab, type

**Section**:
A named page within a Group on poe.ninja — e.g. Currency, Fragments, Abyssal Bones under General; Tablets under Atlas. Each Section contains tradeable items of a similar kind. The site shows all items in a Section whose Value exceeds the configured Threshold, sorted by Value descending. Sections where no items exceed the Threshold are hidden entirely.
_Avoid_: Category (too vague), page, type

**Threshold**:
The minimum Value, expressed in Exalted Orbs, below which items are hidden. Configured alongside the League name and updated manually as the League economy matures (e.g. 50 Exalts two weeks into a League when Divine Orbs have inflated to ~190 Exalts). All Values are normalised to Exalted Orbs for comparison, then displayed in the appropriate denomination.
_Avoid_: Filter, minimum, cutoff

**Stackable**:
A tradeable item with a stack size — currencies, fragments, runes, etc. Found in the General Group. These have clean, consistent prices. The current scope covers Stackables (General Group) and Atlas items, which share the same data shape (name + Value only).
_Avoid_: Currency (too narrow — fragments and runes are also Stackables), item (too broad)

**Equipment**:
A tradeable item a character can wear — unique weapons, armours, etc. Found in the Equipment Group. Planned for a future phase.
_Avoid_: Item (too broad), gear
