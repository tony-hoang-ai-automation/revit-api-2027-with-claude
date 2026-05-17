# Prompt Infographic: Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry

## Output đã render
`../04-infographics-png/20-geometry-use-cases.png`

## ImageGen Prompt Chi Tiết

```text
Use case: infographic-diagram
Asset type: vertical Vietnamese educational infographic poster for BIM developers learning Autodesk Revit API.

Primary request:
Create a polished vertical technical infographic titled "Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry". The poster must explain the topic clearly for a developer who knows C# but is learning Revit API. The visual should feel like a premium developer documentation poster, not a marketing landing page.

Audience:
Vietnamese BIM developers, Revit API add-in developers, C# engineers working with Autodesk Revit.

Subject:
Revit API learning poster about "Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry".

Core message:
Các use case geometry thường gặp gồm bắn tia tìm element, kiểm tra giao cắt, phân tích extrusion và lấy boundary/solid của room-space.

Required visible text, verbatim:
"Geometry Use Cases: Ray Projection, Intersection, Room/Space Geometry"
"Nền tảng Revit API 2014"
"Kiểm tra lại với Revit API version đang dùng"
"Khái niệm chính"
"API cần nhớ"
"Workflow"
"Lỗi thường gặp"
"Ví dụ Revit API"

Required concept bullets, keep short and readable:
- Ray projection hỗ trợ tìm đối tượng theo hướng nhìn/đường bắn.
- Intersection có thể dùng filter hoặc boolean/solid tùy độ chính xác.
- Room/Space geometry cần boundary options và phase/view phù hợp.
- Extrusion analysis giúp nhận diện profile và hướng đùn của solid.

Required API/class/method names, keep spelling exactly:
ReferenceIntersector, ElementIntersectsSolidFilter, BooleanOperationsUtils, SpatialElementGeometryCalculator, Room, Space

Required workflow bullets:
- Xác định câu hỏi hình học
- Chọn ray/filter/solid calculator
- Giới hạn scope
- Tính kết quả
- Highlight/report

Required common mistakes:
- Thiếu 3D view cho ray
- Room boundary không kín
- Element không có solid
- Hiệu năng kém khi boolean quá nhiều

Required example:
Từ một điểm treo thiết bị MEP, bắn ray lên trên để tìm slab gần nhất và tính chiều dài ty treo.

Composition/framing:
- Portrait poster, 9:16 or 2:3 aspect ratio.
- Top header band with a large topic number, large title, and a small badge "Revit API 2014".
- Main body uses 5 stacked sections/cards:
  1. Tóm tắt: one short paragraph.
  2. Khái niệm chính: 3-4 large bullets with simple icons.
  3. API cần nhớ: pill-shaped code chips for class/method names.
  4. Workflow: left-to-right or top-to-bottom numbered process.
  5. Lỗi thường gặp + Ví dụ: warning callouts and one practical Revit API example.
- Add thin connector lines between sections to show learning flow.
- Leave generous margins; no crowded layout; all text must fit inside its panel.

Visual style:
- Modern flat technical infographic, crisp vector-like raster artwork.
- Clean Swiss/documentation typography.
- Subtle BIM/Revit visual language: generic building model wireframe, C# brackets, database/document icon, ribbon/command icon when relevant.
- Use generic icons only. Do not use official Autodesk/Revit/GitHub logos unless already allowed by the model; generic line icons are preferred.
- No photorealistic people. No cartoon characters. No fake software screenshots.

Typography:
- Vietnamese-compatible sans-serif font similar to Be Vietnam Pro, Inter, Arial, or SF Pro.
- Code/API chips in monospace style similar to JetBrains Mono.
- Large readable text: title very large, section headers medium-large, body bullets short.
- Preserve Vietnamese diacritics perfectly: ă, â, ê, ô, ơ, ư, đ, á, à, ả, ã, ạ, ấ, ầ, ẩ, ẫ, ậ.
- Preserve API names exactly, including casing and punctuation.

Color palette:
- Background: deep charcoal/navy #0F172A with subtle grid lines.
- Cards: white or near-white #F8FAFC.
- Text: dark slate #111827 on cards, white on header.
- Accents: cyan #38BDF8, green #22C55E, amber #F59E0B, red #EF4444, violet #A78BFA.
- Avoid a one-color blue/purple gradient. Use balanced accent colors.

Layout constraints:
- No overlapping text.
- No clipped title.
- No tiny paragraphs.
- No lorem ipsum.
- No gibberish pseudo-code.
- Do not invent unrelated API names.
- Keep each text block concise; if too much text, summarize visually instead of shrinking font.
- Every panel must have enough breathing room.

Negative prompt / avoid:
Avoid unreadable text, misspelled Vietnamese, broken diacritics, fake code, malformed API names, overstuffed poster, blurry typography, watermark, signature, random logos, decorative bokeh blobs, dark low-contrast text, overlapping cards, cropped footer, excessive 3D clutter.

Quality target:
High-resolution clean educational poster suitable for a Vietnamese Revit API training deck and printed handout.
```

## Ghi chú sử dụng
Prompt này phù hợp để tạo biến thể bằng ImageGen. Nếu model ảnh render chữ chưa đạt, hãy giảm số bullet hoặc chỉ giữ title + API chips + diagram, rồi dùng bản PNG deterministic trong `04-infographics-png` cho nội dung chính xác.
