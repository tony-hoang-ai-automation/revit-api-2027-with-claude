# Cài Môi Trường, SDK, Reference DLL, .addin

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 6, 12, 13, 18, 21, 368, 396, 397, 399
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Một add-in Revit cần project Class Library, reference đúng `RevitAPI.dll`/`RevitAPIUI.dll`, manifest `.addin` và cấu hình debug mở `Revit.exe`.

## Khái niệm chính
- Reference DLL lấy từ thư mục cài Revit tương ứng version target.
- Đặt `Copy Local = false` để tránh debugger dùng sai bản DLL.
- Manifest `.addin` khai báo Assembly, AddInId, FullClassName, Text, VendorId.
- Đường dẫn manifest thường nằm trong ProgramData Autodesk Revit Addins theo version.

## Class/API cần nhớ
`RevitAPI.dll`, `RevitAPIUI.dll`, `.addin`, `AddInId`, `FullClassName`, `VendorId`

## Workflow
- Tạo Class Library
- Add reference DLL
- Viết entry class
- Build DLL
- Tạo `.addin`
- F5/debug bằng Revit.exe

## Lỗi thường gặp
- Sai version DLL
- Copy Local bật
- FullClassName không khớp namespace
- Manifest đặt sai thư mục

## Ví dụ Revit API thực tế
Project `HelloWorld.dll` có `HelloWorld.Class1`; manifest phải trỏ đúng `<Assembly>` và `<FullClassName>`.

## Infographic
- PNG: `../04-infographics-png/02-environment-sdk-addin.png`
- Prompt: `../03-infographic-prompts/02-environment-sdk-addin.md`
