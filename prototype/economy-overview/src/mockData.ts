// PROTOTYPE — mock data standing in for the real GET /api/economy response

export type Item = {
  name: string;
  valueInExalts: number;
};

export type Section = {
  name: string;
  group: 'General' | 'Atlas';
  items: Item[];
};

export type EconomyData = {
  divineOrbRate: number; // how many Exalted Orbs = 1 Divine Orb
  sections: Section[];
};

export const MOCK_DATA: EconomyData = {
  divineOrbRate: 190,
  sections: [
    {
      name: 'Currency',
      group: 'General',
      items: [
        { name: 'Mirror of Kalandra', valueInExalts: 72000 },
        { name: 'Divine Orb', valueInExalts: 190 },
        { name: 'Orb of Annulment', valueInExalts: 148 },
        { name: 'Vaal Orb', valueInExalts: 22 },
        { name: 'Orb of Alchemy', valueInExalts: 8 },
        { name: 'Chaos Orb', valueInExalts: 3 },
        { name: 'Orb of Alteration', valueInExalts: 1 },
      ],
    },
    {
      name: 'Fragments',
      group: 'General',
      items: [
        { name: 'Simulacrum', valueInExalts: 380 },
        { name: 'Mortal Grief', valueInExalts: 210 },
        { name: 'Mortal Hope', valueInExalts: 195 },
        { name: 'Mortal Ignorance', valueInExalts: 160 },
        { name: 'Sacrifice at Dawn', valueInExalts: 45 },
        { name: 'Sacrifice at Dusk', valueInExalts: 12 },
      ],
    },
    {
      name: 'Runes',
      group: 'General',
      items: [
        { name: 'Rune of Winter', valueInExalts: 320 },
        { name: 'Rune of Flame', valueInExalts: 285 },
        { name: 'Rune of Iron', valueInExalts: 95 },
        { name: 'Rune of Stone', valueInExalts: 40 },
        { name: 'Rune of Earth', valueInExalts: 15 },
        { name: 'Rune of Wind', valueInExalts: 4 },
      ],
    },
    {
      name: 'Resonators',
      group: 'General',
      items: [
        { name: 'Potent Chaotic Resonator', valueInExalts: 28 },
        { name: 'Powerful Chaotic Resonator', valueInExalts: 14 },
        { name: 'Prime Chaotic Resonator', valueInExalts: 6 },
        { name: 'Primitive Chaotic Resonator', valueInExalts: 2 },
      ],
    },
    {
      name: 'Fossils',
      group: 'General',
      items: [
        { name: 'Glyphic Fossil', valueInExalts: 570 },
        { name: 'Faceted Fossil', valueInExalts: 480 },
        { name: 'Hollow Fossil', valueInExalts: 410 },
        { name: 'Sanctified Fossil', valueInExalts: 190 },
        { name: 'Bound Fossil', valueInExalts: 85 },
        { name: 'Corroded Fossil', valueInExalts: 12 },
        { name: 'Pristine Fossil', valueInExalts: 5 },
      ],
    },
    {
      name: 'Essences',
      group: 'General',
      items: [
        { name: 'Essence of Delirium', valueInExalts: 760 },
        { name: 'Essence of Horror', valueInExalts: 720 },
        { name: 'Essence of Hysteria', valueInExalts: 680 },
        { name: 'Essence of Insanity', valueInExalts: 650 },
        { name: 'Shrieking Essence of Woe', valueInExalts: 38 },
        { name: 'Screaming Essence of Woe', valueInExalts: 5 },
        { name: 'Wailing Essence of Woe', valueInExalts: 1 },
      ],
    },
    {
      name: 'Divination Cards',
      group: 'General',
      items: [
        { name: 'The Doctor', valueInExalts: 14440 },
        { name: 'The Fiend', valueInExalts: 8740 },
        { name: 'House of Mirrors', valueInExalts: 7200 },
        { name: 'Seven Years Bad Luck', valueInExalts: 3800 },
        { name: 'The Immortal', valueInExalts: 1900 },
        { name: 'Brother\'s Stash', valueInExalts: 380 },
        { name: 'The Celestial Stone', valueInExalts: 55 },
        { name: 'The Flora\'s Gift', valueInExalts: 8 },
      ],
    },
    {
      name: 'Scarabs',
      group: 'General',
      items: [
        { name: 'Gilded Breach Scarab', valueInExalts: 95 },
        { name: 'Gilded Abyss Scarab', valueInExalts: 76 },
        { name: 'Gilded Elder Scarab', valueInExalts: 52 },
        { name: 'Polished Breach Scarab', valueInExalts: 22 },
        { name: 'Polished Abyss Scarab', valueInExalts: 12 },
        { name: 'Rusted Breach Scarab', valueInExalts: 3 },
      ],
    },
    {
      name: 'Tablets',
      group: 'Atlas',
      items: [
        { name: 'Precursor Tablet', valueInExalts: 285 },
        { name: 'Engraved Tablet', valueInExalts: 190 },
        { name: 'Rare Tablet', valueInExalts: 82 },
        { name: 'Magic Tablet', valueInExalts: 18 },
        { name: 'Normal Tablet', valueInExalts: 3 },
      ],
    },
    {
      name: 'Maps',
      group: 'Atlas',
      items: [
        { name: 'Vaal Temple Map', valueInExalts: 190 },
        { name: 'Colosseum Map', valueInExalts: 95 },
        { name: 'Maze of the Minotaur', valueInExalts: 38 },
        { name: 'Forge of the Phoenix', valueInExalts: 22 },
        { name: 'Pit of the Chimera', valueInExalts: 14 },
        { name: 'Lair of the Hydra', valueInExalts: 12 },
        { name: 'Strand Map', valueInExalts: 2 },
      ],
    },
  ],
};
