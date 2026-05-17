# Tổng Quan Revit API Platform

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 8, 9, 362, 415, 417, 450
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Revit API là lớp .NET cho phép add-in đọc dữ liệu BIM, tạo/chỉnh sửa element, mở rộng UI và tự động hóa workflow trong Revit.

## Khái niệm chính
- API chạy trong tiến trình Revit, chịu ràng buộc bởi document đang mở và trạng thái UI.
- Dữ liệu mô hình chủ yếu là Element, ElementType, Category, Parameter và Geometry.
- Add-in thường là DLL .NET được đăng ký bằng manifest `.addin`.
- Revit API 2014 là nền tảng khái niệm; khi làm Revit 2025-2027 cần kiểm tra breaking changes.

## Class/API cần nhớ
`Autodesk.Revit.DB`, `Autodesk.Revit.UI`, `IExternalCommand`, `IExternalApplication`, `Document`, `Element`

## Workflow
- Hiểu mô hình dữ liệu BIM
- Tạo class entry point
- Đăng ký add-in
- Mở Revit để debug
- Đọc/sửa model trong Transaction

## Lỗi thường gặp
- Không gọi API ngoài context hợp lệ của Revit
- Không sửa model nếu thiếu Transaction
- Không giả định API 2014 còn y nguyên ở bản mới

## Ví dụ Revit API thực tế
Dùng `IExternalCommand` để quét tất cả Wall trong `Document`, đọc parameter và xuất báo cáo khối lượng.

## Infographic
- PNG: `../04-infographics-png/01-revit-api-platform-overview.png`
- Prompt: `../03-infographic-prompts/01-revit-api-platform-overview.md`
