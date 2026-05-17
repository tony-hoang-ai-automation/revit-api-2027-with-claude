# Add-in Manifest, Loading, Debugging

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 6, 14, 31, 34, 399
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Manifest là hợp đồng để Revit tìm DLL và class entry point; debug hiệu quả cần build đúng path, mở đúng Revit version và đặt breakpoint.

## Khái niệm chính
- Manifest XML có thể đăng ký Command, Application hoặc DBApplication.
- `AddInId` nên là GUID ổn định cho add-in.
- Debug bằng cách set Start external program tới `Revit.exe`.
- Khi DLL nằm ở network share, cấu hình bảo mật có thể cần xử lý riêng.

## Class/API cần nhớ
`RevitAddIns`, `AddIn Type`, `Assembly`, `AddInId`, `FullClassName`, `VendorDescription`

## Workflow
- Build
- Copy DLL
- Copy `.addin`
- Launch Revit
- Mở Add-Ins
- Bấm command
- Breakpoint hit

## Lỗi thường gặp
- Manifest cache/path cũ
- Sai platform target
- DLL dependency thiếu
- Không restart Revit sau khi build

## Ví dụ Revit API thực tế
Nếu button không xuất hiện, kiểm tra XML hợp lệ, path DLL tồn tại và class name có namespace đầy đủ.

## Infographic
- PNG: `../04-infographics-png/05-manifest-loading-debugging.png`
- Prompt: `../03-infographic-prompts/05-manifest-loading-debugging.md`
