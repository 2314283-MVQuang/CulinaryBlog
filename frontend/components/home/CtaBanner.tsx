import Link from "next/link";
import { ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { Reveal } from "@/components/ui/Reveal";

/** Dải kêu gọi hành động ở cuối trang chủ — mời người dùng đăng công thức đầu tiên. */
export function CtaBanner() {
  return (
    <section className="container-page">
      <Reveal>
        <div className="shine-on-hover relative overflow-hidden rounded-card-lg bg-brand-gradient px-6 py-12 text-center shadow-lift sm:px-12 sm:py-16">
          {/* Hoa văn chấm + emoji trang trí */}
          <div
            aria-hidden
            className="absolute inset-0 opacity-[0.16]"
            style={{
              backgroundImage: "radial-gradient(#ffffff 1.5px, transparent 1.5px)",
              backgroundSize: "24px 24px",
            }}
          />
          <span aria-hidden className="absolute left-6 top-6 animate-float text-4xl opacity-70 sm:text-5xl">
            🥘
          </span>
          <span
            aria-hidden
            className="absolute bottom-6 right-8 animate-float-slow text-4xl opacity-70 sm:text-5xl"
            style={{ animationDelay: "1200ms" }}
          >
            🍲
          </span>

          <div className="relative">
            <h2 className="text-2xl font-bold text-white sm:text-3xl">Món tủ của bạn xứng đáng được chia sẻ</h2>
            <p className="mx-auto mt-3 max-w-lg text-brand-50">
              Đăng công thức chỉ mất vài phút với trình soạn 4 bước. Lưu nháp bao nhiêu lần cũng được,
              khi nào ưng thì xuất bản.
            </p>
            <Link href="/dashboard/recipes/new" className="mt-7 inline-block">
              <Button size="lg" className="group rounded-full bg-white text-brand-700 shadow-soft hover:bg-brand-50">
                Bắt đầu đăng công thức
                <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
              </Button>
            </Link>
          </div>
        </div>
      </Reveal>
    </section>
  );
}
