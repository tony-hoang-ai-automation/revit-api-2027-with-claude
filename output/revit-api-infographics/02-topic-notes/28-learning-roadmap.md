# Roadmap Học Revit API Từ Beginner Đến Production Add-in

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 5, 7, 9, 10, 11, 13, 88, 279, 315
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Roadmap học hiệu quả bắt đầu từ add-in lifecycle, document/element/parameter, sau đó selection/filtering/transaction, rồi geometry và domain-specific APIs.

## Khái niệm chính
- Beginner: Hello World, manifest, command, document, element.
- Core: selection, filtering, parameter, transaction, editing.
- Modeling: family, views, geometry, materials.
- Production: events, failure handling, tests, packaging, version compatibility.

## Class/API cần nhớ
`IExternalCommand`, `FilteredElementCollector`, `Transaction`, `Parameter`, `GeometryElement`, `Rebar`, `IUpdater`

## Workflow
- Tuần 1 basics
- Tuần 2 element data
- Tuần 3 editing/transactions
- Tuần 4 geometry
- Tuần 5 domain APIs
- Tuần 6 production

## Lỗi thường gặp
- Học geometry quá sớm
- Bỏ qua transaction
- Không build sample nhỏ
- Không kiểm tra version API hiện đại

## Ví dụ Revit API thực tế
Capstone: add-in chọn cột, tạo rebar, xuất report parameter, tạo 3D view kiểm tra và ghi log lỗi.

## Infographic
- PNG: `../04-infographics-png/28-learning-roadmap.png`
- Prompt: `../03-infographic-prompts/28-learning-roadmap.md`
