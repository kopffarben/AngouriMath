# Architektur-Überblick

Diese Datei fasst die Struktur und die wichtigsten Komponenten des Projekts **AngouriMath** zusammen. Sie dient als technische Referenz für die Integration neuer Funktionen wie der Geometric Algebra (vgl. `GA_TASKS.md` Step 1).

## Projektstruktur

- **Sources/AngouriMath.sln** – Hauptlösung mit mehreren Projekten
  - **AngouriMath** – Kernbibliothek in `Sources/AngouriMath/AngouriMath`
  - **Analyzers** – Roslyn-Analyzers unter `Sources/Analyzers`
  - **Wrappers** – Sprach-Wrapper (F#, Interactive, C++, …) unter `Sources/Wrappers`
  - **Terminal** – Kommandozeilenprogramm in F# (`Sources/Terminal`)
  - **Tests** – Unit- und Wrapper-Tests (`Sources/Tests`)
  - **Utils** – Hilfstools zur Code-Generierung (`Sources/Utils`)

Die Namensräume konzentrieren sich auf vier Hauptbereiche:

- `AngouriMath` – Public API und die `Entity`-Hierarchie
- `AngouriMath.Core` – interne Infrastruktur (Parser, Domains, Multithreading)
- `AngouriMath.Functions` – Algorithmen (Simplify, Evaluation, Algebra)
- `AngouriMath.Convenience` – Helferklassen und `MathS`

## Hauptklassen und Konzepte

### Entity-Hierarchie

Alle Ausdrücke werden durch verschachtelte `partial record`-Typen innerhalb von `Entity` repräsentiert. Die wichtigsten Ableitungen sind:

- `Entity.ContinuousNode` – kontinuierliche Ausdrücke (z. B. Funktionen, Zahlen)
- `Entity.Statement` – boolesche Ausdrücke
- `Entity.Number` → `Complex` → `Real` → `Rational` → `Integer`

Operatoren und Funktionen sind als Records wie `Entity.Sumf` oder `Entity.Sin` umgesetzt. Eine vereinfachte Darstellung liefert die UML-Skizze in `AGENTS.md`【F:AGENTS.md†L30-L66】.

### Parser

Der Parser (`Core/Parser.cs`) basiert auf ANTLR4 und erzeugt aus einem String einen Ausdrucksbaum (`Entity`). Fehlergründe werden über Unterklassen von `ReasonOfFailureWhileParsing` kommuniziert【F:Sources/AngouriMath/AngouriMath/Core/Parser.cs†L27-L44】.

### MathS

`MathS` bildet den zentralen Einstiegspunkt für Nutzer. Die Klasse stellt statische Methoden bereit, um Ausdrücke zu erzeugen, zu vereinfachen oder Gleichungen zu lösen【F:Sources/AngouriMath/AngouriMath/Convenience/MathS.cs†L25-L33】【F:Sources/AngouriMath/AngouriMath/Convenience/MathS.cs†L176-L193】.

### EquationSystem

`EquationSystem` kapselt ein Gleichungssystem und verwendet `EquationSolver` zum Lösen der Gleichungen【F:Sources/AngouriMath/AngouriMath/Core/EquationSystem.cs†L56-L73】.

## API-Entry-Points

- **Bibliothek**: Zugriff über `AngouriMath.Entity` und `AngouriMath.MathS`
- **F#‑Wrapper**: Projekt `AngouriMath.FSharp` bietet idiomatische F#‑Funktionen
- **Interactive**: Notebook‑Integration über `AngouriMath.Interactive`
- **Terminal**: F#‑Programm `Sources/Terminal/AngouriMath.Terminal/Program.fs` startet einen REPL【F:Sources/Terminal/AngouriMath.Terminal/Program.fs†L58-L64】

## Abhängigkeiten

Die Kernbibliothek referenziert u. a. folgende Pakete (siehe `Directory.Build.props`):

- `HonkSharp`
- `Antlr4.Runtime.Standard`
- `GenericTensor`
- `PeterO.Numbers`
- `Nullable` (nur PrivateAssets)

Zudem kommen eigene Analyzers als Projekt-Referenz hinzu【F:Sources/AngouriMath/Directory.Build.props†L40-L49】.

## Klassendiagramm

Die UML-Darstellung in `AGENTS.md` gibt einen Überblick über die zentrale `Entity`-Klasse und deren wichtigste Ableitungen. Für Erweiterungen wie Geometric Algebra können neue Entitäten analog in diese Hierarchie eingegliedert werden.

## Hinweise für GA-Integration (Step 1)

- Nichtkommutative Operationen werden bereits bei Matrizen unterstützt, ein ähnlicher Ansatz kann für den GA-Geometrieprodukt genutzt werden.
- Symbolische und numerische Berechnungen greifen auf denselben Baum (`Entity`) zu.
- Bezeichner `e0` bis `e5` sind aktuell frei und können für GA-Basisvektoren reserviert werden.

Diese Analyse soll als Grundlage dienen, um neue `GaBasis`- und `GaMultivector`‑Typen konsistent im bestehenden System zu implementieren.
