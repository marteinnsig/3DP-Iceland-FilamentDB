# 3DPIceland Labs TPU Compression Test

Method version: `3DP-TPU-COMP-v1.0`

## Accepted comparative method

- Equipment: SAUTER TVL manual compression stand and SAUTER FK 500 force gauge.
- Specimen: 9 mm diameter x 10 mm actual initial height cylinder, 100% rectilinear infill.
- Orientation: printed on the flat circular base and compressed in Z.
- Zero: first platen contact, before noticeable compression.
- Approach: even manual lowering over approximately 15 seconds.
- Target: 20% of measured initial height; 2.00 mm for exactly 10.00 mm height.
- Hold: start timing at target displacement and keep displacement fixed.
- Readings: force after 10 seconds and 30 seconds; the 30-second reading is primary.
- Baseline: first compression of independent specimens. Repeat cycles remain separately labelled.

Primary result: **Compression Force at 20% Strain — 30 s hold (N)**.

Supporting result: **Compression Force at 20% Strain — 10 s hold (N)**.

The 30-second point was chosen because the force change had slowed during method development, so a small timing deviation has less
effect than while force is changing faster. This does not mean equilibrium was reached.

This is an in-house comparative method for printed specimens. It is not ASTM D575 or ISO 7743, and results must not be described as
ISO tested, ISO compliant or certified. ISO 7743 informed development, but the geometry, manual approach, 20% target and timed readings
are the 3DPIceland Labs implementation. The result is compression resistance—not fracture strength, Shore hardness or Young's modulus.

The FK 500 measurement range is 500 N. The former 400 N development caution was not a manufacturer limit or rejection threshold. If
20% strain cannot be reached within range, record Target reached as false and retain only factual force/displacement; never invent a
500 N target result.

## Unlinked ten-specimen method-validation evidence

The material was described as 64 Shore D, but this is not a measured Shore result from this test and its exact MaterialID is unknown.
Therefore this round is retained as verification/documentation evidence and is not inserted or published as a named material result.

| Specimen | Force at 10 s (N) | Force at 30 s (N) |
|---:|---:|---:|
| 1 | 450 | 408 |
| 2 | 430 | 405 |
| 3 | 435 | 402 |
| 4 | 410 | 391 |
| 5 | 450 | 418 |
| 6 | 445 | 410 |
| 7 | 426 | 394 |
| 8 | 445 | 410 |
| 9 | 420 | 387 |
| 10 | 419 | 380 |

The specimen-2 30-second value is 405 N; the earlier 305 N value was a transcription error.

| Reading | n | Mean (N) | Sample SD, n−1 (N) | CV (%) | Range (N) |
|---|---:|---:|---:|---:|---:|
| 10 s | 10 | 433.0 | 14.23 | 3.29 | 410–450 |
| 30 s | 10 | 400.5 | 12.02 | 3.00 | 380–418 |

These spread values describe this validation round only. They are not fixed precision, accuracy or uncertainty values for future tests.
Future result groups always show their actual independent-specimen count; ten specimens are not universally required.

Force reduction over the recorded interval may be reported as `(F10 − F30) / F10 × 100`. It must be labelled as reduction from 10 to
30 seconds, not from an unrecorded initial or peak force.
