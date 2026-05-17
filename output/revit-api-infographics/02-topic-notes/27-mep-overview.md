# Revit MEP: Pipes, Ducts, Connectors, Systems

> Priority: **P2**  
> Source PDF: `Revit 2014 Platform API Developers Guidelines`  
> Source page hits: 5, 8, 10, 23, 33, 34, 37, 112, 127, 196, ... (+12 trang khác)
> Version note: Nội dung dựa trên Revit API 2014; khi áp dụng cho Revit 2025-2027 cần đối chiếu API docs hiện hành.

## Tóm tắt
MEP API tập trung vào tạo pipe/duct, placeholder, system, connector, family connector và routing preference.

## Khái niệm chính
- Connector là điểm kết nối logic/geometry giữa MEP elements.
- System gom các element theo luồng HVAC, piping hoặc electrical.
- Routing preferences quyết định fitting/type khi route.
- Settings mechanical/electrical ảnh hưởng tạo và validate MEP elements.

## Class/API cần nhớ
`Pipe`, `Duct`, `Connector`, `ConnectorManager`, `MEPSystem`, `RoutingPreferenceManager`, `MechanicalSettings`, `ElectricalSettings`

## Workflow
- Chọn type/system/level
- Tạo pipe/duct
- Lấy connectors
- Connect elements
- Validate route/system
- Report clashes

## Lỗi thường gặp
- Connector domain mismatch
- Level/system type thiếu
- Routing preference không đủ fitting
- Slope/offset sai unit

## Ví dụ Revit API thực tế
Tạo đoạn pipe nối hai thiết bị bằng connector gần nhất, sau đó kiểm tra system và đường kính.

## Infographic
- PNG: `../04-infographics-png/27-mep-overview.png`
- Prompt: `../03-infographic-prompts/27-mep-overview.md`
