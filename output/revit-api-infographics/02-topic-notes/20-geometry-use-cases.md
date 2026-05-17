# Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 4, 79, 230, 231, 236
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Các use case geometry thường gặp gồm bắn tia tìm element, kiểm tra giao cắt, phân tích extrusion và lấy boundary/solid của room-space.

## Khái niệm chính
- Ray projection hỗ trợ tìm đối tượng theo hướng nhìn/đường bắn.
- Intersection có thể dùng filter hoặc boolean/solid tùy độ chính xác.
- Room/Space geometry cần boundary options và phase/view phù hợp.
- Extrusion analysis giúp nhận diện profile và hướng đùn của solid.

## Class/API cần nhớ
`ReferenceIntersector`, `ElementIntersectsSolidFilter`, `BooleanOperationsUtils`, `SpatialElementGeometryCalculator`, `Room`, `Space`

## Workflow
- Xác định câu hỏi hình học
- Chọn ray/filter/solid calculator
- Giới hạn scope
- Tính kết quả
- Highlight/report

## Lỗi thường gặp
- Thiếu 3D view cho ray
- Room boundary không kín
- Element không có solid
- Hiệu năng kém khi boolean quá nhiều

## Ví dụ Revit API thực tế
Từ một điểm treo thiết bị MEP, bắn ray lên trên để tìm slab gần nhất và tính chiều dài ty treo.

## Infographic
- PNG: `../04-infographics-png/20-geometry-use-cases.png`
- Prompt: `../03-infographic-prompts/20-geometry-use-cases.md`
