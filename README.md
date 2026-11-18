# → Săgeata Dreapta

Programel pentru Windows care apasă automat tasta **săgeată dreapta (→)** la un interval ales, ca să schimbe singur pozele (sau slide-urile).

## Instalare pe un calculator nou

1. Apăsați butonul verde **Code → Download ZIP** de pe pagina asta.
2. Click dreapta pe ZIP → **Extract All** (Extrage tot).
3. În folderul dezarhivat, dați dublu-click pe **Instaleaza.bat**.

Apare shortcut-ul **Săgeata Dreapta** pe Desktop, iar folderul descărcat se poate șterge. Tot așa se face și actualizarea la o versiune nouă.

Dacă Windows afișează „Windows protected your PC”: **More info → Run anyway**.

### Dacă antivirusul îl șterge

Programul apasă singur taste, iar antivirusul Windows (Defender) dă uneori **alarmă falsă** la astfel de programe. Dacă îl șterge, îi spuneți o dată să aibă încredere **în acest fișier**:

1. Deschideți **Windows Security** (Start → scrieți „Windows Security”).
2. **Virus & threat protection** → **Protection history**.
3. Găsiți „Sageata Dreapta”, apăsați **Allow** (și, dacă e cazul, **Restore**).

Asta whitelistează doar programul nostru, nu slăbește protecția pe tot calculatorul.

## Folosire

1. Alegeți intervalul (sau un buton rapid: 3s, 5s, 10s, 30s, 1m).
2. **START**: fereastra se ascunde și aveți 3 secunde să dați click pe fereastra cu pozele.
   **sau F9**: dați click pe poze, apoi apăsați F9, și pornește imediat.
3. **F9** din nou = STOP.

- „Oprește după X apăsări” (0 = merge până îl opriți).
- Cât timp rulează, calculatorul nu intră în sleep.
- Setările se salvează automat.

## Probleme

- Săgeata ajunge în fereastra **activă** (cea pe care ați dat click ultima dată).
- Dacă programul cu poze rulează ca administrator, porniți și Săgeata Dreapta cu click dreapta → *Run as administrator*.
- „Windows protected your PC” → *More info* → *Run anyway*.

## Pentru modificări

Codul e în `src/SageataDreapta.cs`. După modificare rulați `src/build.bat`, care refolosește compilatorul C# inclus în Windows.
