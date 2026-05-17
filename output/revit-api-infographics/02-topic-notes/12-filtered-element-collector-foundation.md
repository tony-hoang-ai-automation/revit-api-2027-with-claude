# FilteredElementCollector Foundation

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 22, 26, 59, 74, 76, 78, 79, 80, 81, ... (+29 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
`FilteredElementCollector` là công cụ chính để lấy element từ Document hoặc từ một View, kết hợp class/category/filter để có tập dữ liệu cần xử lý.

## Khái niệm chính
- Collector có thể quét toàn document hoặc chỉ element visible trong view.
- Nên dùng filter native trước rồi mới LINQ để tối ưu.
- Có thể lấy `Element`, `ElementId`, hoặc element đầu tiên.
- Collector hỗ trợ chaining fluent như `OfClass`, `OfCategory`, `WhereElementIsNotElementType`.

## Class/API cần nhớ
`FilteredElementCollector`, `OfClass`, `OfCategory`, `WherePasses`, `ToElements`, `ToElementIds`

## Workflow
- Tạo collector
- Giới hạn scope
- Apply native filters
- Loại type/instance
- Materialize kết quả
- Iterate

## Lỗi thường gặp
- Quét cả document quá rộng
- LINQ trước filter native
- Nhầm element type với instance

## Ví dụ Revit API thực tế
`new FilteredElementCollector(doc).OfClass(typeof(Wall)).WhereElementIsNotElementType()` để lấy wall instances.

## Infographic
- PNG: `../04-infographics-png/12-filtered-element-collector-foundation.png`
- Prompt: `../03-infographic-prompts/12-filtered-element-collector-foundation.md`
