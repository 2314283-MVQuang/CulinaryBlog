import { cn } from "@/lib/utils";

/**
 * Minh hoạ đầu bếp đang đảo chảo, xung quanh là các món ăn bay lơ lửng.
 *
 * Vì sao vẽ bằng SVG thay vì dùng file ảnh .jpg/.png:
 *  - Sắc nét ở mọi kích thước màn hình (kể cả màn Retina) mà dung lượng chỉ vài KB.
 *  - Không phụ thuộc mạng / CDN ngoài — chạy offline vẫn hiện, không lo ảnh 404.
 *  - Animate được từng bộ phận riêng (hơi nóng bốc lên, món ăn trôi nổi) bằng CSS.
 *
 * Các nhóm <g> có `data-depth` được HeroSection dùng để tạo hiệu ứng parallax:
 * lớp có độ sâu càng lớn thì dịch chuyển càng nhiều khi rê chuột.
 */
export function ChefIllustration({ className }: { className?: string }) {
  return (
    <svg
      viewBox="0 0 520 520"
      role="img"
      aria-label="Đầu bếp đang đảo chảo giữa các món ăn"
      className={cn("h-full w-full", className)}
    >
      <defs>
        <radialGradient id="halo" cx="50%" cy="45%" r="55%">
          <stop offset="0%" stopColor="#ffffff" stopOpacity="0.95" />
          <stop offset="55%" stopColor="#fed7aa" stopOpacity="0.55" />
          <stop offset="100%" stopColor="#fb923c" stopOpacity="0" />
        </radialGradient>
        <linearGradient id="jacket" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stopColor="#ffffff" />
          <stop offset="100%" stopColor="#eef2f7" />
        </linearGradient>
        <linearGradient id="panMetal" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor="#475569" />
          <stop offset="100%" stopColor="#1e293b" />
        </linearGradient>
      </defs>

      {/* Vầng sáng ấm phía sau nhân vật */}
      <circle cx="260" cy="262" r="232" fill="url(#halo)" />

      {/* Vòng tròn nét đứt xoay chậm — tạo cảm giác chuyển động nền */}
      <circle
        cx="260"
        cy="262"
        r="206"
        fill="none"
        stroke="#fb923c"
        strokeOpacity="0.35"
        strokeWidth="2"
        strokeDasharray="10 16"
        strokeLinecap="round"
        className="animate-spin-slow"
        style={{ transformOrigin: "260px 262px" }}
      />

      {/* Bóng đổ dưới chân */}
      <ellipse cx="258" cy="468" rx="150" ry="22" fill="#7c2d12" opacity="0.10" />

      {/* ================= MÓN ĂN BAY LƠ LỬNG ================= */}

      {/*
        Mỗi món ăn bay được bọc 2 lớp <g> có chủ đích:
          - lớp ngoài `data-depth` : HeroSection ghi transform vào đây để tạo parallax theo chuột.
          - lớp trong `animate-*`  : giữ animation trôi nổi của CSS.
        Không gộp 2 lớp làm một vì CSS animation luôn "thắng" inline style — gộp lại thì
        parallax sẽ bị animation ghi đè và không chạy.
      */}

      {/* Miếng pizza — góc trên trái */}
      <g data-depth="3">
        <g className="animate-float" style={{ animationDelay: "0ms" }}>
          <g transform="translate(52,116) rotate(-10)">
            <path d="M0 0 L60 15 L27 64 Z" fill="#fbbf24" stroke="#f59e0b" strokeWidth="3" strokeLinejoin="round" />
            <path d="M0 0 L60 15 L54 26 L5 11 Z" fill="#f59e0b" />
            <circle cx="25" cy="24" r="5" fill="#ef4444" />
            <circle cx="34" cy="40" r="4.5" fill="#ef4444" />
            <circle cx="17" cy="38" r="3.5" fill="#ef4444" />
          </g>
        </g>
      </g>

      {/* Cà chua — góc trên phải */}
      <g data-depth="4">
        <g className="animate-float-slow" style={{ animationDelay: "600ms" }}>
          <g transform="translate(412,74)">
            <circle cx="26" cy="30" r="25" fill="#ef4444" />
            <circle cx="18" cy="22" r="7" fill="#f87171" opacity="0.85" />
            <path d="M12 8 q14 -9 28 0 q-14 8 -28 0 z" fill="#22c55e" />
            <path d="M26 8 v-8" stroke="#16a34a" strokeWidth="4" strokeLinecap="round" />
          </g>
        </g>
      </g>

      {/* Lá rau thơm — cạnh trái */}
      <g data-depth="2">
        <g className="animate-float" style={{ animationDelay: "1200ms" }}>
          <g transform="translate(34,286) rotate(-8)">
            <path d="M0 24 Q20 -6 46 6 Q36 38 0 24 Z" fill="#4ade80" />
            <path d="M0 24 Q22 15 46 6" stroke="#15803d" strokeWidth="2.5" fill="none" strokeLinecap="round" />
          </g>
        </g>
      </g>

      {/* Sushi — góc dưới trái */}
      <g data-depth="5">
        <g className="animate-float-slow" style={{ animationDelay: "300ms" }}>
          <g transform="translate(56,392) rotate(6)">
            <rect x="0" y="16" width="66" height="28" rx="14" fill="#ffffff" stroke="#e2e8f0" strokeWidth="2" />
            <rect x="3" y="4" width="60" height="22" rx="11" fill="#fb7185" />
            <path d="M8 13 h50" stroke="#fda4af" strokeWidth="3" strokeLinecap="round" />
            <rect x="26" y="2" width="14" height="44" rx="3" fill="#334155" />
          </g>
        </g>
      </g>

      {/* Tô mì bốc khói — góc dưới phải */}
      <g data-depth="4">
        <g className="animate-float" style={{ animationDelay: "900ms" }}>
          <g transform="translate(376,392)">
            <ellipse cx="46" cy="16" rx="46" ry="11" fill="#fde68a" />
            <path d="M0 16 h92 a46 40 0 0 1 -92 0 z" fill="#ef4444" />
            <path d="M0 16 h92" stroke="#b91c1c" strokeWidth="3" />
            <circle cx="30" cy="13" r="5" fill="#fffbeb" />
            <circle cx="58" cy="15" r="4" fill="#4ade80" />
            <path d="M64 6 l22 -16" stroke="#c08457" strokeWidth="5" strokeLinecap="round" />
          </g>
        </g>
      </g>

      {/* ================= NHÂN VẬT ĐẦU BẾP ================= */}
      <g data-depth="1">
        {/* Mũ đầu bếp */}
        <g>
          <circle cx="222" cy="112" r="38" fill="#ffffff" />
          <circle cx="260" cy="92" r="45" fill="#ffffff" />
          <circle cx="298" cy="112" r="38" fill="#ffffff" />
          <path d="M208 128 h104 v28 a12 12 0 0 1 -12 12 h-80 a12 12 0 0 1 -12 -12 z" fill="#f8fafc" />
          <path d="M208 140 h104" stroke="#e2e8f0" strokeWidth="3" />
        </g>

        {/* Đầu + mặt */}
        <circle cx="260" cy="200" r="47" fill="#f5c99b" />
        {/* Tai */}
        <circle cx="214" cy="202" r="9" fill="#e8b184" />
        <circle cx="306" cy="202" r="9" fill="#e8b184" />
        {/* Mắt cười */}
        <path d="M237 192 q7 -9 14 0" stroke="#1f2937" strokeWidth="4" strokeLinecap="round" fill="none" />
        <path d="M269 192 q7 -9 14 0" stroke="#1f2937" strokeWidth="4" strokeLinecap="round" fill="none" />
        {/* Má hồng */}
        <ellipse cx="230" cy="210" rx="10" ry="6" fill="#fca5a5" opacity="0.65" />
        <ellipse cx="290" cy="210" rx="10" ry="6" fill="#fca5a5" opacity="0.65" />
        {/* Ria mép */}
        <g fill="#78350f">
          <ellipse cx="249" cy="214" rx="14" ry="7" transform="rotate(-12 249 214)" />
          <ellipse cx="271" cy="214" rx="14" ry="7" transform="rotate(12 271 214)" />
        </g>
        {/* Miệng cười */}
        <path d="M246 228 q14 13 28 0" stroke="#9a3412" strokeWidth="3.5" strokeLinecap="round" fill="none" />

        {/* Cổ */}
        <path d="M242 236 h36 v20 h-36 z" fill="#e8b184" />

        {/* Áo bếp */}
        <path
          d="M212 262 C212 248 228 240 242 236 L260 262 L278 236 C292 240 308 248 308 262 L316 404 Q316 416 304 416 L216 416 Q204 416 204 404 Z"
          fill="url(#jacket)"
          stroke="#e2e8f0"
          strokeWidth="2"
        />
        {/* Khăn quàng cổ đỏ */}
        <path d="M240 236 L260 264 L280 236 L270 230 L260 246 L250 230 Z" fill="#ef4444" />
        {/* Hàng cúc áo */}
        <g fill="#cbd5e1">
          <circle cx="243" cy="300" r="4" />
          <circle cx="243" cy="332" r="4" />
          <circle cx="243" cy="364" r="4" />
          <circle cx="277" cy="300" r="4" />
          <circle cx="277" cy="332" r="4" />
          <circle cx="277" cy="364" r="4" />
        </g>

        {/* Tay trái cầm thìa gỗ */}
        <path
          d="M214 266 C190 278 176 302 174 328"
          stroke="#f1f5f9"
          strokeWidth="38"
          strokeLinecap="round"
          fill="none"
        />
        <path
          d="M214 266 C190 278 176 302 174 328"
          stroke="#ffffff"
          strokeWidth="32"
          strokeLinecap="round"
          fill="none"
        />
        <circle cx="172" cy="332" r="16" fill="#f5c99b" />
        <path d="M166 344 L146 404" stroke="#c08457" strokeWidth="8" strokeLinecap="round" />
        <ellipse cx="142" cy="414" rx="14" ry="10" fill="#d9a06b" transform="rotate(-18 142 414)" />

        {/* Tay phải cầm chảo */}
        <path
          d="M306 262 C336 260 360 272 378 288"
          stroke="#f1f5f9"
          strokeWidth="38"
          strokeLinecap="round"
          fill="none"
        />
        <path
          d="M306 262 C336 260 360 272 378 288"
          stroke="#ffffff"
          strokeWidth="32"
          strokeLinecap="round"
          fill="none"
        />
        <circle cx="380" cy="290" r="16" fill="#f5c99b" />
      </g>

      {/* ================= CHẢO + HƠI NÓNG ================= */}
      <g data-depth="6">
        {/* Cán chảo */}
        <rect x="378" y="283" width="46" height="13" rx="6.5" fill="#64748b" />
        {/* Thân chảo */}
        <ellipse cx="466" cy="290" rx="48" ry="15" fill="url(#panMetal)" />
        <ellipse cx="466" cy="285" rx="41" ry="11" fill="#334155" />
        {/* Đồ ăn trong chảo */}
        <circle cx="450" cy="282" r="7" fill="#ef4444" />
        <circle cx="466" cy="285" r="6" fill="#4ade80" />
        <circle cx="481" cy="281" r="6.5" fill="#fbbf24" />

        {/* Hơi nóng bốc lên — 3 luồng lệch pha nhau cho tự nhiên */}
        <g stroke="#ffffff" strokeWidth="5" strokeLinecap="round" fill="none">
          <path d="M448 266 q-9 -13 0 -25 q9 -12 0 -24" className="animate-steam" style={{ animationDelay: "0ms" }} />
          <path
            d="M468 262 q-9 -13 0 -25 q9 -12 0 -24"
            className="animate-steam"
            style={{ animationDelay: "800ms" }}
          />
          <path
            d="M488 266 q-9 -13 0 -25 q9 -12 0 -24"
            className="animate-steam"
            style={{ animationDelay: "1600ms" }}
          />
        </g>
      </g>

      {/* Các đốm lấp lánh nhỏ */}
      <g fill="#ffffff" opacity="0.9">
        <circle cx="140" cy="96" r="4" className="animate-float" style={{ animationDelay: "400ms" }} />
        <circle cx="386" cy="150" r="3" className="animate-float" style={{ animationDelay: "1100ms" }} />
        <circle cx="96" cy="330" r="3.5" className="animate-float" style={{ animationDelay: "1500ms" }} />
        <circle cx="430" cy="356" r="3" className="animate-float" style={{ animationDelay: "200ms" }} />
      </g>
    </svg>
  );
}
