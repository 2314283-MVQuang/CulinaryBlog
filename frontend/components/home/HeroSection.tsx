"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useRef, useState } from "react";
import { ArrowRight, Search, Sparkles } from "lucide-react";
import { ChefIllustration } from "./ChefIllustration";
import { Button } from "@/components/ui/Button";

/** Các chip nổi quanh hình đầu bếp — `depth` càng lớn thì trôi càng nhiều khi rê chuột. */
const FLOATING_CHIPS = [
  { emoji: "🍜", label: "Phở bò", sub: "45 phút", pos: "left-0 top-16", depth: 9, delay: "0ms" },
  { emoji: "🥗", label: "Salad tươi", sub: "10 phút", pos: "-left-2 bottom-28", depth: 12, delay: "700ms" },
  { emoji: "🍰", label: "Bánh ngọt", sub: "Dễ làm", pos: "right-0 top-32", depth: 10, delay: "1400ms" },
  { emoji: "🍳", label: "Bữa sáng", sub: "15 phút", pos: "right-4 bottom-16", depth: 14, delay: "2100ms" },
] as const;

/**
 * Khu vực đầu trang chủ (hero).
 *
 * Hiệu ứng chuột ở đây gồm 3 lớp chồng nhau:
 *  1. Parallax  — hình đầu bếp và các món ăn dịch chuyển ngược chiều chuột, tạo chiều sâu.
 *  2. Spotlight — vầng sáng bám theo chuột trên nền gradient.
 *  3. Float     — các chip món ăn tự trôi lên xuống (CSS animation, không phụ thuộc chuột).
 *
 * Toàn bộ chạy bằng cách ghi thẳng `style.transform` / biến CSS, KHÔNG dùng setState,
 * nên chuột di chuyển liên tục cũng không làm React render lại (giữ 60fps).
 */
export function HeroSection() {
  const router = useRouter();
  const sceneRef = useRef<HTMLDivElement>(null);
  const [query, setQuery] = useState("");

  function handleMouseMove(e: React.MouseEvent<HTMLElement>) {
    const scene = sceneRef.current;
    const section = e.currentTarget;
    const rect = section.getBoundingClientRect();

    const localX = e.clientX - rect.left;
    const localY = e.clientY - rect.top;

    // Vị trí vầng sáng, tính theo % để dùng trong radial-gradient.
    section.style.setProperty("--spot-x", `${(localX / rect.width) * 100}%`);
    section.style.setProperty("--spot-y", `${(localY / rect.height) * 100}%`);

    if (!scene) return;
    // Quy về khoảng -0.5 → 0.5 với tâm màn hình là 0.
    const nx = localX / rect.width - 0.5;
    const ny = localY / rect.height - 0.5;

    scene.querySelectorAll<HTMLElement | SVGElement>("[data-depth]").forEach((el) => {
      const depth = Number(el.getAttribute("data-depth") ?? 1);
      // Dấu trừ để vật thể chạy NGƯỢC chiều chuột — đúng cảm giác nhìn qua cửa sổ.
      const dx = -nx * depth * 2.4;
      const dy = -ny * depth * 2.4;
      el.style.transform = `translate(${dx.toFixed(2)}px, ${dy.toFixed(2)}px)`;
    });
  }

  function handleMouseLeave() {
    sceneRef.current?.querySelectorAll<HTMLElement | SVGElement>("[data-depth]").forEach((el) => {
      el.style.transform = "translate(0px, 0px)";
    });
  }

  function handleSearch(e: React.FormEvent) {
    e.preventDefault();
    const q = query.trim();
    if (q.length < 2) return;
    router.push(`/search?q=${encodeURIComponent(q)}`);
  }

  return (
    <section
      onMouseMove={handleMouseMove}
      onMouseLeave={handleMouseLeave}
      // `-mt-16 sm:-mt-20`: kéo hero lên đúng bằng chiều cao Header để hero chạy LÊN TẬN
      // đỉnh trang, nằm ngay dưới Header trong suốt. Không có dòng này thì Header (sticky)
      // chiếm chỗ riêng và phần trong suốt sẽ lộ nền kem của body thay vì nền cam của hero.
      className="relative isolate -mt-16 animate-gradient-x overflow-hidden bg-brand-gradient sm:-mt-20"
      style={{ backgroundSize: "220% 220%" }}
    >
      {/* Lớp 1: hoa văn chấm bi mờ */}
      <div
        aria-hidden
        className="absolute inset-0 opacity-[0.14]"
        style={{
          backgroundImage: "radial-gradient(#ffffff 1.5px, transparent 1.5px)",
          backgroundSize: "26px 26px",
        }}
      />

      {/* Lớp 2: vầng sáng bám theo chuột */}
      <div
        aria-hidden
        className="absolute inset-0 transition-opacity duration-500"
        style={{
          background:
            "radial-gradient(600px circle at var(--spot-x, 50%) var(--spot-y, 40%), rgba(255,255,255,0.22), transparent 62%)",
        }}
      />

      {/* Lớp 3: 2 quầng màu mờ trang trí */}
      <div aria-hidden className="absolute -left-24 -top-24 h-80 w-80 rounded-full bg-amber-300/35 blur-3xl" />
      <div aria-hidden className="absolute -bottom-32 right-0 h-96 w-96 rounded-full bg-spice-500/30 blur-3xl" />

      {/* pt lớn hơn pb vì phần trên bị Header (trong suốt) phủ lên — xem chú thích ở thẻ section. */}
      <div className="container-page relative grid items-center gap-10 pb-14 pt-28 lg:grid-cols-2 lg:gap-8 lg:pb-24 lg:pt-36">
        {/* ---------------- CỘT TRÁI: nội dung ---------------- */}
        <div className="text-center lg:text-left">
          <span className="inline-flex animate-fade-up items-center gap-2 rounded-full border border-white/30 bg-white/15 px-4 py-1.5 text-sm font-medium text-white backdrop-blur-md">
            <span className="relative flex h-2 w-2">
              <span className="absolute inline-flex h-full w-full animate-pulse-ring rounded-full bg-amber-200" />
              <span className="relative inline-flex h-2 w-2 rounded-full bg-amber-300" />
            </span>
            Công thức mới mỗi ngày từ cộng đồng
          </span>

          <h1
            className="mt-6 animate-fade-up text-4xl font-bold leading-[1.1] text-white sm:text-5xl lg:text-6xl"
            style={{ animationDelay: "80ms" }}
          >
            Nấu món ngon,
            <br />
            <span className="relative inline-block">
              <span className="relative z-10 text-amber-200">chia sẻ đam mê</span>
              {/* Nét quệt bút highlight phía sau chữ */}
              <svg
                aria-hidden
                viewBox="0 0 300 20"
                preserveAspectRatio="none"
                className="absolute -bottom-1 left-0 z-0 h-3 w-full"
              >
                <path d="M2 14 Q 80 4 150 10 T 298 8" stroke="#fde68a" strokeWidth="6" fill="none" strokeLinecap="round" opacity="0.55" />
              </svg>
            </span>
          </h1>

          <p
            className="mx-auto mt-5 max-w-xl animate-fade-up text-base leading-relaxed text-brand-50 sm:text-lg lg:mx-0"
            style={{ animationDelay: "160ms" }}
          >
            Hàng nghìn công thức từ những người yêu bếp núc — tìm nguyên liệu sẵn có, nấu theo từng bước,
            và lưu lại món tủ của riêng bạn.
          </p>

          {/* Ô tìm kiếm lớn */}
          <form
            onSubmit={handleSearch}
            className="mx-auto mt-8 flex max-w-xl animate-fade-up items-center gap-2 rounded-full bg-white p-2 shadow-lift lg:mx-0"
            style={{ animationDelay: "240ms" }}
          >
            <Search className="ml-3 h-5 w-5 shrink-0 text-neutral-400" />
            <input
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Bạn muốn nấu món gì hôm nay?"
              aria-label="Tìm công thức"
              className="min-w-0 flex-1 bg-transparent py-2 text-sm text-neutral-800 placeholder:text-neutral-400 focus:outline-none sm:text-base"
            />
            <Button type="submit" size="md" className="shrink-0 rounded-full px-5">
              Tìm kiếm
            </Button>
          </form>

          <div
            className="mt-7 flex animate-fade-up flex-wrap items-center justify-center gap-3 lg:justify-start"
            style={{ animationDelay: "320ms" }}
          >
            <Link href="/recipes">
              <Button
                size="lg"
                variant="secondary"
                className="group rounded-full bg-white text-brand-700 shadow-soft hover:bg-brand-50"
              >
                Khám phá công thức
                <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
              </Button>
            </Link>
            <Link href="/dashboard/recipes/new">
              <Button
                size="lg"
                variant="outline"
                className="rounded-full border-white/50 bg-white/10 text-white backdrop-blur-md hover:bg-white/20"
              >
                <Sparkles className="h-4 w-4" />
                Đăng công thức của bạn
              </Button>
            </Link>
          </div>
        </div>

        {/* ---------------- CỘT PHẢI: hình minh hoạ ---------------- */}
        <div ref={sceneRef} className="relative mx-auto w-full max-w-lg lg:max-w-none">
          <div data-depth="4" className="transition-transform duration-300 ease-out">
            <ChefIllustration className="drop-shadow-2xl" />
          </div>

          {/* Chip món ăn nổi quanh hình */}
          {FLOATING_CHIPS.map((chip) => (
            <div
              key={chip.label}
              data-depth={chip.depth}
              className={`absolute hidden transition-transform duration-300 ease-out sm:block ${chip.pos}`}
            >
              <div
                className="flex items-center gap-2.5 rounded-2xl border border-white/50 bg-white/90 px-3.5 py-2.5 shadow-lift backdrop-blur-md animate-float"
                style={{ animationDelay: chip.delay }}
              >
                <span className="text-2xl" aria-hidden>
                  {chip.emoji}
                </span>
                <span className="text-left leading-tight">
                  <span className="block text-sm font-semibold text-neutral-800">{chip.label}</span>
                  <span className="block text-xs text-neutral-500">{chip.sub}</span>
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Đường lượn sóng nối hero với phần nội dung bên dưới */}
      <svg
        aria-hidden
        viewBox="0 0 1440 80"
        preserveAspectRatio="none"
        className="block h-12 w-full sm:h-20"
        // fill trùng màu nền body (cream-50) để trông như hero "tan" vào trang.
        fill="#fffdfa"
      >
        <path d="M0 80 L0 34 C 240 74 480 4 720 24 C 960 44 1200 78 1440 40 L1440 80 Z" />
      </svg>
    </section>
  );
}
