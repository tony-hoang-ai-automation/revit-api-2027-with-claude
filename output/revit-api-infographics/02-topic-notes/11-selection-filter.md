# Selection Filter: ISelectionFilter

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 87, 88, 383
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
`ISelectionFilter` giới hạn thứ user có thể chọn, giúp workflow chính xác hơn và giảm lỗi input.

## Khái niệm chính
- `AllowElement` lọc theo element/category/class.
- `AllowReference` lọc geometry reference như planar face.
- Filter nên trả false nhanh cho category không hợp lệ.
- Dùng message prompt rõ để user biết cần chọn gì.

## Class/API cần nhớ
`ISelectionFilter`, `AllowElement`, `AllowReference`, `ObjectType.Face`, `Reference`, `PlanarFace`

## Workflow
- Tạo class filter
- Implement AllowElement
- Implement AllowReference nếu chọn geometry
- Truyền vào PickObject/PickObjects
- Xử lý cancel

## Lỗi thường gặp
- Throw exception trong filter
- Không kiểm tra null Category
- Lọc face nhưng không verify PlanarFace

## Ví dụ Revit API thực tế
Filter chỉ cho chọn `PlanarFace` của Wall/Floor để tạo AreaReinforcement đúng host face.

## Infographic
- PNG: `../04-infographics-png/11-selection-filter.png`
- Prompt: `../03-infographic-prompts/11-selection-filter.md`
