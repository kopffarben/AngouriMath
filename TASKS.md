# Aufgabenliste: Symbolische PGA‑ und CGA‑Integration in AngouriMath Core

## Referenzen 
Lese dir immer [Integration](Integration von PGA und CGA in AngouriMath Core.pdf) durch
Den [hier](Terathon-Math-Library) findest da den SourceCode der Terathon-Math-Library die als mathematische Reference dient 

## Phase 1: Grundlegende Architektur & Datenmodell

1. **Algebra‐Descriptor erstellen**  
   - Klasse `AlgebraDescriptor` anlegen, die Dimension, Signatur (Liste metrischer Werte) und Basisnamen kapselt.  
   - Instanzen für `PGA3D` (Signatur `[+,+,+,0]`) und `CGA3D` (`[+,+,+,0,0]` mit `e_+·e_−=1`) definieren.

2. **Multivector‐Entity implementieren**  
   - Neue Entity‑Unterklasse `MultivectorEntity`, intern mit `Dictionary<BladeMask, Entity>` für Blade‐Koeffizienten.  
   - Methoden:  
     - Erzeugung von Multivektoren  
     - Zugriff auf Grades (`GetGrades()`)  
     - Zusammenfassen gleicher Blades (Term‐Kombination)

3. **Basisvektoren als Singleton‑Entities**  
   - Factory in `MathS.GA` implementieren, die je nach `AlgebraDescriptor` die Basisvektoren (`e1…en`, `e∞`, `e+`, `e−`) einmalig erzeugt und cached.

---

## Phase 2: Operator‑ und Funktionsknoten

4. **Operator‐Knoten für GA‑Produkte**  
   - Klassen `WedgeNode`, `InnerProductNode`, `GeometricProductNode` im AST implementieren.  
   - Konstruktor, Visitor‑Methoden und `Simplify()`‑Override pro Node.

5. **Symbolische Rechenregeln einpflegen**  
   - **Äußeres Produkt**  
     - Antikommutativität: `e_i ∧ e_i → 0`  
     - Sortierung + Vorzeichenkorrektur  
     - Distribution über Summen  
   - **Inneres Produkt**  
     - `e_i · e_j → ηᵢⱼ`  
     - Grad‐Reduktion und bekannte Identitäten (z. B. $a·(b∧c)$)  
   - **Geometrisches Produkt**  
     - Zerlegung in Dot + Wedge  
     - Spezialfälle `e_i e_i → ηᵢᵢ`

6. **Dualität & Meet/Join**  
   - Methode `MultivectorEntity.Dual()` auf Basis des Pseudoskalars `I`.  
   - API‑Funktionen:  
     - `GA.Join(A, B) = A ∧ B`  
     - `GA.Meet(A, B) = Dual(Dual(A) ∧ Dual(B))`

---

## Phase 3: API‑Erweiterung

7. **`MathS.GA`‑Namespace aufbauen**  
   - Methoden:  
     - `DefineSpace(int dim, Signature sig)`  
     - **PGA**: `Point(x,y,z)`, `Line(P,Q)`, `Plane(a,b,c,d)`  
     - **CGA**: `Point(x,y,z)`, `Sphere(center, r)`, `Circle(P,Q,R)`, …

8. **Operator‑Overloads in C#**  
   - Überladung von `^` für Wedge (nur GA‑Entities).  
   - Methode `Dot(a, b)` oder Overload `&` für InnerProduct.  
   - `*` zum geometrischen Produkt (intern `GeometricProductNode`).

9. **LaTeX‑ und ToString‑Ausgabe**  
   - GA‑Basis als `e_{i}`, `e_{\infty}`, `e_{+}`, `e_{−}` formatieren.  
   - Symbole `\wedge`, `\cdot` in LaTeX‑Output integrieren.

---

## Phase 4: Parser‑Integration (optional / später)

10. **Parser‑Hooks für GA‑Modus**  
    - Kontextflagge `MathS.Settings.UseGA = "PGA3D"|"CGA3D"`.  
    - Nach dem Parsen: Ersetze Bezeichner `e1…en` durch GA‑Basis‑Entities.  
    - Neue Token/Funktion `wedge(a,b)` oder Unicode‑Symbol `∧` in der Grammatik.

---

## Phase 5: Integration in bestehende Engine

11. **Simplification‑Pipeline erweitern**  
    - GA‑Regeln in den globalen Simplifier (Pattern‑Matcher) oder lokal in `MultivectorEntity.Simplify()` einbinden.

12. **Differentiator anpassen**  
    - GA‑Basisvektoren als Konstanten markieren (Ableitung = 0).  
    - Ableitung eines Multivektors komponentenweise auf Koeffizienten anwenden.

13. **Gleichungslöser (Übergabe / spätere Erweiterung)**  
    - Sicherstellen, dass GA‑Entitäten im Solver nicht abstürzen (z. B. Meldung “GA nicht unterstützt”).  
    - Später: Spezialfälle (Meet = 0 ⇒ Schnittpunkt) über Komponentengleichungen lösen.

---

## Phase 6: Testing, Dokumentation & Release

14. **Unit‑Tests schreiben**  
    - GA‑Identitäten:  
      - `e1 ∧ e1 == 0`  
      - `e1*e2 + e2*e1 == 0`  
    - Punkte, Kreise, Ebenen: Korrekte Meet/Join‑Ergebnisse  
    - Dualität: `Dual(Dual(A)) == A`

15. **Beispiele & Tutorials**  
    - Wiki‑Seiten mit PGA- und CGA‑Beispielen (Punkt‑Gerade‑Schnitt, KreisdurchDreiPunkte, Kugel‑Linien‑Schnitt).  
    - Code‑Snippets für `MathS.GA.PGA` und `MathS.GA.CGA`.

16. **Modularisierung & Packaging**  
    - Optional: GA‑Untermodul in eigene Assembly `AngouriMath.GA` auslagern.  
    - NuGet‑Paket mit Optional‑Abhängigkeit auf Core.

---

