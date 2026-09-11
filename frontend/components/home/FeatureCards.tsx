import { CookingPot, ListOrdered, Search } from "lucide-react";
import { Reveal } from "@/components/ui/Reveal";
import { TiltCard } from "@/components/ui/TiltCard";

const FEATURES = [
  {
    icon: Search,
    emoji: "🔎",
    title: "Tìm theo nguyên liệu",
    desc: "Còn gì trong tủ lạnh thì nấu món đó — lọc nhanh theo danh mục, độ khó và thời gian nấu.",
  },
  {
    icon: ListOrdered,
    emoji: "📋",
    title: "Hướng dẫn từng bước",
    desc: "Mỗi công thức chia rõ nguyên liệu và các bước, kèm thời gian và khẩu phần cụ thể.",
  },
  {
    icon: CookingPot,
    emoji: "👨‍🍳",
    title: "Đăng món của bạn",
    desc: "Tạo công thức qua trình soạn 4 bước, lưu nháp thoải mái rồi xuất bản khi đã ưng ý.",
  },
];

/** Ba thẻ giới thiệu tính năng chính — mỗi thẻ nghiêng 3D theo chuột (TiltCard). */
export function FeatureCards() {
  return (
    <section className="container-page">
      <Reveal className="mb-8 text-center">
        <h2 className="text-2xl font-bold sm:text-3xl">Nấu ăn dễ hơn bạn nghĩ</h2>
        <p className="mx-auto mt-2 max-w-lg text-neutral-500">
          Ba thứ giúp bạn từ &ldquo;không biết nấu gì&rdquo; đến bữa cơm nóng hổi trên bàn.
        </p>
      </Reveal>

      <div className="grid gap-6 sm:grid-cols-3">
        {FEATURES.map(({ icon: Icon, emoji, title, desc }, index) => (
          <Reveal key={title} delay={index * 120}>
            <TiltCard maxTilt={6}>
              <article className="shine-on-hover group relative h-full overflow-hidden rounded-card-lg border border-brand-100 bg-white p-6 shadow-soft">
                {/* Vệt gradient mờ ở góc, đậm dần khi hover */}
                <span
                  aria-hidden
                  className="pointer-events-none absolute -right-10 -top-10 h-32 w-32 rounded-full bg-brand-gradient opacity-10 blur-2xl transition-opacity duration-500 group-hover:opacity-30"
                />

                <span className="relative flex h-14 w-14 items-center justify-center rounded-2xl bg-brand-50 text-brand-600 transition-transform duration-500 ease-springy group-hover:-rotate-6 group-hover:scale-110">
                  <Icon className="h-6 w-6" />
                  {/* Emoji nhỏ nảy ra ở góc icon khi hover */}
                  <span className="absolute -right-2 -top-2 scale-0 text-lg transition-transform duration-300 ease-springy group-hover:scale-100">
                    {emoji}
                  </span>
                </span>

                <h3 className="relative mt-5 text-lg font-semibold text-neutral-900">{title}</h3>
                <p className="relative mt-2 text-sm leading-relaxed text-neutral-500">{desc}</p>
              </article>
            </TiltCard>
          </Reveal>
        ))}
      </div>
    </section>
  );
}
