# Views: View3D, ViewPlan, ViewSheet, ViewSchedule, Crop, UIView

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 3, 6, 8, 9, 52, 55, 56, 58, 59, 64, ... (+55 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
View API kiểm soát cách nhìn và trình bày model: tạo 3D/plan/sheet/schedule, crop, filter và thao tác với view đang mở.

## Khái niệm chính
- Collector theo view chỉ lấy element visible trong view đó.
- ViewSchedule có TableData và field/sort/filter riêng.
- Crop box/section box hỗ trợ focus vùng mô hình.
- `UIView` thuộc UI API, dùng cho zoom/center và view window.

## Class/API cần nhớ
`View`, `View3D`, `ViewPlan`, `ViewSheet`, `ViewSchedule`, `TableData`, `UIView`

## Workflow
- Chọn loại view
- Tạo/cấu hình view
- Apply filters/crop
- Đặt lên sheet nếu cần
- Dùng collector theo view

## Lỗi thường gặp
- View template khóa setting
- Collector theo view bỏ element invisible
- Schedule API khác model element API

## Ví dụ Revit API thực tế
Tạo 3D view section box quanh selection để user kiểm tra va chạm MEP trong một vùng.

## Infographic
- PNG: `../04-infographics-png/16-views.png`
- Prompt: `../03-infographic-prompts/16-views.md`
