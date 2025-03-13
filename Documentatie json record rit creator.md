
#Inleiding 

Deze applicatie leest json records en voegt ze toe als een rit in EDAZ

---

## Functionaliteit

#CreateRitten

1. Leest het aantal te genereren ritten.
2. Voert een login uit om een authenticatietoken te verkrijgen.
3. Genereert nepdata voor de ritten.
4. Stuurt de ritten naar de API en geeft de status terug.


#LoginAsync

Authenticate bij de API en haalt een token op.

#CreateRitAsync

Verstuurt ritgegevens naar de API.

---
## API-verzoeken

#Login-verzoek

- URL
- Methode: `POST`
- Body:
  ```json
   {
  "grant_type": "password",
  "username": "gebruikersnaam",
  "password": "wachtwoord",
  "clientid": "1"
  }
   ```
- Response: `{ "access_token": "token_waarde" }` dit is een bearer token.

#Rit-verzoek

- URL: 
- Methode: `POST`
- Headers:
    - `Authorization: Bearer {authToken}`
- Body: JSON met ritgegevens

---
## Gebruik

- Start je powershell of CMD op.
- Voer in cd de plek waar je 'createritten' hebt opgeslagen.
- Voer in dotnet run -- --filePath "de plek waar je txt file staat opgeslagen"
- Als het werkt geeft die aan dat de rit is aangemaakt met het nummer erbij.

---
## Foutafhandeling

- Als het inloggen mislukt, wordt een foutmelding getoond.
- Als een rit niet succesvol wordt verzonden, wordt een foutmelding getoond in de console.

---
