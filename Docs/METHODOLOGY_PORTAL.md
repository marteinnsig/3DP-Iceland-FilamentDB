# Methodology Portal

## Public scope

The website Methodology tab is the Level 2 Engineering explanation of the 3DPIceland Labs comparative mechanical testing workflow.

## Canonical content owner

`App/FilamentDbApp/Assets/Website/MethodologyPortal.html`

The file is compiled as an embedded resource and injected into the canonical single-file website export by `BuildMethodologyPortalHtml()` and `ApplyNativeWebsitePortalNavigation()`.

## Test sections

- Tensile Strength and Layer Adhesion Strength
- Impact Resistance
- Stiffness
- Heat Deflection (`3dp-thermal-deflection-fixture-v1`)
- Printing Standard
- Statistics, CV and confidence
- Known limitations
- FAQ
- Level 3 whitepaper handoff

## Procedure videos

- Tensile: https://www.youtube.com/watch?v=kax8Ha_AGcQ
- Impact: https://www.youtube.com/watch?v=ibjS_tWL6sg
- Stiffness: https://www.youtube.com/watch?v=nv9PexjvFRw

## Governance

Native database keys and calculation ownership remain unchanged. Public terminology and explanatory content are presentation-layer documentation. Technical constants shown publicly must remain aligned with the native calculation engine and Verification Center.

Heat Deflection documents the 127 × 12.7 × 3.2 mm flat specimen, 110 mm clear span, nominal 54 g M20 nut load, 2.00 mm endpoint and nearby BlueDOT probe-indicated fixture temperature. It must remain explicitly comparative and must not be described as specimen-core temperature, ASTM D648 or ISO 75 HDT.
