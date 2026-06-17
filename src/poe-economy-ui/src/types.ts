export interface Item {
  name: string
  valueInExalts: number
}

export interface Section {
  name: string
  group: string
  items: Item[]
}

export interface EconomyData {
  sections: Section[]
  divineOrbRate: number
}
