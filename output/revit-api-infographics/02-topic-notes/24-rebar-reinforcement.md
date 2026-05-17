# Rebar/Reinforcement: Rebar, Area, Path, Cover, Host

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 70, 72, 77, 83, 156, 196, 217, 279, 280, 281, ... (+6 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Rebar API tạo thanh thép từ curves hoặc shape, quản lý layout, hook, cover và host; Area/Path reinforcement dùng cho sàn/tường theo mặt phẳng.

## Khái niệm chính
- `Rebar.CreateFromCurves` tạo thép từ các curve cùng mặt phẳng.
- `CreateFromRebarShape` dùng shape có sẵn và scale/location.
- `AreaReinforcement`/`PathReinforcement` cần host planar face phù hợp.
- `RebarHostData` và `RebarCoverType` quản lý host/các lớp cover.

## Class/API cần nhớ
`Rebar`, `Rebar.CreateFromCurves`, `RebarBarType`, `RebarHookType`, `RebarShape`, `AreaReinforcement`, `PathReinforcement`, `RebarHostData`

## Workflow
- Chọn host
- Lấy planar face/cover
- Chọn bar type/hook/shape
- Tạo curves
- Create rebar
- Set layout rule

## Lỗi thường gặp
- Curves không đồng phẳng
- Host không hợp lệ
- Hook orientation sai
- ElementIntersects filter không bắt rebar vì không có solid

## Ví dụ Revit API thực tế
Tạo thép đai cột: lấy bounding box column, tạo loop curves, gọi `Rebar.CreateFromCurves`, set spacing layout.

## Infographic
- PNG: `../04-infographics-png/24-rebar-reinforcement.png`
- Prompt: `../03-infographic-prompts/24-rebar-reinforcement.md`
