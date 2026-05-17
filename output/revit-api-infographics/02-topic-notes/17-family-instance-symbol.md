# Family Instance và Family Symbol

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 3, 9, 59, 64, 68, 69, 70, 75, 80, 83, ... (+37 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
FamilySymbol là type có thể đặt; FamilyInstance là object đã đặt trong model với host, location, level và parameters riêng.

## Khái niệm chính
- Symbol cần active trước khi tạo instance ở nhiều version API.
- FamilyInstance có thể host-based, level-based hoặc face-based.
- Family document khác project document, có API tạo form/annotation/parameter riêng.
- Type parameter nằm ở symbol/type, instance parameter nằm ở instance.

## Class/API cần nhớ
`Family`, `FamilySymbol`, `FamilyInstance`, `NewFamilyInstance`, `FamilyDocument`, `LoadFamily`

## Workflow
- Load/tìm family
- Chọn symbol
- Activate nếu cần
- Start transaction
- Place instance
- Set parameters

## Lỗi thường gặp
- Symbol chưa active
- Sai overload placement
- Nhầm instance/type parameter
- Host không hợp lệ

## Ví dụ Revit API thực tế
Đặt hanger family lên duct bằng face reference, sau đó set spacing/diameter parameter cho từng instance.

## Infographic
- PNG: `../04-infographics-png/17-family-instance-symbol.png`
- Prompt: `../03-infographic-prompts/17-family-instance-symbol.md`
