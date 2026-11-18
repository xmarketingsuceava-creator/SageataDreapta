# → Săgeata Dreapta

Programel pentru Windows care apasă automat tasta **săgeată dreapta (→)** la un interval ales, ca să schimbe singur pozele (sau slide-urile).

## Instalare pe un calculator nou

**Varianta 1. O singură comandă:** deschideți **PowerShell** (Start → scrieți „PowerShell”) și lipiți:

```powershell
irm https://raw.githubusercontent.com/xmarketingsuceava-creator/SageataDreapta/main/instaleaza.ps1 | iex
```

**Varianta 2. Din ZIP:** apăsați butonul verde **Code → Download ZIP** de pe pagina asta, dezarhivați și dați dublu-click pe **Instaleaza.bat**.

În ambele cazuri apare shortcut-ul **Săgeata Dreapta** pe Desktop. Tot așa se face și actualizarea la o versiune nouă.

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
