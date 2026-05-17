# Geometry Overview: Options, GeometryElement, GeometryObject

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 4, 5, 9, 10, 72, 78, 79, 83, 84, 87, ... (+82 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Geometry API đọc hình học hiển thị/physical của element qua `Options`, trả về `GeometryElement` chứa solid, curve, mesh, point hoặc instance geometry.

## Khái niệm chính
- `Options` quyết định detail level, references và view context.
- Geometry có thể khác nhau theo view, detail level và family instance transform.
- `GeometryInstance` cần transform để đưa symbol geometry về model coordinates.
- Không phải element nào cũng có solid geometry.

## Class/API cần nhớ
`Options`, `Element.get_Geometry`, `GeometryElement`, `GeometryObject`, `GeometryInstance`, `Transform`

## Workflow
- Tạo Options
- Get geometry
- Iterate objects
- Handle GeometryInstance
- Extract solid/curve
- Transform coordinates

## Lỗi thường gặp
- Quên IncludeNonVisibleObjects/ComputeReferences khi cần
- Dùng geometry null
- Bỏ qua transform instance

## Ví dụ Revit API thực tế
Đọc geometry wall để lấy faces, edges và tính diện tích bề mặt phục vụ bóc tách vật liệu.

## Infographic
- PNG: `../04-infographics-png/18-geometry-overview.png`
- Prompt: `../03-infographic-prompts/18-geometry-overview.md`
