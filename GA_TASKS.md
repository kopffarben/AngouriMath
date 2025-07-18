# Integration of Geometric Algebra (PGA & CGA) into AngouriMath

## Overview and Goals

This plan details how to extend **AngouriMath** with support for **Geometric Algebra (GA)**, focusing on **3D Projective GA (PGA)** and **3D Conformal GA (CGA)**. The goal is to enable **symbolic manipulation** of GA expressions (simplifying and rearranging multivector formulas) while also providing **high-performance numeric evaluation** suitable for **real-time computer graphics**. Geometric Algebra offers a unified way to represent transformations (rotations, translations, etc.), and by integrating GA into AngouriMath, developers can derive formulas symbolically and then evaluate them as efficiently as hand-optimized code. The following roadmap is broken into concrete steps.

---

## Step 1: Review AngouriMath’s Architecture

1. **Expression Tree Understanding**

   * Study how **AngouriMath** represents mathematical expressions internally via the `Entity` class hierarchy.
   * Identify how existing types (numbers, symbols, matrices) are structured and how operations (addition, multiplication) are implemented.
2. **Non‑Commutative Operations**

   * Examine how AngouriMath handles non‑commutative products (e.g., matrix multiplication).
   * Since the **geometric product** in GA is non‑commutative, mirror the approach used for matrices.
3. **Symbolic vs Numeric Duality**

   * AngouriMath supports both symbolic manipulation and numeric evaluation.
   * Plan a unified representation that allows symbolic algebraic manipulation and numeric evaluation within the same framework.
4. **Naming & Conflict Checks**

   * Confirm that identifiers like `e0, e1, …, e5` are free for GA basis vectors.

**Outcome:** A clear understanding of where and how to implement new GA entity types and operations within AngouriMath’s architecture.

---

## Step 2: Design the GA Representation

1. **Define Basis & Metric**

   * **PGA3D:** basis `e1,e2,e3` with $e_i^2 = +1$ and null vector `e0` with $e_0^2 = 0$.
   * **CGA3D:** basis `e1,e2,e3` with $+1$; two extras (e.g. `e4^2 = +1`, `e5^2 = -1`) or null pair `(e0,e∞)` with $e_0 \cdot e_∞ = 1$.
2. **Blade Representation via Bitmask**

   * Use an unsigned 64-bit integer (`ulong`) to represent basis blades: each bit corresponds to a basis vector.
   * Example (3D CGA ≤5 dimensions):

     * Scalar: `00000` (no bits set)
     * $e_1$: `00001`; $e_2$: `00010`; $e_1e_2$: `00011`; etc.
3. **Multivector Data Structure**

   * Create a class `GaMultivector : Entity` storing a map from `bladeMask → Entity coefficient`.
   * Coefficients can be symbolic (`Entity`) or numeric.
4. **Metric Descriptor**

   * Static table mapping each basis index → its square (e.g., `{0:0, 1:+1, …}`).
5. **Scalar Promotion**

   * Ensure that scalar `Entity` values (numbers) can be automatically promoted to grade‑0 multivectors in mixed operations.

**Outcome:** A detailed design for `GaBasis`, `GaMultivector`, metric tables, and blade encoding, ready to implement in AngouriMath.

---

## Step 3: Implement Basis Entities and Parser Support

1. **Entity Subclasses**

   * **`GaBasis`**: represents a single basis vector $e_i$.
   * **`GaMultivector`**: represents sums of blades with coefficients.
2. **Parser Extension**

   * Recognize tokens `"e0"…"e5"` in user input and construct `GaBasis` nodes.
   * Intercept generic `Product(e_i, e_j)` during parsing to immediately apply GA multiplication rules, producing a `GaMultivector`.
3. **Algebra Context Selection**

   * Decide whether PGA and CGA bases coexist (all `e0…e5` available) or require explicit contexts (e.g., `PGA3D.e1` vs `CGA3D.e1`).
4. **Display & LaTeX Output**

   * Override `ToString()` and LaTeX converter for GA entities (e.g., print blades as `e1e2`, LaTeX as `$e_{12}$`).

**Outcome:** Users can input and view GA expressions seamlessly, with `GaBasis` and `GaMultivector` integrated into the parser and display logic.

---

## Step 4: Implement Geometric Algebra Operations

1. **Addition & Subtraction**

   * Combine like‑blade terms by matching bitmasks, summing their coefficients, and dropping zero results.
2. **Geometric Product (Multiplication)**

   * **Basis × Basis**:

     * If same index: $e_i e_i = g_{ii}$ (yield scalar from metric).
     * If different indices: sort the indices ascending, count swaps (each swap adds a –1 sign), compute result blade mask via XOR.
   * **Blade Encoding Algorithm**:

     ```text
     resultMask = maskA XOR maskB  # blades without common factors
     common = maskA AND maskB      # bits present in both
     scalarFactor = product of metric[basis] for each bit in 'common'
     sign = (-1)^(number of index swaps to sort basis sequence)
     coefficient = sign * scalarFactor * (coeffA * coeffB)
     ```
   * **Multivector × Multivector**: distributive double loop over terms, multiply blades as above, then combine like terms.
   * **Scalar × GA**: multiply each blade’s coefficient by the scalar.
3. **Eager Evaluation**

   * Override the `*` operator so GA products immediately yield a simplified `GaMultivector`. No unsimplified `Product` nodes remain.
4. **Optional Outer & Inner Products**

   * **Wedge (∧)**: only combine blades with no common indices (`common == 0`), result blade = `maskA XOR maskB`.
   * **Dot (⋅)**: extract lower‑grade contraction part from geometric product.

**Outcome:** AngouriMath correctly applies GA multiplication rules, handling anti‑commutation, metric contractions, and scalar promotion in a single unified operation.

---

## Step 5: Integrate Simplification and Algebraic Simplifications

1. **Canonical Form by Construction**

   * Since GA multiplication is eager, results are already in canonical sum‑of‑blades form when created.
2. **Non‑Commutativity Guard**

   * Mark GA entities non‑commutative so general simplification rules (e.g., swapping factors) are disabled or checked.
3. **Combine Like Terms on Simplify**

   * Ensure that `Simplify()` / `Expand()` preserves canonical form and merges like blades.
4. **Document Limitations**

   * Note unsupported functionality (e.g., GA equation solving, full symbolic differentiation outside coefficient‑wise operations).

**Outcome:** GA expressions remain simplified and correct under AngouriMath’s simplification routines, respecting non‑commutativity and metric rules.

---

## Step 6: Optimize Numeric Performance for Real-Time Graphics

1. **Compile‑to‑Lambda Compatibility**

   * Extend AngouriMath’s expression compiler to generate efficient code for `GaMultivector` operations.
2. **Dense Numeric Layout**

   * Define a C# `struct GaMultivectorValue` with fixed fields or array for each blade coefficient to minimize allocations.
3. **Precomputed Multiplication Tables**

   * Optional: build a lookup table of size 2^n × 2^n (n = number of basis vectors) mapping `(maskA, maskB) → (resultMask, sign, metricFactor)` to speed up blade products.
4. **SIMD & JIT Vectorization**

   * Use `System.Numerics.Vector<double>` or rely on .NET JIT optimizations for operations over coefficient arrays.
5. **Benchmark & Profile**

   * Test typical GA transforms (rotors, motors) against quaternion/matrix equivalents; identify hotspots and iterate.

**Outcome:** Numeric GA computations run near native speed, leveraging AngouriMath’s compile-to-lambda and optimized data layouts for real-time use.

---

## Step 7: Testing, Validation, and Examples

1. **Unit Tests – Algebraic Correctness**

   * Verify basis multiplication tables (anticommutation, metric squares) for both PGA3D and CGA3D.
   * Test multivector algebra: distributivity, idempotence, scalar mixing.
2. **Unit Tests – Simplification & Edge Cases**

   * Ensure expressions like `e2*e1 + e1*e2` simplify to 0.
   * Confirm no invalid generic simplifications occur.
3. **Performance Benchmarks**

   * Compile and repeatedly apply a CGA rotor in a loop; compare throughput vs. quaternion rotation.
   * Monitor memory usage and stability over prolonged runs.
4. **Examples & Documentation**

   * **PGA**: Construct a line through points with `P ∧ Q ∧ e0`.
   * **CGA**: Rotate a point using `R P R̃`.
   * Show reflections, translations, and sphere-plane intersections.
   * Provide user guide on API, notation, compilation tips, and known limitations.

**Outcome:** A robust test suite and documentation ensure correctness, performance, and ease of use for GA in AngouriMath.

---

*End of Integration Plan*
