
#Inleiding

Deze applicatie gegenereerd ritgegevens in txt formaat. De data wordt aangemaakt met behulp van de `Bogus`-library.

---

## Structuur

#Rit

- `team`: bevat het team dat de rit uitvoert.
- `gmsRitId`: ID van de rit.
- `afgesloten`: geeft aan of de rit is afgesloten. Deze staat in de generator altijd op false.
- `inzetNummer`: nummer van de inzet.
- `inzetDatumTijd`: datum en tijd van inzet.
- `handmatigAangemaakt`: geeft aan of de rit handmatig aangemaakt is.
- `gmsWaarden`: bevat informatie over de rit zoals adres en urgentie.

#Team

- `voertuignummer`: nummer van het voertuig.
- `verpleegkundige`: naam van de verpleegkundige.

#gmsWaarden

- `kladblok`: notities.
- `urgentie`: urgentieniveau van de rit.
- `statussen`: statusupdates met tijdstippen.
- `inzetAdres`: het adres.
- `bestemmingAdres`: het bestemmingsadres beschreven in GPS-coördinaten.

#Adres 

- `straat`, `huisnummer`, `postcode`, `plaats`: Adresinformatie.
- `latitude`, `longitude`: GPS-coördinaten.

#Statussen

- `apa`, `mld`, `opd`, `vtr`: Tijdstippen van verschillende statussen.

---

## Functionaliteit

#GenerateFakeRitData

Maakt willekeurige ritgegevens met de `Bogus`-library.

---
## Gebruik

- Start je powershell of CMD op.
- Voer in cd de plek waar je 'Bogus' hebt opgeslagen.
- Voer in dotnet run -- --aantalRitten 5.(of een ander getal)
- Als het werkt geeft die aan dat het bestand succesvol is gemaakt.

---


