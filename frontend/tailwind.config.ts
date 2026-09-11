import type { Config } from "tailwindcss";

/**
 * Cấu hình Tailwind CSS cho Culinary Blog.
 *
 * Bảng màu:
 *  - `brand`  : cam/vàng ấm áp — màu chủ đạo, gợi cảm giác món ăn nóng hổi.
 *  - `spice`  : đỏ ớt — dùng cho điểm nhấn phụ (gradient, badge nổi bật).
 *  - `herb`   : xanh rau thơm — dùng cho trạng thái "thành công", tag nguyên liệu tươi.
 *  - `cream`  : nền kem ấm, thay cho màu xám lạnh mặc định.
 *
 * Interns: mọi màu trong UI nên dùng qua token (bg-brand-500, text-spice-600...) thay vì
 * hard-code mã hex — đổi theme sau này chỉ cần sửa đúng file này.
 *
 * Phần `keyframes`/`animation` bên dưới là "thư viện hiệu ứng" dùng chung: muốn một phần tử
 * trôi nhẹ thì thêm class `animate-float`, muốn nút loé sáng khi hover thì `animate-shine`...
 * Không cần cài thêm framer-motion — hiệu ứng chạy bằng CSS nên nhẹ và mượt hơn.
 */
const config: Config = {
  content: ["./app/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          50: "#fff7ed",
          100: "#ffedd5",
          200: "#fed7aa",
          300: "#fdba74",
          400: "#fb923c",
          500: "#f97316",
          600: "#ea580c",
          700: "#c2410c",
          800: "#9a3412",
          900: "#7c2d12",
        },
        spice: {
          50: "#fef2f2",
          100: "#fee2e2",
          200: "#fecaca",
          300: "#fca5a5",
          400: "#f87171",
          500: "#ef4444",
          600: "#dc2626",
          700: "#b91c1c",
          800: "#991b1b",
          900: "#7f1d1d",
        },
        herb: {
          50: "#f0fdf4",
          100: "#dcfce7",
          200: "#bbf7d0",
          300: "#86efac",
          400: "#4ade80",
          500: "#22c55e",
          600: "#16a34a",
          700: "#15803d",
          800: "#166534",
          900: "#14532d",
        },
        cream: {
          50: "#fffdfa",
          100: "#fef8f0",
          200: "#fdf0e2",
          300: "#f9e6d2",
        },
      },
      fontFamily: {
        sans: ["var(--font-sans)", "system-ui", "sans-serif"],
        // Font tiêu đề có chân (serif) — tạo cảm giác "tạp chí ẩm thực" sang trọng.
        display: ["var(--font-display)", "Georgia", "serif"],
      },
      borderRadius: {
        card: "1rem",
        "card-lg": "1.5rem",
      },
      boxShadow: {
        // Shadow ấm (ngả cam) thay vì đen xám — hợp tông ẩm thực hơn.
        soft: "0 2px 8px -2px rgba(124, 45, 18, 0.08), 0 4px 16px -4px rgba(124, 45, 18, 0.06)",
        lift: "0 12px 32px -8px rgba(124, 45, 18, 0.18), 0 4px 12px -4px rgba(124, 45, 18, 0.10)",
        glow: "0 0 0 1px rgba(249, 115, 22, 0.20), 0 8px 32px -8px rgba(249, 115, 22, 0.45)",
        "inner-top": "inset 0 1px 0 0 rgba(255, 255, 255, 0.45)",
      },
      backgroundImage: {
        "brand-gradient": "linear-gradient(135deg, #f97316 0%, #ef4444 50%, #c2410c 100%)",
        "warm-gradient": "linear-gradient(135deg, #fff7ed 0%, #ffedd5 50%, #fef2f2 100%)",
        "shine-sweep":
          "linear-gradient(105deg, transparent 35%, rgba(255,255,255,0.55) 50%, transparent 65%)",
      },
      keyframes: {
        /* Hiện dần + trượt lên — dùng khi phần tử lọt vào màn hình (component Reveal). */
        "fade-up": {
          "0%": { opacity: "0", transform: "translateY(24px)" },
          "100%": { opacity: "1", transform: "translateY(0)" },
        },
        "fade-in": {
          "0%": { opacity: "0" },
          "100%": { opacity: "1" },
        },
        /* Trôi lơ lửng — dùng cho các icon món ăn bay quanh hero. */
        float: {
          "0%, 100%": { transform: "translateY(0) rotate(0deg)" },
          "50%": { transform: "translateY(-18px) rotate(4deg)" },
        },
        "float-slow": {
          "0%, 100%": { transform: "translateY(0) rotate(-3deg)" },
          "50%": { transform: "translateY(-26px) rotate(3deg)" },
        },
        /* Vệt sáng quét ngang — hiệu ứng hover cho nút và card. */
        shine: {
          "0%": { transform: "translateX(-120%)" },
          "100%": { transform: "translateX(220%)" },
        },
        /* Gradient chạy qua lại — dùng cho nền hero và chữ gradient. */
        "gradient-x": {
          "0%, 100%": { backgroundPosition: "0% 50%" },
          "50%": { backgroundPosition: "100% 50%" },
        },
        /* Hơi nóng bốc lên từ nồi/đĩa thức ăn. */
        steam: {
          "0%": { opacity: "0", transform: "translateY(0) scaleX(1)" },
          "35%": { opacity: "0.55" },
          "100%": { opacity: "0", transform: "translateY(-28px) scaleX(1.7)" },
        },
        /* Nhịp đập nhẹ cho chấm "đang hoạt động", badge mới. */
        "pulse-ring": {
          "0%": { transform: "scale(0.9)", opacity: "0.7" },
          "70%": { transform: "scale(1.6)", opacity: "0" },
          "100%": { transform: "scale(1.6)", opacity: "0" },
        },
        /* Nảy nhẹ khi xuất hiện — dùng cho badge, chip. */
        pop: {
          "0%": { transform: "scale(0.85)", opacity: "0" },
          "60%": { transform: "scale(1.04)", opacity: "1" },
          "100%": { transform: "scale(1)", opacity: "1" },
        },
        /* Lắc nhẹ — dùng cho icon khi hover (vd. mũ đầu bếp ở logo). */
        wiggle: {
          "0%, 100%": { transform: "rotate(0deg)" },
          "25%": { transform: "rotate(-8deg)" },
          "75%": { transform: "rotate(8deg)" },
        },
        /* Khung xương loading (skeleton). */
        shimmer: {
          "0%": { backgroundPosition: "-500px 0" },
          "100%": { backgroundPosition: "500px 0" },
        },
        "spin-slow": {
          "0%": { transform: "rotate(0deg)" },
          "100%": { transform: "rotate(360deg)" },
        },
        /* Dải chữ chạy ngang vô tận (marquee) ở khu "món đang hot". */
        marquee: {
          "0%": { transform: "translateX(0)" },
          "100%": { transform: "translateX(-50%)" },
        },
      },
      animation: {
        "fade-up": "fade-up 0.6s cubic-bezier(0.22, 1, 0.36, 1) both",
        "fade-in": "fade-in 0.5s ease-out both",
        float: "float 6s ease-in-out infinite",
        "float-slow": "float-slow 9s ease-in-out infinite",
        shine: "shine 1.1s ease-out",
        "gradient-x": "gradient-x 8s ease infinite",
        steam: "steam 3s ease-out infinite",
        "pulse-ring": "pulse-ring 2s cubic-bezier(0.4, 0, 0.6, 1) infinite",
        pop: "pop 0.35s cubic-bezier(0.22, 1, 0.36, 1) both",
        wiggle: "wiggle 0.5s ease-in-out",
        shimmer: "shimmer 1.6s linear infinite",
        "spin-slow": "spin-slow 22s linear infinite",
        marquee: "marquee 28s linear infinite",
      },
      transitionTimingFunction: {
        // Easing "bật nhẹ" — dùng cho hover card/nút cho cảm giác mềm mại hơn ease mặc định.
        springy: "cubic-bezier(0.22, 1, 0.36, 1)",
      },
    },
  },
  plugins: [],
};

export default config;
