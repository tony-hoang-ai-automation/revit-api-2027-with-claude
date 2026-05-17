# Filtering Nâng Cao: Class, Category, Rule, LINQ, Bounding Box, Intersection

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 74, 75, 83
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Advanced filtering kết hợp quick filters, slow filters, parameter rules, bounding box và intersection để tìm đúng phần tử mà vẫn giữ hiệu năng.

## Khái niệm chính
- Quick filters giảm candidate trước khi load element đầy đủ.
- Parameter filters dùng rule để lọc theo giá trị BIM.
- Bounding box filter phù hợp tìm element trong vùng không gian.
- Intersection filter cần solid geometry và có giới hạn với element không có solid như rebar.

## Class/API cần nhớ
`ElementClassFilter`, `ElementCategoryFilter`, `ElementParameterFilter`, `BoundingBoxIntersectsFilter`, `ElementIntersectsElementFilter`, `LogicalAndFilter`

## Workflow
- Chọn scope
- Apply quick filters
- Apply rule/intersection khi cần
- LINQ post-process
- Kiểm tra performance

## Lỗi thường gặp
- Intersection không bắt element không có solid
- BoundingBox không thay thế geometry chính xác
- Filter rule sai unit

## Ví dụ Revit API thực tế
Tìm door giao với wall: lọc door instances, dùng intersection/bounding box để phát hiện xung đột sơ bộ.

## Infographic
- PNG: `../04-infographics-png/13-advanced-filtering.png`
- Prompt: `../03-infographic-prompts/13-advanced-filtering.md`
