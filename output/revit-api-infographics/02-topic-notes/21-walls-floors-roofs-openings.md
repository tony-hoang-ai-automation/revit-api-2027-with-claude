# Walls, Floors, Roofs, Openings và Compound Structure

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 3, 5, 8, 9, 25, 26, 28, 57, 58, 64, ... (+69 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Các building elements có type, host, layer/compound structure, opening và material layer; API cho phép tạo, đọc cấu tạo và phân tích thông tin xây dựng.

## Khái niệm chính
- Wall/Floor/Roof là system family với creation API và type riêng.
- CompoundStructure mô tả layer vật liệu, độ dày, function.
- Opening có thể gắn host và ảnh hưởng geometry/khối lượng.
- Thermal/material properties phục vụ phân tích năng lượng và quantity.

## Class/API cần nhớ
`Wall`, `Floor`, `RoofBase`, `Opening`, `CompoundStructure`, `HostObject`, `WallType`

## Workflow
- Lọc host objects
- Đọc type
- Đọc compound layers
- Tính geometry/material
- Tạo opening nếu cần

## Lỗi thường gặp
- Layer index sai
- Opening làm thay đổi geometry
- HostObject không phải mọi category
- Unit thickness nội bộ

## Ví dụ Revit API thực tế
Bóc tách diện tích từng lớp wall bằng compound structure và material ids.

## Infographic
- PNG: `../04-infographics-png/21-walls-floors-roofs-openings.png`
- Prompt: `../03-infographic-prompts/21-walls-floors-roofs-openings.md`
