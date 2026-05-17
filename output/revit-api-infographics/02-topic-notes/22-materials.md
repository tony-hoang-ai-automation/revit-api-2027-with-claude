# Materials: Data, Quantities, Paint Face

> Priority: **P1**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 4, 6, 9, 60, 61, 62, 64, 66, 69, 70, ... (+40 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
Material là element chứa thông tin vật liệu, appearance/physical data và liên kết với element hoặc face để bóc tách khối lượng.

## Khái niệm chính
- Element có thể có material từ type, layer hoặc face paint.
- Material quantity có thể lấy theo area/volume tùy element hỗ trợ.
- Paint face gán material ở cấp face, khác material cấu tạo.
- Material properties dùng internal units và schema theo version.

## Class/API cần nhớ
`Material`, `GetMaterialIds`, `GetMaterialArea`, `GetMaterialVolume`, `Document.Paint`, `Document.IsPainted`

## Workflow
- Lấy material ids
- Đọc name/properties
- Tính area/volume
- Kiểm tra painted faces
- Xuất schedule/report

## Lỗi thường gặp
- Nhầm painted material với layer material
- Element không hỗ trợ quantity
- Đơn vị nội bộ
- Material asset API thay đổi theo version

## Ví dụ Revit API thực tế
Tính diện tích sơn từng phòng bằng painted faces và xuất bảng kiểm tra vật liệu hoàn thiện.

## Infographic
- PNG: `../04-infographics-png/22-materials.png`
- Prompt: `../03-infographic-prompts/22-materials.md`
