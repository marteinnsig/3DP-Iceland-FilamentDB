# Website Platform

## Responsibility

The Website Platform owns visualization and public website generation. It consumes verified Engineering Platform outputs and must not perform independent engineering calculations.

## Services

- WebsiteDataPipeline
- WebsiteChartGeneratorService
- WebsiteRadarGeneratorService
- WebsiteHtmlRendererService
- WebsiteVerificationService
- ExperimentalWebsiteService

## Inputs

- Verified Material Summary data
- Material metadata
- Product URLs and YouTube URLs
- Website template
- Published experimental series and persisted Experimental Results/Analytics outputs

## Outputs

- Website DATA payload
- Chart payloads
- Radar payloads
- Rendered HTML
- Website verification and publish readiness status
- Experimental dashboard, chart payload and accessible result tables

## Verification gates

- Website pipeline source
- Website chart generator
- Website payload coverage
- Website radar generator
- Website radar payload coverage
- Website HTML renderer
- Website renderer payload
- Website verification suite
- Website publish readiness

## Publish rule

Website output is publish-ready only when Verification Center reports Website Publish Readiness as READY FOR PUBLISH.
