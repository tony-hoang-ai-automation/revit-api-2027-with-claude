# Application, UIApplication, Document, UIDocument

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 9, 21, 25, 26, 28, 49, 52, 55, 56, ... (+40 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Application đại diện môi trường Revit; Document là model file; UIDocument là cầu nối thao tác UI như selection và active view.

## Khái niệm chính
- `Application`/`ControlledApplication` chứa thông tin cấp ứng dụng và events.
- `Document` chứa elements, settings, units, file operations và tạo/sửa dữ liệu.
- `UIDocument` dùng cho selection, active view và tương tác người dùng.
- Phân biệt DB API và UI API giúp code rõ trách nhiệm.

## Class/API cần nhớ
`Application`, `UIApplication`, `ControlledApplication`, `Document`, `UIDocument`, `ActiveUIDocument`

## Workflow
- Lấy UIApplication
- Lấy UIDocument
- Lấy Document
- Đọc settings/units
- Thao tác element
- Cập nhật UI khi cần

## Lỗi thường gặp
- Dùng UIDocument trong DB-only context
- Giữ reference Document quá lâu
- Không kiểm tra ActiveUIDocument null

## Ví dụ Revit API thực tế
`commandData.Application.ActiveUIDocument.Document` là đường lấy `Document` phổ biến trong command.

## Infographic
- PNG: `../04-infographics-png/07-application-document-uidocument.png`
- Prompt: `../03-infographic-prompts/07-application-document-uidocument.md`
