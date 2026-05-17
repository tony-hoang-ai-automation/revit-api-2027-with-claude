# Geometry Chi Tiết: Curve, Solid, Face, Edge, Mesh, GeometryInstance

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 4, 9, 72, 141, 143, 144, 165, 168, 170, 174, ... (+44 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Làm geometry hiệu quả cần hiểu từng object: curve có parameterization, solid có face/edge, face có UV, mesh dùng cho biểu diễn tam giác.

## Khái niệm chính
- Curve có Line, Arc, Ellipse, HermiteSpline và các phép evaluate/project.
- Solid gồm Faces và Edges; volume/area dùng cho phân tích.
- Face dùng UV parameter, normal và triangulation.
- Mesh hữu ích để visualize hoặc phân tích hình học phức tạp nhẹ.

## Class/API cần nhớ
`Curve`, `Line`, `Arc`, `Solid`, `Face`, `PlanarFace`, `Edge`, `Mesh`

## Workflow
- Phân loại GeometryObject
- Lọc Solid volume > 0
- Iterate faces
- Đọc edges/loops
- Triangulate nếu cần
- Tính toán

## Lỗi thường gặp
- So sánh double tuyệt đối
- Assume face phẳng
- Không xử lý nested instance
- Dùng mesh thay solid cho tính chính xác cao

## Ví dụ Revit API thực tế
Tìm planar face lớn nhất của floor để đặt area reinforcement hoặc annotation theo mặt.

## Infographic
- PNG: `../04-infographics-png/19-geometry-details.png`
- Prompt: `../03-infographic-prompts/19-geometry-details.md`
