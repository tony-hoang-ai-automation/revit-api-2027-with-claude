# Revit API 2014 Developers Guidelines - Vietnamese Infographic Knowledge Base

Nguồn: `/Users/tonyhoang/Desktop/Revit 2014 Platform API Developers Guidlines.pdf`  
Đầu ra: ghi chú Markdown + infographic PNG tiếng Việt. Nội dung được tóm tắt/diễn giải lại để học Revit API, không tái bản nguyên văn sách.

## Lộ trình học khuyến nghị
1. Bắt đầu với poster 01-07 để hiểu add-in lifecycle, môi trường, command/application, ribbon và document context.
2. Học poster 08-14 để nắm dữ liệu cốt lõi: Element, Parameter, Selection, Filtering, Transaction.
3. Học poster 15-22 để làm việc với model: edit element, view, family, geometry, wall/floor/material.
4. Học poster 23-27 theo domain: Structure, Rebar, Shared Parameters/Storage, Events/DMU, MEP.
5. Dùng poster 28 như roadmap để biến kiến thức thành production add-in.

## Danh sách infographic
| # | Priority | Topic note | PNG | Prompt | Source page hits |
|---|---|---|---|---|---|
| 01 | P0 | [Tổng Quan Revit API Platform](02-topic-notes/01-revit-api-platform-overview.md) | [PNG](04-infographics-png/01-revit-api-platform-overview.png) | [Prompt](03-infographic-prompts/01-revit-api-platform-overview.md) | 2, 8, 9, 362, ... (+3 trang khác) |
| 02 | P0 | [Cài Môi Trường, SDK, Reference DLL, .addin](02-topic-notes/02-environment-sdk-addin.md) | [PNG](04-infographics-png/02-environment-sdk-addin.png) | [Prompt](03-infographic-prompts/02-environment-sdk-addin.md) | 6, 12, 13, 18, ... (+5 trang khác) |
| 03 | P0 | [Hello World ExternalCommand](02-topic-notes/03-hello-world-external-command.md) | [PNG](04-infographics-png/03-hello-world-external-command.png) | [Prompt](03-infographic-prompts/03-hello-world-external-command.md) | 2, 11, 12, 13, ... (+96 trang khác) |
| 04 | P0 | [ExternalCommand vs ExternalApplication vs DB ExternalApplication](02-topic-notes/04-command-application-dbapplication.md) | [PNG](04-infographics-png/04-command-application-dbapplication.png) | [Prompt](03-infographic-prompts/04-command-application-dbapplication.md) | 2, 19, 23, 24, ... (+12 trang khác) |
| 05 | P0 | [Add-in Manifest, Loading, Debugging](02-topic-notes/05-manifest-loading-debugging.md) | [PNG](04-infographics-png/05-manifest-loading-debugging.png) | [Prompt](03-infographic-prompts/05-manifest-loading-debugging.md) | 2, 6, 14, 31, ... (+2 trang khác) |
| 06 | P0 | [Ribbon, Panel, PushButton, SplitButton, StackedButton](02-topic-notes/06-ribbon-panels-controls.md) | [PNG](04-infographics-png/06-ribbon-panels-controls.png) | [Prompt](03-infographic-prompts/06-ribbon-panels-controls.md) | 2, 19, 29, 41, ... (+7 trang khác) |
| 07 | P0 | [Application, UIApplication, Document, UIDocument](02-topic-notes/07-application-document-uidocument.md) | [PNG](04-infographics-png/07-application-document-uidocument.png) | [Prompt](03-infographic-prompts/07-application-document-uidocument.md) | 2, 9, 21, 25, ... (+46 trang khác) |
| 08 | P0 | [Element Essentials: Element, ElementId, Category, Type/Instance](02-topic-notes/08-element-essentials.md) | [PNG](04-infographics-png/08-element-essentials.png) | [Prompt](03-infographic-prompts/08-element-essentials.md) | 2, 9, 55, 56, ... (+8 trang khác) |
| 09 | P1 | [Parameters: BuiltInParameter, StorageType, AsValueString, SetValueString](02-topic-notes/09-parameters.md) | [PNG](04-infographics-png/09-parameters.png) | [Prompt](03-infographic-prompts/09-parameters.md) | 2, 3, 5, 6, ... (+78 trang khác) |
| 10 | P0 | [Selection: Current Selection, PickObject, PickObjects, PickPoint](02-topic-notes/10-selection.md) | [PNG](04-infographics-png/10-selection.png) | [Prompt](03-infographic-prompts/10-selection.md) | 2, 3, 9, 21, ... (+44 trang khác) |
| 11 | P0 | [Selection Filter: ISelectionFilter](02-topic-notes/11-selection-filter.md) | [PNG](04-infographics-png/11-selection-filter.png) | [Prompt](03-infographic-prompts/11-selection-filter.md) | 2, 87, 88, 383 |
| 12 | P0 | [FilteredElementCollector Foundation](02-topic-notes/12-filtered-element-collector-foundation.md) | [PNG](04-infographics-png/12-filtered-element-collector-foundation.png) | [Prompt](03-infographic-prompts/12-filtered-element-collector-foundation.md) | 2, 22, 26, 59, ... (+35 trang khác) |
| 13 | P0 | [Filtering Nâng Cao: Class, Category, Rule, LINQ, Bounding Box, Intersection](02-topic-notes/13-advanced-filtering.md) | [PNG](04-infographics-png/13-advanced-filtering.png) | [Prompt](03-infographic-prompts/13-advanced-filtering.md) | 2, 74, 75, 83 |
| 14 | P0 | [Transactions: Transaction, SubTransaction, TransactionGroup](02-topic-notes/14-transactions.md) | [PNG](04-infographics-png/14-transactions.png) | [Prompt](03-infographic-prompts/14-transactions.md) | 5, 39, 260, 271, ... (+16 trang khác) |
| 15 | P1 | [Editing Elements: Move, Copy, Rotate, Mirror, Array, Delete, Pinned](02-topic-notes/15-editing-elements.md) | [PNG](04-infographics-png/15-editing-elements.png) | [Prompt](03-infographic-prompts/15-editing-elements.md) | 3, 9, 52, 64, ... (+9 trang khác) |
| 16 | P1 | [Views: View3D, ViewPlan, ViewSheet, ViewSchedule, Crop, UIView](02-topic-notes/16-views.md) | [PNG](04-infographics-png/16-views.png) | [Prompt](03-infographic-prompts/16-views.md) | 3, 6, 8, 9, ... (+61 trang khác) |
| 17 | P1 | [Family Instance và Family Symbol](02-topic-notes/17-family-instance-symbol.md) | [PNG](04-infographics-png/17-family-instance-symbol.png) | [Prompt](03-infographic-prompts/17-family-instance-symbol.md) | 3, 9, 59, 64, ... (+43 trang khác) |
| 18 | P1 | [Geometry Overview: Options, GeometryElement, GeometryObject](02-topic-notes/18-geometry-overview.md) | [PNG](04-infographics-png/18-geometry-overview.png) | [Prompt](03-infographic-prompts/18-geometry-overview.md) | 4, 5, 9, 10, ... (+88 trang khác) |
| 19 | P1 | [Geometry Chi Tiết: Curve, Solid, Face, Edge, Mesh, GeometryInstance](02-topic-notes/19-geometry-details.md) | [PNG](04-infographics-png/19-geometry-details.png) | [Prompt](03-infographic-prompts/19-geometry-details.md) | 4, 9, 72, 141, ... (+50 trang khác) |
| 20 | P1 | [Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry](02-topic-notes/20-geometry-use-cases.md) | [PNG](04-infographics-png/20-geometry-use-cases.png) | [Prompt](03-infographic-prompts/20-geometry-use-cases.md) | 4, 79, 230, 231, ... (+1 trang khác) |
| 21 | P1 | [Walls, Floors, Roofs, Openings và Compound Structure](02-topic-notes/21-walls-floors-roofs-openings.md) | [PNG](04-infographics-png/21-walls-floors-roofs-openings.png) | [Prompt](03-infographic-prompts/21-walls-floors-roofs-openings.md) | 3, 5, 8, 9, ... (+75 trang khác) |
| 22 | P1 | [Materials: Data, Quantities, Paint Face](02-topic-notes/22-materials.md) | [PNG](04-infographics-png/22-materials.png) | [Prompt](03-infographic-prompts/22-materials.md) | 4, 6, 9, 60, ... (+46 trang khác) |
| 23 | P1 | [Revit Structure: Structural Elements và Analytical Model](02-topic-notes/23-revit-structure-analytical.md) | [PNG](04-infographics-png/23-revit-structure-analytical.png) | [Prompt](03-infographic-prompts/23-revit-structure-analytical.md) | 4, 6, 8, 10, ... (+53 trang khác) |
| 24 | P1 | [Rebar/Reinforcement: Rebar, Area, Path, Cover, Host](02-topic-notes/24-rebar-reinforcement.md) | [PNG](04-infographics-png/24-rebar-reinforcement.png) | [Prompt](03-infographic-prompts/24-rebar-reinforcement.md) | 70, 72, 77, 83, ... (+12 trang khác) |
| 25 | P2 | [Shared Parameters và Extensible Storage](02-topic-notes/25-shared-parameters-extensible-storage.md) | [PNG](04-infographics-png/25-shared-parameters-extensible-storage.png) | [Prompt](03-infographic-prompts/25-shared-parameters-extensible-storage.md) | 5, 10, 24, 56, ... (+15 trang khác) |
| 26 | P2 | [Events, Dynamic Model Update, Failure Posting](02-topic-notes/26-events-dmu-failure.md) | [PNG](04-infographics-png/26-events-dmu-failure.png) | [Prompt](03-infographic-prompts/26-events-dmu-failure.md) | 5, 6, 9, 10, ... (+25 trang khác) |
| 27 | P2 | [Revit MEP: Pipes, Ducts, Connectors, Systems](02-topic-notes/27-mep-overview.md) | [PNG](04-infographics-png/27-mep-overview.png) | [Prompt](03-infographic-prompts/27-mep-overview.md) | 5, 8, 10, 23, ... (+18 trang khác) |
| 28 | P0 | [Roadmap Học Revit API Từ Beginner Đến Production Add-in](02-topic-notes/28-learning-roadmap.md) | [PNG](04-infographics-png/28-learning-roadmap.png) | [Prompt](03-infographic-prompts/28-learning-roadmap.md) | 2, 5, 7, 9, ... (+6 trang khác) |

## Ghi chú version
Tài liệu nguồn thuộc Revit API 2014. Các class cốt lõi vẫn hữu ích về mặt tư duy, nhưng khi code cho Revit 2025-2027 cần kiểm tra lại chữ ký method, namespace, unit API, transaction behavior và API bị deprecated.
