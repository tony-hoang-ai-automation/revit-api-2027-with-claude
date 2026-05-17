# Events, Dynamic Model Update, Failure Posting

> Priority: **P2**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 5, 6, 9, 10, 13, 23, 24, 29, 46, 47, ... (+19 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Events và DMU cho phép add-in phản ứng khi document/UI thay đổi; failure API giúp post và xử lý cảnh báo/lỗi theo chuẩn Revit.

## Khái niệm chính
- Database events như DocumentChanged dùng để quan sát thay đổi.
- UI events xử lý lifecycle và tương tác người dùng.
- Dynamic Model Update dùng Updater để phản ứng khi element thay đổi.
- Failure handling cần cẩn trọng để không che lỗi quan trọng.

## Class/API cần nhớ
`DocumentChanged`, `Idling`, `IUpdater`, `UpdaterRegistry`, `FailureMessage`, `IFailuresPreprocessor`

## Workflow
- Đăng ký event/updater
- Lọc trigger
- Giữ handler nhẹ
- Thực hiện thay đổi hợp lệ
- Unregister khi shutdown

## Lỗi thường gặp
- Event handler quá nặng
- Vòng lặp update
- Sửa model trong event không cho phép
- Nuốt failure nguy hiểm

## Ví dụ Revit API thực tế
Updater tự gắn parameter kiểm tra khi wall mới được tạo, nhưng chỉ chạy trên category và change type cần thiết.

## Infographic
- PNG: `../04-infographics-png/26-events-dmu-failure.png`
- Prompt: `../03-infographic-prompts/26-events-dmu-failure.md`
