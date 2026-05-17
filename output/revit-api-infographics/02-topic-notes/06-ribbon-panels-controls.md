# Ribbon, Panel, PushButton, SplitButton, StackedButton

> Priority: **P0**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 2, 19, 29, 41, 42, 43, 44, 45, 47, 48, ... (+1 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Ribbon API giúp đưa workflow add-in vào UI Revit bằng tab, panel, button, split button, radio group và contextual help.

## Khái niệm chính
- Ribbon thường được tạo trong `OnStartup` của `IExternalApplication`.
- Mỗi control có lớp Data để tạo và lớp Item sau khi add vào panel.
- Button trỏ tới assembly và class `IExternalCommand`.
- Availability, tooltip, large image và contextual help cải thiện UX.

## Class/API cần nhớ
`CreateRibbonTab`, `CreateRibbonPanel`, `PushButtonData`, `SplitButtonData`, `RibbonItem`, `ContextualHelp`

## Workflow
- Create tab
- Create panel
- Create button data
- Add item
- Set icon/tooltip/help
- Bind command

## Lỗi thường gặp
- Tên tab trùng
- Icon resource sai path
- Button quá nhiều gây rối ribbon
- Thiếu availability cho command theo context

## Ví dụ Revit API thực tế
Panel 'Rebar Tools' có PushButton 'Create Stirrup' gọi command tạo thép đai cho column đang chọn.

## Infographic
- PNG: `../04-infographics-png/06-ribbon-panels-controls.png`
- Prompt: `../03-infographic-prompts/06-ribbon-panels-controls.md`
