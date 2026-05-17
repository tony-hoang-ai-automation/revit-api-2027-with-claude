# Hello World ExternalCommand

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 11, 12, 13, 15, 16, 17, 18, 21, 22, ... (+90 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
`IExternalCommand` là entry point đơn giản nhất: user bấm lệnh, Revit gọi `Execute`, command trả về `Succeeded`, `Failed` hoặc `Cancelled`.

## Khái niệm chính
- `Execute` nhận `ExternalCommandData`, `message` và `ElementSet` lỗi.
- Command có thể đọc `UIApplication`, `UIDocument`, `Document` từ `commandData`.
- Thuộc tính `Transaction` quyết định command có được sửa model hay không.
- `TaskDialog` phù hợp để hiển thị thông báo Revit-style.

## Class/API cần nhớ
`IExternalCommand`, `Execute`, `ExternalCommandData`, `Result.Succeeded`, `TaskDialog`, `TransactionAttribute`

## Workflow
- Tạo class implement interface
- Thêm `[Transaction]`
- Lấy context
- Chạy logic
- Return Result

## Lỗi thường gặp
- Quên namespace Autodesk.Revit.UI/DB
- Dùng TransactionMode sai
- Không set message khi fail

## Ví dụ Revit API thực tế
`TaskDialog.Show("Revit", "Hello World")` để kiểm tra add-in đã load và command chạy được.

## Infographic
- PNG: `../04-infographics-png/03-hello-world-external-command.png`
- Prompt: `../03-infographic-prompts/03-hello-world-external-command.md`
