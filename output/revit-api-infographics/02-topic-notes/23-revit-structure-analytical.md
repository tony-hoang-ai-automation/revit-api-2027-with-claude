# Revit Structure: Structural Elements và Analytical Model

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 4, 6, 8, 10, 14, 23, 33, 37, 56, 58, ... (+47 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Structure API bổ sung structural element, analytical model, load, boundary condition và liên kết phân tích cho workflow kết cấu.

## Khái niệm chính
- Structural elements gồm beam, column, brace, foundation, rebar và reinforcement.
- Analytical model có thể khác physical model và phục vụ phân tích kết cấu.
- Load/Boundary/Analytical links cần kiểm tra product/version support.
- Host và geometry ảnh hưởng tạo reinforcement.

## Class/API cần nhớ
`FamilyInstance`, `StructuralType`, `AnalyticalModel`, `Load`, `BoundaryConditions`, `RebarHostData`

## Workflow
- Lọc structural category
- Đọc structural type
- Kiểm tra analytical model
- Đọc/tạo loads
- Liên kết reinforcement

## Lỗi thường gặp
- Analytical model disabled
- StructuralType sai
- Product-specific API
- Host không hỗ trợ reinforcement

## Ví dụ Revit API thực tế
Quét cột/dầm structural, kiểm tra analytical model missing và tạo report QA cho kỹ sư kết cấu.

## Infographic
- PNG: `../04-infographics-png/23-revit-structure-analytical.png`
- Prompt: `../03-infographic-prompts/23-revit-structure-analytical.md`
