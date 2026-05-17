# Selection: Current Selection, PickObject, PickObjects, PickPoint

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 3, 9, 21, 23, 28, 39, 52, 56, 67, ... (+38 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Selection API xử lý cả selection hiện tại và prompt người dùng chọn element, face, edge hoặc point trong giao diện Revit.

## Khái niệm chính
- `UIDocument.Selection` quản lý tập chọn hiện tại.
- `GetElementIds` phù hợp với API hiện đại hơn `Selection.Elements` cũ.
- `PickObject`/`PickObjects` trả về `Reference` để truy tới element hoặc geometry.
- `PickPoint` có thể dùng object snap để lấy tọa độ chính xác.

## Class/API cần nhớ
`UIDocument.Selection`, `Selection.GetElementIds`, `PickObject`, `PickObjects`, `PickPoint`, `ObjectType`

## Workflow
- Lấy selection hiện tại
- Nếu rỗng thì prompt user
- Chuyển Reference thành Element
- Validate category
- Chạy logic

## Lỗi thường gặp
- Không xử lý user cancel
- Không lọc category
- Dùng selection UI trong context không UI
- Reference face khác ElementId

## Ví dụ Revit API thực tế
Prompt user chọn một cột bê tông, sau đó lấy `FamilyInstance` để tạo rebar theo bounding box.

## Infographic
- PNG: `../04-infographics-png/10-selection.png`
- Prompt: `../03-infographic-prompts/10-selection.md`
