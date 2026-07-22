using System.Globalization;
using System.IO;
using System.Text;
using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class DocumentationEngineService
{
    public const string WhitepaperFileName = "3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf";

    public DocumentationDocument BuildDocument(DateTime generatedAt) => new(
        "3DPIceland Labs Mechanical Testing Methodology",
        "1.0",
        generatedAt,
        BuildSections());

    private static IReadOnlyList<DocumentationSection> BuildSections() => new List<DocumentationSection>
    {
        S("introduction", "1. Introduction and scope",
            "Purpose, intended use, terminology and boundaries of the 3DPIceland Labs mechanical-testing programme.",
            "## Purpose",
            "3DPIceland Labs maintains a comparative database of fused-filament-fabrication materials. The purpose of the programme is to print specimens under controlled and repeatable conditions, measure their mechanical response with the same equipment and calculation path, and make the resulting comparisons understandable to makers, designers and filament manufacturers.",
            "The project is designed around consistency rather than accreditation. A result is most useful when it can be compared directly with other results produced by the same workflow. Every specimen therefore passes through a common chain: material preparation, controlled slicing, printing, specimen identification, physical testing, native calculation, verification and publication.",
            "## What the whitepaper covers",
            "This document describes the current printing standard, specimen handling, tensile and layer-adhesion testing, pendulum impact testing, stiffness testing, statistical processing, confidence indicators, data governance and publication pipeline. It also documents known limitations and the conditions under which results should be interpreted.",
            "## What the whitepaper does not claim",
            "The equipment is not represented as an accredited ISO or ASTM laboratory system. The specimen geometries, fixtures and calculations are optimized for internal comparison and practical repeatability. Values should not be substituted automatically for certified material-datasheet values, structural design allowables or safety-critical engineering calculations.",
            "NOTE: A published number should always be read together with print orientation, sample count, spread and methodology version."),

        S("philosophy", "2. Testing philosophy",
            "Why the programme emphasizes comparative engineering, stable procedures and transparent uncertainty.",
            "## Comparative engineering",
            "FFF material performance depends on more than the polymer name. Pigment, reinforcement, moisture, extrusion consistency, nozzle temperature, cooling, layer bonding and specimen orientation can all change the result. For that reason, the programme asks a practical question: how does this spool behave when tested through the same workflow as the rest of the database?",
            "Absolute agreement with an external laboratory is useful but is not the primary objective. The primary objective is that two materials tested months apart remain meaningfully comparable because the printer, specimen geometry, fixtures, equations and acceptance rules are controlled and documented.",
            "## Controlled variables",
            "The same operator, equipment family, measurement sequence and native calculation services are used wherever practical. Changes to a fixture, formula or reference constant are treated as methodology changes and must be recorded in revision history before mixed-version comparisons are presented without qualification.",
            "## Orientation is part of the result",
            "FFF parts are anisotropic. A specimen loaded mainly along deposited roads can behave very differently from one loaded across layer interfaces. The database therefore keeps tensile-strength and layer-adhesion-strength results separate rather than combining them into one generic strength number.",
            "## Repeatability before novelty",
            "A new material is not considered fully characterized merely because one specimen produced a plausible number. Repeated specimens are used to reveal print inconsistency, handling errors and natural material variation. A stable average with an understandable spread is more valuable than a single impressive peak.",
            "## Traceability",
            "Raw measurements remain distinct from calculated summaries. Native calculation services create verified material summaries, and website/reporting systems consume only the verified outputs. This separation reduces the risk that display logic silently changes engineering values."),

        S("printing", "3. Standard printing configuration",
            "How specimens are prepared so that results remain comparable across materials and time.",
            "## Printer and slicer control",
            "Specimens are produced on a known printer configuration using a controlled slicer profile. The objective is not to force every polymer into one temperature profile, but to keep geometry and structural settings stable while selecting sensible material-specific thermal settings.",
            "## Standard structural settings",
            "The standard test-print framework uses a 0.4 mm nozzle for unfilled materials and may use a 0.6 mm hardened nozzle for abrasive carbon-fibre or glass-fibre materials. Typical geometry settings are 0.20 mm layer height, two perimeters, two top layers, two bottom layers and zero percent infill. Any deviation that changes the effective section or failure mechanism must be documented.",
            "## Thermal settings",
            "Nozzle temperature, bed temperature, chamber conditions and cooling are selected to produce a reliable specimen for the tested material. They are held stable within a test batch. Temperature changes are not used casually to chase a better individual result after testing has begun, because that would reduce comparability.",
            "## Drying and moisture",
            "Moisture-sensitive materials can lose strength, produce voids or show inconsistent extrusion when wet. Drying status and storage history are therefore treated as relevant specimen metadata. A result from inadequately conditioned nylon, for example, may represent the wet print rather than the material's attainable performance.",
            "## Spool, colour and batch effects",
            "The result applies directly to the tested spool and conditions. Different colours, production batches or reinforcement percentages may behave differently even when sold under the same product family. The database identifies materials at product level and preserves batch-related purchasing information where available.",
            "## Print acceptance",
            "Specimens are visually checked for major defects, under-extrusion, severe warping, interrupted layers and obvious handling damage. Rejected prints are not silently included. Minor surface character that is representative of the material remains part of the real-world result.",
            "NOTE: Comparable does not mean identical thermal settings; it means controlled geometry, documented material settings and a repeatable decision process."),

        S("specimens", "4. Specimen design and orientation",
            "Geometry principles, specimen identification and the meaning of flat and upright printing.",
            "## Geometry objective",
            "The tensile specimen uses a narrowed gauge region so that failure is encouraged away from the grips. The impact and stiffness specimens are shaped for the corresponding fixtures. The designs are practical FFF specimens rather than direct copies of every detail in a formal standard.",
            "## Effective cross-section",
            "Because the tensile specimen is printed with shells and no infill, the apparent outer rectangle is not the same as the load-bearing polymer area. The native tensile calculation therefore uses an effective break-section area of 6.5016 mm2. This correction is essential; using the full bounding rectangle would systematically understate stress.",
            "## Flat orientation",
            "A flat tensile specimen is printed lying on the build plate. Its main load path follows deposited roads and perimeter structure. The resulting MPa value is published as Tensile Strength because it primarily represents strength within the printed plane.",
            "## Upright orientation",
            "An upright tensile specimen is printed so that the test load crosses layer interfaces. The resulting MPa value is published as Layer Adhesion Strength. This is often the more demanding orientation and is especially sensitive to temperature, cooling, moisture and warping.",
            "## Identification",
            "Each specimen group must remain associated with material, orientation and run. Mixing flat and upright specimens or combining specimens from different batches without traceability can invalidate the summary. The application stores measurement rows separately and calculates orientation-specific outputs.",
            "DIAGRAM: Flat specimen -> deposited roads mainly parallel to the load. Upright specimen -> layer interfaces mainly perpendicular to the load.",
            "## Failure location",
            "Tensile results are most interpretable when failure occurs in the narrowed test region. Grip failures, slipping or fractures at an unintended geometry transition should be reviewed before acceptance."),

        S("tensile", "5. Tensile and layer-adhesion testing",
            "Equipment, preparation, test sequence, calculations, failure modes and quality controls.",
            "## Equipment",
            "The tensile system is a purpose-built mechanical tester fitted with a force gauge capable of peak-hold measurement. The fixture applies increasing axial load while the gauge records the highest force reached before fracture or release.",
            "## Why a purpose-built specimen is used",
            "Early prototypes showed that generic printed shapes could fail near grips or transitions. The adopted specimen geometry focuses stress into a defined narrow region so that repeated tests break in a more consistent location. This improves comparison even though the geometry is not claimed as an accredited standard coupon.",
            "## Sample plan",
            "A complete tensile programme normally uses ten flat specimens and ten upright specimens. This provides enough repeated observations to calculate an average and meaningful spread while keeping the workload practical. Incomplete groups remain visible as incomplete and should not be interpreted with the same confidence as full groups.",
            "## Test preparation",
            "1. Confirm material identity, orientation and run. 2. Inspect the specimen for obvious defects. 3. Verify that grips and contact surfaces are clean. 4. Install the specimen symmetrically. 5. Zero or confirm the force gauge. 6. Enable peak hold before loading.",
            "## Loading sequence",
            "Load is increased steadily until the specimen fractures or the test is otherwise terminated. Sudden impacts, lateral bending and grip movement are avoided. The peak force in Newtons is recorded immediately with the correct specimen row.",
            "FORMULA: Tensile stress (MPa) = peak force (N) / effective cross-sectional area (mm2). Since 1 N/mm2 equals 1 MPa, no additional unit factor is required.",
            "FORMULA: Current effective cross-sectional area = 6.5016 mm2.",
            "## Flat-result interpretation",
            "The flat value describes the printed structure loaded primarily within the layer plane. High values can result from a strong polymer, good extrusion, effective perimeter alignment and reinforcement. A reinforced filament may increase stiffness or tensile strength without improving every other property.",
            "## Upright-result interpretation",
            "The upright value describes interlayer performance. It is influenced strongly by interlayer diffusion, cooling, chamber temperature, moisture and geometry stability. The ratio between upright and flat strength can be useful, but the two raw values should always remain available.",
            "## Failure review",
            "Acceptable failures normally occur in or near the gauge region. A specimen that slips, breaks in a grip, contains a major print defect or was installed incorrectly should be flagged for review. Excluding a result must be based on an observable test issue, not because the number is inconvenient.",
            "## Video reference",
            "The practical tensile-test demonstration is available at https://www.youtube.com/watch?v=kax8Ha_AGcQ. The video shows the homemade tester, specimen development, peak-force recording, flat/upright workflow and conversion to MPa.",
            "NOTE: Tensile Strength and Layer Adhesion Strength are separate engineering outputs. They must not be relabelled as interchangeable orientations in consumer-facing reports."),

        S("impact", "6. Pendulum impact testing",
            "How the custom pendulum tester compares fracture and deformation energy across orientations.",
            "## Equipment concept",
            "The impact tester uses a pendulum hammer, a controlled starting position, a specimen fixture and an indicator that records remaining travel after impact. The difference between reference travel and post-impact travel is converted through the native calculation path into impact resistance.",
            "## Development and repeatability",
            "The fixture and locking arrangement were refined through multiple prototypes. More than seven hundred fractures during development and database production provided practical evidence about handling, repeatability and common failure modes. This history is important because custom equipment must earn trust through stable use and transparent limitations.",
            "## Specimen orientation",
            "Flat and upright impact specimens are tracked independently. Orientation changes crack path, layer-interface exposure and deformation behaviour. A material that is tough in one orientation may still separate readily between layers in the other.",
            "## Test sequence",
            "1. Confirm specimen identity and orientation. 2. Inspect the fixture and pointer. 3. Place and lock the specimen consistently. 4. Raise the hammer to the defined reference position. 5. Release without adding external force. 6. Record the indicated remaining angle or travel. 7. Note whether the specimen fractured, bent, delaminated or remained intact.",
            "FORMULA: The native impact calculation uses a reference angle of 105.411 degrees and a specimen-area constant of 0.00004816 m2, together with the tester's pendulum-energy relationship.",
            "## Non-fracture results",
            "Flexible materials may bend and allow the hammer to continue without a clean brittle fracture. This remains a meaningful response, but it should not be described as the same failure mode as a snapped rigid specimen. Notes about deformation and hammer travel are therefore important for TPU and other highly ductile materials.",
            "## Fracture-mode interpretation",
            "A brittle fracture suggests low energy absorption before crack propagation. A rough, stretched or partially hinged fracture may indicate greater plastic deformation. Reinforced materials can become stiffer while also becoming more notch-sensitive or orientation-sensitive; carbon fibre does not automatically mean superior impact resistance.",
            "## Error sources",
            "Potential errors include inconsistent starting angle, pointer friction, specimen movement, fixture wear, off-centre impact and transcription mistakes. Repeated specimens and periodic empty/reference checks help identify drift.",
            "## Video reference",
            "The impact-test demonstration is available at https://www.youtube.com/watch?v=ibjS_tWL6sg. It documents the pendulum concept, locking fixture, prototype iterations, statistical workflow and orientation effects."),

        S("stiffness", "7. Stiffness testing",
            "Known-load deflection testing and conversion to an engineering stiffness value.",
            "## Measurement principle",
            "A specimen is supported over a known span and loaded with a known force. Deflection is measured through the tester's screw/indicator movement. Smaller deflection under the same geometry and load corresponds to higher calculated stiffness.",
            "## Current constants",
            "The native calculation uses 1.058774 mm movement per complete revolution, a 118.2 mm span, a 5.47 N load and a 34.679467 mm4 second moment of area. These constants belong to the current fixture and specimen geometry and must be revised together if the hardware or section changes.",
            "## Test sequence",
            "1. Inspect and identify the specimen. 2. Place it consistently on the supports. 3. Bring the measurement point into contact without preload beyond the defined zero. 4. Zero the indicator. 5. Apply the known weight/load. 6. Record complete turns and additional degrees. 7. Remove the load and check for obvious permanent set or seating error.",
            "FORMULA: Deflection (mm) = complete revolutions x 1.058774 + (degrees / 360) x 1.058774.",
            "FORMULA: For the current simply-supported central-load model, elastic modulus is derived from load, span, second moment of area and measured deflection. The application performs this conversion natively and reports MPa.",
            "## Elastic interpretation",
            "The calculation assumes that the specimen response is sufficiently close to the elastic beam model used by the fixture. Large permanent deformation, support slip or nonlinear response reduces the validity of a simple modulus interpretation and should be noted.",
            "## Repeatability",
            "The purchased tester and standardized placement provide practical repeatability, commonly around five percent in stable groups. This does not remove the need for repeated specimens; it defines the approximate scale at which small differences should be interpreted cautiously.",
            "## Stiffness is not strength",
            "A high stiffness value means the specimen resists bending. It does not prove that the material has high tensile strength, high impact resistance or good layer adhesion. Stiff but brittle materials and flexible but tough materials are both common.",
            "## Video reference",
            "The stiffness-test demonstration is available at https://www.youtube.com/watch?v=nv9PexjvFRw. It shows zero-load calibration, the known weight, turn/degree recording and conversion to MPa."),

        S("statistics", "8. Statistical processing",
            "Averages, standard deviation, coefficient of variation, coverage and outlier handling.",
            "## Arithmetic mean",
            "The arithmetic mean is the primary summary for a completed orientation/test group. It represents the centre of the repeated measurements and is suitable for direct ranking when sample coverage and spread are also considered.",
            "FORMULA: Mean = sum of accepted measurements / number of accepted measurements.",
            "## Standard deviation",
            "Standard deviation describes absolute spread in the same unit as the measurement. A larger standard deviation means individual specimens differed more strongly from the group average.",
            "## Coefficient of variation",
            "The coefficient of variation expresses standard deviation relative to the mean. It is useful when comparing consistency between properties with different magnitudes.",
            "FORMULA: CV percent = standard deviation / mean x 100.",
            "## Internal repeatability score",
            "FORMULA: Consistency score = 100 - average available tensile/impact CV percent - incomplete-sample penalty.",
            $"The {ConsistencyCalibrationService.ScaleName} uses these score bands: {ConsistencyCalibrationService.ScoreBandSummary}",
            "With complete ten-specimen sets, the score bands correspond approximately to average CV ranges of 0-10%, 10-15%, 15-20%, 20-30%, 30-40% and above 40%. The sample penalty means score and average CV are not always exact inverses.",
            "This is an internal comparative scale derived from the established 3DPIceland workflow. It is not an industry standard, an accredited uncertainty budget or proof of absolute instrument accuracy.",
            "## Coverage",
            "Coverage records how many expected measurements are present. A missing impact-flat value, tensile value or stiffness value can prevent a complete baseline or delta calculation. The platform therefore distinguishes absent data from a genuine zero.",
            "## Outliers",
            "A value is not removed merely because it is far from the average. Removal requires a test-specific reason such as grip slip, visible print failure, incorrect orientation, fixture error or transcription mistake. The original raw row should remain traceable whenever practical.",
            "## Rounding",
            "Calculations should retain adequate internal precision and round only for display. Early rounding can create small inconsistencies between database, report and website outputs.",
            "## Practical significance",
            "A numerical difference smaller than normal repeatability should not automatically be presented as a meaningful material advantage. Rankings are most useful when combined with spread, sample count and the intended application."),

        S("confidence", "9. Confidence and scoring",
            "How confidence indicators support, but do not replace, the underlying measurements.",
            "## Purpose",
            "Confidence communicates how complete and repeatable the available evidence is. It is not a subjective award for a preferred brand and it does not alter the measured MPa or impact value.",
            "## Inputs",
            "Confidence may consider sample count, coverage across required orientations, observed variability, availability of all core tests and whether verification gates pass. A fully populated, repeatable dataset deserves more confidence than a partial group with only a few specimens.",
            "## Star interpretation",
            "One star indicates very limited evidence or incomplete coverage. Two stars indicate early comparative data. Three stars indicate useful but not fully mature evidence. Four stars indicate strong coverage and acceptable repeatability. Five stars indicate the most complete and internally consistent evidence available under the current methodology.",
            "## Scoring boundaries",
            "Performance scores and recommendation logic are derived from verified material summaries. A composite score can help navigation, but it must never hide trade-offs. Strength, impact, stiffness, layer adhesion, consistency, cost and printability answer different questions.",
            "NOTE: Confidence measures confidence in the dataset, not certainty that every future spool or printed part will produce the same value."),

        S("data", "10. Data architecture and calculation governance",
            "How raw measurements become verified summaries, website payloads and reports.",
            "## SQLite source of truth",
            "The native SQLite database is the canonical operational source. Materials use canonical MaterialID values, and measurement rows remain associated with their material and run context.",
            "## Raw versus calculated data",
            "Raw measurements are stored independently from calculated summaries. Native calculation services convert force, angle and deflection inputs into engineering values. The Material Summary layer then assembles verified outputs for each material.",
            "## Verification Center",
            "Verification gates check that calculation engines, summary coverage, report payloads and website payloads agree with expected results. Publication should stop when required gates fail rather than exporting an attractive but unverified number.",
            "## Website pipeline",
            "The website consumes verified Material Summary data. It does not independently recalculate raw rows in browser JavaScript. This reduces duplicate formulas and keeps the native calculation engine as the engineering authority.",
            "## Reporting pipeline",
            "HTML and PDF reports use the same verified model. Documentation generation is separated from measurement calculation but follows the same release, verification and packaging discipline.",
            "DIAGRAM: Raw measurements -> Native calculation services -> Verified Material Summary -> Website, reports, analytics and recommendations.",
            "## Change control",
            "A change to a constant, formula, specimen area, fixture geometry or interpretation rule requires documentation, version updates and regression testing. Historic results should not be silently recomputed under a materially different methodology without clear migration records."),

        S("interpretation", "11. Interpreting published results",
            "Practical guidance for comparing materials without overstating what a ranking means.",
            "## Compare the property that matters",
            "For a loaded bracket, tensile and layer adhesion may matter. For a protective part, impact response may dominate. For a beam or fixture, stiffness can be more important than ultimate strength. No single property defines the best filament for every use.",
            "## Read both orientations",
            "A material with excellent flat tensile strength but weak upright adhesion may perform well in carefully oriented parts and poorly in geometry that loads across layers. Orientation-aware design remains necessary even when the material ranks highly.",
            "## Consider consistency",
            "Two materials with similar averages can differ in repeatability. The one with lower spread may be easier to design around, while the one with a slightly higher peak but large variability may require more process tuning.",
            "## Compare like with like",
            "Material-family averages and direct comparisons are most meaningful when print settings, nozzle class and test methodology are compatible. A reinforced high-temperature polymer should not be judged only against ordinary PLA without acknowledging cost, print difficulty and application.",
            "## Avoid datasheet substitution",
            "Manufacturer datasheets often use injection-moulded or standardized specimens. Those values describe a different production process and can be useful references, but they should not be expected to match the 3DPIceland printed-specimen workflow exactly."),

        S("limitations", "12. Known limitations and uncertainty",
            "Transparent boundaries of the equipment, samples, environment and interpretation.",
            "## Equipment class",
            "The tensile and impact systems are custom or small-scale devices. They do not provide the calibration traceability, environmental control, extensometry or automated acquisition of an accredited laboratory. Their value comes from consistent internal comparison and documented use.",
            "## Environmental conditions",
            "Room temperature and humidity are not equivalent to a fully controlled chamber. Moisture-sensitive polymers may change during storage, printing or testing. Results therefore represent the recorded practical conditions rather than every possible service environment.",
            "## Impact pointer and fixture sensitivity",
            "Impact readings can be influenced by pointer pivot tension, fastener adjustment and equipment temperature. A colder pointer can move less freely, and the fastener retaining the stop/pointer mechanism may require periodic adjustment. Free movement, reference position and fastener security should be checked before a session.",
            "## Tensile low-force floor",
            "Carriage and slide resistance create a practical low-force measurement floor, commonly around 7-8 N. Very weak specimens that separate near this level should be identified as near the fixture floor rather than interpreted as equally precise to higher-force failures. CV alone cannot detect or correct this systematic limitation.",
            "## Stiffness angular variation",
            "Repeated stiffness placement and reading can vary by roughly 10-15 degrees on the current fixture. Differences close to this range should be treated cautiously and should not be presented as decisive without repeat confirmation.",
            "## Strain and toughness detail",
            "Peak-force tensile testing does not provide a complete stress-strain curve, yield strain or elongation at break. Pendulum impact output does not capture every detail of crack initiation and ductile deformation. These methods provide comparative indices, not a complete constitutive model.",
            "## Geometry dependence",
            "Printed shells, zero infill, nozzle diameter and layer height define the specimen structure. A thick solid part, foamed print, different raster pattern or alternate nozzle may produce different behaviour.",
            "## Batch and colour variation",
            "One spool cannot prove that every spool sold under the same name is identical. Resin supply, pigment and reinforcement distribution can change. Repeated future batches are valuable for identifying product drift.",
            "## Flexible-material impact",
            "Materials that bend without clean fracture can exceed the simple assumptions of a brittle pendulum comparison. Their results should include deformation notes and should not be interpreted solely through a fracture-energy ranking.",
            "## Small differences",
            "Differences close to the normal CV or fixture repeatability may not be practically significant. Users should avoid declaring a winner based on tiny numerical separation without considering spread and sample count."),

        S("quality", "13. Quality assurance and operating checklist",
            "Minimum checks before, during and after a test series.",
            "## Before printing",
            "Confirm material identity, nozzle suitability, drying requirement, profile selection and specimen orientation. Verify that the intended specimen file and structural settings match the current methodology.",
            "## Before testing",
            "Inspect fixtures, zero the instrument, confirm reference positions, prepare the correct measurement screen and ensure the specimen label matches the selected material and run. Confirm that the impact pointer moves freely at the current room/equipment temperature, its retaining fastener is secure, the tensile carriage moves normally and the stiffness contact/angle reference returns consistently.",
            "## During testing",
            "Use a consistent loading/release action. Record the value immediately. Add a note when fracture mode, deformation, slip or equipment behaviour is unusual. Do not rely on memory to reconstruct measurements later.",
            "## After a group",
            "Review sample count, mean, standard deviation, CV and visible outliers. Confirm that every required orientation is present. Run verification before publication or delta/baseline analysis.",
            "## Periodic maintenance",
            "Inspect grips, pivots, pointers, supports and contact surfaces for wear. Recheck reference travel and known-load behaviour. A methodology can remain unchanged only while the equipment continues to represent the documented geometry and constants."),

        S("intelligence-handoffs", "14. Governed Engineering Intelligence handoffs",
            "How reports, whitepaper context and video planning reuse verified engineering interpretations without creating new calculation paths.",
            "## Shared source boundary",
            EngineeringIntelligenceHandoffService.GovernanceStatement,
            "Engineering reports may present advisor evidence, repeatability, price, inventory, manufacturer, peer-position and alternative context only after those interpretations have been produced by their existing services. The report renderer formats the supplied context; it does not own measurement or score calculations.",
            "## Whitepaper role",
            "This whitepaper documents the calculation boundary and interpretation rules. It does not embed live per-material rankings or turn an advisor interpretation into a new measured property.",
            "## Video-planning role",
            "When a recommendation is sent to Video Planner, its canonical MaterialID, existing engineering score axes and governed context travel with the idea. Editorial titles and hooks may be refined later, but the evidence statement remains traceable to the recommendation that created the idea.",
            "NOTE: Report, whitepaper and video-planning text must disclose unavailable evidence rather than infer missing values."),

        S("future", "15. Future improvements",
            "Planned directions that can increase traceability and measurement depth.",
            "Potential improvements include controlled environmental conditioning, calibrated reference specimens, automated force/angle/deflection logging, richer fracture-mode photography, batch-repeat studies and formal uncertainty budgets.",
            "Any future instrument upgrade should first run in parallel with the current method. Overlap testing can quantify systematic differences and determine whether historic and new results can share one ranking or require a methodology boundary.",
            "Additional properties such as heat deflection, creep, fatigue, abrasion and chemical resistance may eventually complement the current tensile, impact and stiffness framework. Each new property should have its own specimen, calculation, verification and limitation documentation."),

        S("references", "16. References and demonstrations",
            "Primary demonstrations and external context used by the programme.",
            "## Procedure demonstrations",
            "Tensile testing: https://www.youtube.com/watch?v=kax8Ha_AGcQ",
            "Impact testing: https://www.youtube.com/watch?v=ibjS_tWL6sg",
            "Stiffness testing: https://www.youtube.com/watch?v=nv9PexjvFRw",
            "## Standards context",
            "ISO and ASTM tensile, impact and flexural standards are useful context for terminology and laboratory practice. The 3DPIceland workflow is not represented as direct certification to those standards unless a future revision explicitly documents such compliance.",
            "## Manufacturer documentation",
            "Manufacturer datasheets are used as contextual references where appropriate, but website measurements are generated from the native 3DPIceland test workflow."),

        S("revision", "17. Revision history",
            "Controlled history of methodology and documentation changes.",
            "Version 1.0 - Initial full engineering whitepaper. Expanded equipment descriptions, specimen preparation, test procedures, calculations, statistics, confidence, architecture, limitations and operating controls.",
            "Platform v41.5 - Added governed Engineering Intelligence handoff documentation for canonical reports and video planning. No methodology constants or measurement calculations changed.",
            "Platform v41.6 - Calibrated repeatability interpretation to the internal 3DPIceland score scale and documented known impact-pointer, tensile low-force and stiffness-angle limitations. The established consistency-score formula and historic rankings were preserved.",
            "Platform implementation: v40.15.3 - Manufacturer Edit Transaction Safety.",
            "Future revisions must state the date, affected test, reason for change, whether constants changed and whether historic results were migrated or remain under an earlier methodology version.")
    };

    private static DocumentationSection S(string id, string title, string summary, params string[] details) =>
        new(id, title, summary, details);

    public byte[] RenderPdf(DocumentationDocument document)
    {
        var pages = BuildPages(document);
        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "3dp-iceland-labs-logo-pdf.jpg");
        if (!File.Exists(logoPath))
            logoPath = Path.Combine(AppContext.BaseDirectory, "3dp-iceland-labs-logo-pdf.jpg");
        var logoBytes = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : Array.Empty<byte>();

        var objects = new List<byte[]>
        {
            A("<< /Type /Catalog /Pages 2 0 R >>"), A("__PAGES__"),
            A("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"),
            A("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"),
            BuildImageObject(logoBytes, 801, 482)
        };
        var pageNumbers = new List<int>();
        for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
        {
            var pageNo = objects.Count + 1;
            var contentNo = pageNo + 1;
            pageNumbers.Add(pageNo);
            objects.Add(A($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R /F2 4 0 R >> /XObject << /Im1 5 0 R >> >> /Contents {contentNo} 0 R >>"));
            var stream = BuildPageStream(pages[pageIndex], pageIndex + 1, pages.Count, document, logoBytes.Length > 0);
            var bytes = Encoding.ASCII.GetBytes(stream);
            objects.Add(A($"<< /Length {bytes.Length} >>\nstream\n{stream}\nendstream"));
        }
        objects[1] = A($"<< /Type /Pages /Kids [{string.Join(" ", pageNumbers.Select(x => x + " 0 R"))}] /Count {pageNumbers.Count} >>");
        using var output = new MemoryStream();
        var offsets = new List<long> { 0 };
        Write(output, "%PDF-1.4\n% 3DPIceland Documentation Engine\n");
        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(output.Position); Write(output, $"{i + 1} 0 obj\n"); output.Write(objects[i]); Write(output, "\nendobj\n");
        }
        var xref = output.Position;
        Write(output, $"xref\n0 {objects.Count + 1}\n0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1)) Write(output, $"{offset:0000000000} 00000 n \n");
        Write(output, $"trailer\n<< /Size {objects.Count + 1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return output.ToArray();
    }

    private sealed record PdfLine(string Text, int Size = 10, bool Bold = false, int SpaceAfter = 5, int Indent = 0);
    private sealed record PdfPage(string Kind, IReadOnlyList<PdfLine> Lines, string Title = "");

    private static List<PdfPage> BuildPages(DocumentationDocument document)
    {
        var pages = new List<PdfPage>
        {
            new("cover", Array.Empty<PdfLine>()),
            new("toc", document.Sections.Where(x => x.VisibleInWhitepaper)
                .Select(x => new PdfLine(x.Title, 10, false, 7)).ToList(), "Contents"),
            new("matrix", Array.Empty<PdfLine>(), "Engineering test matrix"),
            new("diagram", Array.Empty<PdfLine>(), "Verified publication architecture"),
            new("graph", Array.Empty<PdfLine>(), "Interpreting repeatability and confidence")
        };

        foreach (var section in document.Sections.Where(x => x.VisibleInWhitepaper))
        {
            var current = NewChapterPage(section.Title, section.Summary);
            pages.Add(new PdfPage("content", current, section.Title));
            var yUsed = EstimateHeight(current);
            foreach (var raw in section.Details)
            {
                foreach (var line in FormatDetail(raw))
                {
                    var height = line.Size + line.SpaceAfter + 3;
                    if (yUsed + height > 575)
                    {
                        current = NewContinuationPage(section.Title);
                        pages.Add(new PdfPage("content", current, section.Title));
                        yUsed = EstimateHeight(current);
                    }
                    current.Add(line);
                    yUsed += height;
                }
            }
        }
        return pages;
    }

    private static List<PdfLine> NewChapterPage(string title, string summary) => new()
    {
        new(title, 19, true, 13), new(summary, 11, false, 17)
    };

    private static List<PdfLine> NewContinuationPage(string title) => new()
    {
        new(title + " (continued)", 15, true, 14)
    };

    private static int EstimateHeight(IEnumerable<PdfLine> lines) => lines.Sum(x => x.Size + x.SpaceAfter + 3);

    private static IEnumerable<PdfLine> FormatDetail(string raw)
    {
        if (raw.StartsWith("## "))
        {
            yield return new PdfLine(raw[3..], 13, true, 9);
            yield break;
        }
        var prefix = "";
        var bold = false;
        var indent = 0;
        var size = 10;
        var space = 8;
        if (raw.StartsWith("FORMULA: ")) { prefix = "Formula: "; raw = raw[9..]; bold = true; indent = 12; }
        else if (raw.StartsWith("NOTE: ")) { prefix = "Engineering note: "; raw = raw[6..]; bold = true; indent = 12; }
        else if (raw.StartsWith("DIAGRAM: ")) { prefix = "Data flow: "; raw = raw[9..]; bold = true; indent = 12; }
        var wrapped = Wrap(prefix + raw, indent > 0 ? 81 : 89).ToList();
        for (var i = 0; i < wrapped.Count; i++)
            yield return new PdfLine(wrapped[i], size, bold && i == 0, i == wrapped.Count - 1 ? space : 3, indent);
    }

    private static IEnumerable<string> Wrap(string text, int width)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var line = new StringBuilder();
        foreach (var word in words)
        {
            if (line.Length > 0 && line.Length + word.Length + 1 > width)
            {
                yield return line.ToString(); line.Clear();
            }
            if (line.Length > 0) line.Append(' ');
            line.Append(word);
        }
        if (line.Length > 0) yield return line.ToString();
    }

    private static string BuildPageStream(PdfPage page, int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        return page.Kind switch
        {
            "cover" => BuildCoverStream(document, pageNumber, pageCount, hasLogo),
            "toc" => BuildTocStream(page, pageNumber, pageCount, document, hasLogo),
            "matrix" => BuildMatrixStream(pageNumber, pageCount, document, hasLogo),
            "diagram" => BuildDiagramStream(pageNumber, pageCount, document, hasLogo),
            "graph" => BuildGraphStream(pageNumber, pageCount, document, hasLogo),
            _ => BuildContentStream(page, pageNumber, pageCount, document, hasLogo)
        };
    }

    private static string BuildCoverStream(DocumentationDocument document, int pageNumber, int pageCount, bool hasLogo)
    {
        var sb = new StringBuilder();
        sb.AppendLine("1 1 1 rg 0 0 612 792 re f");
        sb.AppendLine("0.95 0.97 0.99 rg 0 0 612 205 re f");
        sb.AppendLine("0.05 0.39 0.82 rg 0 202 612 5 re f");
        if (hasLogo) sb.AppendLine("q 300 0 0 180 156 548 cm /Im1 Do Q");
        Text(sb, "ENGINEERING WHITEPAPER", 46, 477, 12, true, 0.05, 0.39, 0.82);
        Text(sb, "Mechanical Testing Methodology", 46, 432, 27, true, 0.10, 0.12, 0.15);
        Text(sb, "Comparative testing of FFF filament specimens", 46, 397, 14, false, 0.25, 0.29, 0.33);
        Text(sb, "Tensile strength  |  Layer adhesion  |  Impact resistance  |  Stiffness", 46, 368, 10, false, 0.25, 0.29, 0.33);
        Text(sb, "Version " + document.Version, 46, 158, 13, true, 0.10, 0.12, 0.15);
        Text(sb, "Platform implementation: v40.15.3", 46, 135, 10, false, 0.25, 0.29, 0.33);
        Text(sb, "Generated " + document.GeneratedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), 46, 116, 10, false, 0.25, 0.29, 0.33);
        Text(sb, "3DPIceland Labs", 46, 78, 13, true, 0.05, 0.39, 0.82);
        Text(sb, "Comparative engineering - transparent methods - verified publication", 46, 57, 9, false, 0.25, 0.29, 0.33);
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static string BuildTocStream(PdfPage page, int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = NewStandardPage(page.Title, pageNumber, pageCount, document, hasLogo);
        var y = 682;
        var index = 1;
        foreach (var line in page.Lines)
        {
            Text(sb, index.ToString("00", CultureInfo.InvariantCulture), 48, y, 9, true, 0.05, 0.39, 0.82);
            Text(sb, line.Text, 78, y, 10, false, 0.12, 0.14, 0.17);
            sb.AppendLine($"0.84 0.87 0.90 RG 48 {y - 7} m 564 {y - 7} l S");
            y -= 31;
            index++;
        }
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static string BuildContentStream(PdfPage page, int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = NewStandardPage(page.Title, pageNumber, pageCount, document, hasLogo);
        var y = 682;
        foreach (var line in page.Lines)
        {
            if (line.Indent > 0)
            {
                sb.AppendLine($"0.94 0.97 1.00 rg 48 {y - line.Size - 6} 516 {line.Size + line.SpaceAfter + 11} re f");
                sb.AppendLine($"0.05 0.39 0.82 rg 48 {y - line.Size - 6} 3 {line.Size + line.SpaceAfter + 11} re f");
            }
            var c = line.Size >= 13 ? (0.10, 0.12, 0.15) : (0.16, 0.18, 0.21);
            Text(sb, line.Text, 52 + line.Indent, y, line.Size, line.Bold, c.Item1, c.Item2, c.Item3);
            y -= line.Size + line.SpaceAfter + 3;
        }
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static string BuildMatrixStream(int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = NewStandardPage("Engineering test matrix", pageNumber, pageCount, document, hasLogo);
        Text(sb, "The programme separates properties, orientations, inputs and verified outputs.", 48, 680, 10, false, 0.25, 0.29, 0.33);
        var xs = new[] { 48, 152, 272, 386, 564 };
        var yTop = 638;
        var rowH = 58;
        sb.AppendLine($"0.09 0.16 0.24 rg 48 {yTop} 516 34 re f");
        var headers = new[] { "Test", "Primary input", "Orientation", "Published output" };
        for (var i = 0; i < headers.Length; i++) Text(sb, headers[i], xs[i] + 8, yTop + 12, 9, true, 1, 1, 1);
        var rows = new[]
        {
            new[] { "Tensile", "Peak force (N)", "Flat", "Tensile strength (MPa)" },
            new[] { "Tensile", "Peak force (N)", "Upright", "Layer adhesion (MPa)" },
            new[] { "Impact", "Pendulum return (%)", "Flat / upright", "Impact resistance (kJ/m2)" },
            new[] { "Stiffness", "Revolutions + angle", "Beam fixture", "Stiffness index (MPa)" },
            new[] { "Statistics", "Verified sample set", "Per test/orientation", "Mean, SD, CV, confidence" }
        };
        for (var r = 0; r < rows.Length; r++)
        {
            var y = yTop - (r + 1) * rowH;
            if (r % 2 == 0) sb.AppendLine($"0.96 0.97 0.98 rg 48 {y} 516 {rowH} re f");
            for (var c = 0; c < 4; c++)
            {
                foreach (var (part, k) in Wrap(rows[r][c], c == 3 ? 24 : 18).Select((v, k) => (v, k)))
                    Text(sb, part, xs[c] + 8, y + 37 - k * 13, 9, c == 0, 0.15, 0.17, 0.20);
            }
        }
        for (var i = 0; i < xs.Length; i++) sb.AppendLine($"0.72 0.76 0.80 RG {xs[i]} {yTop - rows.Length * rowH} m {xs[i]} {yTop + 34} l S");
        for (var r = 0; r <= rows.Length; r++) { var y = yTop - r * rowH; sb.AppendLine($"0.72 0.76 0.80 RG 48 {y} m 564 {y} l S"); }
        sb.AppendLine($"0.72 0.76 0.80 RG 48 {yTop + 34} m 564 {yTop + 34} l S");
        Text(sb, "Publication rule", 48, 285, 13, true, 0.10, 0.12, 0.15);
        Text(sb, "Only verified Material Summary outputs are consumed by Website, Reports and Documentation.", 48, 260, 10, false, 0.20, 0.23, 0.27);
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static string BuildDiagramStream(int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = NewStandardPage("Verified publication architecture", pageNumber, pageCount, document, hasLogo);
        Text(sb, "One calculation authority feeds every published surface.", 48, 680, 10, false, 0.25, 0.29, 0.33);
        Box(sb, 58, 548, 130, 74, "SQLite", "Raw measurements");
        Box(sb, 240, 548, 132, 74, "Native services", "Calculation engines");
        Box(sb, 424, 548, 130, 74, "Verification", "Material Summary");
        Arrow(sb, 188, 585, 240, 585); Arrow(sb, 372, 585, 424, 585);
        Box(sb, 58, 372, 130, 74, "Website", "Interactive database");
        Box(sb, 240, 372, 132, 74, "Reports", "HTML and PDF");
        Box(sb, 424, 372, 130, 74, "Documentation", "Whitepaper");
        Arrow(sb, 489, 548, 123, 446); Arrow(sb, 489, 548, 306, 446); Arrow(sb, 489, 548, 489, 446);
        Text(sb, "Architecture control", 48, 286, 13, true, 0.10, 0.12, 0.15);
        var notes = new[]
        {
            "SQLite remains the Single Source of Truth.",
            "Display layers do not independently recalculate engineering values.",
            "Verification Center gates publication before export.",
            "Methodology changes are versioned and documented."
        };
        var y = 258;
        foreach (var note in notes) { Text(sb, "- " + note, 58, y, 10, false, 0.18, 0.21, 0.24); y -= 25; }
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static string BuildGraphStream(int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = NewStandardPage("Interpreting repeatability and confidence", pageNumber, pageCount, document, hasLogo);
        Text(sb, "3DPIceland internal consistency-score bands - not an industry standard.", 48, 680, 10, false, 0.25, 0.29, 0.33);
        var labels = new[] { "Excellent", "Very good", "Good", "Moderate", "Low", "Very low" };
        var ranges = new[] { "90-100", "85-89.9", "80-84.9", "70-79.9", "60-69.9", "Below 60" };
        var widths = new[] { 360, 340, 320, 280, 240, 160 };
        var y0 = 600;
        for (var i = 0; i < labels.Length; i++)
        {
            var y = y0 - i * 58;
            Text(sb, labels[i], 48, y + 16, 10, true, 0.15, 0.17, 0.20);
            sb.AppendLine($"0.91 0.93 0.95 rg 158 {y} 380 38 re f");
            var w = widths[i];
            sb.AppendLine($"0.05 0.39 0.82 rg 158 {y} {w} 38 re f");
            Text(sb, ranges[i] + " score", 166 + w, y + 13, 10, true, 0.12, 0.14, 0.17);
        }
        Text(sb, "Interpretation", 48, 218, 13, true, 0.10, 0.12, 0.15);
        Text(sb, "Score = 100 - average tensile/impact CV% - incomplete-sample penalty.", 48, 192, 10, false, 0.20, 0.23, 0.27);
        Text(sb, "Read scores with sample count, fixture limits, orientation and fracture notes.", 48, 171, 10, false, 0.20, 0.23, 0.27);
        Footer(sb, pageNumber, pageCount, document);
        return sb.ToString();
    }

    private static StringBuilder NewStandardPage(string title, int pageNumber, int pageCount, DocumentationDocument document, bool hasLogo)
    {
        var sb = new StringBuilder();
        sb.AppendLine("1 1 1 rg 0 0 612 792 re f");
        sb.AppendLine("0.96 0.97 0.98 rg 0 728 612 64 re f");
        sb.AppendLine("0.05 0.39 0.82 rg 0 725 612 3 re f");
        if (hasLogo) sb.AppendLine("q 74 0 0 44 46 740 cm /Im1 Do Q");
        Text(sb, title, 136, 754, 12, true, 0.10, 0.12, 0.15);
        Text(sb, "3DPIceland Labs Engineering Whitepaper", 136, 738, 8, false, 0.32, 0.36, 0.40);
        return sb;
    }

    private static void Footer(StringBuilder sb, int pageNumber, int pageCount, DocumentationDocument document)
    {
        sb.AppendLine("0.78 0.81 0.84 RG 46 43 m 566 43 l S");
        Text(sb, "3DPIceland Labs - Mechanical Testing Methodology v" + document.Version, 46, 27, 8, false, 0.34, 0.38, 0.42);
        Text(sb, pageNumber + " / " + pageCount, 526, 27, 8, false, 0.34, 0.38, 0.42);
    }

    private static void Box(StringBuilder sb, int x, int y, int w, int h, string title, string subtitle)
    {
        sb.AppendLine($"0.96 0.98 1.00 rg {x} {y} {w} {h} re f");
        sb.AppendLine($"0.05 0.39 0.82 RG 1.5 w {x} {y} {w} {h} re S");
        Text(sb, title, x + 12, y + 45, 12, true, 0.08, 0.12, 0.18);
        Text(sb, subtitle, x + 12, y + 24, 8, false, 0.25, 0.29, 0.33);
    }

    private static void Arrow(StringBuilder sb, int x1, int y1, int x2, int y2)
    {
        sb.AppendLine($"0.05 0.39 0.82 RG 1.5 w {x1} {y1} m {x2} {y2} l S");
        sb.AppendLine($"0.05 0.39 0.82 rg {x2} {y2} m {x2 - 8} {y2 + 4} l {x2 - 8} {y2 - 4} l h f");
    }

    private static void Text(StringBuilder sb, string value, int x, int y, int size, bool bold, double r, double g, double b)
    {
        sb.AppendLine($"{r.ToString("0.00", CultureInfo.InvariantCulture)} {g.ToString("0.00", CultureInfo.InvariantCulture)} {b.ToString("0.00", CultureInfo.InvariantCulture)} rg");
        sb.AppendLine($"BT /{(bold ? "F2" : "F1")} {size} Tf {x} {y} Td ({Pdf(value)}) Tj ET");
    }

    private static byte[] BuildImageObject(byte[] jpeg, int width, int height)
    {
        if (jpeg.Length == 0) return A("<< /Length 0 >>\nstream\n\nendstream");
        var header = Encoding.ASCII.GetBytes($"<< /Type /XObject /Subtype /Image /Width {width} /Height {height} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpeg.Length} >>\nstream\n");
        var footer = Encoding.ASCII.GetBytes("\nendstream");
        var result = new byte[header.Length + jpeg.Length + footer.Length];
        Buffer.BlockCopy(header, 0, result, 0, header.Length);
        Buffer.BlockCopy(jpeg, 0, result, header.Length, jpeg.Length);
        Buffer.BlockCopy(footer, 0, result, header.Length + jpeg.Length, footer.Length);
        return result;
    }

    private static string Pdf(string value) => new string(value.Normalize(NormalizationForm.FormD)
        .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c < 128).ToArray())
        .Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

    private static byte[] A(string value) => Encoding.ASCII.GetBytes(value);
    private static void Write(Stream stream, string value) { var bytes = Encoding.ASCII.GetBytes(value); stream.Write(bytes); }
}
