# ExternalCommand vs ExternalApplication vs DB ExternalApplication

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 19, 23, 24, 25, 29, 31, 34, 35, 37, ... (+6 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Ba loại entry point giải quyết ba nhu cầu: lệnh một lần, bootstrap UI/event khi Revit khởi động, hoặc xử lý database không UI.

## Khái niệm chính
- `ExternalCommand`: chạy khi user gọi lệnh, phù hợp task cụ thể.
- `ExternalApplication`: `OnStartup`/`OnShutdown`, tạo ribbon, đăng ký event UI.
- `ExternalDBApplication`: không có UI, dùng cho database-level event hoặc automation nền.
- Ribbon button thường do Application tạo và trỏ tới Command.

## Class/API cần nhớ
`IExternalCommand`, `IExternalApplication`, `IExternalDBApplication`, `OnStartup`, `OnShutdown`, `UIControlledApplication`

## Workflow
- Chọn loại add-in
- Khai báo đúng Type trong manifest
- Tạo UI nếu cần
- Đăng ký/dọn event đúng vòng đời

## Lỗi thường gặp
- Tạo UI trong DBApplication
- Đăng ký event nhưng không unregister
- Dồn business logic vào OnStartup

## Ví dụ Revit API thực tế
Application tạo panel 'QA Tools'; PushButton gọi `CheckWarningsCommand` khi user cần kiểm tra model.

## Infographic
- PNG: `../04-infographics-png/04-command-application-dbapplication.png`
- Prompt: `../03-infographic-prompts/04-command-application-dbapplication.md`
