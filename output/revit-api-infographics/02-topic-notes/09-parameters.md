# Parameters: BuiltInParameter, StorageType, AsValueString, SetValueString

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 3, 5, 6, 8, 9, 10, 21, 23, 24, ... (+72 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Parameter chứa phần lớn thông tin BIM; đọc/ghi đúng cần biết definition, storage type, unit conversion và binding.

## Khái niệm chính
- `BuiltInParameter` truy cập parameter chuẩn của Revit.
- `StorageType` quyết định dùng `AsString`, `AsDouble`, `AsInteger` hay `AsElementId`.
- `AsValueString`/`SetValueString` xử lý chuỗi theo unit hiển thị.
- Shared/project parameters cần binding vào category phù hợp.

## Class/API cần nhớ
`Parameter`, `Definition`, `BuiltInParameter`, `StorageType`, `AsValueString`, `SetValueString`, `BindingMap`

## Workflow
- Tìm parameter
- Kiểm tra HasValue/ReadOnly
- Đọc theo StorageType
- Convert unit nếu cần
- Set trong Transaction

## Lỗi thường gặp
- Set sai type
- Quên unit nội bộ
- Ghi parameter read-only
- LookupParameter phụ thuộc ngôn ngữ tên

## Ví dụ Revit API thực tế
Đọc chiều dài wall bằng BuiltInParameter, convert từ internal feet sang đơn vị dự án trước khi xuất Excel.

## Infographic
- PNG: `../04-infographics-png/09-parameters.png`
- Prompt: `../03-infographic-prompts/09-parameters.md`
